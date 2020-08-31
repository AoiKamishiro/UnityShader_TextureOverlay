/*
 * Copyright (c) 2020 AoiKamishiro/神城葵
 * 
 * This code is provided under the MIT license.
 * 
 * This program uses the following code, which is provided under the MIT License.
 * https://qiita.com/gam0022/items/c26a73e244dbbde9b034
 * 
*/

using System;
using UnityEditor;
using UnityEngine;

public class AKSTextureOverlay6GUI : ShaderGUI
{
    private bool __alphaOption = false;
    private enum BlendMode
    {
        Opaque,
        Cutout,
        Transparent,
    }
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        EditorGUILayout.LabelField("TextureOverlay6_v1.0 by AoiKamishiro/神城葵", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        DrawBlendMode(properties);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Textures", EditorStyles.boldLabel);
        MaterialProperty mainTex = FindProperty("_MainTex", properties);
        MaterialProperty mainColor = FindProperty("_MainColor", properties);
        materialEditor.TexturePropertyWithHDRColor(new GUIContent(mainTex.displayName), mainTex, mainColor, true);
        MaterialProperty tex2 = FindProperty("_Tex2", properties);
        MaterialProperty color2 = FindProperty("_Color2", properties);
        materialEditor.TexturePropertyWithHDRColor(new GUIContent(tex2.displayName), tex2, color2, true);
        MaterialProperty tex3 = FindProperty("_Tex3", properties);
        MaterialProperty color3 = FindProperty("_Color3", properties);
        materialEditor.TexturePropertyWithHDRColor(new GUIContent(tex3.displayName), tex3, color3, true);
        MaterialProperty tex4 = FindProperty("_Tex4", properties);
        MaterialProperty color4 = FindProperty("_Color4", properties);
        materialEditor.TexturePropertyWithHDRColor(new GUIContent(tex4.displayName), tex4, color4, true);
        MaterialProperty tex5 = FindProperty("_Tex5", properties);
        MaterialProperty color5 = FindProperty("_Color5", properties);
        materialEditor.TexturePropertyWithHDRColor(new GUIContent(tex5.displayName), tex5, color5, true);
        MaterialProperty tex6 = FindProperty("_Tex6", properties);
        MaterialProperty color6 = FindProperty("_Color6", properties);
        materialEditor.TexturePropertyWithHDRColor(new GUIContent(tex6.displayName), tex6, color6, true);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Overlay Target", EditorStyles.boldLabel);
        MaterialProperty enableVR = FindProperty("_EnableVR", properties);
        materialEditor.ShaderProperty(enableVR, enableVR.displayName);
        MaterialProperty enableDesktop = FindProperty("_EnableDesktop", properties);
        materialEditor.ShaderProperty(enableDesktop, enableDesktop.displayName);
        MaterialProperty enableSS = FindProperty("_EnableSS", properties);
        materialEditor.ShaderProperty(enableSS, enableSS.displayName);
        MaterialProperty enableOther = FindProperty("_EnableOther", properties);
        materialEditor.ShaderProperty(enableOther, enableOther.displayName);
        if (__alphaOption)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Alpha Option", EditorStyles.boldLabel);
            MaterialProperty cutoff = FindProperty("_Cutoff", properties);
            materialEditor.ShaderProperty(cutoff, cutoff.displayName);
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Rendering Option", EditorStyles.boldLabel);
        MaterialProperty ztest = FindProperty("_ZTest", properties);
        materialEditor.ShaderProperty(ztest, ztest.displayName);
        MaterialProperty zwrite = FindProperty("_ZWrite", properties);
        materialEditor.ShaderProperty(zwrite, zwrite.displayName);
        MaterialProperty srcFactor = FindProperty("_SrcFactor", properties);
        materialEditor.ShaderProperty(srcFactor, srcFactor.displayName);
        MaterialProperty dstFactor = FindProperty("_DstFactor", properties);
        materialEditor.ShaderProperty(dstFactor, dstFactor.displayName);
        materialEditor.RenderQueueField();
    }
    private void DrawBlendMode(MaterialProperty[] properties)
    {
        MaterialProperty blendMode = FindProperty("_BlendMode", properties);
        BlendMode mode = (BlendMode)blendMode.floatValue;

        using (EditorGUI.ChangeCheckScope scope = new EditorGUI.ChangeCheckScope())
        {
            mode = (BlendMode)EditorGUILayout.Popup("Blend Mode", (int)mode, Enum.GetNames(typeof(BlendMode)));
            if (scope.changed)
            {
                blendMode.floatValue = (float)mode;
                foreach (UnityEngine.Object obj in blendMode.targets)
                {
                    ApplyBlendMode(obj as Material, mode);
                }
            }
        }
        __alphaOption = mode == BlendMode.Cutout;
    }
    private static void ApplyBlendMode(Material material, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                material.SetOverrideTag("RenderType", "");
                material.SetInt("_SrcFactor", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstFactor", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.SetInt("_AlphaToMask", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.renderQueue = -1;
                break;

            case BlendMode.Cutout:
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetInt("_SrcFactor", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstFactor", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.SetInt("_AlphaToMask", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                break;

            case BlendMode.Transparent:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcFactor", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstFactor", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.SetInt("_AlphaToMask", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;

            default:
                throw new ArgumentOutOfRangeException("blendMode", blendMode, null);
        }
    }
    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);
        ApplyBlendMode(material, (BlendMode)material.GetFloat("_BlendMode"));
    }
}
