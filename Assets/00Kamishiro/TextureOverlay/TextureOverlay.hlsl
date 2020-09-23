/*
 * Copyright (c) 2020 AoiKamishiro
 * 
 * This code is provided under the MIT license.
 * 
 */

#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float4 uv : TEXCOORD0;
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float4 uv: TEXCOORD0;
};

int _BlendMode;
sampler2D _MainTex;
float4 _MainColor;
float _Cutoff;
bool _EnableVR;
bool _EnableDesktop;
bool _EnableSS;
bool _EnableOther;

#include "CameraAnalyzer.hlsl"

bool doOverlay()
{
    if(isOrthographic() || isInMirror())
    {
        return 0;
    }
    else if(isVRView())
    {
        return _EnableVR ? 1: 0;
    }
    else if(isPlayerView())
    {
        return _EnableDesktop?1: 0;
    }
    else if(isSS())
    {
        return _EnableSS?1: 0;
    }
    else
    {
        return _EnableOther?1: 0;
    }
}

v2f vert(appdata v)
{
    v2f o;
    o.vertex = doOverlay() ? float4(v.uv.x * 2 - 1, 1 - v.uv.y * 2, 0, 1) : float4(-2, -2, -2, -2);
    o.uv = ComputeScreenPos(o.vertex);
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    float4 c = float4(0, 0, 0, 0);
    c = tex2Dproj(_MainTex, i.uv) * _MainColor;
    if (_BlendMode == 1)
    {
        clip(c.a-_Cutoff);        
    }
    return c;
}