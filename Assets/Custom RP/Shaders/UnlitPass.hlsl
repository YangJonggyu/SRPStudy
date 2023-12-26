#ifndef CUSTOM_UNLIT_PASS_INCLUDED
// 이 조건부 컴파일 지시어는 CUSTOM_UNLIT_PASS_INCLUDED가 이미 정의되었는지 확인합니다.
// 이는 이 코드 블록이 중복해서 포함되는 것을 방지하기 위한 것입니다.
#define CUSTOM_UNLIT_PASS_INCLUDED
// CUSTOM_UNLIT_PASS_INCLUDED가 정의되지 않았다면, 이 매크로를 정의하여 이후의 중복 포함을 방지합니다.

// Unity의 기본 셰이더 라이브러리를 포함합니다.
#include "../ShaderLibrary/Common.hlsl"

// 텍스처와 샘플러의 정의.
TEXTURE2D(_BaseMap);       // 베이스 맵 텍스처.
SAMPLER(sampler_BaseMap);  // 베이스 맵 텍스처를 샘플링하기 위한 샘플러.

// 인스턴싱을 위한 사용자 정의 속성을 정의합니다.
UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)  // 텍스처의 UV 스케일링 및 변환.
    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)   // 베이스 컬러.
    UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)       // 알파 클리핑 임계값.
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

// 버텍스 셰이더의 입력 구조체.
struct Attributes {
    float3 positionOS : POSITION;  // 오브젝트 공간에서의 위치.
    float2 baseUV : TEXCOORD0;     // 베이스 UV 좌표.
    UNITY_VERTEX_INPUT_INSTANCE_ID // 인스턴스 ID.
};

// 버텍스 셰이더의 출력 구조체.
struct Varyings {
    float4 positionCS : SV_POSITION; // 클립 공간에서의 위치.
    float2 baseUV : VAR_BASE_UV;     // 변환된 UV 좌표.
    UNITY_VERTEX_INPUT_INSTANCE_ID   // 인스턴스 ID.
};

// 버텍스 셰이더 정의.
Varyings UnlitPassVertex (Attributes input) {
    Varyings output;
    UNITY_SETUP_INSTANCE_ID(input);  // 인스턴스 ID 설정.
    UNITY_TRANSFER_INSTANCE_ID(input, output); // 인스턴스 ID 전달.
    float3 positionWS = TransformObjectToWorld(input.positionOS); // 월드 공간으로 변환.
    output.positionCS = TransformWorldToHClip(positionWS); // 클립 공간으로 변환.

    float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseMap_ST); // UV 변환 값 접근.
    output.baseUV = input.baseUV * baseST.xy + baseST.zw; // UV 좌표 변환.
    return output;
}

// 프래그먼트 셰이더 정의.
float4 UnlitPassFragment (Varyings input) : SV_TARGET {
    UNITY_SETUP_INSTANCE_ID(input); // 인스턴스 ID 설정.
    float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.baseUV); // 텍스처 샘플링.
    float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor); // 베이스 컬러 접근.
    float4 base = baseMap * baseColor; // 텍스처와 컬러 결합.

    // _CLIPPING이 정의된 경우 알파 클리핑 적용.
    #if defined(_CLIPPING)
        clip(base.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Cutoff));
    #endif
    return base;
}

#endif