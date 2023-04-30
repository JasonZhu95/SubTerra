namespace Game2DWaterKit
{
    using UnityEditor;
    using UnityEngine;

    public class Game2DWaterShaderGUI : Game2DWaterKitShaderGUI
    {
        private static readonly string[] _reflectionFadingOptions = new[] { "Linear", "Exponential - Two", "Exponential - Three", "Exponential - Four" };

        protected override void Initialize()
        {
            base.Initialize();

            foreach (UnityEngine.Material material in _materialEditor.targets)
            {
                float renderMode = material.GetFloat("_Mode");
                if(renderMode == 3000f)
                {
                    material.SetFloat("_Mode", 2000f);
                    material.SetFloat("_Water2D_IsRefractionEnabled", 0f);
                    material.SetFloat("_Water2D_IsReflectionEnabled", 0f);
                }
            }
        }

        protected override void DrawMaterialProperties()
        {
            MaterialProperty surfaceKeywordState = FindProperty("_Water2D_IsSurfaceEnabled", _materialProperties);
            MaterialProperty refractionKeywordState = FindProperty("_Water2D_IsRefractionEnabled", _materialProperties);
            MaterialProperty reflectionKeywordState = FindProperty("_Water2D_IsReflectionEnabled", _materialProperties);
            MaterialProperty emissionKeywordState = FindProperty("_Water2D_IsEmissionColorEnabled", _materialProperties);

            DrawPropertiesGroup("Body Properties", DrawBodyProperties);
            DrawPropertiesGroup("Surface Properties", DrawSurfaceProperties, surfaceKeywordState);
            DrawPropertiesGroup("Refraction Properties", DrawRefractionProperties, refractionKeywordState);
            DrawPropertiesGroup("Reflection Properties", DrawReflectionProperties, reflectionKeywordState);
            if (!_shader.name.Contains("Unlit"))
                DrawPropertiesGroup("Emission Properties", DrawEmissionColorProperties, emissionKeywordState);
        }

        private void DrawBodyProperties()
        {
            BeginPropertiesSubGroup("Color Properties");
            DrawColorProperties("_WaterColor");
            EndPropertiesSubGroup();

            BeginPropertiesSubGroup("Texture Properties");
            DrawTextureProperties("_WaterTexture", 3);
            EndPropertiesSubGroup();
        }

        private void DrawSurfaceProperties()
        {
            BeginPropertiesSubGroup();

            EditorGUI.BeginChangeCheck();

            MaterialProperty isFakePerspectiveKeywordStateProperty = FindProperty("_Water2D_IsFakePerspectiveEnabled", _materialProperties);
            MaterialProperty surfaceHasAbsoluteThicknessKeywordState = FindProperty("_Water2D_IsSurfaceHasAbsoluteThicknessEnabled", _materialProperties);

            MaterialProperty surfaceLevelProperty = FindProperty("_SurfaceLevel", _materialProperties);
            MaterialProperty submergeLevelProperty = FindProperty("_SubmergeLevel", _materialProperties);

            bool hasAbsoluteSurfaceThickness = surfaceHasAbsoluteThicknessKeywordState.floatValue != 0.0;

            float surfaceThickness, currentSubmergeLevel;

            if (!hasAbsoluteSurfaceThickness)
            {
                surfaceThickness = 1f - Mathf.Clamp01(surfaceLevelProperty.floatValue);
                currentSubmergeLevel = Mathf.InverseLerp(surfaceLevelProperty.floatValue, 1f, submergeLevelProperty.floatValue);
            }
            else
            {
                surfaceThickness = surfaceLevelProperty.floatValue;
                currentSubmergeLevel = Mathf.Clamp01(submergeLevelProperty.floatValue);
            }

            // Thickness Property
            EditorGUIUtility.labelWidth = 62f;
            EditorGUI.showMixedValue = surfaceLevelProperty.hasMixedValue;
            EditorGUI.BeginChangeCheck();
            var thicknessPropertyRect = EditorGUILayout.GetControlRect();
            var thicknessPropertyRectXMax = thicknessPropertyRect.xMax;
            thicknessPropertyRect.xMax -= 80f;
            if (!hasAbsoluteSurfaceThickness)
                surfaceThickness = EditorGUI.Slider(thicknessPropertyRect, "Thickness", surfaceThickness, 0.001f, 1f);
            else
                surfaceThickness = EditorGUI.FloatField(thicknessPropertyRect, "Thickness", surfaceThickness);
            if (EditorGUI.EndChangeCheck())
            {
                if (!hasAbsoluteSurfaceThickness)
                {
                    surfaceLevelProperty.floatValue = 1f - surfaceThickness;
                    submergeLevelProperty.floatValue = Mathf.Lerp(surfaceLevelProperty.floatValue, 1f, currentSubmergeLevel);
                }
                else surfaceLevelProperty.floatValue = surfaceThickness;
            }
            EditorGUI.showMixedValue = false;

            thicknessPropertyRect.xMin = thicknessPropertyRect.xMax + 2f;
            thicknessPropertyRect.xMax = thicknessPropertyRectXMax;

            EditorGUI.showMixedValue = surfaceHasAbsoluteThicknessKeywordState.hasMixedValue;
            EditorGUI.BeginChangeCheck();
            hasAbsoluteSurfaceThickness = EditorGUI.Popup(thicknessPropertyRect, hasAbsoluteSurfaceThickness ? 1 : 0, new string[] { "Relative", "Absolute" }) == 1 ? true : false;
            if (EditorGUI.EndChangeCheck())
            {
                surfaceHasAbsoluteThicknessKeywordState.floatValue = hasAbsoluteSurfaceThickness ? 1f : 0f;

                if (hasAbsoluteSurfaceThickness)
                {
                    // relative to absolute
                    surfaceLevelProperty.floatValue = surfaceThickness;
                    submergeLevelProperty.floatValue = currentSubmergeLevel;
                }
                else
                {
                    // absolute to relative
                    surfaceLevelProperty.floatValue = 1f - Mathf.Clamp(surfaceThickness, 0.001f, 1f);
                    submergeLevelProperty.floatValue = Mathf.Lerp(surfaceLevelProperty.floatValue, 1f, currentSubmergeLevel);
                }
            }
            EditorGUI.showMixedValue = false;

            // Submerge Level Property
            bool isRefractionEnabled = FindProperty("_Water2D_IsRefractionEnabled", _materialProperties).floatValue == 1f;
            var rect = EditorGUILayout.GetControlRect();
            float xmax = rect.xMax;
            rect.width = 14f;
            DrawShaderKeywordPropertyToggle(rect, isFakePerspectiveKeywordStateProperty, string.Empty, true, true);
            rect.xMax = xmax;
            rect.xMin += 14f;
            EditorGUI.BeginDisabledGroup(isFakePerspectiveKeywordStateProperty.floatValue == 0f);
            EditorGUI.showMixedValue = submergeLevelProperty.hasMixedValue;
            EditorGUIUtility.labelWidth = 106f;
            EditorGUI.BeginChangeCheck();
            currentSubmergeLevel = EditorGUI.Slider(rect, "Submerge Level", currentSubmergeLevel, 0.001f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                if (!hasAbsoluteSurfaceThickness)
                    submergeLevelProperty.floatValue = Mathf.Lerp(surfaceLevelProperty.floatValue, 1f, currentSubmergeLevel);
                else
                    submergeLevelProperty.floatValue = currentSubmergeLevel;
            }
            EditorGUI.showMixedValue = false;
            EditorGUI.EndDisabledGroup();

            if (isFakePerspectiveKeywordStateProperty.floatValue == 1f)
            {
                if (isRefractionEnabled)
                    EditorGUILayout.HelpBox("You can choose which object layers to render as partially submerged into water under the \"Refraction Properties\" and the \"Reflection Properties\" in the water component inspector.", MessageType.Info);
                else
                    EditorGUILayout.HelpBox("You can choose which object layers to render as partially submerged into water under the \"Fake Perspective Properties\" in the water component inspector.", MessageType.Info);
            }

            if (EditorGUI.EndChangeCheck() && _inspectedWaterKitObjectValidatePropertiesDelegate != null)
                _inspectedWaterKitObjectValidatePropertiesDelegate.Invoke();

            EndPropertiesSubGroup();

            BeginPropertiesSubGroup("Color Properties");

            EditorGUILayout.LabelField("Surface", EditorStyles.miniBoldLabel);
            DrawColorProperties("_SurfaceColor");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Outlines", EditorStyles.miniBoldLabel);
            DrawEdgeLineProperties("Top", "Top Edge");
            DrawEdgeLineProperties("SurfaceLevel", "Surface Level");
            if (isFakePerspectiveKeywordStateProperty.floatValue == 1f)
                DrawEdgeLineProperties("SubmergeLevel", "Submerge Level");

            EndPropertiesSubGroup();

            BeginPropertiesSubGroup("Texture Properties");
            DrawTextureProperties("_SurfaceTexture", 2);
            EndPropertiesSubGroup();
        }

        private void DrawEdgeLineProperties(string edgeName, string label)
        {
            var rect = EditorGUILayout.GetControlRect();
            var rectXMax = rect.xMax;

            var shaderKeywordEnabledProperty = FindProperty("_Water2D_Is" + edgeName + "EdgeLineEnabled", _materialProperties);
            var lineThicknessProperty = FindProperty("_" + edgeName + "EdgeLineThickness", _materialProperties);
            var lineColorProperty = FindProperty("_" + edgeName + "EdgeLineColor", _materialProperties);

            // toggle
            rect.xMax = rect.xMin + 114f;
            EditorGUI.BeginChangeCheck();
            EditorGUIUtility.labelWidth = 90f;
            var isEnabled = EditorGUI.ToggleLeft(rect, Game2DWaterKitInspector.Game2DWaterKitStyles.GetTempLabel(label), shaderKeywordEnabledProperty.floatValue == 1f);
            if (EditorGUI.EndChangeCheck())
                shaderKeywordEnabledProperty.floatValue = isEnabled ? 1f : 0f;

            // thickness
            rect.xMin = rect.xMax + 3f;
            rect.xMax = rectXMax - 65f;
            _materialEditor.ShaderProperty(rect, lineThicknessProperty, string.Empty);

            // color
            rect.xMin = rect.xMax + 3f;
            rect.xMax = rectXMax;
            EditorGUI.BeginChangeCheck();
            var color = EditorGUI.ColorField(rect, lineColorProperty.colorValue);
            if (EditorGUI.EndChangeCheck())
                lineColorProperty.colorValue = color;
        }

        private void DrawRefractionProperties()
        {
            BeginPropertiesSubGroup();
            EditorGUIUtility.labelWidth = 55f;
            MaterialProperty bendingAmountProperty = FindProperty("_RefractionAmountOfBending", _materialProperties);
            var rect = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(), TempContent("Bending"));
            _materialEditor.ShaderProperty(rect, bendingAmountProperty, GUIContent.none);
            EndPropertiesSubGroup();

            BeginPropertiesSubGroup("Distortion Effect");
            DrawDistortionEffectProperties("_Refraction", 0);
            EndPropertiesSubGroup();
        }

        private void DrawReflectionProperties()
        {
            BeginPropertiesSubGroup();

            EditorGUIUtility.labelWidth = 55f;

            MaterialProperty visibilityProperty = FindProperty("_ReflectionVisibility", _materialProperties);
            var visibilityPropertyRect = EditorGUILayout.GetControlRect();
            visibilityPropertyRect = EditorGUI.PrefixLabel(visibilityPropertyRect, TempContent("Visibility"));
            _materialEditor.ShaderProperty(visibilityPropertyRect, visibilityProperty, string.Empty);

            MaterialProperty fadingParametersProperty = FindProperty("_ReflectionFadingParameters", _materialProperties);
            var fadingParameters = fadingParametersProperty.vectorValue;

            var fadingPropertiesRect = EditorGUILayout.GetControlRect();
            float fadingPropertiesRectXmax = fadingPropertiesRect.xMax;

            EditorGUI.showMixedValue = fadingParametersProperty.hasMixedValue;
            EditorGUI.BeginChangeCheck();
            fadingPropertiesRect.xMax = fadingPropertiesRect.xMin + 55f;
            fadingParameters.x = EditorGUI.ToggleLeft(fadingPropertiesRect, "Fade", fadingParameters.x == 1f) ? 1f : 0f;
            fadingPropertiesRect.xMax = fadingPropertiesRectXmax;
            fadingPropertiesRect.xMin += 55f;
            EditorGUI.BeginDisabledGroup(fadingParameters.x == 0f);
            fadingParameters.y = EditorGUI.Popup(fadingPropertiesRect, (int)fadingParameters.y, _reflectionFadingOptions);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
                fadingParametersProperty.vectorValue = fadingParameters;
            EditorGUI.showMixedValue = false;

            EndPropertiesSubGroup();

            BeginPropertiesSubGroup("Distortion Effect");
            DrawDistortionEffectProperties("_Reflection", 1);
            EndPropertiesSubGroup();
        }

        private void DrawEmissionColorProperties()
        {
            MaterialProperty emissionColor = FindProperty("_WaterEmissionColor", _materialProperties);
            MaterialProperty emissionIntensity = FindProperty("_WaterEmissionColorIntensity", _materialProperties);

            _materialEditor.ShaderProperty(emissionColor, "Color");
            _materialEditor.ShaderProperty(emissionIntensity, "Intensity");
        }

        protected override void SetMaterialKeywords(UnityEngine.Material material)
        {
            bool applyTintOnTopOfTexture = material.GetFloat("_Water2D_IsApplyTintColorOnTopOfTextureEnabled") == 1f;
            SetKeywordState(material, "Water2D_ApplyTintColorBeforeTexture", !applyTintOnTopOfTexture);

            bool smoothLines = material.GetFloat("_Water2D_IsSmoothLinesEnabled") == 1f;
            SetKeywordState(material, "Water2D_SmoothLines", smoothLines);

            //Water Body Keywords
            bool hasWaterBodyTexture = material.GetTexture("_WaterTexture") != null;
            bool isWaterBodyTextureSheetEnabled = material.GetFloat("_Water2D_IsWaterTextureSheetEnabled") == 1.0f;
            bool isWaterBodyTextureSheetWithLerpEnabled = material.GetFloat("_Water2D_IsWaterTextureSheetWithLerpEnabled") == 1.0;
            Vector4 waterBodyTextureTilingParameters = material.GetVector("_WaterTextureTilingParameters");
            bool isWaterBodyTextureTilingModeSetToStretch = waterBodyTextureTilingParameters.x == 1f;
            bool isWaterBodyTextureStretchTilingModeKeepAspect = waterBodyTextureTilingParameters.y == 1f;
            bool isWaterBodyTextureStretchTilingModeAutoX = waterBodyTextureTilingParameters.z == 1f;
            bool isWaterBodyTextureScrollingEnabled = material.GetFloat("_WaterTextureScrollingSpeedX") != 0f || material.GetFloat("_WaterTextureScrollingSpeedY") != 0f;

            SetKeywordState(material, "Water2D_WaterTexture", hasWaterBodyTexture && !isWaterBodyTextureSheetEnabled);
            SetKeywordState(material, "Water2D_WaterTextureSheet", hasWaterBodyTexture && isWaterBodyTextureSheetEnabled && !isWaterBodyTextureSheetWithLerpEnabled);
            SetKeywordState(material, "Water2D_WaterTextureSheetWithLerp", hasWaterBodyTexture && isWaterBodyTextureSheetEnabled && isWaterBodyTextureSheetWithLerpEnabled);
            SetKeywordState(material, "Water2D_WaterNoise", hasWaterBodyTexture && material.GetFloat("_Water2D_IsWaterNoiseEnabled") == 1.0f);
            SetKeywordState(material, "Water2D_WaterTextureStretch", hasWaterBodyTexture && isWaterBodyTextureTilingModeSetToStretch && !isWaterBodyTextureStretchTilingModeKeepAspect);
            SetKeywordState(material, "Water2D_WaterTextureStretchAutoX", hasWaterBodyTexture && isWaterBodyTextureTilingModeSetToStretch && isWaterBodyTextureStretchTilingModeKeepAspect && isWaterBodyTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Water2D_WaterTextureStretchAutoY", hasWaterBodyTexture && isWaterBodyTextureTilingModeSetToStretch && isWaterBodyTextureStretchTilingModeKeepAspect && !isWaterBodyTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Water2D_ColorGradient", material.GetFloat("_Water2D_IsColorGradientEnabled") == 1.0f);
            SetKeywordState(material, "Water2D_WaterTextureScroll", hasWaterBodyTexture && isWaterBodyTextureScrollingEnabled);

            //Refraction & Reflection Keywords
            bool isReflectionEnabled = material.GetFloat("_Water2D_IsReflectionEnabled") == 1.0f;
            bool isRefractionEnabled = material.GetFloat("_Water2D_IsRefractionEnabled") == 1.0f;
            Vector4 reflectionFadingParameters = material.GetVector("_ReflectionFadingParameters");
            bool isReflectionFadingEnabled = reflectionFadingParameters.x == 1f;
            SetKeywordState(material, "Water2D_Refraction", isRefractionEnabled);
            SetKeywordState(material, "Water2D_Reflection", isReflectionEnabled);
            SetKeywordState(material, "Water2D_ReflectionFadeLinear", isReflectionEnabled && isReflectionFadingEnabled && reflectionFadingParameters.y == 0f);
            SetKeywordState(material, "Water2D_ReflectionFadeExponentialTwo", isReflectionEnabled && isReflectionFadingEnabled && reflectionFadingParameters.y == 1f);
            SetKeywordState(material, "Water2D_ReflectionFadeExponentialThree", isReflectionEnabled && isReflectionFadingEnabled && reflectionFadingParameters.y == 2f);
            SetKeywordState(material, "Water2D_ReflectionFadeExponentialFour", isReflectionEnabled && isReflectionFadingEnabled && reflectionFadingParameters.y == 3f);

            //Water Surface Keywords
            bool isSurfaceEnabled = material.GetFloat("_Water2D_IsSurfaceEnabled") == 1.0f;
            bool isSurfaceHasAbsoluteThicknessEnabled = material.GetFloat("_Water2D_IsSurfaceHasAbsoluteThicknessEnabled") == 1.0f;
            bool hasSurfaceTexture = material.GetTexture("_SurfaceTexture") != null;
            bool isSurfaceTextureSheetEnabled = material.GetFloat("_Water2D_IsWaterSurfaceTextureSheetEnabled") == 1.0f;
            bool isSurfaceTextureSheetWithLerpEnbaled = material.GetFloat("_Water2D_IsWaterSurfaceTextureSheetWithLerpEnabled") == 1.0f;
            Vector4 waterSurfaceTextureTilingParameters = material.GetVector("_SurfaceTextureTilingParameters");
            bool isSurfaceTextureTilingModeSetToStretch = waterSurfaceTextureTilingParameters.x == 1f;
            bool isSurfaceTextureStretchTilingModeKeepAspect = waterSurfaceTextureTilingParameters.y == 1f;
            bool isSurfaceTextureStretchTilingModeAutoX = waterSurfaceTextureTilingParameters.z == 1f;
            bool isSurfaceTextureScrollingEnabled = material.GetFloat("_SurfaceTextureScrollingSpeedX") != 0f || material.GetFloat("_SurfaceTextureScrollingSpeedY") != 0f;
            SetKeywordState(material, "Water2D_Surface", isSurfaceEnabled);
            SetKeywordState(material, "Water2D_SurfaceHasAbsoluteThickness", isSurfaceEnabled && isSurfaceHasAbsoluteThicknessEnabled);
            SetKeywordState(material, "Water2D_SurfaceTexture", isSurfaceEnabled && hasSurfaceTexture && !isSurfaceTextureSheetEnabled);
            SetKeywordState(material, "Water2D_SurfaceTextureSheet", isSurfaceEnabled && hasSurfaceTexture && isSurfaceTextureSheetEnabled && !isSurfaceTextureSheetWithLerpEnbaled);
            SetKeywordState(material, "Water2D_SurfaceTextureSheetWithLerp", isSurfaceEnabled && hasSurfaceTexture && isSurfaceTextureSheetEnabled && isSurfaceTextureSheetWithLerpEnbaled);
            SetKeywordState(material, "Water2D_SurfaceNoise", isSurfaceEnabled && hasSurfaceTexture && material.GetFloat("_Water2D_IsSurfaceNoiseEnabled") == 1.0f);
            SetKeywordState(material, "Water2D_SurfaceTextureStretch", isSurfaceEnabled && hasSurfaceTexture && isSurfaceTextureTilingModeSetToStretch && !isSurfaceTextureStretchTilingModeKeepAspect);
            SetKeywordState(material, "Water2D_SurfaceTextureStretchAutoX", isSurfaceEnabled && hasSurfaceTexture && isSurfaceTextureTilingModeSetToStretch && isSurfaceTextureStretchTilingModeKeepAspect && isSurfaceTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Water2D_SurfaceTextureStretchAutoY", isSurfaceEnabled && hasSurfaceTexture && isSurfaceTextureTilingModeSetToStretch && isSurfaceTextureStretchTilingModeKeepAspect && !isSurfaceTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Water2D_SurfaceColorGradient", isSurfaceEnabled && material.GetFloat("_Water2D_IsSurfaceColorGradientEnabled") == 1.0f);
            SetKeywordState(material, "Water2D_SurfaceTextureScroll", isSurfaceEnabled && hasSurfaceTexture && isSurfaceTextureScrollingEnabled);

            //Water Fake Perspective
            bool isFakePerspectiveEnabled = isSurfaceEnabled && (material.GetFloat("_Water2D_IsFakePerspectiveEnabled") == 1.0f);
            SetKeywordState(material, "Water2D_FakePerspective", isFakePerspectiveEnabled);

            //Outlines Keywords
            bool isTopEdgeLineEnabled = material.GetFloat("_Water2D_IsTopEdgeLineEnabled") == 1f;
            bool isSurfaceLevelEdgeLineEnabled = material.GetFloat("_Water2D_IsSurfaceLevelEdgeLineEnabled") == 1f;
            bool isSubmergeLevelEdgeLineEnabled = material.GetFloat("_Water2D_IsSubmergeLevelEdgeLineEnabled") == 1f;
            SetKeywordState(material, "Water2D_TopEdgeLine", isSurfaceEnabled && isTopEdgeLineEnabled);
            SetKeywordState(material, "Water2D_SurfaceLevelEdgeLine", isSurfaceEnabled && isSurfaceLevelEdgeLineEnabled);
            SetKeywordState(material, "Water2D_SubmergeLevelEdgeLine", isSurfaceEnabled && isFakePerspectiveEnabled && isSubmergeLevelEdgeLineEnabled);

            //Lighting keywords
            SetKeywordState(material, "Water2D_ApplyEmissionColor", material.GetFloat("_Water2D_IsEmissionColorEnabled") == 1.0f);
        }

        private enum RenderingMode
        {
            Opaque = 2000,
            Transparent = 3000
        }
    }
}
