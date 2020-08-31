/*
 * Copyright (c) 2020 AoiKamishiro/�_�鈨
 * 
 * This code is provided under the MIT license.
 *
 */

#include "UnityCG.cginc"

#define ONE_TWO 0.5

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
float4 _MainColor;
float4 _Color2;
float4 _Color3;
float4 _Color4;
float _Cutoff;
bool _EnableVR;
bool _EnableDesktop;
bool _EnableSS;
bool _EnableOther;

#include "CameraAnalyzer.hlsl"

bool doOverlay()
{
    if (isOrthographic() || isInMirror()) return 0;
    if (_EnableVR && isVRView()) return 1;
    if (_EnableDesktop && isPlayerView()) return 1;
    if (_EnableSS && isSS()) return 1;
    if (_EnableOther) return 1;
    return 0;
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
    float2 _UV = float2(((i.uv.x <= ONE_TWO) ? i.uv.x * 2 : (i.uv.x - ONE_TWO) * 2), ((i.uv.y <= ONE_TWO) ? i.uv.y * 2 : (i.uv.y - ONE_TWO) * 2));
    c = ((i.uv.y > ONE_TWO) ? (i.uv.x <= ONE_TWO) ? tex2D(_MainTex, _UV) * _MainColor : tex2D(_Tex2, _UV) * _Color2 : (i.uv.x <= ONE_TWO) ? tex2D(_Tex3, _UV) * _Color3 : tex2D(_Tex4, _UV) * _Color4);
    if (_BlendMode == 1)
    {
        clip(c.a-_Cutoff);        
    }
    return c;
}