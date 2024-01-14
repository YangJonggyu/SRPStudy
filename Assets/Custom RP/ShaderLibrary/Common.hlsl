#ifndef CUSTOM_COMMON_INCLUDED
// 이 지시어는 CUSTOM_COMMON_INCLUDED가 이전에 정의되지 않았다면 아래 코드를 포함시킵니다.
// 이는 같은 코드가 여러 번 포함되는 것을 방지하기 위한 것입니다.
#define CUSTOM_COMMON_INCLUDED

// Unity의 기본 셰이더 라이브러리 포함.
// Common.hlsl에는 기본적인 셰이더 변수와 함수들이 정의되어 있습니다.
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
// Unity의 입력 구조체와 변수들을 정의하는 라이브러리 포함.
#include "UnityInput.hlsl"

// Unity의 표준 행렬 이름들을 사용하기 위한 매크로 정의.
#define UNITY_MATRIX_M unity_ObjectToWorld      // 오브젝트의 로컬 공간에서 월드 공간으로의 변환 행렬.
#define UNITY_MATRIX_I_M unity_WorldToObject    // 월드 공간에서 오브젝트의 로컬 공간으로의 역변환 행렬.
#define UNITY_MATRIX_V unity_MatrixV            // 뷰 변환 행렬.
#define UNITY_MATRIX_I_V unity_MatrixInvV       // 뷰 변환 행렬의 역변환 행렬.
#define UNITY_MATRIX_VP unity_MatrixVP          // 뷰-투영 변환 행렬.
#define UNITY_PREV_MATRIX_M unity_prev_MatrixM  // 이전 프레임의 변환 행렬 (모션 블러 등에 사용).
#define UNITY_PREV_MATRIX_I_M unity_prev_MatrixIM // 이전 프레임의 역변환 행렬.
#define UNITY_MATRIX_P glstate_matrix_projection // 투영 행렬 (OpenGL 스타일).

// GPU 인스턴싱에 필요한 함수와 매크로를 포함하는 라이브러리.
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
// 공간 변환(예: 월드 좌표계에서 뷰 좌표계로의 변환)에 사용되는 함수들을 포함하는 라이브러리.
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

float Square (float x) {
    return x * x;
}

#endif