namespace Game2DWaterKit
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.AnimatedValues;
    using UnityEngine.Rendering;
    using System.Collections.Generic;
    using UnityEditor.SceneManagement;
    using UnityEngine.Profiling;

    public abstract class Game2DWaterKitShaderGUI : ShaderGUI
    {
#if UNITY_2017_1_OR_NEWER
        private static readonly string[] _wrapModeOptions = new[] { "Mirror", "Repeat" };
#endif
        private static readonly string[] _noiseTextureSizeOptions = {
            "32"
            , "64"
            , "128"
            , "256"
            , "512"
            , "1024"
			//, "2048"
			//, "4096"
			//, "8192"
		};
        private static readonly string[] _shaderTypesBuiltinRP = new[] { "Unlit", "Pixel Lit", "Vertex Lit", "Vertex Lit (Only Directional Lights)" };
#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
        private static readonly string[] _shaderTypesSRP = new[] { "Unlit", "Lit (2D Renderer)" };
#endif
        private static readonly string[] _renderQueueOptions = new[] { "Geometry (2000)", "Transparent (3000)" };
        private static readonly string[] _colorModeOptions = new[] { "Solid Color", "Gradient Color" };
        private static readonly string[] _filterModeOptions = new[] { "Bilinear", "Point" };
        private static readonly GUIContent _tempContent = new GUIContent();

        private static GUIStyle _helpBoxStyle;
        private static GUIStyle _groupBoxStyle;
        private static Texture2D[] _noiseTexturePreviews = new Texture2D[4];
        private static Dictionary<string, AnimBool> _foldoutsAnimBools = new Dictionary<string, AnimBool>();
        private static System.Action _foldoutAnimBoolsAction;

        protected UnityEngine.Material _material;
        protected MaterialEditor _materialEditor;
        protected MaterialProperty[] _materialProperties;
        protected Shader _shader;
        protected string _shaderKeywordsPrefix;
        protected bool _isWaterfallShader;

        private bool _firstTimeApply = true;
        private bool _undoRedoPerformed;

        protected System.Action _inspectedWaterKitObjectValidatePropertiesDelegate;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            _materialEditor = materialEditor;
            _materialProperties = properties;
            _material = _materialEditor.target as UnityEngine.Material;
            _shader = _material.shader;

            _isWaterfallShader = _shader.name.EndsWith("Waterfall");
            bool isValidShader = _isWaterfallShader == (FindProperty("_Game2DWaterKit_MaterialType", _materialProperties).floatValue == 1.0f);

            if (!isValidShader)
            {
                string message = string.Format("Invalid Shader: Please use a {0} shader!", _isWaterfallShader ? "water" : "waterfall");
                EditorGUILayout.HelpBox(message, MessageType.Warning);
                return;
            }

            _shaderKeywordsPrefix = _isWaterfallShader ? "_Waterfall2D_Is" : "_Water2D_Is";

            if (_firstTimeApply)
            {
                EditorApplication.playModeStateChanged -= SaveFoldoutsStates;
                EditorApplication.playModeStateChanged += SaveFoldoutsStates;

                Initialize();
            }

            EditorGUI.BeginChangeCheck();

            BeginBoxGroup(true);
#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
            DrawShaderTypeOptionsSRP();
#else
            DrawShaderTypeOptionsBuiltinRP();
#endif
            EndBoxGroup();

            DrawPropertiesGroup("Rendering Options", DrawRenderingOptions);
            DrawMaterialProperties();
            if (FindProperty("_NoiseTexture", _materialProperties).textureValue != null)
                DrawPropertiesGroup("Noise Texture Settings", DrawNoiseTextureSettings);
            DrawNoiseTextureMissingAssetWarning();

            if (_undoRedoPerformed)
            {
                _undoRedoPerformed = false;
                GenerateNoiseTexture();
            }

            if (EditorGUI.EndChangeCheck())
            {
                UpdateMaterials();

                if (!EditorApplication.isPlaying)
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            EditorGUIUtility.fieldWidth = 0f;
        }

        protected abstract void DrawMaterialProperties();

        protected abstract void SetMaterialKeywords(UnityEngine.Material material);

        protected virtual void Initialize()
        {
            if (Selection.activeTransform != null && Selection.activeTransform.GetComponent<Game2DWaterKitObject>() != null)
                _inspectedWaterKitObjectValidatePropertiesDelegate = (System.Action)System.Delegate.CreateDelegate(typeof(System.Action), Selection.activeTransform.GetComponent<Game2DWaterKitObject>(), "OnValidate");

            UpdateMaterials();

            GenerateNoiseTexture();

            Undo.undoRedoPerformed -= UndoRedoPerformed;
            Undo.undoRedoPerformed += UndoRedoPerformed;

            _foldoutAnimBoolsAction = _materialEditor.Repaint;

            _helpBoxStyle = new GUIStyle("HelpBox");
            _groupBoxStyle = new GUIStyle("GroupBox");

            _firstTimeApply = false;
        }

        private void DrawRenderingOptions()
        {
            // Material Render Queue

            var renderQueuePopupIndex = _material.renderQueue == 2000 ? 0 : 1;
            EditorGUI.BeginChangeCheck();
            renderQueuePopupIndex = EditorGUILayout.Popup("Rendering Queue", renderQueuePopupIndex, _renderQueueOptions);
            if (EditorGUI.EndChangeCheck())
                _material.renderQueue = renderQueuePopupIndex == 0 ? 2000 : 3000;

            // Sprite Mask Interaction

            var spriteMaskInteractionProperty = FindProperty("_SpriteMaskInteraction", _materialProperties);
            var spriteMaskInteractionRefProperty = FindProperty("_SpriteMaskInteractionRef", _materialProperties);

            if (spriteMaskInteractionProperty.floatValue == 0)
                spriteMaskInteractionProperty.floatValue = 8;

            var rect = EditorGUILayout.GetControlRect();

            const float maskIneractionRefPropertyWidth = 65f;
            var maskIneractionPopupRect = new Rect(rect.x, rect.y, rect.width - maskIneractionRefPropertyWidth, rect.height);
            var maskInteractionRefRect = new Rect(rect.x + maskIneractionPopupRect.width, rect.y, maskIneractionRefPropertyWidth, rect.height);

            _materialEditor.ShaderProperty(maskIneractionPopupRect, spriteMaskInteractionProperty, "Mask Interaction");
            EditorGUIUtility.labelWidth = 25f;
            EditorGUI.BeginChangeCheck();
            int maskInteractionRefrenceValue = EditorGUI.IntField(maskInteractionRefRect, "Ref", (int)spriteMaskInteractionRefProperty.floatValue);
            if (EditorGUI.EndChangeCheck())
                spriteMaskInteractionRefProperty.floatValue = Mathf.Clamp(maskInteractionRefrenceValue, 0, 255);
            EditorGUIUtility.labelWidth = 0f;

            if (_material.renderQueue == 2000 && spriteMaskInteractionProperty.floatValue != 0f)
                EditorGUILayout.HelpBox("Material \"Rendering Queue\" should be set to \"Transparent\" for the sprite mask interaction to work properly!", MessageType.Info);

            // Tint color / Texture Order

            var applyTintColorOnTopOfTextureKeywordState = FindProperty(_shaderKeywordsPrefix + "ApplyTintColorOnTopOfTextureEnabled", _materialProperties);
            DrawShaderKeywordPropertyToggle(EditorGUILayout.GetControlRect(), applyTintColorOnTopOfTextureKeywordState, "Apply Tint Color(s) On Top Of Texture(s)", true);

            if (!_isWaterfallShader)
            {
                var smoothLinesKeywordState = FindProperty(_shaderKeywordsPrefix + "SmoothLinesEnabled", _materialProperties);
                DrawShaderKeywordPropertyToggle(EditorGUILayout.GetControlRect(), smoothLinesKeywordState, "Smooth Water Surface And Lines", true);
            }
        }

        private void DrawShaderTypeOptionsBuiltinRP()
        {
            var shaderName = _material.shader.name;

            int current;

            if (!shaderName.Contains("Built-in Render Pipeline"))
                current = -1;
            else if (shaderName.Contains("Unlit"))
                current = 0;
            else if (shaderName.Contains("PixelLit"))
                current = 1;
            else if (shaderName.Contains("Directional"))
                current = 3;
            else
                current = 2;

            if (current == -1)
                EditorGUILayout.HelpBox("Please use a shader compatible with the Built-in Render Pipeline!", MessageType.Warning);
            else
            {
                EditorGUI.BeginChangeCheck();
                current = EditorGUILayout.Popup("Shader Type", current, _shaderTypesBuiltinRP);
                if (EditorGUI.EndChangeCheck())
                {
                    string newShaderName;

                    switch (current)
                    {
                        case 1:
                            newShaderName = "Game2DWaterKit/Built-in Render Pipeline/PixelLit (Supports Lightmaps)/";
                            break;
                        case 2:
                            newShaderName = "Game2DWaterKit/Built-in Render Pipeline/VertexLit (Supports Lightmaps)/";
                            break;
                        case 3:
                            newShaderName = "Game2DWaterKit/Built-in Render Pipeline/VertexLit (Only Directional Lights, Supports Lightmaps)/";
                            break;
                        default:
                            newShaderName = "Game2DWaterKit/Built-in Render Pipeline/Unlit/";
                            break;
                    }

                    newShaderName += _isWaterfallShader ? "Waterfall" : "Water";

                    _material.shader = Shader.Find(newShaderName);
                }
            }
        }

#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
        private void DrawShaderTypeOptionsSRP()
        {
            var shaderName = _material.shader.name;

#if GAME_2D_WATER_KIT_LWRP
            var renderPipelineName = "Lightweight Render Pipeline";
#else
            var renderPipelineName = "Universal Render Pipeline";
#endif

            int current;

            if (!shaderName.Contains(renderPipelineName))
                current = -1;
            else if (shaderName.Contains("Unlit"))
                current = 0;
            else
                current = 1; // lit shader

            if (current == -1)
            {
                EditorGUILayout.HelpBox(string.Format("Please use a shader compatible with the {0}!", renderPipelineName), MessageType.Warning);

                string shaderType = shaderName.Contains("Unlit") ? "Unlit" : "Lit";

                if (GUILayout.Button(string.Format("Use a compatible {0} shader", shaderType)))
                    _material.shader = Shader.Find(string.Format("Game2DWaterKit/{0}/{1}/{2}", renderPipelineName, shaderType, _isWaterfallShader ? "Waterfall" : "Water"));
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                current = EditorGUILayout.Popup("Shader Type", current, _shaderTypesSRP);
                if (EditorGUI.EndChangeCheck())
                    _material.shader = Shader.Find(string.Format("Game2DWaterKit/{0}/{1}/{2}", renderPipelineName, current == 0 ? "Unlit" : "Lit", _isWaterfallShader ? "Waterfall" : "Water"));
            }
        }
#endif

        protected void DrawPropertiesGroup(string groupName, System.Action propertiesLayoutProc, MaterialProperty groupKeywordState = null)
        {
            var fadeGroupAnimBool = GetFoldoutAnimBool(groupName);

            if (groupKeywordState == null)
            {
                EditorGUI.BeginChangeCheck();
                var foldout = EditorGUILayout.Foldout(fadeGroupAnimBool.value, groupName, true);
                if (EditorGUI.EndChangeCheck())
                    fadeGroupAnimBool.target = foldout;
            }
            else
            {
                var rect = EditorGUILayout.GetControlRect();

                float xmax = rect.xMax;

                rect.xMin += 2f;
                rect.xMax = rect.xMin + 14f;

                DrawShaderKeywordPropertyToggle(rect, groupKeywordState, string.Empty, true);

                rect.xMin -= 2f;
                rect.xMax = xmax;
                EditorGUI.BeginChangeCheck();

#if UNITY_2019_3_OR_NEWER
                string content = "      " + groupName;
#else
                string content = "    " + groupName;
#endif

                var foldout = EditorGUI.Foldout(rect, fadeGroupAnimBool.value, content, true);
                if (EditorGUI.EndChangeCheck())
                    fadeGroupAnimBool.target = foldout;
            }

            using (var group = new EditorGUILayout.FadeGroupScope(fadeGroupAnimBool.faded))
            {
                if (group.visible)
                {
                    BeginBoxGroup();
                    bool isDisabled = groupKeywordState == null ? false : groupKeywordState.floatValue != 1f;
                    EditorGUI.BeginDisabledGroup(isDisabled);
                    propertiesLayoutProc.Invoke();
                    EditorGUI.EndDisabledGroup();
                    EndBoxGroup();
                    EditorGUIUtility.labelWidth = 0f;
                }
            }
        }

        protected void BeginPropertiesSubGroup(string name = null)
        {
            if (name != null)
            {
                Rect labelRect = EditorGUILayout.GetControlRect();
                labelRect.y += 5f;
                labelRect.x += 3f;

                EditorGUI.LabelField(labelRect, name, EditorStyles.boldLabel);
            }

            BeginBoxGroup(false);
        }

        protected void EndPropertiesSubGroup()
        {
            EndBoxGroup();
            EditorGUIUtility.labelWidth = 0f;
        }

        protected void DrawColorProperties(string name)
        {
            EditorGUIUtility.labelWidth = 95f;
            string keywordName = _isWaterfallShader ? name.Replace("BodyColor", "Color") : name.Replace("WaterColor", "Color");
            var colorGradientKeywordStateProperty = FindProperty(_shaderKeywordsPrefix + keywordName.TrimStart('_') + "GradientEnabled", _materialProperties);

            var colorProperty = FindProperty(name, _materialProperties);
            var colorGradientStartProperty = FindProperty(name + "GradientStart", _materialProperties);
            var colorGradientEndProperty = FindProperty(name + "GradientEnd", _materialProperties);
            var colorGradientOffsetProperty = FindProperty(name + "GradientOffset", _materialProperties);

            colorGradientKeywordStateProperty.floatValue = EditorGUILayout.Popup("Color Mode", (int)colorGradientKeywordStateProperty.floatValue, _colorModeOptions);

            if (colorGradientKeywordStateProperty.floatValue == 1f)
            {
                _materialEditor.ShaderProperty(colorGradientStartProperty, "Gradient Start");
                _materialEditor.ShaderProperty(colorGradientEndProperty, "Gradient End");

                float gradientOffset = colorGradientOffsetProperty.floatValue;

                EditorGUI.showMixedValue = colorGradientOffsetProperty.hasMixedValue;
                EditorGUI.BeginChangeCheck();
                gradientOffset = EditorGUILayout.Slider("Gradient Offset", gradientOffset, -1f, 1f);
                if (EditorGUI.EndChangeCheck())
                    colorGradientOffsetProperty.floatValue = gradientOffset;
                EditorGUI.showMixedValue = false;
            }
            else _materialEditor.ShaderProperty(colorProperty, "Color");
            EditorGUIUtility.labelWidth = 0f;
        }

        protected void DrawTextureProperties(string name, int distortionEffectChannelIndex)
        {
            float defaultLabelWidth = EditorGUIUtility.labelWidth;

            var textureProperty = FindProperty(name, _materialProperties);
            var textureOpacityProperty = FindProperty(name + "Opacity", _materialProperties);

            var textureSheetKeywordState = FindProperty(_shaderKeywordsPrefix + name.Replace("Surface", "WaterSurface").TrimStart('_') + "SheetEnabled", _materialProperties);
            var textureLerpKeywordState = FindProperty(_shaderKeywordsPrefix + name.Replace("Surface", "WaterSurface").TrimStart('_') + "SheetWithLerpEnabled", _materialProperties);
            var textureColumns = FindProperty(name + "SheetColumns", _materialProperties);
            var textureInverseColumns = FindProperty(name + "SheetInverseColumns", _materialProperties);
            var textureRows = FindProperty(name + "SheetRows", _materialProperties);
            var textureInverseRows = FindProperty(name + "SheetInverseRows", _materialProperties);
            var textureFramesPerSecond = FindProperty(name + "SheetFramesPerSecond", _materialProperties);
            var textureFramesCount = FindProperty(name + "SheetFramesCount", _materialProperties);

            Texture texture = textureProperty.textureValue;

            Rect rect = EditorGUILayout.GetControlRect(true, MaterialEditor.GetDefaultPropertyHeight(textureProperty));
            EditorGUI.BeginChangeCheck();
            _materialEditor.TextureProperty(rect, textureProperty, string.Empty, false);
            bool hasChangedTexture = EditorGUI.EndChangeCheck();

            EditorGUI.BeginDisabledGroup(texture == null);

            rect = _materialEditor.GetTexturePropertyCustomArea(rect);
            rect.height = 16f;
            rect.y -= 19f;
            rect.xMin -= 14f;
            float rectWidth = rect.width;
            float rectX = rect.x;

            EditorGUIUtility.labelWidth = 115f;

            DrawShaderKeywordPropertyToggle(rect, textureSheetKeywordState, "Is A Texture Sheet", true);

            bool isTextureSheet = textureSheetKeywordState.floatValue == 1f;

            EditorGUI.BeginDisabledGroup(!isTextureSheet);

            rect.y += 17f;

            if (rectWidth > 190f)
            {
                rect = EditorGUI.PrefixLabel(rect, TempContent("Columns & Rows"));
                EditorGUIUtility.labelWidth = 14f;
            }
            else EditorGUIUtility.labelWidth = 28f;

            rect.xMax -= rect.width * 0.5f;

            EditorGUI.BeginChangeCheck();

            _materialEditor.ShaderProperty(rect, textureColumns, "C");

            rect.x += rect.width;

            _materialEditor.ShaderProperty(rect, textureRows, "R");

            if (EditorGUI.EndChangeCheck())
            {
                textureFramesCount.floatValue = textureColumns.floatValue * textureRows.floatValue;
                textureInverseColumns.floatValue = 1f / textureColumns.floatValue;
                textureInverseRows.floatValue = 1f / textureRows.floatValue;
            }

            rect.Set(rectX, rect.y + 17f, rectWidth - 43f, 16f);

            string framesPerSecondLabel;
            if (rectWidth > 190f)
            {
                EditorGUIUtility.labelWidth = 115f;
                framesPerSecondLabel = "Frames Per Second";
            }
            else
            {
                EditorGUIUtility.labelWidth = 28f;
                framesPerSecondLabel = "FPS";
            }
            _materialEditor.ShaderProperty(rect, textureFramesPerSecond, framesPerSecondLabel);

            rect.xMax += 43f;
            rect.xMin = rect.xMax - 43f;

#if UNITY_2019_1_OR_NEWER
            EditorGUIUtility.labelWidth = 26f;
#else
            EditorGUIUtility.labelWidth = 30f;
#endif

            DrawShaderKeywordPropertyToggle(rect, textureLerpKeywordState, "Lerp");

            EditorGUI.EndDisabledGroup();

            rect.Set(rectX, rect.y + 17f, rectWidth, 16f);

            if (rectWidth > 180f)
            {
                EditorGUIUtility.labelWidth = 48f;
                rect = EditorGUI.PrefixLabel(rect, TempContent("Opacity"));
            }
            else
            {
                EditorGUIUtility.labelWidth = 28f;
                rect = EditorGUI.PrefixLabel(rect, TempContent("O"));
            }

            _materialEditor.RangeProperty(rect, textureOpacityProperty, string.Empty);

            if (!_isWaterfallShader)
            {
                MaterialProperty scrollingSpeedX = FindProperty(name + "ScrollingSpeedX", _materialProperties);
                MaterialProperty scrollingSpeedY = FindProperty(name + "ScrollingSpeedY", _materialProperties);
                Vector2 scrollingSpeed = new Vector2(scrollingSpeedX.floatValue, scrollingSpeedY.floatValue);

                EditorGUIUtility.labelWidth = 95f;
                var scrollingSpeedRect = EditorGUILayout.GetControlRect();
                scrollingSpeedRect.y -= 5f;
                EditorGUI.showMixedValue = scrollingSpeedX.hasMixedValue || scrollingSpeedY.hasMixedValue;
                EditorGUI.BeginChangeCheck();
                scrollingSpeedRect = EditorGUI.PrefixLabel(scrollingSpeedRect, TempContent("Scrolling Speed"));
                scrollingSpeed = EditorGUI.Vector2Field(scrollingSpeedRect, string.Empty, scrollingSpeed);
                if (EditorGUI.EndChangeCheck())
                {
                    scrollingSpeedX.floatValue = scrollingSpeed.x;
                    scrollingSpeedY.floatValue = scrollingSpeed.y;
                }
            }
            else if (!(name.Contains("Top") || name.Contains("Bottom")))
            {
                MaterialProperty scrollSpeed = FindProperty(name + "ScrollingSpeed", _materialProperties);
                EditorGUIUtility.labelWidth = 115f;
                var scrollSpeedRect = EditorGUILayout.GetControlRect();
                scrollSpeedRect.y -= 5f;
                _materialEditor.ShaderProperty(scrollSpeedRect, scrollSpeed, "Scrolling Speed");
            }

            if (textureProperty.textureValue != null && textureProperty.textureValue.wrapMode == TextureWrapMode.Clamp)
            {
                EditorGUILayout.HelpBox("Please make sure that the texture wrap mode is set to \"Repeat\" in the texture import settings!", MessageType.Warning);
                if (GUILayout.Button("Change Wrap Mode to \"Repeat\""))
                {
                    TextureImporter textureImportSettings = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
                    textureImportSettings.wrapMode = TextureWrapMode.Repeat;
                    textureImportSettings.SaveAndReimport();
                }
            }

            EditorGUIUtility.labelWidth = 55f;

            if (isTextureSheet)
                DrawTextureTilingModeProperties(name, hasChangedTexture, textureColumns.floatValue, textureRows.floatValue);
            else
                DrawTextureTilingModeProperties(name, hasChangedTexture);

            if (distortionEffectChannelIndex != -1)
                DrawDistortionEffectProperties(name, distortionEffectChannelIndex);

            EditorGUI.EndDisabledGroup();

            EditorGUIUtility.labelWidth = defaultLabelWidth;
        }

        protected void DrawShaderKeywordPropertyToggle(Rect rect, MaterialProperty shaderKeywordState, string label, bool leftToggle = false, bool miniBoldLabel = false)
        {
            EditorGUI.showMixedValue = shaderKeywordState.hasMixedValue;

            GUIStyle labelStyle = miniBoldLabel ? EditorStyles.miniBoldLabel : EditorStyles.label;
            bool isEnabled = shaderKeywordState.floatValue == 1f;

            EditorGUI.BeginChangeCheck();
            if (leftToggle)
                isEnabled = EditorGUI.ToggleLeft(rect, label, isEnabled, labelStyle);
            else
            {
                rect = EditorGUI.PrefixLabel(rect, TempContent(label), labelStyle);
                isEnabled = EditorGUI.Toggle(rect, isEnabled);
            }
            if (EditorGUI.EndChangeCheck())
                shaderKeywordState.floatValue = isEnabled ? 1f : 0f;

            EditorGUI.showMixedValue = false;
        }

        protected void DrawDistortionEffectProperties(string name, int channelIndex)
        {
            name = name.Replace("Texture", "");

            MaterialProperty scaleOffsetProperty = FindProperty(name + "NoiseScaleOffset", _materialProperties);
            MaterialProperty strengthProperty = FindProperty(name + "NoiseStrength", _materialProperties);
            MaterialProperty speedProperty = FindProperty(name + "NoiseSpeed", _materialProperties);

            var noiseTexturePreview = _noiseTexturePreviews[channelIndex];
            if (noiseTexturePreview == null)
                noiseTexturePreview = GenerateNoiseTexturePreview(channelIndex);

            bool isDistortionEffectActive = true;

            if (!name.Contains("Refraction") && !name.Contains("Reflection"))
            {
                EditorGUILayout.Space();

                MaterialProperty distortionEffectKeywordState = FindProperty(_shaderKeywordsPrefix + name.Trim('_') + "NoiseEnabled", _materialProperties);
                DrawShaderKeywordPropertyToggle(EditorGUILayout.GetControlRect(), distortionEffectKeywordState, "Distortion Effect", true, true);

                isDistortionEffectActive = distortionEffectKeywordState.floatValue == 1f;
            }

            EditorGUI.BeginDisabledGroup(!isDistortionEffectActive);

            float previewTextureDimension = 2f * EditorGUIUtility.singleLineHeight;
            Rect rect = EditorGUILayout.GetControlRect(true, previewTextureDimension);

            rect.width -= previewTextureDimension + 3f;

            Rect offsetPropertyRect = new Rect(rect);
            offsetPropertyRect.height = EditorGUIUtility.singleLineHeight;
            offsetPropertyRect.y += EditorGUIUtility.singleLineHeight + 1f;
            Rect scalePropertyRect = new Rect(offsetPropertyRect);
            scalePropertyRect.y -= EditorGUIUtility.singleLineHeight + 1;

            EditorGUIUtility.labelWidth = 55f;

            Vector4 scaleOffset = scaleOffsetProperty.vectorValue;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = scaleOffsetProperty.hasMixedValue;
            scalePropertyRect = EditorGUI.PrefixLabel(scalePropertyRect, TempContent("Scale"));
            Vector2 scale = EditorGUI.Vector2Field(scalePropertyRect, string.Empty, new Vector2(scaleOffset.x, scaleOffset.y));
            offsetPropertyRect = EditorGUI.PrefixLabel(offsetPropertyRect, TempContent("Offset"));
            Vector2 offset = EditorGUI.Vector2Field(offsetPropertyRect, string.Empty, new Vector2(scaleOffset.z, scaleOffset.w));
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                scaleOffsetProperty.vectorValue = new Vector4(scale.x, scale.y, offset.x, offset.y);

                GenerateNoiseTexture();
                GenerateNoiseTexturePreview(channelIndex);
            }

            rect.x += rect.width + 3f;
            rect.width = previewTextureDimension;
            GUI.DrawTexture(rect, noiseTexturePreview, ScaleMode.StretchToFill);

            EditorGUILayout.Space();

            _materialEditor.RangeProperty(EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(), TempContent("Strength")), strengthProperty, string.Empty);

            speedProperty.floatValue = EditorGUILayout.FloatField("Speed", speedProperty.floatValue * 100f) / 100f;

            // Noise Tiling Property
            MaterialProperty tilingProperty = FindProperty(name + "NoiseTiling", _materialProperties);

            var tiling = tilingProperty.vectorValue;

            var tilingPropertyRect = EditorGUILayout.GetControlRect(true, 32f);
            tilingPropertyRect.height = 16f;

            tilingPropertyRect = EditorGUI.PrefixLabel(tilingPropertyRect, TempContent("Tiling"));

            EditorGUIUtility.labelWidth = 20f;
            EditorGUI.showMixedValue = tilingProperty.hasMixedValue;
            EditorGUI.BeginChangeCheck();
            tiling.x = EditorGUI.Slider(tilingPropertyRect, "X:", tiling.x, 0f, 1f);
            tilingPropertyRect.y += 16f;
            tiling.y = EditorGUI.Slider(tilingPropertyRect, "Y:", tiling.y, 0f, 1f);
            if (EditorGUI.EndChangeCheck())
                tilingProperty.vectorValue = tiling;

            EditorGUI.showMixedValue = false;

            EditorGUI.EndDisabledGroup();
        }

        protected void SetKeywordState(UnityEngine.Material material, string keyword, bool activate)
        {
            if (activate)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }

        protected void BeginBoxGroup(bool useHelpBoxStyle = true)
        {
            EditorGUILayout.BeginVertical(useHelpBoxStyle ? _helpBoxStyle : _groupBoxStyle);
        }

        protected void EndBoxGroup()
        {
            EditorGUILayout.EndVertical();
        }

        protected GUIContent TempContent(string text)
        {
            _tempContent.text = text;
            return _tempContent;
        }


        private void DrawNoiseTextureMissingAssetWarning()
        {
            var noiseTexture = FindProperty("_NoiseTexture", _materialProperties).textureValue;
            bool textureAssetAlreadyExist = AssetDatabase.Contains(noiseTexture);

            if (textureAssetAlreadyExist)
                return;

            bool materialAssetAlreadyExist = AssetDatabase.Contains(_material);

            if (materialAssetAlreadyExist)
            {
                BeginBoxGroup();
                EditorGUILayout.HelpBox("The noise texture should be saved along with its material in your project!", MessageType.Warning);
                if (GUILayout.Button("Add Missing Noise Texture To Project"))
                {
                    string materialPath = AssetDatabase.GetAssetPath(_material);
                    string materialName = System.IO.Path.GetFileNameWithoutExtension(materialPath);
                    AssetDatabase.CreateAsset(noiseTexture, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(materialPath), materialName + "_noiseTexture.asset"));
                }
                EndBoxGroup();
            }
        }

        private void UpdateMaterials()
        {
            foreach (UnityEngine.Material material in _materialEditor.targets)
            {
                SetupMaterialBlendMode(material);
                SetMaterialKeywords(material);
            }
        }

        private void UndoRedoPerformed()
        {
            _undoRedoPerformed = true;
        }

        private void DrawNoiseTextureSettings()
        {
            MaterialProperty noiseTextureProperty = FindProperty("_NoiseTexture", _materialProperties);
            var noiseTexture = noiseTextureProperty.textureValue as Texture2D;

            BeginPropertiesSubGroup();
            EditorGUIUtility.labelWidth = 40f;
            _materialEditor.TextureScaleOffsetProperty(noiseTextureProperty);
            EndPropertiesSubGroup();

            BeginPropertiesSubGroup();

            EditorGUIUtility.labelWidth = 80f;
            int noiseTextureSize = noiseTexture.width;
            int textuerSelectedSizeIndex = ArrayUtility.IndexOf(_noiseTextureSizeOptions, noiseTextureSize.ToString());
            EditorGUI.BeginChangeCheck();
            textuerSelectedSizeIndex = EditorGUILayout.Popup("Size", textuerSelectedSizeIndex, _noiseTextureSizeOptions);
            if (EditorGUI.EndChangeCheck())
                GenerateNoiseTexture(int.Parse(_noiseTextureSizeOptions[textuerSelectedSizeIndex]));

#if UNITY_2017_1_OR_NEWER
            TextureWrapMode noiseTextureWrapMode = noiseTexture.wrapMode;
            int wrapModeIndex = noiseTextureWrapMode == TextureWrapMode.Mirror ? 0 : 1;
            EditorGUI.BeginChangeCheck();
            wrapModeIndex = EditorGUILayout.Popup("Wrap Mode", wrapModeIndex, _wrapModeOptions);
            if (EditorGUI.EndChangeCheck())
                noiseTexture.wrapMode = wrapModeIndex == 0 ? TextureWrapMode.Mirror : TextureWrapMode.Repeat;
#endif

            FilterMode noiseTextureFilterMode = noiseTexture.filterMode;
            int filterModeIndex = noiseTextureFilterMode == FilterMode.Bilinear ? 0 : 1;
            EditorGUI.BeginChangeCheck();
            filterModeIndex = EditorGUILayout.Popup("Filter Mode", filterModeIndex, _filterModeOptions);
            if (EditorGUI.EndChangeCheck())
                noiseTexture.filterMode = filterModeIndex == 0 ? FilterMode.Bilinear : FilterMode.Point;

            EndPropertiesSubGroup();

            BeginPropertiesSubGroup();

            EditorGUILayout.LabelField(string.Format("Format: {0}", noiseTexture.format));
            EditorGUILayout.LabelField(string.Format("Size: {0} KB", Profiler.GetRuntimeMemorySizeLong(noiseTexture) / 1024));

            bool textureAssetAlreadyExist = AssetDatabase.Contains(noiseTexture);
            if (textureAssetAlreadyExist)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(GUIContent.none, noiseTexture, typeof(object), false);
                EditorGUI.EndDisabledGroup();
            }

            EndPropertiesSubGroup();
        }

        private void DrawTextureTilingModeProperties(string name, bool forceUpdateTilingModeProperties, float columnCount = 1f, float rowCount = 1f)
        {
            EditorGUILayout.LabelField("Tiling Properties", EditorStyles.miniBoldLabel);

            MaterialProperty textureProperty = FindProperty(name, _materialProperties);
            MaterialProperty textureTilingParametersProperty = FindProperty(name + "TilingParameters", _materialProperties);

            var texture = textureProperty.textureValue;
            var textureTilingParameters = textureTilingParametersProperty.vectorValue;

            var scaleOffset = textureProperty.textureScaleAndOffset;

            EditorGUI.showMixedValue = textureProperty.hasMixedValue;
            EditorGUI.BeginChangeCheck();

            // Tiling Mode Popup
            EditorGUIUtility.labelWidth = 80f;
            EditorGUI.BeginChangeCheck();
            bool isTilingModeSetToStretch = EditorGUILayout.Popup("Tiling Mode", (int)textureTilingParameters.x, new[] { "Repeat", "Stretch" }) == 1;
            if (EditorGUI.EndChangeCheck() && isTilingModeSetToStretch != (textureTilingParameters.x == 1f))
            {
                scaleOffset.x = 1f / scaleOffset.x;
                scaleOffset.y = 1f / scaleOffset.y;
            }

            var rect = EditorGUILayout.GetControlRect(true, 66f);

            // Offset Property
            rect.height = 16f;
            rect.y += 52f;
            Vector2 offset = EditorGUI.Vector2Field(EditorGUI.PrefixLabel(rect, TempContent("Offset")), GUIContent.none, new Vector2(scaleOffset.z, scaleOffset.w));

            // Scale Properties
            Vector2 scale;
            if (isTilingModeSetToStretch)
                scale = new Vector2(scaleOffset.x, scaleOffset.y);
            else
                scale = new Vector2(1f / scaleOffset.x, 1f / scaleOffset.y);

            rect.y -= 52f;
            rect = EditorGUI.PrefixLabel(rect, TempContent(isTilingModeSetToStretch ? "Tiling" : "Scale"));
            float xmax = rect.xMax;
            float xmin = rect.xMin;

            // Scale Properties --> Keep Aspect Ratio Property
            EditorGUIUtility.labelWidth = 110f;
            bool keepAspect = EditorGUI.Toggle(rect, "Keep Aspect Ratio", textureTilingParameters.y == 1f);

            bool autoX = textureTilingParameters.z == 1f;
            bool autoY = !autoX;

            // Scale Properties --> Scale X
            rect.y += 17f;
            rect.xMax = xmax - 45f;
            EditorGUIUtility.labelWidth = 14f;
            EditorGUI.BeginDisabledGroup(keepAspect && autoX);
            scale.x = EditorGUI.FloatField(rect, "X", scale.x);
            EditorGUI.EndDisabledGroup();

            rect.xMax = xmax;
            rect.xMin = xmax - 45f;
            EditorGUIUtility.labelWidth = 28f;
            EditorGUI.BeginDisabledGroup(!keepAspect);
            EditorGUI.BeginChangeCheck();
            autoX = EditorGUI.Toggle(rect, "Auto", keepAspect ? autoX : false);
            if (EditorGUI.EndChangeCheck())
                autoY = !autoX;
            EditorGUI.EndDisabledGroup();

            // Scale Properties --> Scale Y
            rect.y += 17f;
            rect.xMax = xmax - 45f;
            rect.xMin = xmin;
            EditorGUIUtility.labelWidth = 14f;
            EditorGUI.BeginDisabledGroup(keepAspect && autoY);
            scale.y = EditorGUI.FloatField(rect, "Y", scale.y);
            EditorGUI.EndDisabledGroup();

            rect.xMax = xmax;
            rect.xMin = xmax - 45f;
            EditorGUIUtility.labelWidth = 28f;
            EditorGUI.BeginDisabledGroup(!keepAspect);
            EditorGUI.BeginChangeCheck();
            autoY = EditorGUI.Toggle(rect, "Auto", keepAspect ? autoY : false);
            if (EditorGUI.EndChangeCheck())
                autoX = !autoY;
            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck() || forceUpdateTilingModeProperties)
            {
                scaleOffset.z = offset.x;
                scaleOffset.w = offset.y;

                float aspect = texture == null ? 1f : (texture.width / columnCount) / (texture.height / rowCount);

                if (isTilingModeSetToStretch)
                {
                    if (keepAspect)
                    {
                        scaleOffset.x = autoX ? scale.y / aspect : scale.x;
                        scaleOffset.y = autoY ? scale.x * aspect : scale.y;
                    }
                    else
                    {
                        scaleOffset.x = scale.x;
                        scaleOffset.y = scale.y;
                    }
                }
                else
                {
                    if (keepAspect)
                    {
                        scaleOffset.x = autoX ? (1f / scale.y) / aspect : 1f / scale.x;
                        scaleOffset.y = autoY ? (1f / scale.x) * aspect : 1f / scale.y;
                    }
                    else
                    {
                        scaleOffset.x = 1f / scale.x;
                        scaleOffset.y = 1f / scale.y;
                    }
                }

                textureProperty.textureScaleAndOffset = scaleOffset;

                textureTilingParameters.x = isTilingModeSetToStretch ? 1f : 0f;
                textureTilingParameters.y = keepAspect ? 1f : 0f;
                if (keepAspect)
                    textureTilingParameters.z = autoX ? 1f : 0f;

                textureTilingParametersProperty.vectorValue = textureTilingParameters;
            }
            EditorGUI.showMixedValue = false;
        }

        private Texture2D GenerateNoiseTexturePreview(int channelIndex)
        {
            var noiseTextureProperty = FindProperty("_NoiseTexture", _materialProperties);

            if (noiseTextureProperty.textureValue == null)
                noiseTextureProperty.textureValue = GenerateNoiseTexture();

            var noiseTexture = noiseTextureProperty.textureValue as Texture2D;
            int noiseTextureSize = noiseTexture.width;

            if (_noiseTexturePreviews[channelIndex] == null)
                _noiseTexturePreviews[channelIndex] = new Texture2D(noiseTextureSize, noiseTextureSize, TextureFormat.ARGB32, false, true);

#if UNITY_2021_2_OR_NEWER
            if (_noiseTexturePreviews[channelIndex].width != noiseTextureSize)
                _noiseTexturePreviews[channelIndex].Reinitialize(noiseTextureSize, noiseTextureSize);
#else
            if (_noiseTexturePreviews[channelIndex].width != noiseTextureSize)
                _noiseTexturePreviews[channelIndex].Resize(noiseTextureSize, noiseTextureSize);
#endif

            var previewTex = _noiseTexturePreviews[channelIndex];

            Color[] noiseTexPixels = noiseTexture.GetPixels();
            Color[] previewTexPixels = new Color[noiseTextureSize * noiseTextureSize];

            for (int i = 0; i < noiseTextureSize; i++)
            {
                for (int j = 0; j < noiseTextureSize; j++)
                {
                    int pixelIndex = i * noiseTextureSize + j;

                    float colorValue = noiseTexPixels[pixelIndex][channelIndex];

                    previewTexPixels[pixelIndex] = new Color(colorValue, colorValue, colorValue, 1.0f);
                }
            }

            previewTex.SetPixels(previewTexPixels);
            previewTex.Apply();

            return previewTex;
        }

        private Texture2D GenerateNoiseTexture(int size = -1)
        {
            var noiseTextureProperty = FindProperty("_NoiseTexture", _materialProperties);

            if (noiseTextureProperty.textureValue == null)
            {
                const int defaultSize = 128;
                var newTexture = new Texture2D(defaultSize, defaultSize, TextureFormat.RGBA32, false, true);
#if UNITY_2017_1_OR_NEWER
                newTexture.wrapMode = TextureWrapMode.Mirror;
#else
                newTexture.wrapMode = TextureWrapMode.Repeat;
#endif
                newTexture.filterMode = FilterMode.Bilinear;
                noiseTextureProperty.textureValue = newTexture;
            }

            Texture2D noiseTexture = noiseTextureProperty.textureValue as Texture2D;

            Vector4 firstChannelScaleOffset;
            Vector4 secondChannelScaleOffset;
            Vector4 thirdChannelScaleOffset;
            Vector4 fourthChannelScaleOffset;

            if (_isWaterfallShader)
            {
                firstChannelScaleOffset = FindProperty("_RefractionNoiseScaleOffset", _materialProperties).vectorValue;
                secondChannelScaleOffset = FindProperty("_LeftRightEdgesNoiseScaleOffset", _materialProperties).vectorValue;
                thirdChannelScaleOffset = FindProperty("_TopBottomEdgesNoiseScaleOffset", _materialProperties).vectorValue;
                fourthChannelScaleOffset = FindProperty("_BodyNoiseScaleOffset", _materialProperties).vectorValue;
            }
            else
            {
                firstChannelScaleOffset = FindProperty("_RefractionNoiseScaleOffset", _materialProperties).vectorValue;
                secondChannelScaleOffset = FindProperty("_ReflectionNoiseScaleOffset", _materialProperties).vectorValue;
                thirdChannelScaleOffset = FindProperty("_SurfaceNoiseScaleOffset", _materialProperties).vectorValue;
                fourthChannelScaleOffset = FindProperty("_WaterNoiseScaleOffset", _materialProperties).vectorValue;
            }

            int noiseTextureSize = noiseTexture.width;

            if (size != -1 && noiseTextureSize != size)
            {
#if UNITY_2021_2_OR_NEWER
                noiseTexture.Reinitialize(size, size, TextureFormat.ARGB32, false);
#else
                noiseTexture.Resize(size, size, TextureFormat.ARGB32, false);
#endif
                noiseTextureSize = size;
            }

            Color[] noiseTexPixels = new Color[noiseTextureSize * noiseTextureSize];

            for (int i = 0; i < noiseTextureSize; i++)
            {
                for (int j = 0; j < noiseTextureSize; j++)
                {
                    Color color;

                    color.r = GetPerlinNoise(noiseTextureSize, i, j, firstChannelScaleOffset);
                    color.g = GetPerlinNoise(noiseTextureSize, i, j, secondChannelScaleOffset);
                    color.b = GetPerlinNoise(noiseTextureSize, i, j, thirdChannelScaleOffset);
                    color.a = GetPerlinNoise(noiseTextureSize, i, j, fourthChannelScaleOffset);

                    noiseTexPixels[i * noiseTextureSize + j] = color;
                }
            }

            noiseTexture.SetPixels(noiseTexPixels);
            noiseTexture.Apply();

            noiseTextureProperty.textureValue = noiseTexture;

            return noiseTexture;
        }

        private float GetPerlinNoise(int texDimension, int i, int j, Vector4 scaleOffset)
        {
            float x = (j / (float)(1 - texDimension)) * scaleOffset.x + scaleOffset.z;
            float y = (i / (float)(1 - texDimension)) * scaleOffset.y + scaleOffset.w;

            return Mathf.PerlinNoise(x, y);
        }

        private void SetupMaterialBlendMode(UnityEngine.Material material)
        {
            bool isUsingGeometryRenderQueue = material.renderQueue == 2000;

            material.SetOverrideTag("RenderType", isUsingGeometryRenderQueue ? "Opaque" : "Transparent");
            material.SetInt("_ZWrite", isUsingGeometryRenderQueue ? 1 : 0);

            if (isUsingGeometryRenderQueue)
            {
                material.SetInt("_SrcBlend", (int)BlendMode.One);
                material.SetInt("_DstBlend", (int)BlendMode.Zero);
            }
            else
            {
                material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            }
        }

        private static AnimBool GetFoldoutAnimBool(string name)
        {
            AnimBool foldoutAnimBool;

            if (!_foldoutsAnimBools.TryGetValue(name, out foldoutAnimBool))
            {
                bool isUnfolded = false;

                if (EditorPrefs.HasKey(name))
                {
                    isUnfolded = true;
                    EditorPrefs.DeleteKey(name);
                }

                foldoutAnimBool = new AnimBool(() => _foldoutAnimBoolsAction());
                foldoutAnimBool.value = isUnfolded;

                _foldoutsAnimBools.Add(name, foldoutAnimBool);
            }

            return foldoutAnimBool;
        }

        private static void SaveFoldoutsStates(PlayModeStateChange playModeStateChange)
        {
            foreach (var foldoutState in _foldoutsAnimBools)
            {
                if (foldoutState.Value.value)
                    EditorPrefs.SetBool(foldoutState.Key, foldoutState.Value.value);
            }
        }

    }
}