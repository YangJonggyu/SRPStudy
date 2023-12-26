Shader "Custom RP/Unlit" {
    // 사용자 정의 무광택 셰이더의 시작.
    // "Custom RP/Unlit"는 셰이더의 이름입니다.

    Properties {
        // 셰이더에서 사용할 속성들을 정의합니다.

        _BaseMap("Texture", 2D) = "white" {}
        // 2D 텍스처 속성. 기본값은 흰색 텍스처입니다.

        _BaseColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        // 기본 색상 속성. 기본값은 흰색입니다.

        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        // 알파 임계값 속성. 텍스처의 알파 값에 기반한 클리핑에 사용됩니다.

        [Toggle(_CLIPPING)] _Clipping ("Alpha Clipping", Float) = 0
        // 알파 클리핑 토글. 활성화 시 _CLIPPING이 정의됩니다.

        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
        // 소스 블렌드 모드. Unity의 블렌딩 모드 중 선택 가능.

        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
        // 목적지(대상) 블렌드 모드.

        [Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
        // Z 버퍼 쓰기 옵션.
    }

    SubShader {
        // 서브 셰이더의 정의.

        Pass {
            // 개별 패스의 정의.

            Blend [_SrcBlend] [_DstBlend]
            // 블렌딩 설정. 소스와 목적지 블렌드 모드 사용.

            ZWrite [_ZWrite]
            // Z 버퍼 쓰기 설정.

            HLSLPROGRAM
            // HLSL 코드 블록 시작.

            #pragma shader_feature _CLIPPING
            // _CLIPPING 기능을 선택적으로 사용할 수 있도록 함.

            #pragma multi_compile_instancing
            // 인스턴싱을 위한 다중 컴파일 지시어.

            #pragma vertex UnlitPassVertex
            // 버텍스 셰이더 함수 지정.

            #pragma fragment UnlitPassFragment
            // 프래그먼트 셰이더 함수 지정.

            #include "UnlitPass.hlsl"
            // 버텍스 및 프래그먼트 셰이더 코드 포함.

            ENDHLSL
            // HLSL 코드 블록 종료.
        }
    }
}