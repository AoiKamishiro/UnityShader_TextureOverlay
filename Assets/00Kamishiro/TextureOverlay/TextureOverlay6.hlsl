/*
 * Copyright (c) 2020 AoiKamishiro
 * 
 * This code is provided under the MIT license.
 * 
 */

#include "UnityCG.cginc"

#define ONE_TWO 0.5
#define ONE_THREE 0.33333
#define TWO_THREE 0.66667

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
sampler2D _Tex2;
sampler2D _Tex3;
sampler2D _Tex4;
sampler2D _Tex5;
sampler2D _Tex6;
float4 _MainColor;
float4 _Color2;
float4 _Color3;
float4 _Color4;
float4 _Color5;
float4 _Color6;
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
    float2 _UV = float2((i.uv.x <= ONE_THREE) ? i.uv.x * 3 : (i.uv.x <= TWO_THREE) ? (i.uv.x - ONE_THREE) * 3 : (i.uv.x - TWO_THREE) * 3, (i.uv.y <= 0.5) ? i.uv.y * 2 : (i.uv.y - 0.5) * 2);
    c = (i.uv.y > ONE_TWO) ? (i.uv.x <= ONE_THREE) ? tex2D(_MainTex, _UV) * _MainColor : (i.uv.x <= TWO_THREE) ? tex2D(_Tex2, _UV) * _Color2 : tex2D(_Tex3, _UV) * _Color3 : (i.uv.x <= ONE_THREE) ? tex2D(_Tex4, _UV) * _Color4 : (i.uv.x <= TWO_THREE) ? tex2D(_Tex5, _UV) * _Color5 : tex2D(_Tex6, _UV) * _Color6;
    if (_BlendMode == 1)
    {
        clip(c.a-_Cutoff);        
    }
    return c;
}