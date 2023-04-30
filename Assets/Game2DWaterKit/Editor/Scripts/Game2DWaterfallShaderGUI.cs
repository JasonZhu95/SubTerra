namespace Game2DWaterKit
{
    using UnityEngine;
    using UnityEditor;

    public class Game2DWaterfallShaderGUI : Game2DWaterKitShaderGUI
    {
        private static string[] _absoluteRelativePopupLabels = new[] { "Relative", "Absolute" };

        protected override void DrawMaterialProperties()
        {
            MaterialProperty topBottomEdgesKeywordState = FindProperty("_Waterfall2D_IsTopBottomEdgesEnabled", _materialProperties);
            MaterialProperty leftRightEdgesKeywordState = FindProperty("_Waterfall2D_IsLeftRightEdgesEnabled", _materialProperties);
            MaterialProperty refractionKeywordState = FindProperty("_Waterfall2D_IsRefractionEnabled", _materialProperties);
            MaterialProperty emissionKeywordState = FindProperty("_Waterfall2D_IsEmissionColorEnabled", _materialProperties);

            DrawPropertiesGroup("Body Properties", DrawBodyProperties);
            DrawPropertiesGroup("Left-Right Edges Properties", () => DrawEdgesProperties("Left","Right", 1), leftRightEdgesKeywordState);
            DrawPropertiesGroup("Top-Bottom Edges Properties", () => DrawEdgesProperties("Top", "Bottom", 2), topBottomEdgesKeywordState);
            DrawPropertiesGroup("Refraction Properties", DrawRefractionProperties, refractionKeywordState);
            if (!_shader.name.Contains("Unlit"))
                DrawPropertiesGroup("Emission Properties", DrawEmissionColorProperties, emissionKeywordState);
        }

        private void DrawBodyProperties()
        {
            BeginPropertiesSubGroup("Color Properties");
            DrawColorProperties("_BodyColor");
            EndPropertiesSubGroup();

            BeginPropertiesSubGroup("Texture Properties");

            DrawAlphaCutoffToggleWithSlider("Body");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Main Texture", EditorStyles.miniBoldLabel);
            DrawTextureProperties("_BodyTexture", -1);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Secondary Texture", EditorStyles.miniBoldLabel);
            DrawTextureProperties("_BodySecondTexture", -1);

            bool areDistortionEffectPropertiesDisabled = !(FindProperty("_BodyTexture", _materialProperties).textureValue != null || FindProperty("_BodySecondTexture", _materialProperties).textureValue != null);

            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(areDistortionEffectPropertiesDisabled);
            DrawDistortionEffectProperties("_BodyTexture", 3);
            EditorGUI.EndDisabledGroup();

            EndPropertiesSubGroup();
        }

        private void DrawEdgesProperties(string firstEdgeName, string secondEdgeName, int noiseTextureChannelIndex)
        {
            bool isDrawingLeftRightEdgesProperties = firstEdgeName == "Left";

            BeginPropertiesSubGroup();

            MaterialProperty firstEdgeKeywordState = FindProperty("_Waterfall2D_Is" + firstEdgeName + "EdgeEnabled", _materialProperties);
            MaterialProperty secondEdgeKeywordState = FindProperty("_Waterfall2D_Is" + secondEdgeName + "EdgeEnabled", _materialProperties);
            MaterialProperty useAbsoluteValuesKeywordState = FindProperty("_Waterfall2D_Is" + firstEdgeName + secondEdgeName + "EdgesAbsoluteThicknessAndOffsetEnabled", _materialProperties);
            MaterialProperty edgesThicknessProperty = FindProperty("_" + firstEdgeName + secondEdgeName + "EdgesThickness", _materialProperties);
            MaterialProperty edgesOffsetProperty = FindProperty("_" + firstEdgeName + secondEdgeName + "EdgesOffset", _materialProperties);

            DrawShaderKeywordPropertyToggle(EditorGUILayout.GetControlRect(), firstEdgeKeywordState, firstEdgeName + " Edge", true, false);
            DrawShaderKeywordPropertyToggle(EditorGUILayout.GetControlRect(), secondEdgeKeywordState, secondEdgeName + " Edge", true, false);
            EditorGUILayout.Space();
            DrawEdgesAbsoluteRelativeValuesPopup(useAbsoluteValuesKeywordState, edgesThicknessProperty, edgesOffsetProperty);

            bool isFirstEdgeEnabled = firstEdgeKeywordState.floatValue == 1f;
            bool isSecondEdgeEnabled = secondEdgeKeywordState.floatValue == 1f;
            bool useAbsoluteValues = useAbsoluteValuesKeywordState.floatValue == 1f;
            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(!isFirstEdgeEnabled);
            EditorGUILayout.LabelField(firstEdgeName + " Edge", EditorStyles.miniBoldLabel);
            DrawEdgeThicknessProperties(firstEdgeName, edgesThicknessProperty, useAbsoluteValues);
            DrawEdgeOffsetProperties(firstEdgeName, edgesOffsetProperty, useAbsoluteValues);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(!isSecondEdgeEnabled);
            EditorGUILayout.LabelField(secondEdgeName + " Edge", EditorStyles.miniBoldLabel);
            DrawEdgeThicknessProperties(secondEdgeName, edgesThicknessProperty, useAbsoluteValues);
            DrawEdgeOffsetProperties(secondEdgeName, edgesOffsetProperty, useAbsoluteValues);
            EditorGUI.EndDisabledGroup();

            EndPropertiesSubGroup();

            bool useSameTexture = false;

            if (isFirstEdgeEnabled || isSecondEdgeEnabled)
            {
                BeginPropertiesSubGroup("Texture Properties");

                if (isFirstEdgeEnabled && isSecondEdgeEnabled)
                {
                    MaterialProperty useSameTextureKeywordState = FindProperty("_Waterfall2D_Is" + firstEdgeName + secondEdgeName + "EdgesUseSameTextureEnabled", _materialProperties);
                    DrawShaderKeywordPropertyToggle(EditorGUILayout.GetControlRect(), useSameTextureKeywordState, "Use Same Texture", true);
                    useSameTexture = useSameTextureKeywordState.floatValue == 1f;

                    MaterialProperty edgesTextureFlippingParametersProperty = FindProperty("_" + firstEdgeName + secondEdgeName + "EdgesTextureFlipParameters", _materialProperties);
                    var edgesTextureFlippingParameters = edgesTextureFlippingParametersProperty.vectorValue;
                    var textureFlippingPropertiesRect = EditorGUILayout.GetControlRect();
                    float textureFlippingPropertiesRectWidth = textureFlippingPropertiesRect.width;
                    float textureFlippingPropertiesRectXmax = textureFlippingPropertiesRect.xMax;

                    EditorGUI.showMixedValue = edgesTextureFlippingParametersProperty.hasMixedValue;
                    EditorGUI.BeginDisabledGroup(!useSameTexture);

                    EditorGUI.BeginChangeCheck();

                    textureFlippingPropertiesRect.xMax = textureFlippingPropertiesRect.xMin + 90f;
                    edgesTextureFlippingParameters.x = EditorGUI.ToggleLeft(textureFlippingPropertiesRect, "Flip Texture:", useSameTexture ? edgesTextureFlippingParameters.x == 1f : false) ? 1f : 0f;
                    bool isTextureFlippingEnabled = edgesTextureFlippingParameters.x == 1f;

                    textureFlippingPropertiesRect.xMax = textureFlippingPropertiesRectXmax;
                    textureFlippingPropertiesRect.xMin += 90f;

                    EditorGUI.BeginDisabledGroup(!isTextureFlippingEnabled);

                    if(textureFlippingPropertiesRectWidth > 242f)
                    {
                        textureFlippingPropertiesRect.width = 71f;
                        EditorGUIUtility.labelWidth = 57f;
                        edgesTextureFlippingParameters.y = EditorGUI.Toggle(textureFlippingPropertiesRect, firstEdgeName + " Edge", isTextureFlippingEnabled ? edgesTextureFlippingParameters.y == 1f : false) ? 1f : 0f;

                        textureFlippingPropertiesRect.x += 74f;
                        textureFlippingPropertiesRect.width = 89f;
                        EditorGUIUtility.labelWidth = 75f;
                        edgesTextureFlippingParameters.y = EditorGUI.Toggle(textureFlippingPropertiesRect, secondEdgeName + " Edge", isTextureFlippingEnabled ? edgesTextureFlippingParameters.y == 0f : false) ? 0f : 1f;
                    }
                    else
                    {
                        textureFlippingPropertiesRect.width = 41f;
                        EditorGUIUtility.labelWidth = 27f;
                        edgesTextureFlippingParameters.y = EditorGUI.Toggle(textureFlippingPropertiesRect, firstEdgeName, isTextureFlippingEnabled ? edgesTextureFlippingParameters.y == 1f : false) ? 1f : 0f;

                        textureFlippingPropertiesRect.x += 41f;
                        textureFlippingPropertiesRect.width = 55f;
                        EditorGUIUtility.labelWidth = 41f;
                        edgesTextureFlippingParameters.y = EditorGUI.Toggle(textureFlippingPropertiesRect, secondEdgeName, isTextureFlippingEnabled ? edgesTextureFlippingParameters.y == 0f : false) ? 0f : 1f;
                    }
                    EditorGUI.EndDisabledGroup();

                    if (EditorGUI.EndChangeCheck())
                        edgesTextureFlippingParametersProperty.vectorValue = edgesTextureFlippingParameters;

                    EditorGUI.EndDisabledGroup();
                    EditorGUI.showMixedValue = false;

                    if (!isDrawingLeftRightEdgesProperties)
                        EditorGUILayout.Space();
                }


                if (isDrawingLeftRightEdgesProperties)
                {
                    DrawAlphaCutoffToggleWithSlider(firstEdgeName + secondEdgeName + "Edges");
                    EditorGUILayout.Space();
                }

                if (!useSameTexture)
                {
                    if (isFirstEdgeEnabled)
                    {
                        EditorGUILayout.LabelField(firstEdgeName + " Edge Texture", EditorStyles.miniBoldLabel);
                        DrawTextureProperties("_" + firstEdgeName + "EdgeTexture", -1);
                    }

                    if (isSecondEdgeEnabled)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField(secondEdgeName + " Edge Texture", EditorStyles.miniBoldLabel);
                        DrawTextureProperties("_" + secondEdgeName + "EdgeTexture", -1);
                    }

                    bool areDistortionEffectPropertiesDisabled = true;

                    if (isFirstEdgeEnabled && FindProperty("_" + firstEdgeName + "EdgeTexture", _materialProperties).textureValue != null)
                        areDistortionEffectPropertiesDisabled = false;

                    if (areDistortionEffectPropertiesDisabled && isSecondEdgeEnabled && FindProperty("_" + secondEdgeName + "EdgeTexture", _materialProperties).textureValue != null)
                        areDistortionEffectPropertiesDisabled = false;

                    EditorGUI.BeginDisabledGroup(areDistortionEffectPropertiesDisabled);
                    DrawDistortionEffectProperties("_" + firstEdgeName + secondEdgeName + "Edges", noiseTextureChannelIndex);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    EditorGUILayout.LabelField(firstEdgeName + "-" + secondEdgeName + " Edges Texture", EditorStyles.miniBoldLabel);
                    DrawTextureProperties("_" + firstEdgeName + secondEdgeName + "EdgesTexture", noiseTextureChannelIndex);
                }

                EndPropertiesSubGroup();
            }
        }

        private void DrawAlphaCutoffToggleWithSlider(string name)
        {
            var alphaCutoffKeywordState = FindProperty("_Waterfall2D_Is" + name + "TextureAlphaCutoffEnabled", _materialProperties);

            var rect = EditorGUILayout.GetControlRect();
            float rectXmax = rect.xMax;

            rect.xMax = rect.xMin + 95f;
            DrawShaderKeywordPropertyToggle(rect, alphaCutoffKeywordState, "Alpha Cutoff", true);

            rect.xMax = rectXmax;
            rect.xMin += 95f;

            EditorGUI.BeginDisabledGroup(alphaCutoffKeywordState.floatValue == 0f);
            _materialEditor.RangeProperty(rect, FindProperty("_" + name + "TextureAlphaCutoff", _materialProperties), string.Empty);
            EditorGUI.EndDisabledGroup();
        }

        private void DrawEdgeThicknessProperties(string edgeName, MaterialProperty edgesThicknessProperty, bool useAbsoluteValues)
        {
            var edgesThickness = edgesThicknessProperty.vectorValue;

            float currentEdgeThickness;

            if (edgeName == "Top" || edgeName == "Left")
                currentEdgeThickness = edgesThickness.x;
            else
                currentEdgeThickness = edgesThickness.z;

            var rect = EditorGUILayout.GetControlRect();
            float rectXmax = rect.xMax;

            EditorGUI.showMixedValue = edgesThicknessProperty.hasMixedValue;
            EditorGUI.BeginChangeCheck();

            EditorGUIUtility.labelWidth = 65f;

            if (useAbsoluteValues)
                currentEdgeThickness = EditorGUI.FloatField(rect, "Thickness", currentEdgeThickness);
            else
                currentEdgeThickness = EditorGUI.Slider(rect, "Thickness", currentEdgeThickness, 0f, 1f);

            if (EditorGUI.EndChangeCheck())
            {
                if (edgeName == "Top" || edgeName == "Left")
                {
                    edgesThickness.x = currentEdgeThickness;
                    edgesThickness.y = 1f / currentEdgeThickness;
                }
                else
                {
                    edgesThickness.z = currentEdgeThickness;
                    edgesThickness.w = 1f / currentEdgeThickness;
                }

                edgesThicknessProperty.vectorValue = edgesThickness;
            }
            EditorGUI.showMixedValue = false;
        }

        private void DrawEdgeOffsetProperties(string edgeName, MaterialProperty edgesOffsetProperty, bool useAbsoluteValues)
        {
            var edgesOffset = edgesOffsetProperty.vectorValue;

            float currentEdgeOffset;

            if (edgeName == "Top" || edgeName == "Left")
                currentEdgeOffset = edgesOffset.x;
            else
                currentEdgeOffset = edgesOffset.z;

            var rect = EditorGUILayout.GetControlRect();
            float rectXmax = rect.xMax;

            EditorGUI.showMixedValue = edgesOffsetProperty.hasMixedValue;
            EditorGUI.BeginChangeCheck();

            EditorGUIUtility.labelWidth = 65f;

            bool isBottomOrLeftEdge = edgeName == "Bottom" || edgeName == "Left";

            if (useAbsoluteValues)
                currentEdgeOffset = EditorGUI.FloatField(rect, "Offset", isBottomOrLeftEdge ? currentEdgeOffset : -currentEdgeOffset);
            else
                currentEdgeOffset = EditorGUI.Slider(rect, "Offset", isBottomOrLeftEdge ? currentEdgeOffset : -currentEdgeOffset, isBottomOrLeftEdge ? 0f : -1f, isBottomOrLeftEdge ? 1f : 0f);

            if (EditorGUI.EndChangeCheck())
            {
                if (!isBottomOrLeftEdge)
                    currentEdgeOffset *= -1;

                if (edgeName == "Top" || edgeName == "Left")
                {
                    edgesOffset.x = currentEdgeOffset;
                    edgesOffset.y = 1f / currentEdgeOffset;
                }
                else
                {
                    edgesOffset.z = currentEdgeOffset;
                    edgesOffset.w = 1f / currentEdgeOffset;
                }

                edgesOffsetProperty.vectorValue = edgesOffset;
            }
            EditorGUI.showMixedValue = false;
        }

        private void DrawEdgesAbsoluteRelativeValuesPopup(MaterialProperty useAbsoluteValuesKeywordState, MaterialProperty edgesThicknessProperty, MaterialProperty edgesOffsetProperty)
        {
            EditorGUI.BeginChangeCheck();
            bool useAbsoluteValues = EditorGUILayout.Popup((int)useAbsoluteValuesKeywordState.floatValue, _absoluteRelativePopupLabels) == 1;
            if (EditorGUI.EndChangeCheck())
            {
                useAbsoluteValuesKeywordState.floatValue = useAbsoluteValues ? 1f : 0f;

                Vector4 thickness = edgesThicknessProperty.vectorValue;
                Vector4 offset = edgesOffsetProperty.vectorValue;

                if (useAbsoluteValues) // relative to absolute
                {
                    thickness = Vector4.one;
                    offset = Vector4.zero;
                }
                else // absolute to relative
                {
                    thickness = new Vector4(0.2f, 5f, 0.2f, 5f);
                    offset = Vector4.zero;
                }

                edgesThicknessProperty.vectorValue = thickness;
                edgesOffsetProperty.vectorValue = offset;
            }
        }

        private void DrawRefractionProperties()
        {
            BeginPropertiesSubGroup("Distortion Effect");
            DrawDistortionEffectProperties("_Refraction", 0);
            EndPropertiesSubGroup();
        }

        private void DrawEmissionColorProperties()
        {
            MaterialProperty emissionColor = FindProperty("_EmissionColor", _materialProperties);
            MaterialProperty emissionIntensity = FindProperty("_EmissionColorIntensity", _materialProperties);

            _materialEditor.ShaderProperty(emissionColor, "Color");
            _materialEditor.ShaderProperty(emissionIntensity, "Intensity");
        }

        protected override void SetMaterialKeywords(UnityEngine.Material material)
        {
            bool applyTintOnTopOfTexture = material.GetFloat("_Waterfall2D_IsApplyTintColorOnTopOfTextureEnabled") == 1f;
            SetKeywordState(material, "Waterfall2D_ApplyTintColorBeforeTexture", !applyTintOnTopOfTexture);

            // Body Keywords
            bool hasBodyTexture = material.GetTexture("_BodyTexture") != null;
            bool isBodyTextureAlphaCutoffEnabled = material.GetInt("_Waterfall2D_IsBodyTextureAlphaCutoffEnabled") == 1;
            bool isBodyTextureSheetEnabled = material.GetInt("_Waterfall2D_IsBodyTextureSheetEnabled") == 1;
            bool isBodyTextureSheetWithLerpEnabled = material.GetInt("_Waterfall2D_IsBodyTextureSheetWithLerpEnabled") == 1;
            Vector4 bodyTextureTilingModeParameters = material.GetVector("_BodyTextureTilingParameters");
            bool isBodyTextureTilingModeSetToStretch = bodyTextureTilingModeParameters.x == 1f;
            bool isBodyTextureStretchTilingModeKeepAspect = bodyTextureTilingModeParameters.y == 1f;
            bool isBodyTextureStretchTilingModeAutoX = bodyTextureTilingModeParameters.z == 1f;
            bool hasBodySecondTexture = material.GetTexture("_BodySecondTexture") != null;
            bool isBodySecondTextureSheetEnabled = material.GetInt("_Waterfall2D_IsBodySecondTextureSheetEnabled") == 1;
            bool isBodySecondTextureSheetWithLerpEnabled = material.GetInt("_Waterfall2D_IsBodySecondTextureSheetWithLerpEnabled") == 1;
            Vector4 bodySecondTextureTilingModeParameters = material.GetVector("_BodySecondTextureTilingParameters");
            bool isBodySecondTextureTilingModeSetToStretch = bodySecondTextureTilingModeParameters.x == 1f;
            bool isBodySecondTextureStretchTilingModeKeepAspect = bodySecondTextureTilingModeParameters.y == 1f;
            bool isBodySecondTextureStretchTilingModeAutoX = bodySecondTextureTilingModeParameters.z == 1f;

            SetKeywordState(material, "Waterfall2D_BodyTexture", hasBodyTexture && !isBodyTextureSheetEnabled && !(isBodyTextureSheetEnabled && isBodyTextureSheetWithLerpEnabled));
            SetKeywordState(material, "Waterfall2D_BodyTextureSheet", hasBodyTexture && isBodyTextureSheetEnabled & !isBodyTextureSheetWithLerpEnabled);
            SetKeywordState(material, "Waterfall2D_BodyTextureSheetWithLerp", hasBodyTexture && isBodyTextureSheetEnabled & isBodyTextureSheetWithLerpEnabled);
            SetKeywordState(material, "Waterfall2D_BodyTextureStretch", hasBodyTexture && isBodyTextureTilingModeSetToStretch && !isBodyTextureStretchTilingModeKeepAspect);
            SetKeywordState(material, "Waterfall2D_BodyTextureStretchAutoX", hasBodyTexture && isBodyTextureTilingModeSetToStretch && isBodyTextureStretchTilingModeKeepAspect && isBodyTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Waterfall2D_BodyTextureStretchAutoY", hasBodyTexture && isBodyTextureTilingModeSetToStretch && isBodyTextureStretchTilingModeKeepAspect && !isBodyTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Waterfall2D_BodySecondTexture", hasBodySecondTexture && !isBodySecondTextureSheetEnabled && !(isBodySecondTextureSheetEnabled && isBodySecondTextureSheetWithLerpEnabled));
            SetKeywordState(material, "Waterfall2D_BodySecondTextureSheet", hasBodySecondTexture && isBodySecondTextureSheetEnabled & !isBodySecondTextureSheetWithLerpEnabled);
            SetKeywordState(material, "Waterfall2D_BodySecondTextureSheetWithLerp", hasBodySecondTexture && isBodySecondTextureSheetEnabled & isBodySecondTextureSheetWithLerpEnabled);
            SetKeywordState(material, "Waterfall2D_BodySecondTextureStretch", hasBodySecondTexture && isBodySecondTextureTilingModeSetToStretch && !isBodySecondTextureStretchTilingModeKeepAspect);
            SetKeywordState(material, "Waterfall2D_BodySecondTextureStretchAutoX", hasBodySecondTexture && isBodySecondTextureTilingModeSetToStretch && isBodySecondTextureStretchTilingModeKeepAspect && isBodySecondTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Waterfall2D_BodySecondTextureStretchAutoY", hasBodySecondTexture && isBodySecondTextureTilingModeSetToStretch && isBodySecondTextureStretchTilingModeKeepAspect && !isBodySecondTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Waterfall2D_BodyColorGradient", material.GetInt("_Waterfall2D_IsColorGradientEnabled") == 1);
            SetKeywordState(material, "Waterfall2D_BodyTextureNoise", hasBodyTexture && (material.GetInt("_Waterfall2D_IsBodyNoiseEnabled") == 1));
            SetKeywordState(material, "Waterfall2D_BodyTextureAlphaCutoff", (hasBodyTexture || hasBodySecondTexture) && isBodyTextureAlphaCutoffEnabled);

            // Top-Bottom / Left-Right Edges
            SetEdgesKeywords(material, "Top", "Bottom");
            SetEdgesKeywords(material, "Left", "Right");

            // Refraction
            bool isRefractionEnabled = material.GetInt("_Waterfall2D_IsRefractionEnabled") == 1;
            SetKeywordState(material, "Waterfall2D_Refraction", isRefractionEnabled);

            // Emission
            bool isEmissionEnabled = material.GetInt("_Waterfall2D_IsEmissionColorEnabled") == 1;
            SetKeywordState(material, "Waterfall2D_ApplyEmissionColor", isEmissionEnabled);
        }

        private void SetEdgesKeywords(UnityEngine.Material material, string firstEdgeName, string secondEdgeName)
        {
            bool isSettingTopBottomEdgesKeywords = firstEdgeName == "Top";

            bool areFirstSecondEdgesEnabled = material.GetInt("_Waterfall2D_Is" + firstEdgeName + secondEdgeName + "EdgesEnabled") == 1;
            bool isFirstEdgeEnabled = material.GetInt("_Waterfall2D_Is" + firstEdgeName + "EdgeEnabled") == 1;
            bool isSecondEdgeEnabled = material.GetInt("_Waterfall2D_Is" + secondEdgeName + "EdgeEnabled") == 1;
            bool areFirstSecondEdgesUsingSameTexture = material.GetInt("_Waterfall2D_Is" + firstEdgeName + secondEdgeName + "EdgesUseSameTextureEnabled") == 1;
            if (!areFirstSecondEdgesUsingSameTexture)
            {
                isFirstEdgeEnabled &= material.GetTexture("_" + firstEdgeName + "EdgeTexture") != null;
                isSecondEdgeEnabled &= material.GetTexture("_" + secondEdgeName + "EdgeTexture") != null;
            }
            else areFirstSecondEdgesEnabled &= material.GetTexture("_" + firstEdgeName + secondEdgeName + "EdgesTexture") != null;
            bool isFirstSecondEdgesDistortionEffectEnabled = material.GetInt("_Waterfall2D_Is" + firstEdgeName + secondEdgeName + "EdgesNoiseEnabled") == 1;
            bool isFirstEdgeTextureSheetEnabled = material.GetInt("_Waterfall2D_Is" + firstEdgeName + "EdgeTextureSheetEnabled") == 1;
            bool isSecondEdgeTextureSheetEnabled = material.GetInt("_Waterfall2D_Is" + secondEdgeName + "EdgeTextureSheetEnabled") == 1;
            bool isFirstSecondEdgesTextureSheetEnabled = material.GetInt("_Waterfall2D_Is" + firstEdgeName + secondEdgeName + "EdgesTextureSheetEnabled") == 1;
            bool isFirstEdgeTextureSheetLerpEnabled = material.GetInt("_Waterfall2D_Is" + firstEdgeName + "EdgeTextureSheetWithLerpEnabled") == 1;
            bool isSecondEdgeTextureSheetLerpEnabled = material.GetInt("_Waterfall2D_Is" + secondEdgeName + "EdgeTextureSheetWithLerpEnabled") == 1;
            bool isFirstSecondEdgesTextureSheetLerpEnabled = material.GetInt("_Waterfall2D_Is" + firstEdgeName + secondEdgeName + "EdgesTextureSheetWithLerpEnabled") == 1;
            Vector4 firstEdgeTextureTilingModeParameters = material.GetVector("_" + firstEdgeName + "EdgeTextureTilingParameters");
            Vector4 secondEdgeTextureTilingModeParameters = material.GetVector("_" + secondEdgeName + "EdgeTextureTilingParameters");
            Vector4 firstSecondEdgesTextureTilingModeParameters = material.GetVector("_" + firstEdgeName + secondEdgeName + "EdgesTextureTilingParameters");
            bool isFirstEdgeTextureTilingModeSetToStretch = firstEdgeTextureTilingModeParameters.x == 1f;
            bool isFirstEdgeTextureStretchTilingModeKeepAspect = firstEdgeTextureTilingModeParameters.y == 1f;
            bool isFirstEdgeTextureStretchTilingModeAutoX = firstEdgeTextureTilingModeParameters.z == 1f;
            bool isSecondEdgeTextureTilingModeSetToStretch = secondEdgeTextureTilingModeParameters.x == 1f;
            bool isSecondEdgeTextureStretchTilingModeKeepAspect = secondEdgeTextureTilingModeParameters.y == 1f;
            bool isSecondEdgeTextureStretchTilingModeAutoX = secondEdgeTextureTilingModeParameters.z == 1f;
            bool isFirstSecondEdgesTextureTilingModeSetToStretch = firstSecondEdgesTextureTilingModeParameters.x == 1f;
            bool isFirstSecondEdgesTextureStretchTilingModeKeepAspect = firstSecondEdgesTextureTilingModeParameters.y == 1f;
            bool isFirstSecondEdgesTextureStretchTilingModeAutoX = firstSecondEdgesTextureTilingModeParameters.z == 1f;
            Vector4 textureFlippingParametes = material.GetVector("_" + firstEdgeName + secondEdgeName + "EdgesTextureFlipParameters");
            bool isTextureFlippingEnabled = textureFlippingParametes.x == 1f;
            bool isTextureFlippingFirstEdge = textureFlippingParametes.y == 1f;
            bool isFirstSecondEdgesAbsoluteThicknessAndOffsetAbsolute = material.GetInt("_Waterfall2D_Is" + firstEdgeName + secondEdgeName + "EdgesAbsoluteThicknessAndOffsetEnabled") == 1;
            
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + secondEdgeName + "EdgesSameTexture", areFirstSecondEdgesEnabled && isFirstEdgeEnabled && isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture && !isFirstSecondEdgesTextureSheetEnabled && !(isFirstSecondEdgesTextureSheetEnabled && isFirstSecondEdgesTextureSheetLerpEnabled));
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + secondEdgeName + "EdgesSameTextureSheet", areFirstSecondEdgesEnabled && isFirstEdgeEnabled && isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture && isFirstSecondEdgesTextureSheetEnabled && !isFirstSecondEdgesTextureSheetLerpEnabled);
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + secondEdgeName + "EdgesSameTextureSheetWithLerp", areFirstSecondEdgesEnabled && isFirstEdgeEnabled && isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture && isFirstSecondEdgesTextureSheetEnabled && isFirstSecondEdgesTextureSheetLerpEnabled);
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + "Edge", areFirstSecondEdgesEnabled && isFirstEdgeEnabled && !(isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture) && !isFirstEdgeTextureSheetEnabled && !(isFirstEdgeTextureSheetEnabled && isFirstEdgeTextureSheetLerpEnabled));
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + "EdgeTextureSheet", areFirstSecondEdgesEnabled && isFirstEdgeEnabled && !(isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture) && isFirstEdgeTextureSheetEnabled && !isFirstEdgeTextureSheetLerpEnabled);
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + "EdgeTextureSheetWithLerp", areFirstSecondEdgesEnabled && isFirstEdgeEnabled && !(isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture) && isFirstEdgeTextureSheetEnabled && isFirstEdgeTextureSheetLerpEnabled);
            SetKeywordState(material, "Waterfall2D_" + secondEdgeName + "Edge", areFirstSecondEdgesEnabled && isSecondEdgeEnabled && !(isFirstEdgeEnabled && areFirstSecondEdgesUsingSameTexture) && !isSecondEdgeTextureSheetEnabled && !(isSecondEdgeTextureSheetEnabled && isSecondEdgeTextureSheetLerpEnabled));
            SetKeywordState(material, "Waterfall2D_" + secondEdgeName + "EdgeTextureSheet", areFirstSecondEdgesEnabled && isSecondEdgeEnabled && !(isFirstEdgeEnabled && areFirstSecondEdgesUsingSameTexture) && isSecondEdgeTextureSheetEnabled && !isSecondEdgeTextureSheetLerpEnabled);
            SetKeywordState(material, "Waterfall2D_" + secondEdgeName + "EdgeTextureSheetWithLerp", areFirstSecondEdgesEnabled && isSecondEdgeEnabled && !(isFirstEdgeEnabled && areFirstSecondEdgesUsingSameTexture) && isSecondEdgeTextureSheetEnabled && isSecondEdgeTextureSheetLerpEnabled);
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + secondEdgeName + "EdgesNoise", areFirstSecondEdgesEnabled && (isFirstEdgeEnabled || isSecondEdgeEnabled) && isFirstSecondEdgesDistortionEffectEnabled);
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + "EdgeTextureStretch", areFirstSecondEdgesEnabled && isFirstEdgeEnabled && !(isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture) && isFirstEdgeTextureTilingModeSetToStretch && !isFirstEdgeTextureStretchTilingModeKeepAspect);
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + "EdgeTextureStretchAutoX", areFirstSecondEdgesEnabled && isFirstEdgeEnabled && !(isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture) && isFirstEdgeTextureTilingModeSetToStretch && isFirstEdgeTextureStretchTilingModeKeepAspect && isFirstEdgeTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + "EdgeTextureStretchAutoY", areFirstSecondEdgesEnabled && isFirstEdgeEnabled && !(isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture) && isFirstEdgeTextureTilingModeSetToStretch && isFirstEdgeTextureStretchTilingModeKeepAspect && !isFirstEdgeTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Waterfall2D_" + secondEdgeName + "EdgeTextureStretch", areFirstSecondEdgesEnabled && isSecondEdgeEnabled && !(isFirstEdgeEnabled && areFirstSecondEdgesUsingSameTexture) && isSecondEdgeTextureTilingModeSetToStretch && !isSecondEdgeTextureStretchTilingModeKeepAspect);
            SetKeywordState(material, "Waterfall2D_" + secondEdgeName + "EdgeTextureStretchAutoX", areFirstSecondEdgesEnabled && isSecondEdgeEnabled && !(isFirstEdgeEnabled && areFirstSecondEdgesUsingSameTexture) && isSecondEdgeTextureTilingModeSetToStretch && isSecondEdgeTextureStretchTilingModeKeepAspect && isSecondEdgeTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Waterfall2D_" + secondEdgeName + "EdgeTextureStretchAutoY", areFirstSecondEdgesEnabled && isSecondEdgeEnabled && !(isFirstEdgeEnabled && areFirstSecondEdgesUsingSameTexture) && isSecondEdgeTextureTilingModeSetToStretch && isSecondEdgeTextureStretchTilingModeKeepAspect && !isSecondEdgeTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + secondEdgeName + "EdgesTextureStretch", areFirstSecondEdgesEnabled && isFirstEdgeEnabled && isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture && isFirstSecondEdgesTextureTilingModeSetToStretch && !isFirstSecondEdgesTextureStretchTilingModeKeepAspect);
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + secondEdgeName + "EdgesTextureStretchAutoX", areFirstSecondEdgesEnabled && isFirstEdgeEnabled && isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture && isFirstSecondEdgesTextureTilingModeSetToStretch && isFirstSecondEdgesTextureStretchTilingModeKeepAspect && isFirstSecondEdgesTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + secondEdgeName + "EdgesTextureStretchAutoY", areFirstSecondEdgesEnabled && isFirstEdgeEnabled && isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture && isFirstSecondEdgesTextureTilingModeSetToStretch && isFirstSecondEdgesTextureStretchTilingModeKeepAspect && !isFirstSecondEdgesTextureStretchTilingModeAutoX);
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + secondEdgeName + "EdgesFlip" + firstEdgeName + "Edge" + (isSettingTopBottomEdgesKeywords ? "Y" : "X"), areFirstSecondEdgesEnabled && isFirstEdgeEnabled && isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture && isTextureFlippingEnabled && isTextureFlippingFirstEdge);
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + secondEdgeName + "EdgesFlip" + secondEdgeName + "Edge" + (isSettingTopBottomEdgesKeywords ? "Y" : "X"), areFirstSecondEdgesEnabled && isFirstEdgeEnabled && isSecondEdgeEnabled && areFirstSecondEdgesUsingSameTexture && isTextureFlippingEnabled && !isTextureFlippingFirstEdge);
            SetKeywordState(material, "Waterfall2D_" + firstEdgeName + secondEdgeName + "EdgesAbsoluteThicknessAndOffset", areFirstSecondEdgesEnabled && (isFirstEdgeEnabled || isSecondEdgeEnabled) && isFirstSecondEdgesAbsoluteThicknessAndOffsetAbsolute);

            if (!isSettingTopBottomEdgesKeywords)
            {
                bool isTextureAlphaCutoffEnabled = FindProperty("_Waterfall2D_Is" + firstEdgeName + secondEdgeName + "EdgesTextureAlphaCutoffEnabled", _materialProperties).floatValue == 1f;
                SetKeywordState(material, "Waterfall2D_" + firstEdgeName + secondEdgeName + "EdgesTextureAlphaCutoff", (areFirstSecondEdgesEnabled || isFirstEdgeEnabled || isSecondEdgeEnabled) && isTextureAlphaCutoffEnabled);
            }
        }
    }

}