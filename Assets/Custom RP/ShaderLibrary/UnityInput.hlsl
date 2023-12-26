#ifndef CUSTOM_UNITY_INPUT_INCLUDED
// 이 조건부 컴파일 지시어는 CUSTOM_UNITY_INPUT_INCLUDED가 이미 정의되었는지 확인합니다.
// 이는 이 코드 블록이 중복해서 포함되는 것을 방지하기 위한 것입니다.
#define CUSTOM_UNITY_INPUT_INCLUDED
// CUSTOM_UNITY_INPUT_INCLUDED가 정의되지 않았다면, 이 매크로를 정의하여 이후의 중복 포함을 방지합니다.

CBUFFER_START(UnityPerDraw)
	// UnityPerDraw 상수 버퍼: 각 드로우 콜(draw call)에 대해 필요한 데이터를 정의합니다.

	float4x4 unity_ObjectToWorld;
// 오브젝트의 로컬 공간에서 월드 공간으로의 변환 행렬.

float4x4 unity_WorldToObject;
// 월드 공간에서 오브젝트의 로컬 공간으로의 역변환 행렬.

float4 unity_LODFade;
// 레벨 오브 디테일(LOD) 페이딩에 사용되는 파라미터.

real4 unity_WorldTransformParams;
// 오브젝트의 월드 변환과 관련된 추가적인 파라미터.
CBUFFER_END

float4x4 unity_MatrixVP;
// 뷰 변환 행렬과 투영 변환 행렬의 결합.

float4x4 unity_MatrixV;
// 카메라의 뷰 변환 행렬.

float4x4 unity_MatrixInvV;
// 카메라의 뷰 변환 행렬의 역변환 행렬.

float4x4 unity_prev_MatrixM;
// 이전 프레임의 모델 변환 행렬 (모션 블러 등에 사용될 수 있음).

float4x4 unity_prev_MatrixIM;
// 이전 프레임의 모델 변환 행렬의 역변환 행렬.

float4x4 glstate_matrix_projection;
// OpenGL 스타일의 투영 변환 행렬.

#endif