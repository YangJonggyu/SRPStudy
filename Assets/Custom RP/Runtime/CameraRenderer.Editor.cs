using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

partial class CameraRenderer {

    // 에디터 확장을 위한 partial 메소드 선언
    partial void DrawGizmos ();
    partial void DrawUnsupportedShaders ();
    partial void PrepareForSceneWindow ();
    partial void PrepareBuffer ();

#if UNITY_EDITOR

    // 레거시 쉐이더 태그의 배열을 정의합니다.
    static ShaderTagId[] legacyShaderTagIds = {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };

    static Material errorMaterial; // 오류를 표시하기 위한 재질

    string SampleName { get; set; } // 프로파일링 샘플 이름을 위한 프로퍼티

    // 기즈모를 그리는 함수
    partial void DrawGizmos () {
        if (Handles.ShouldRenderGizmos()) {
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
    }

    // 지원되지 않는 쉐이더를 그리는 함수
    partial void DrawUnsupportedShaders () {
        // 에러 재질이 아직 정의되지 않았다면, 내부 에러 쉐이더를 사용하여 재질을 생성합니다.
        // 이 재질은 지원되지 않는 쉐이더를 시각적으로 표시하는 데 사용됩니다.
        if (errorMaterial == null) {
            errorMaterial =
                new Material(Shader.Find("Hidden/InternalErrorShader"));
        }

        // 렌더링 설정을 초기화합니다. 첫 번째 레거시 쉐이더 태그를 사용하여 설정을 시작합니다.
        // 이 설정은 에러 재질을 렌더링할 때 사용됩니다.
        var drawingSettings = new DrawingSettings(
        legacyShaderTagIds[0], new SortingSettings(camera)
        ) {
            overrideMaterial = errorMaterial
        };

        // 나머지 레거시 쉐이더 태그에 대해 반복하며, 각 쉐이더 패스 이름을 설정합니다.
        // 이를 통해 다양한 유형의 레거시 쉐이더들이 처리될 수 있도록 합니다.
        for (int i = 1; i < legacyShaderTagIds.Length; i++) {
            drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
        }

        // 기본 필터링 설정을 사용합니다.
        // 이는 어떤 렌더러가 렌더링될지를 결정하는 기준을 정의합니다.
        var filteringSettings = FilteringSettings.defaultValue;

        // 컬링 결과와 렌더링 설정을 사용하여 렌더러들을 그립니다.
        // 여기서는 지원되지 않는 쉐이더를 사용하는 렌더러들이 에러 재질로 렌더링됩니다.
        context.DrawRenderers(
        cullingResults, ref drawingSettings, ref filteringSettings
        );
    }

    // 씬 윈도우를 위한 준비 작업을 하는 함수
    partial void PrepareForSceneWindow () {
        if (camera.cameraType == CameraType.SceneView) {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }

    // 버퍼를 준비하는 함수
    partial void PrepareBuffer () {
        Profiler.BeginSample("Editor Only");
        buffer.name = SampleName = camera.name; // 버퍼 이름을 카메라 이름으로 설정
        Profiler.EndSample();
    }

#else

    const string SampleName = bufferName; // 샘플 이름을 버퍼 이름으로 설정

#endif
}