using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer {

    // 카메라 렌더링을 위한 버퍼의 이름을 정의합니다.
    const string bufferName = "Render Camera";

    // SRP 기본 불투명(unlit) 쉐이더를 위한 태그 식별자입니다.
	static ShaderTagId
		unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit"),
		litShaderTagId = new ShaderTagId("CustomLit");

	// 커맨드 버퍼를 생성합니다. 이 버퍼는 렌더링 명령을 저장합니다.
	CommandBuffer buffer = new CommandBuffer {
		name = bufferName
	};

	// 스크립터블 렌더링 컨텍스트와 카메라, 컬링 결과를 저장하기 위한 변수들입니다.
	ScriptableRenderContext context;

	Camera camera;

	CullingResults cullingResults;

	Lighting lighting = new Lighting();

	// 카메라 렌더링을 수행하는 메인 함수입니다.
	public void Render (
		ScriptableRenderContext context, Camera camera,
		bool useDynamicBatching, bool useGPUInstancing, ShadowSettings shadowSettings
	) {
		this.context = context;
		this.camera = camera;

		// 1. PrepareBuffer()
		// 커맨드 버퍼를 초기화하고 준비하는 함수입니다.
		// 이 과정에서 렌더링에 필요한 명령들이 버퍼에 저장됩니다.
		PrepareBuffer();
		
		// 2. PrepareForSceneWindow()
		// 에디터의 씬 윈도우에 대한 준비 작업을 수행합니다.
		// 주로 에디터 모드에서의 렌더링을 위한 설정을 합니다.
		PrepareForSceneWindow();
		
		// 3. Cull()
		// 카메라의 뷰 프러스텀 내에 있는 렌더링할 객체들을 결정하는 컬링 과정을 수행합니다.
		// 이 함수는 카메라에 보이는 객체들만 선택하여 불필요한 렌더링을 줄여 성능을 최적화합니다.
		if (!Cull(shadowSettings.maxDistance)) {
			return;
		}

		// 4. Setup()
		// 렌더링을 위한 카메라와 렌더링 환경 설정을 합니다.
		// 카메라 속성 설정, 클리어 플래그에 따른 렌더 타겟 클리어 등을 포함합니다.
		buffer.BeginSample(SampleName);
		ExecuteBuffer();
		lighting.Setup(context, cullingResults, shadowSettings);
		buffer.EndSample(SampleName);
		Setup();
		
		// 5. DrawVisibleGeometry()
		// 화면에 보여질 기하학적 객체들을 렌더링합니다.
		// 불투명 객체와 투명 객체를 각각 렌더링하는 과정을 포함합니다.
		DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
		
		// 6. DrawUnsupportedShaders()
		// SRP에서 지원하지 않는 쉐이더를 사용하는 렌더러를 위한 대체 렌더링 과정입니다.
		// 예를 들어, 레거시 쉐이더를 지원하기 위한 작업을 포함합니다.
		DrawUnsupportedShaders();
		
		// 7. DrawGizmos()
		// 개발 과정에서 도움을 주는 시각적 도구인 기즈모를 렌더링합니다.
		// 이는 주로 에디터 모드에서 사용되며, 게임 실행 중에는 렌더링되지 않습니다.
		DrawGizmos();
		
		// 8. Submit()
		// 모든 렌더링 명령들을 실제로 실행하기 위해 커맨드 버퍼를 제출하는 함수입니다.
		// 이 과정을 통해 최종적으로 렌더링된 이미지가 화면에 표시됩니다.
		lighting.Cleanup();
		Submit();
	}

	// 카메라 뷰에 따라 렌더링할 객체를 결정하는 컬링 과정입니다.
	bool Cull (float maxShadowDistance) {
		if (camera.TryGetCullingParameters(out ScriptableCullingParameters p)) {
			p.shadowDistance = Mathf.Min(maxShadowDistance, camera.farClipPlane);
			cullingResults = context.Cull(ref p);
			return true;
		}
		return false;
	}

	// 렌더링을 위한 카메라 설정 및 클리어 작업을 수행합니다.
	void Setup () {
		context.SetupCameraProperties(camera);
		CameraClearFlags flags = camera.clearFlags;
		buffer.ClearRenderTarget(
			flags <= CameraClearFlags.Depth,
			flags <= CameraClearFlags.Color,
			flags == CameraClearFlags.Color ?
				camera.backgroundColor.linear : Color.clear
		);
		buffer.BeginSample(SampleName);
		ExecuteBuffer();
	}

	// 준비된 커맨드 버퍼를 제출하고 렌더링을 완료합니다.
	void Submit () {
		buffer.EndSample(SampleName);
		ExecuteBuffer();
		context.Submit();
	}

	// 커맨드 버퍼에 저장된 명령을 실행하고 버퍼를 클리어합니다.
	void ExecuteBuffer () {
		context.ExecuteCommandBuffer(buffer);
		buffer.Clear();
	}

	// 화면에 보여질 객체들을 렌더링하는 과정입니다.
	void DrawVisibleGeometry (bool useDynamicBatching, bool useGPUInstancing) {
		// 카메라를 기준으로 하는 정렬 설정을 생성합니다.
		// 이 설정은 불투명한 객체들을 먼저 그리기 위해 사용됩니다.
		var sortingSettings = new SortingSettings(camera) {
			criteria = SortingCriteria.CommonOpaque
		};
		// 렌더링 설정을 초기화합니다. 여기서는 SRP 기본 불투명 쉐이더를 사용합니다.
		var drawingSettings = new DrawingSettings(
			unlitShaderTagId, sortingSettings
		) {
			enableDynamicBatching = useDynamicBatching,
			enableInstancing = useGPUInstancing
		};
		drawingSettings.SetShaderPassName(1, litShaderTagId);

		// 필터링 설정을 초기화합니다. 이 설정은 불투명한 객체들만 그리기 위해 사용됩니다.
		var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

		// 렌더링 컨텍스트를 사용하여 불투명한 객체들을 그립니다.
		context.DrawRenderers(
			cullingResults, ref drawingSettings, ref filteringSettings
		);

		// 스카이박스를 렌더링합니다. 이는 배경을 그리는 데 사용됩니다.
		context.DrawSkybox(camera);

		sortingSettings.criteria = SortingCriteria.CommonTransparent;
		drawingSettings.sortingSettings = sortingSettings;
		// 필터링 설정도 업데이트하여 투명한 객체들만 그리도록 합니다.
		filteringSettings.renderQueueRange = RenderQueueRange.transparent;

		// 렌더링 컨텍스트를 사용하여 투명한 객체들을 그립니다.
		context.DrawRenderers(
			cullingResults, ref drawingSettings, ref filteringSettings
		);
	}
}