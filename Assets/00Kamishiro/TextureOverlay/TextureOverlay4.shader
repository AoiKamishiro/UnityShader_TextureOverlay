/*
 * Copyright (c) 2020 AoiKamishiro
 *
 * This code is provided under the MIT license.
 *
*/

Shader "Kamishiro/TextureOverlay/TextureOverlay4"
{
    Properties{
        [HideInInspector] _BlendMode("__mode", Int) = 2
        [NoScaleOffset] _MainTex("Texture1", 2D) = "black" {}
        _MainColor("Color1",Color) = (1,1,1,1)
        [NoScaleOffset] _Tex2("Texture2", 2D) = "black" {}
        _Color2("Color2",Color) = (1,1,1,1)
        [NoScaleOffset] _Tex3("Texture3", 2D) = "black" {}
        _Color3("Color3",Color) = (1,1,1,1)
        [NoScaleOffset] _Tex4("Texture4", 2D) = "black" {}
        _Color4("Color4",Color) = (1,1,1,1)
        [Toggle] _EnableVR("Player View (VR)", Int) = 0
        [Toggle] _EnableDesktop("Player View (Desktop)", Int) = 1
        [Toggle] _EnableSS("Screen Shot",Int) = 1
        [Toggle] _EnableOther("Others", Int) = 1
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        [Enum(Off, 0, On, 1)] _AlphaToMask("Alpha To Mask", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 8
        [Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcFactor("Src Factor", Int) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstFactor("Dst Factor", Int) = 10
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "DisableBatching" = "True"
            "IgnoreProjector" = "True"
        }

        LOD 100
        
        Cull[_Cull]
        ZTest[_ZTest]
        ZWrite[_ZWrite]
        Blend[_SrcFactor][_DstFactor]
        AlphaToMask [_AlphaToMask]

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "TextureOverlay4.hlsl"

            ENDHLSL
        }
    }
    Fallback "Unlit/Transparent"
    CustomEditor "AKSTextureOverlay.AKSTextureOverlay4GUI"
}