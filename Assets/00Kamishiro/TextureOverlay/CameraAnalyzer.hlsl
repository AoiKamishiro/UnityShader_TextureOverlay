/*
 * Copyright (c) 2020 AoiKamishiro
 * 
 * This code is provided under the MIT license.
 +
 * This program uses the following code, which is provided under the MIT License.
 * https://gist.github.com/bdunderscore/d77bab9e8b63dc430cdd47c8ca0f8135
 * 
 */

#include "UnityCG.cginc"

#define VR_EPSILON 0.0001
#define DESKTOP_FOV 60.0
#define FOV_EPSILON 0.01
#define ROT_EPSILON 0.0001

bool isOrthographic()
{
    return UNITY_MATRIX_P[3][3] == 1;
}

bool isInMirror()
{
    return unity_CameraProjection[2][0] != 0.f || unity_CameraProjection[2][1] != 0.f;
}

bool isPlayerView()
{
#if defined(USING_STEREO_MATRICES)
    return 1;
#endif
    if (isOrthographic())
        return 0;
    float t = unity_CameraProjection._m11;
    const float Rad2Deg = 180 / UNITY_PI;
    float fov = atan(1.0f / t) * 2.0 * Rad2Deg;
    if (abs(fov - DESKTOP_FOV) >= FOV_EPSILON)
        return 0;
    float3 y_vec = float3(0, 1, 0) + _WorldSpaceCameraPos;
    float4 center_vec = UnityWorldToClipPos(_WorldSpaceCameraPos);
    float4 projected = UnityWorldToClipPos(y_vec);
    float4 offset = center_vec - projected;
    if (abs(offset.x) >= ROT_EPSILON)
        return 0;
    return 1;
}

bool isVRView()
{
#if defined(USING_STEREO_MATRICES)
    return 1;
#endif
    return 0;
}

bool isSS()
{
    int w = _ScreenParams.x;
    int h = _ScreenParams.y;
    if (w == 1280 && h == 720)
        return 1;
    if (w == 1920 && h == 1080)
        return 1;
    if (w == 3840 && h == 2160)
        return 1;
    return 0;
}