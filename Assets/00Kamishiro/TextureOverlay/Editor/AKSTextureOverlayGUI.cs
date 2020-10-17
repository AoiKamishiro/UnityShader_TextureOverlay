/*
 * Copyright (c) 2020 AoiKamishiro/神城葵
 * 
 * This code is provided under the MIT license.
 * 
 * This program uses the following code, which is provided under the MIT License.
 * https://download.unity3d.com/download_unity/008688490035/builtin_shaders-2018.4.20f1.zip?_ga=2.171325672.957521966.1599549120-262519615.1592172043
 * https://github.com/synqark/Arktoon-Shaders
 * 
 */

using System;
using UnityEditor;
using UnityEngine;
namespace AKSTextureOverlay {
    public class AKSTextureOverlayGUI : ShaderGUI {
        public static readonly string[] blendNames = Enum.GetNames (typeof (BlendMode));
        private bool __alphaOption = false;
        private bool m_FirstTimeApply = true;
        private MaterialEditor m_MaterialEditor;

        private enum BlendMode {
            Opaque,
            Cutout,
            Transparent,
        }

        #region Matdrial Property
        private MaterialProperty blendMode;
        private MaterialProperty mainTex;
        private MaterialProperty mainColor;
        private MaterialProperty enableDesktop;
        private MaterialProperty enableVR;
        private MaterialProperty enableSS;
        private MaterialProperty enableOther;
        private MaterialProperty cutoff;
        private MaterialProperty ztest;
        private MaterialProperty zwrite;
        private MaterialProperty srcFactor;
        private MaterialProperty dstFactor;
        #endregion
        private void FindProperties (MaterialProperty[] props) {
            blendMode = FindProperty ("_BlendMode", props);
            mainTex = FindProperty ("_MainTex", props);
            mainColor = FindProperty ("_MainColor", props);
            enableVR = FindProperty ("_EnableVR", props);
            enableDesktop = FindProperty ("_EnableDesktop", props);
            enableSS = FindProperty ("_EnableSS", props);
            enableOther = FindProperty ("_EnableOther", props);
            ztest = FindProperty ("_ZTest", props);
            zwrite = FindProperty ("_ZWrite", props);
            srcFactor = FindProperty ("_SrcFactor", props);
            dstFactor = FindProperty ("_DstFactor", props);
            cutoff = FindProperty ("_Cutoff", props);
        }
        public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties) {
            FindProperties (properties);
            m_MaterialEditor = materialEditor;
            Material material = materialEditor.target as Material;
            if (m_FirstTimeApply) {
                MaterialChanged (material);
                m_FirstTimeApply = false;
            }
            ShaderPropertiesGUI (material);
        }
        private void ShaderPropertiesGUI (Material material) {
            EditorGUI.BeginChangeCheck (); {
                BlendModePopup ();
                AKSUIHelper.ShurikenHeader ("Main");
                EditorGUILayout.Space ();
                EditorGUILayout.LabelField ("Textures", EditorStyles.boldLabel);
                m_MaterialEditor.TexturePropertyWithHDRColor (new GUIContent (mainTex.displayName), mainTex, mainColor, true);
                EditorGUILayout.Space ();
                EditorGUILayout.LabelField ("Overlay Target", EditorStyles.boldLabel);
                m_MaterialEditor.ShaderProperty (enableVR, enableVR.displayName);
                m_MaterialEditor.ShaderProperty (enableDesktop, enableDesktop.displayName);
                m_MaterialEditor.ShaderProperty (enableSS, enableSS.displayName);
                m_MaterialEditor.ShaderProperty (enableOther, enableOther.displayName);
                if (__alphaOption) {
                    EditorGUILayout.Space ();
                    EditorGUILayout.LabelField ("Alpha Option", EditorStyles.boldLabel);
                    m_MaterialEditor.ShaderProperty (cutoff, cutoff.displayName);
                }
                EditorGUILayout.Space ();
                EditorGUILayout.LabelField ("Rendering Option", EditorStyles.boldLabel);
                m_MaterialEditor.ShaderProperty (ztest, ztest.displayName);
                //m_MaterialEditor.ShaderProperty(zwrite, zwrite.displayName);
                //m_MaterialEditor.ShaderProperty(srcFactor, srcFactor.displayName);
                //m_MaterialEditor.ShaderProperty(dstFactor, dstFactor.displayName);
                m_MaterialEditor.RenderQueueField ();
            }
            if (EditorGUI.EndChangeCheck ()) {
                MaterialChanged (material);
            }
            AKSUIHelper.ShurikenHeader (AKSStyles.nameAKTextureOverlay);
            EditorGUILayout.LabelField (AKSStyles.author);
            AKSManager.DisplayVersion ();
            EditorGUILayout.Space ();
            EditorGUILayout.BeginHorizontal ();
            if (GUILayout.Button (AKSStyles.btnReadme)) { AKSUIHelper.OpenLink (AKSStyles.linkReadme); }
            if (GUILayout.Button (AKSStyles.btnDescription)) { AKSUIHelper.OpenLink (AKSStyles.linkDescription); }
            EditorGUILayout.EndHorizontal ();
        }
        public override void AssignNewShaderToMaterial (Material material, Shader oldShader, Shader newShader) {
            base.AssignNewShaderToMaterial (material, oldShader, newShader);
            MaterialChanged (material);
        }
        private void BlendModePopup () {
            EditorGUI.showMixedValue = blendMode.hasMixedValue;
            BlendMode mode = (BlendMode) blendMode.floatValue;

            EditorGUI.BeginChangeCheck ();
            mode = (BlendMode) EditorGUILayout.Popup (AKSStyles.renderingMode, (int) mode, blendNames);
            if (EditorGUI.EndChangeCheck ()) {
                m_MaterialEditor.RegisterPropertyChangeUndo ("Rendering Mode");
                blendMode.floatValue = (float) mode;
                foreach (UnityEngine.Object obj in blendMode.targets) {
                    MaterialChanged ((Material) obj);
                }
            }

            EditorGUI.showMixedValue = false;
        }
        private static void SetupMaterialWithBlendMode (Material material, BlendMode blendMode) {
            switch (blendMode) {
                case BlendMode.Opaque:
                    material.SetOverrideTag ("RenderType", "");
                    material.SetInt ("_SrcFactor", (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt ("_DstFactor", (int) UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt ("_AlphaToMask", 0);
                    material.SetInt ("_ZWrite", 1);
                    material.DisableKeyword ("_ALPHATEST_ON");
                    if (material.renderQueue == (int) UnityEngine.Rendering.RenderQueue.Geometry || material.renderQueue == (int) UnityEngine.Rendering.RenderQueue.AlphaTest || material.renderQueue == (int) UnityEngine.Rendering.RenderQueue.Transparent) {
                        material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Geometry;
                    }
                    break;
                case BlendMode.Cutout:
                    material.SetOverrideTag ("RenderType", "TransparentCutout");
                    material.SetInt ("_SrcFactor", (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt ("_DstFactor", (int) UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt ("_ZWrite", 1);
                    material.SetInt ("_AlphaToMask", 1);
                    material.EnableKeyword ("_ALPHATEST_ON");
                    if (material.renderQueue == (int) UnityEngine.Rendering.RenderQueue.Geometry || material.renderQueue == (int) UnityEngine.Rendering.RenderQueue.AlphaTest || material.renderQueue == (int) UnityEngine.Rendering.RenderQueue.Transparent) {
                        material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.AlphaTest;
                    }
                    break;
                case BlendMode.Transparent:
                    material.SetOverrideTag ("RenderType", "Transparent");
                    material.SetInt ("_SrcFactor", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt ("_DstFactor", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt ("_ZWrite", 0);
                    material.SetInt ("_AlphaToMask", 0);
                    material.DisableKeyword ("_ALPHATEST_ON");
                    if (material.renderQueue == (int) UnityEngine.Rendering.RenderQueue.Geometry || material.renderQueue == (int) UnityEngine.Rendering.RenderQueue.AlphaTest || material.renderQueue == (int) UnityEngine.Rendering.RenderQueue.Transparent) {
                        material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
                    }
                    break;
            }
        }
        private void MaterialChanged (Material material) {
            BlendMode mode = (BlendMode) material.GetFloat ("_BlendMode");
            __alphaOption = mode == BlendMode.Cutout;
            SetupMaterialWithBlendMode (material, mode);
        }
    }
}