namespace Game2DWaterKit
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.AnimatedValues;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;

    [InitializeOnLoad]
    public abstract partial class Game2DWaterKitInspector : Editor
    {
        private static Dictionary<string, AnimBool> _foldoutsAnimBools = new Dictionary<string, AnimBool>();
        private static System.Action _foldoutsAnimBoolsCallback;

        protected static bool _isInSimulationMode;
        protected static bool _isSimulationModeOwnedByWaterfallEditor;

        protected static bool _isEditingMeshMask;
        protected static bool _previewMeshMaskOutline;
        protected static bool _previewObjectOutline;
        protected static bool _previewEdgeCollider;
        protected static bool _previewPolygonCollider;

        protected SerializedObject _meshRendererSerializedObject;
        protected SerializedProperty _materialProperty;

        protected bool _isMultiEditing;

        protected static bool _isEditingWaterObject;
        protected static bool _isInspectingPrefab;

        static Game2DWaterKitInspector()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= Game2DWaterKitObjectResizeTool.WatchForObjectSizeChanges;
            SceneView.duringSceneGui += Game2DWaterKitObjectResizeTool.WatchForObjectSizeChanges;
#else
            SceneView.onSceneGUIDelegate -= Game2DWaterKitObjectResizeTool.WatchForObjectSizeChanges;
            SceneView.onSceneGUIDelegate += Game2DWaterKitObjectResizeTool.WatchForObjectSizeChanges;
#endif
        }

        private void OnEnable()
        {
            _isEditingWaterObject = target.GetType().Name == "Game2DWater";
            _isInspectingPrefab = false;

            foreach (Game2DWaterKitObject waterKitObject in targets)
            {
                bool isPrefab = waterKitObject.gameObject.scene.rootCount == 0;

#if UNITY_2018_3_OR_NEWER
                if (!waterKitObject.IsInitialized && !isPrefab)
                    waterKitObject.InitializeModules();
#else
                if (!waterKitObject.IsInitialized)
                    waterKitObject.InitializeModules();
#endif

                _isInspectingPrefab |= isPrefab;
            }

#if UNITY_2018_3_OR_NEWER
            if (_isInspectingPrefab)
                return;
#endif

            LoadSettings();

            Game2DWaterKitObjectResizeTool.RepaintInspector = Repaint;
            _foldoutsAnimBoolsCallback += Repaint;
            _isMultiEditing = targets.Length > 1;

            Game2DWaterKitMeshMaskTool.Initialize(target, Repaint);

            Initiliaze();

            EditorApplication.playModeStateChanged -= SaveSettings;
            EditorApplication.playModeStateChanged += SaveSettings;

            if (_isEditingMeshMask && _isInSimulationMode)
            {
                Game2DWaterKitSimulationPreviewMode.Stop();

                Game2DWaterKitSimulationPreviewMode.IterateSimulation = null;
                Game2DWaterKitSimulationPreviewMode.RestartSimulation = null;

                _isInSimulationMode = false;
            }
        }

        private void OnDisable()
        {
            _foldoutsAnimBoolsCallback -= Repaint;
            Tools.hidden = false;

            SaveSettings();
        }

        public override void OnInspectorGUI()
        {
#if UNITY_2018_3_OR_NEWER
            if (_isInspectingPrefab)
            {
                EditorGUILayout.HelpBox("Please open the prefab for full editing support.", MessageType.Info);
                return;
            }
#endif

            if (!Game2DWaterKitStyles.IsInitialized)
                Game2DWaterKitStyles.Initialize();

            if (_meshRendererSerializedObject == null)
            {
                if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Game2DWaterKitObject>() != null)
                    _meshRendererSerializedObject = new SerializedObject(Selection.gameObjects.Select(go => go.GetComponent<Renderer>()).ToArray());
                else
                    _meshRendererSerializedObject = new SerializedObject((target as Game2DWaterKitObject).GetComponent<Renderer>());

                _materialProperty = _meshRendererSerializedObject.FindProperty("m_Materials.Array.data[0]");
            }

            serializedObject.Update();
            _meshRendererSerializedObject.Update();

            if (!EditorApplication.isPlayingOrWillChangePlaymode && Game2DWaterKitManagerWindow.HasActionRequired())
            {
                BeginBoxGroup(true);
                EditorGUILayout.HelpBox("Action is required! " + Game2DWaterKitManagerWindow.ActionRequiredMessage, MessageType.Error);
                if (GUILayout.Button("Take Action!"))
                    Game2DWaterKitManagerWindow.ShowWindow();
                EndBoxGroup();
            }

            _isEditingMeshMask &= !EditorApplication.isPlayingOrWillChangePlaymode;

            DrawProperties();

            if (_isEditingMeshMask)
            {
                if (_isMultiEditing)
                    EditorGUILayout.HelpBox("Mesh Mask Shape Editor does not support multiediting.", MessageType.Info);

                EditorGUI.BeginDisabledGroup(_isMultiEditing);
                BeginPropertiesGroup(true, "Mesh Mask Shape Editor");
                DrawMeshMaskProperties();
                EndPropertiesGroup();
                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.BeginDisabledGroup(Game2DWaterKitManagerWindow.IsOpen);
            if (GUILayout.Button("Show Asset Manager Window"))
                Game2DWaterKitManagerWindow.ShowWindow();
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
            _meshRendererSerializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
#if UNITY_2018_3_OR_NEWER
            if (_isInspectingPrefab)
                return;
#endif

            Tools.hidden = _isEditingMeshMask;

            DrawSceneView();

            if (_previewMeshMaskOutline)
                Game2DWaterKitMeshMaskTool.DrawMeshMaskOutlinePreview();

            if (_previewObjectOutline)
                DrawObjectOutlinePreview();

            if (_previewEdgeCollider)
                DrawEdgeColliderPreview();

            if (_previewPolygonCollider)
                DrawPolygonColliderPreview();

            if (_isEditingMeshMask)
                Game2DWaterKitMeshMaskTool.DrawSceneViewHandles();
        }

        protected abstract void Initiliaze();
        protected abstract void DrawProperties();
        protected abstract void DrawSceneView();
        protected abstract void IterateSimulationPreview();
        protected abstract void RestartSimulationPreview();

        protected void DrawPrefabUtilityProperties()
        {
            bool showPrefabUtility = !_isMultiEditing && !_isInspectingPrefab;

#if UNITY_2018_3_OR_NEWER
#if UNITY_2021_2_OR_NEWER
            showPrefabUtility &= UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null;
#else
            showPrefabUtility &= UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null;
#endif
#endif

            if (showPrefabUtility)
                DrawPropertiesFadeGroup("Prefab Utility", DrawPrefabUtility, true, false, false);
        }

        protected void FrameTargetObject()
        {
#if UNITY_2018_1_OR_NEWER
            Selection.activeObject = target;
            SceneView.FrameLastActiveSceneView();
#endif
        }

        protected void DrawSimulationPreviewProperties(bool isWaterfall, System.Action DoExtraPropertiesLayout = null)
        {
            var isSimulationModeActive = !EditorApplication.isPlayingOrWillChangePlaymode && _isInSimulationMode && (isWaterfall == _isSimulationModeOwnedByWaterfallEditor);

            BeginPropertiesGroup(false, "Simulation Preview");

            if (_isInSimulationMode && isWaterfall != _isSimulationModeOwnedByWaterfallEditor)
                EditorGUILayout.HelpBox(string.Format("Another {0} simulation is running! It will stop if you enter this simulation mode!", _isSimulationModeOwnedByWaterfallEditor ? "waterfall" : "water"), MessageType.Info);

            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            isSimulationModeActive = GUI.Toggle(EditorGUILayout.GetControlRect(), isSimulationModeActive, isSimulationModeActive ? Game2DWaterKitStyles.PreviewSimulationOnButtonLabel : Game2DWaterKitStyles.PreviewSimulationOffButtonLabel, Game2DWaterKitStyles.ButtonStyle);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                if (isSimulationModeActive)
                {
                    FrameTargetObject();

                    if (Game2DWaterKitSimulationPreviewMode.IsRunning)
                        Game2DWaterKitSimulationPreviewMode.Stop();

                    Game2DWaterKitSimulationPreviewMode.IterateSimulation = IterateSimulationPreview;
                    Game2DWaterKitSimulationPreviewMode.RestartSimulation = RestartSimulationPreview;

                    Game2DWaterKitSimulationPreviewMode.Start();

                    _isInSimulationMode = true;
                    _isSimulationModeOwnedByWaterfallEditor = isWaterfall;

                }
                else
                {
                    Game2DWaterKitSimulationPreviewMode.Stop();

                    Game2DWaterKitSimulationPreviewMode.IterateSimulation = null;
                    Game2DWaterKitSimulationPreviewMode.RestartSimulation = null;

                    _isInSimulationMode = false;
                }
            }

            if (isSimulationModeActive)
            {
                EditorGUILayout.Space();

                if (DoExtraPropertiesLayout != null)
                    DoExtraPropertiesLayout.Invoke();

                BeginPropertiesGroup();

                var rect = EditorGUILayout.GetControlRect();
                float xMax = rect.xMax;

                rect.width = rect.width * 0.5f - 1f;

                EditorGUI.BeginChangeCheck();

                var targetFPSLabel = Game2DWaterKitStyles.SimulationModeTargetFrameratePropertyLabel;
                SetEditorGUIUtilityLabelWidth(targetFPSLabel.WidthRegular, true);
                EditorGUI.BeginChangeCheck();
                float targetFPS = EditorGUI.FloatField(rect, targetFPSLabel.Content, 1f / Game2DWaterKitSimulationPreviewMode.TimeStep);
                if (EditorGUI.EndChangeCheck())
                    Game2DWaterKitSimulationPreviewMode.TimeStep = 1f / Mathf.Clamp(targetFPS, 25f, float.MaxValue);

                rect.x += rect.width + 2f;

                var simulationTimeStepLabel = Game2DWaterKitStyles.SimulationModeTimeStepPropertyLabel;
                SetEditorGUIUtilityLabelWidth(simulationTimeStepLabel.WidthRegular, true);
                EditorGUI.BeginChangeCheck();
                float timeStep = EditorGUI.FloatField(rect, simulationTimeStepLabel.Content, Game2DWaterKitSimulationPreviewMode.TimeStep);
                if (EditorGUI.EndChangeCheck())
                    Game2DWaterKitSimulationPreviewMode.TimeStep = Mathf.Clamp(timeStep, 0.0001f, 0.04f);

                if (GUILayout.Button("Match to Project Fixed Delta Time"))
                    Game2DWaterKitSimulationPreviewMode.TimeStep = Time.fixedDeltaTime;

                if (EditorGUI.EndChangeCheck())
                    Game2DWaterKitSimulationPreviewMode.RestartSimulation();

                if (Game2DWaterKitSimulationPreviewMode.IsRunning)
                {
                    int speed = Mathf.Clamp(Mathf.RoundToInt(Game2DWaterKitSimulationPreviewMode.RelativeAnimationSpeed * 100f), 0, 100);
                    EditorGUILayout.HelpBox(string.Format("The simualtion is running at {0}% its actual speed (its target framerate)", speed), MessageType.Info);
                }

                EndPropertiesGroup();
            }

            EndPropertiesGroup();
        }

        protected void DrawSimulationPreviewStopRestartButtons()
        {
            EditorGUI.BeginDisabledGroup(!Game2DWaterKitSimulationPreviewMode.IsRunning);
            if (GUI.Button(EditorGUILayout.GetControlRect(), Game2DWaterKitStyles.StopSimulationButtonLabel))
                Game2DWaterKitSimulationPreviewMode.Stop();
            EditorGUI.EndDisabledGroup();

            if (GUI.Button(EditorGUILayout.GetControlRect(), Game2DWaterKitStyles.RestartSimulationButtonLabel))
                Game2DWaterKitSimulationPreviewMode.Restart();
        }

        protected void DrawSizeProperty()
        {
            var sizeProperty = serializedObject.FindProperty("_size");

            EditorGUI.BeginDisabledGroup(_isMultiEditing && sizeProperty.hasMultipleDifferentValues);

            Vector3 scale = (target as MonoBehaviour).transform.localScale;
            Vector2 size = Game2DWaterKitObjectResizeTool.IsResizing ? Vector2.Scale(sizeProperty.vector2Value, scale) : sizeProperty.vector2Value;

            if (!Game2DWaterKitObjectResizeTool.IsResizing && scale != Vector3.one)
            {
                EditorGUILayout.HelpBox("For consistent water behavior, please make sure that the scale is set to (1,1,1). You need tweak the size property instead.", MessageType.Warning);
                if (GUILayout.Button("Fix Scale"))
                {
                    sizeProperty.vector2Value = Vector2.Scale(sizeProperty.vector2Value, scale);
                    (target as MonoBehaviour).transform.localScale = Vector3.one;
                }
            }

            var rect = EditorGUILayout.GetControlRect();
            float xMax = rect.xMax;

            rect.xMax -= 27f;

            SetEditorGUIUtilityLabelWidth(sizeProperty, Game2DWaterKitStyles.SizePropertyLabel, true);
            var sizeLabel = EditorGUI.BeginProperty(rect, Game2DWaterKitStyles.SizePropertyLabel.Content, sizeProperty);
            rect = EditorGUI.PrefixLabel(rect, sizeLabel);
            EditorGUI.BeginChangeCheck();
            size = EditorGUI.Vector2Field(rect, string.Empty, size);
            if (EditorGUI.EndChangeCheck())
                sizeProperty.vector2Value = size;
            EditorGUI.EndProperty();

            rect.xMax = xMax;
            rect.xMin = xMax - 25f;

            bool editSize = Tools.current == Tool.Rect;
            EditorGUI.BeginChangeCheck();
            editSize = GUI.Toggle(rect, editSize, editSize ? Game2DWaterKitStyles.EditSizeIconOnButtonLabel : Game2DWaterKitStyles.EditSizeIconOffButtonLabel, Game2DWaterKitStyles.ButtonStyle);
            if (EditorGUI.EndChangeCheck())
            {
                if (editSize)
                {
                    FrameTargetObject();
                    Tools.current = Tool.Rect;
                }
                else Tools.current = Tool.View;
            }

            EditorGUI.EndDisabledGroup();
        }

        protected void DrawRenderTextureProperties(string propertyName)
        {
            BeginPropertiesGroup(false, "Render-Texture Properties");

            var renderTextureUseFixedSize = serializedObject.FindProperty(propertyName + "RenderTextureUseFixedSize");

            DrawProperty(renderTextureUseFixedSize, Game2DWaterKitStyles.RenderTextureUseFixedSizePropertyLabel);

            if (renderTextureUseFixedSize.boolValue)
                DrawProperty(propertyName + "RenderTextureFixedSize", Game2DWaterKitStyles.RenderTextureFixedSizePropertyLabel);
            else
                DrawProperty(propertyName + "RenderTextureResizeFactor", Game2DWaterKitStyles.RenderTextureResizingFactorPropertyLabel);

            DrawProperty(propertyName + "RenderTextureFilterMode", Game2DWaterKitStyles.RenderTextureFilterModePropertyLabel);

            EndPropertiesGroup();
        }

        protected void DrawRenderingModuleProperties(bool hasAnActiveEffect)
        {
            BeginPropertiesGroup();
            DrawProperty(_materialProperty, Game2DWaterKitStyles.MaterialPropertyLabel);
            EndPropertiesGroup();

            BeginPropertiesGroup();

            bool hasDifferentMaterials = _materialProperty.hasMultipleDifferentValues;
            bool disabled = hasDifferentMaterials || !hasAnActiveEffect;

            if (hasDifferentMaterials)
                EditorGUILayout.HelpBox(Game2DWaterKitStyles.CantMultiEditBecauseUsingDifferentMaterialMessage, MessageType.Info);

            EditorGUI.BeginDisabledGroup(disabled);
            DrawProperty("_renderingModuleFarClipPlane", Game2DWaterKitStyles.FarClipPlanePropertyLabel);

#if !GAME_2D_WATER_KIT_LWRP && !GAME_2D_WATER_KIT_URP
            DrawProperty("_renderingModuleRenderPixelLights", Game2DWaterKitStyles.RenderPixelLightsPropertyLabel);
#endif

            bool isAntiAliasingEnabledInQualitySettings = QualitySettings.antiAliasing > 1;
            EditorGUI.BeginDisabledGroup(!isAntiAliasingEnabledInQualitySettings);
            var allowMSAAProperty = serializedObject.FindProperty("_renderingModuleAllowMSAA");
            var allowMSAAPropertyRect = EditorGUILayout.GetControlRect();
            var allowMSAAPropertyLabel = EditorGUI.BeginProperty(allowMSAAPropertyRect, Game2DWaterKitStyles.AllowMSAAPropertyLabel.Content, allowMSAAProperty);
            SetEditorGUIUtilityLabelWidth(allowMSAAProperty, Game2DWaterKitStyles.AllowMSAAPropertyLabel);
            EditorGUI.BeginChangeCheck();
            bool allowMSAA = EditorGUI.Toggle(allowMSAAPropertyRect, allowMSAAPropertyLabel, isAntiAliasingEnabledInQualitySettings && allowMSAAProperty.boolValue);
            if (EditorGUI.EndChangeCheck())
                allowMSAAProperty.boolValue = allowMSAA;
            EditorGUI.EndProperty();
            EditorGUI.EndDisabledGroup();

            DrawProperty("_renderingModuleAllowHDR", Game2DWaterKitStyles.AllowHDRPropertyLabel);
            EditorGUI.EndDisabledGroup();

            EndPropertiesGroup();

            BeginPropertiesGroup();
            DrawSortingLayerField(serializedObject.FindProperty("_renderingModuleSortingLayerID"), serializedObject.FindProperty("_renderingModuleSortingOrder"));
            EndPropertiesGroup();

            var isMeshMaskActiveProperty = serializedObject.FindProperty("_meshMaskIsActive");
            BeginPropertiesGroup(false, "Mask Properties", isMeshMaskActiveProperty);
            if (_isMultiEditing)
                EditorGUILayout.HelpBox("Can't multi-edit mesh mask shapes.", MessageType.Info);
            EditorGUI.BeginDisabledGroup(!isMeshMaskActiveProperty.boolValue || _isMultiEditing);
            DrawMeshMaskProperties();
            EditorGUI.EndDisabledGroup();
            EndPropertiesGroup();
        }

        protected void DrawMeshMaskProperties()
        {
            var isMeshMaskActive = serializedObject.FindProperty("_meshMaskIsActive").boolValue;

            if (_materialProperty.objectReferenceValue != null)
            {
                var mat = _materialProperty.objectReferenceValue as UnityEngine.Material;

                int maskInteractionIndex = mat.GetInt("_SpriteMaskInteraction");

                if (maskInteractionIndex == 8)
                    maskInteractionIndex = 0;

                string maskInteractionDisplayName;
                if (maskInteractionIndex == 0)
                    maskInteractionDisplayName = "None";
                else if (maskInteractionIndex == 4)
                    maskInteractionDisplayName = "Visible Inside Mask";
                else
                    maskInteractionDisplayName = "Visible Outside Mask";

                MessageType messageType = isMeshMaskActive && maskInteractionIndex > 0 ? MessageType.Info : MessageType.Warning;

                EditorGUILayout.HelpBox(string.Format("The \"Mask Interaction\" property is set to \"{0}\", under the \"Rendering Options\" in the material inspector.", maskInteractionDisplayName), messageType);
            }

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);

            BeginBoxGroup(false);

            EditorGUI.BeginChangeCheck();
            _isEditingMeshMask = GUILayout.Toggle(_isEditingMeshMask, !_isEditingMeshMask ? "Edit Mask Shape" : "Exit Mask Shape Editor", "button");
            if (EditorGUI.EndChangeCheck())
                _previewMeshMaskOutline = false;

            EditorGUI.BeginDisabledGroup(!isMeshMaskActive);
            if (_isEditingMeshMask && GUILayout.Button("Reset Mask Shape"))
                Game2DWaterKitMeshMaskTool.ResetShape();
            EditorGUI.EndDisabledGroup();

            EndBoxGroup();

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!isMeshMaskActive);

            BeginBoxGroup(false);
            _previewObjectOutline = DrawLabelWithPreviewButton(_isEditingWaterObject ? "Water Outline" : "Waterfall Outline", _previewObjectOutline);
            _previewMeshMaskOutline = DrawLabelWithPreviewButton("Mask Outline", _previewMeshMaskOutline, !_isEditingMeshMask);
            EndBoxGroup();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);

            BeginBoxGroup(false);
            EditorGUI.BeginChangeCheck();
            var meshMaskArePositionAndSizeLockedProperty = serializedObject.FindProperty("_meshMaskArePositionAndSizeLocked");
            DrawProperty(meshMaskArePositionAndSizeLockedProperty, Game2DWaterKitStyles.MeshMaskArePositionAndSizeLockedPropertyLabel);
            if (EditorGUI.EndChangeCheck())
            {
                var meshMaskPositionProperty = serializedObject.FindProperty("_meshMaskPosition");
                var meshMaskSizeProperty = serializedObject.FindProperty("_meshMaskSize");
                var sizeProperty = serializedObject.FindProperty("_size");

                var targetObjectTransform = (target as Game2DWaterKitObject).transform;

                Vector3 scale = targetObjectTransform.lossyScale;
                Vector2 size = sizeProperty.vector2Value;

                if (meshMaskArePositionAndSizeLockedProperty.boolValue)
                {
                    meshMaskPositionProperty.vector3Value = targetObjectTransform.position;
                    meshMaskSizeProperty.vector3Value = new Vector3(size.x * scale.x, size.y * scale.y, 1f);
                }

                Game2DWaterKitMeshMaskTool.UpdateEdgeCollider(meshMaskArePositionAndSizeLockedProperty.boolValue, size, meshMaskSizeProperty.vector3Value);
                Game2DWaterKitMeshMaskTool.UpdatePolygonCollider(meshMaskArePositionAndSizeLockedProperty.boolValue, size, meshMaskSizeProperty.vector3Value);
            }
            EndBoxGroup();

            if (_isEditingMeshMask)
                Game2DWaterKitMeshMaskTool.DrawInspectorProperties();

            EditorGUI.EndDisabledGroup();

            EditorGUI.EndDisabledGroup();
        }

        protected void DrawTimeIntervalProperties(string propertyName)
        {
            BeginPropertiesGroup(false, "Time Interval Properties");

            var randomizeInterval = serializedObject.FindProperty(propertyName + "RandomizeTimeInterval");

            DrawProperty(randomizeInterval, Game2DWaterKitStyles.RandomizeTimeIntervalPropertyLabel);

            if (randomizeInterval.boolValue)
            {
                DrawProperty(propertyName + "MinimumTimeInterval", Game2DWaterKitStyles.MinimumTimeIntervalPropertyLabel, true);
                DrawProperty(propertyName + "MaximumTimeInterval", Game2DWaterKitStyles.MaximumTimeIntervalPropertyLabel, true);
            }
            else DrawProperty(propertyName + "TimeInterval", Game2DWaterKitStyles.TimeIntervalPropertyLabel, true);

            EndPropertiesGroup();
        }

        protected void DrawProperty(string propertyName, Game2DWaterKitStyles.Game2DWaterKitPropertyLabel propertyLabel, bool delayedField = false, bool forceLabelWidth = false)
        {
            DrawProperty(serializedObject.FindProperty(propertyName), propertyLabel, delayedField, forceLabelWidth);
        }

        protected void DrawProperty(Rect rect, string propertyName, Game2DWaterKitStyles.Game2DWaterKitPropertyLabel propertyLabel, bool delayedField = false, bool forceLabelWidth = false)
        {
            DrawProperty(rect, serializedObject.FindProperty(propertyName), propertyLabel, delayedField, forceLabelWidth);
        }

        protected static void DrawProperty(Rect rect, SerializedProperty property, Game2DWaterKitStyles.Game2DWaterKitPropertyLabel propertyLabel, bool delayedField = false, bool forceLabelWidth = false)
        {
            SetEditorGUIUtilityLabelWidth(property, propertyLabel, forceLabelWidth);

            if (delayedField && property.propertyType == SerializedPropertyType.Float)
                EditorGUI.DelayedFloatField(rect, property, propertyLabel.Content);
            else
                EditorGUI.PropertyField(rect, property, propertyLabel.Content, true);
        }

        protected static void DrawProperty(SerializedProperty property, Game2DWaterKitStyles.Game2DWaterKitPropertyLabel propertyLabel, bool delayedField = false, bool forceLabelWidth = false)
        {
            SetEditorGUIUtilityLabelWidth(property, propertyLabel, forceLabelWidth);

            if (delayedField && property.propertyType == SerializedPropertyType.Float)
                EditorGUILayout.DelayedFloatField(property, propertyLabel.Content);
            else
                EditorGUILayout.PropertyField(property, propertyLabel.Content, true);
        }

        protected void DrawPropertyLeftToggle(string propertyName, Game2DWaterKitStyles.Game2DWaterKitPropertyLabel propertyLabel, bool forceLabelWidth = false)
        {
            DrawPropertyLeftToggle(serializedObject.FindProperty(propertyName), propertyLabel, forceLabelWidth);
        }

        protected static void DrawPropertyLeftToggle(SerializedProperty property, Game2DWaterKitStyles.Game2DWaterKitPropertyLabel propertyLabel, bool forceLabelWidth = false)
        {
            SetEditorGUIUtilityLabelWidth(property, propertyLabel, forceLabelWidth);

            EditorGUI.BeginChangeCheck();
            bool value = EditorGUILayout.ToggleLeft(propertyLabel.Content, property.boolValue);
            if (EditorGUI.EndChangeCheck())
                property.boolValue = value;
        }

        protected static bool DrawPropertyWithPreviewButton(SerializedProperty property, Game2DWaterKitStyles.Game2DWaterKitPropertyLabel propertyLabel, bool previewState, bool isPreviewButtonEnabled = true)
        {
            var rect = DrawPropertyPrecedingActionButton(property, propertyLabel);

            return DrawPreviewButton(rect, previewState, isPreviewButtonEnabled);
        }

        protected static bool DrawLeftToggleWithPreviewButton(SerializedProperty property, Game2DWaterKitStyles.Game2DWaterKitPropertyLabel propertyLabel, bool previewState, bool isPreviewButtonEnabled = true)
        {
            var rect = EditorGUILayout.GetControlRect();

            float xMax = rect.xMax;

            rect.xMax -= 27;

            SetEditorGUIUtilityLabelWidth(property, propertyLabel);

            EditorGUI.BeginProperty(rect, propertyLabel.Content, property);
            EditorGUI.BeginChangeCheck();
            var propertyValue = EditorGUI.ToggleLeft(rect, propertyLabel.Content, property.boolValue);
            if (EditorGUI.EndChangeCheck())
                property.boolValue = propertyValue;
            EditorGUI.EndProperty();

            rect.xMax = xMax;
            rect.xMin = xMax - 25;

            return DrawPreviewButton(rect, previewState, isPreviewButtonEnabled);
        }

        protected static bool DrawLabelWithPreviewButton(string label, bool previewState, bool isPreviewButtonEnabled = true)
        {
            EditorGUI.BeginDisabledGroup(!isPreviewButtonEnabled);

            var rect = EditorGUILayout.GetControlRect();

            float xMax = rect.xMax;

            rect.xMax -= 27;
            EditorGUI.LabelField(rect, label);

            rect.xMax = xMax;
            rect.xMin = xMax - 25;
            previewState = DrawPreviewButton(rect, previewState);

            EditorGUI.EndDisabledGroup();

            return previewState;
        }

        protected static bool DrawPreviewButton(Rect rect, bool previewState, bool isPreviewButtonEnabled = true)
        {
            EditorGUI.BeginDisabledGroup(!isPreviewButtonEnabled);
            previewState = GUI.Toggle(rect, previewState, previewState ? Game2DWaterKitStyles.PreviewIconOnButtonLabel : Game2DWaterKitStyles.PreviewIconOffButtonLabel, Game2DWaterKitStyles.ButtonStyle);
            EditorGUI.EndDisabledGroup();

            return previewState;
        }

        protected static bool DrawPropertyWithActionButton(SerializedProperty property, Game2DWaterKitStyles.Game2DWaterKitPropertyLabel propertyLabel, GUIContent actionButtonLabel)
        {
            var rect = DrawPropertyPrecedingActionButton(property, propertyLabel);
            return GUI.Button(rect, actionButtonLabel);
        }

        protected static bool DrawSliderWithActionButton(ref float value, float min, float max, Game2DWaterKitStyles.Game2DWaterKitPropertyLabel propertyLabel, GUIContent actionButtonLabel)
        {
            var rect = EditorGUILayout.GetControlRect();

            float xMax = rect.xMax;

            rect.xMax -= 27;
            value = EditorGUI.Slider(rect, propertyLabel.Content, value, min, max);

            rect.xMax = xMax;
            rect.xMin = xMax - 25;
            return GUI.Button(rect, Game2DWaterKitStyles.RunSimulationButtonLabel);
        }

        protected static bool DrawPropertiesFadeGroup(string groupName, System.Action DoPropertiesLayout, bool useHelpBoxStyle = true, bool indent = false, bool bold = false, SerializedProperty groupToggleProperty = null)
        {
            bool hasChangedGroupToggleState = false;

            var fadeGroupAnimBool = GetFoldoutAnimBool(groupName);

            if (groupToggleProperty == null)
            {
                EditorGUI.BeginChangeCheck();
                if (indent)
                    EditorGUI.indentLevel++;
                var foldout = EditorGUILayout.Foldout(fadeGroupAnimBool.value, RemovePropertyID(groupName), true, bold ? Game2DWaterKitStyles.BoldFoldoutStyle : EditorStyles.foldout);
                if (indent)
                    EditorGUI.indentLevel--;
                if (EditorGUI.EndChangeCheck())
                    fadeGroupAnimBool.target = foldout;
            }
            else
            {
                var rect = EditorGUILayout.GetControlRect();

                if (indent)
                {
                    EditorGUI.indentLevel++;
                    rect = EditorGUI.IndentedRect(rect);
                    EditorGUI.indentLevel--;
                }

                float xmax = rect.xMax;

                rect.xMin += 2f;
                rect.xMax = rect.xMin + 14f;

                EditorGUI.BeginChangeCheck();
                DrawGroupToggle(rect, groupToggleProperty, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                    hasChangedGroupToggleState = true;

                rect.xMin -= 2f;
                rect.xMax = xmax;
                EditorGUI.BeginChangeCheck();

#if UNITY_2019_3_OR_NEWER
                string content = "      " + RemovePropertyID(groupName);
#else
                string content = "    " + RemovePropertyID(groupName);
#endif

                var foldout = EditorGUI.Foldout(rect, fadeGroupAnimBool.value, content, true, bold ? Game2DWaterKitStyles.BoldFoldoutStyle : EditorStyles.foldout);

                if (EditorGUI.EndChangeCheck())
                    fadeGroupAnimBool.target = foldout;
            }

            using (var group = new EditorGUILayout.FadeGroupScope(fadeGroupAnimBool.faded))
            {
                if (group.visible)
                {
                    BeginBoxGroup(useHelpBoxStyle);
                    bool isDisabled = groupToggleProperty == null ? false : !groupToggleProperty.boolValue;
                    EditorGUI.BeginDisabledGroup(isDisabled);
                    DoPropertiesLayout.Invoke();
                    EditorGUI.EndDisabledGroup();
                    EndBoxGroup();
                }
            }

            return hasChangedGroupToggleState;
        }

        protected static bool DrawPropertiesGroup(System.Action DoPropertiesLayout, bool useHelpBoxStyle = false, string groupName = null, SerializedProperty groupToggleProperty = null)
        {
            bool hasToggleStateChanged = BeginPropertiesGroup(useHelpBoxStyle, groupName, groupToggleProperty);
            DoPropertiesLayout.Invoke();
            EndPropertiesGroup();

            return hasToggleStateChanged;
        }

        protected static bool BeginPropertiesGroup(bool useHelpBoxStyle = false, string groupName = null, SerializedProperty groupToggleProperty = null)
        {
            bool hasToggleStateChanges = false;

            if (!string.IsNullOrEmpty(groupName))
            {
                Rect labelRect = EditorGUILayout.GetControlRect();

                if (!useHelpBoxStyle)
                {
                    labelRect.x += 3f;
                    labelRect.y += 5f;
                }

                if (groupToggleProperty == null)
                    EditorGUI.LabelField(labelRect, RemovePropertyID(groupName), EditorStyles.boldLabel);
                else
                    hasToggleStateChanges = DrawGroupToggle(labelRect, groupToggleProperty, Game2DWaterKitStyles.GetTempLabel(RemovePropertyID(groupName)), true);
            }

            BeginBoxGroup(useHelpBoxStyle);

            return hasToggleStateChanges;
        }

        protected static void EndPropertiesGroup()
        {
            EndBoxGroup();
        }

        protected static void SetEditorGUIUtilityLabelWidth(float labelWidth, bool forceUsingLabelWidth = false)
        {
            if (forceUsingLabelWidth)
                EditorGUIUtility.labelWidth = labelWidth;
            else
                EditorGUIUtility.labelWidth = labelWidth > Game2DWaterKitStyles.MinimumLabelWidth ? labelWidth : Game2DWaterKitStyles.MinimumLabelWidth;
        }

        protected static void SetEditorGUIUtilityLabelWidth(SerializedProperty property, Game2DWaterKitStyles.Game2DWaterKitPropertyLabel label, bool forceUsingLabelWidth = false)
        {
            SetEditorGUIUtilityLabelWidth(property.prefabOverride ? label.WidthBold : label.WidthRegular, forceUsingLabelWidth);
        }

        protected static void DrawSortingLayerField(SerializedProperty layerID, SerializedProperty orderInLayer)
        {
            MethodInfo methodInfo = typeof(EditorGUILayout).GetMethod("SortingLayerField", BindingFlags.Static | BindingFlags.NonPublic, null, new[] {
                typeof( GUIContent ),
                typeof( SerializedProperty ),
                typeof( GUIStyle ),
                typeof( GUIStyle )
            }, null);

            if (methodInfo != null)
            {
                var orderInLayerLabel = Game2DWaterKitStyles.SortingOrderInLayerPropertyLabel;
                var sortingLayerLabel = Game2DWaterKitStyles.SortingLayerPropertyLabel;

                SetEditorGUIUtilityLabelWidth(layerID, sortingLayerLabel);
                object[] parameters = { sortingLayerLabel.Content, layerID, EditorStyles.popup, EditorStyles.label };
                methodInfo.Invoke(null, parameters);

                DrawProperty(orderInLayer, orderInLayerLabel);
            }
        }

        protected static string[] GetAllLayersNamesInMask(int mask)
        {
            List<string> layers = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                if (mask == (mask | (1 << i)) && !string.IsNullOrEmpty(LayerMask.LayerToName(i)))
                {
                    layers.Add(LayerMask.LayerToName(i));
                }
            }
            return layers.ToArray();
        }

        protected static bool MaskContainsLayer(int mask, int layerIndex)
        {
            return mask == (mask | (1 << layerIndex));
        }

        protected static int LayerMaskToConcatenatedLayersMask(int mask, string[] displayedOptions)
        {
            int concatenatedMask = 0;

            for (int i = 0; i < displayedOptions.Length; i++)
            {
                int layer = LayerMask.NameToLayer(displayedOptions[i]);
                if (MaskContainsLayer(mask, layer))
                {
                    concatenatedMask |= (1 << i);
                }
            }

            return concatenatedMask;
        }

        protected static int ConcatenatedLayersMaskToLayerMask(int concatMask, string[] displayedOptions)
        {
            int mask = 0;
            for (int i = 0; i < displayedOptions.Length; i++)
            {
                if (MaskContainsLayer(concatMask, i))
                {
                    mask |= (1 << LayerMask.NameToLayer(displayedOptions[i]));
                }
            }
            return mask;
        }

        protected static void DrawRectInSceneView(Vector2 position, Vector2 size, Color color)
        {
            Vector2 topLeft = position + new Vector2(0f, size.y);
            Vector2 topRight = position + size;
            Vector3 bottomRight = position + new Vector2(size.x, 0f);

            Handles.color = color;
            Handles.DrawPolyLine(position, topLeft, topRight, bottomRight, position);
        }

        private static Rect DrawPropertyPrecedingActionButton(SerializedProperty property, Game2DWaterKitStyles.Game2DWaterKitPropertyLabel propertyLabel)
        {
            var rect = EditorGUILayout.GetControlRect();

            float xMax = rect.xMax;

            rect.xMax -= 27;
            DrawProperty(rect, property, propertyLabel);

            rect.xMax = xMax;
            rect.xMin = xMax - 25;

            return rect;
        }

        private static void BeginBoxGroup(bool useHelpBoxStyle = true)
        {
            EditorGUILayout.BeginVertical(useHelpBoxStyle ? Game2DWaterKitStyles.HelpBoxStyle : Game2DWaterKitStyles.GroupBoxStyle);
        }

        private static void EndBoxGroup()
        {
            EditorGUILayout.EndVertical();
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

                foldoutAnimBool = new AnimBool(() => _foldoutsAnimBoolsCallback.Invoke());
                foldoutAnimBool.value = isUnfolded;

                _foldoutsAnimBools.Add(name, foldoutAnimBool);
            }

            return foldoutAnimBool;
        }

        private static string RemovePropertyID(string groupName)
        {
            int hashIndex = groupName.IndexOf('#');
            return hashIndex == -1 ? groupName : groupName.Substring(0, hashIndex);
        }

        private static bool DrawGroupToggle(Rect rect, SerializedProperty groupToggleProperty, GUIContent label, bool boldLabel = false)
        {
            bool hasToggleStateChanged = false;

            EditorGUI.showMixedValue = groupToggleProperty.hasMultipleDifferentValues;

            EditorGUI.BeginChangeCheck();
            bool isEnabled = EditorGUI.ToggleLeft(rect, label, groupToggleProperty.boolValue, boldLabel ? EditorStyles.boldLabel : EditorStyles.label);
            if (EditorGUI.EndChangeCheck())
            {
                groupToggleProperty.boolValue = isEnabled;
                hasToggleStateChanged = true;
            }

            EditorGUI.showMixedValue = false;

            return hasToggleStateChanged;
        }

        private static string GetValidAssetFileName(string prefabsPath, string assetName, string assetExtension, System.Type assetType)
        {
            string fileName = assetName;

            string path = prefabsPath + fileName + assetExtension;
            bool prefabWithSameNameExist = AssetDatabase.LoadAssetAtPath(path, assetType) != null;
            if (prefabWithSameNameExist)
            {
                int i = 1;
                while (prefabWithSameNameExist)
                {
                    fileName = assetName + " " + i;
                    path = prefabsPath + fileName + assetExtension;
                    prefabWithSameNameExist = AssetDatabase.LoadAssetAtPath(path, assetType) != null;
                    i++;
                }
            }

            return fileName;
        }

        private void DrawObjectOutlinePreview()
        {
            var targetTransform = (target as Game2DWaterKitObject).transform;

            var vertices = targetTransform.GetComponent<MeshFilter>().sharedMesh.vertices;
            using (new Handles.DrawingScope(Game2DWaterKitStyles.MeshMaskToolObjectOutlinePreviewColor, targetTransform.localToWorldMatrix))
            {
                if (_isEditingWaterObject)
                {
                    var halfVertexCount = vertices.Length / 2;
                    Handles.DrawAAPolyLine(5f, halfVertexCount, vertices);
                    Handles.DrawAAPolyLine(5f, vertices[halfVertexCount - 1], vertices[vertices.Length - 1], vertices[halfVertexCount], vertices[0]);
                }
                else
                {
                    Handles.DrawAAPolyLine(5f, vertices);
                    Handles.DrawAAPolyLine(5f, vertices[vertices.Length - 1], vertices[0]);
                }
            }
        }

        private void DrawEdgeColliderPreview()
        {
            var targetTransform = (target as Game2DWaterKitObject).transform;
            var edgeCollider = targetTransform.GetComponent<EdgeCollider2D>();

            if (edgeCollider == null)
                return;

            using (new Handles.DrawingScope(Game2DWaterKitStyles.MeshMaskToolEdgeColliderOutlinePreviewColor, targetTransform.localToWorldMatrix))
            {
                var points = edgeCollider.points;
                var offset = edgeCollider.offset;

                for (int i = 0, imax = points.Length - 1; i < imax; i++)
                {
                    var nextIndex = i + 1;

                    Handles.DrawAAPolyLine(5f, points[i] + offset, points[nextIndex] + offset);
                }
            }
        }

        private void DrawPolygonColliderPreview()
        {
            var targetTransform = (target as Game2DWaterKitObject).transform;
            var polygonCollider = targetTransform.GetComponent<PolygonCollider2D>();

            if (polygonCollider == null)
                return;

            using (new Handles.DrawingScope(Game2DWaterKitStyles.MeshMaskToolPolygonColliderOutlinePreviewColor, targetTransform.localToWorldMatrix))
            {
                for (int i = 0, imax = polygonCollider.pathCount; i < imax; i++)
                {
                    var points = polygonCollider.GetPath(i);
                    var offset = polygonCollider.offset;

                    for (int j = 0, jmax = points.Length; j < jmax; j++)
                    {
                        var nextIndex = j + 1 < jmax ? j + 1 : 0;

                        Handles.DrawAAPolyLine(5f, points[j] + offset, points[nextIndex] + offset);
                    }
                }
            }
        }

        private static void SaveSettings(PlayModeStateChange playModeStateChange)
        {
            SaveSettings();
        }

        private static void SaveSettings()
        {
            foreach (var foldoutState in _foldoutsAnimBools)
            {
                if (foldoutState.Value.value)
                    EditorPrefs.SetBool(foldoutState.Key, foldoutState.Value.value);
            }

            EditorPrefs.SetInt("Water2D_MeshMaskEditor_SnapMode", Game2DWaterKitMeshMaskTool.snapMode);
            EditorPrefs.SetFloat("Water2D_MeshMaskEditor_SnapToGridStepSizeX", Game2DWaterKitMeshMaskTool.snapToGridStepSize.x);
            EditorPrefs.SetFloat("Water2D_MeshMaskEditor_SnapToGridStepSizeY", Game2DWaterKitMeshMaskTool.snapToGridStepSize.y);
        }

        private static void LoadSettings()
        {
            Game2DWaterKitMeshMaskTool.snapMode = EditorPrefs.GetInt("Water2D_MeshMaskEditor_SnapMode", 0);
            Game2DWaterKitMeshMaskTool.snapToGridStepSize.x = EditorPrefs.GetFloat("Water2D_MeshMaskEditor_SnapToGridStepSizeX", 0.5f);
            Game2DWaterKitMeshMaskTool.snapToGridStepSize.y = EditorPrefs.GetFloat("Water2D_MeshMaskEditor_SnapToGridStepSizeY", 0.5f);
        }

        private void DrawPrefabUtility()
        {
            string prefabsPath = EditorPrefs.GetString("Water2D_Paths_PrefabUtility_Path", "Assets/");

            GameObject go = (target as MonoBehaviour).gameObject;

            var material = go.GetComponent<MeshRenderer>().sharedMaterial;

            Texture waterNoiseTexture = material != null && material.HasProperty("_NoiseTexture") ? material.GetTexture("_NoiseTexture") : null;

#if UNITY_2018_3_OR_NEWER
            bool isPrefabInstance = PrefabUtility.GetPrefabInstanceStatus(go) == PrefabInstanceStatus.Connected;
#else
            PrefabType prefabType = PrefabUtility.GetPrefabType(go);
            bool isPrefabInstance = prefabType == PrefabType.PrefabInstance;
            bool isPrefabInstanceDisconnected = prefabType == PrefabType.DisconnectedPrefabInstance;
#endif

            bool materialAssetAlreadyExist = material != null && AssetDatabase.Contains(material);
            bool textureAssetAlreadyExist = waterNoiseTexture != null && AssetDatabase.Contains(waterNoiseTexture);

            if (!isPrefabInstance)
            {
                EditorGUILayout.HelpBox("You can use this utility, or drag the object to the project view to save it as a prefab.", MessageType.Info);
            }

            EditorGUI.BeginDisabledGroup(true);
#if UNITY_2018_2_OR_NEWER
            Object prefabObjct = isPrefabInstance ? PrefabUtility.GetCorrespondingObjectFromSource(go) : null;
#else
            Object prefabObjct = isPrefabInstance ? PrefabUtility.GetPrefabParent(go) : null;
#endif
            EditorGUILayout.ObjectField(prefabObjct, typeof(Object), false);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(prefabsPath, string.Format("Prefab Path: {0}", prefabsPath)), EditorStyles.textField);
            if (GUILayout.Button(".", EditorStyles.miniButton, GUILayout.MaxWidth(14f)))
            {
                string newPrefabsPath = EditorUtility.OpenFolderPanel("Select prefabs path", "Assets", "");
                if (!string.IsNullOrEmpty(newPrefabsPath))
                {
                    newPrefabsPath = newPrefabsPath.Substring(Application.dataPath.Length);
                    prefabsPath = "Assets" + newPrefabsPath + "/";
                    EditorPrefs.SetString("Water2D_Paths_PrefabUtility_Path", prefabsPath);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (!isPrefabInstance)
            {
                if (GUILayout.Button("Create Prefab"))
                {
                    string fileName = GetValidAssetFileName(prefabsPath, go.name, ".prefab", typeof(GameObject));

                    if (!textureAssetAlreadyExist && waterNoiseTexture != null)
                    {
                        string noiseTexturePath = prefabsPath + fileName + "_noiseTexture.asset";
                        AssetDatabase.CreateAsset(waterNoiseTexture, noiseTexturePath);
                    }

                    if (!materialAssetAlreadyExist && material != null)
                    {
                        string materialPath = prefabsPath + fileName + ".mat";
                        AssetDatabase.CreateAsset(material, materialPath);
                    }

                    string prefabPath = prefabsPath + fileName + ".prefab";
#if UNITY_2018_3_OR_NEWER
                    PrefabUtility.SaveAsPrefabAssetAndConnect(go, prefabPath, InteractionMode.AutomatedAction);
#else
                    PrefabUtility.CreatePrefab(prefabPath, go, ReplacePrefabOptions.ConnectToPrefab);
#endif
                }
            }
#if UNITY_2018_3_OR_NEWER
            /*
            As of Unity 2018.3, disconnecting (unlinking) and relinking a Prefab instance are no longer supported.
            Alternatively, we can now unpack a Prefab instance if we want to entirely remove its link to its Prefab asset 
            and thus be able to restructure the resulting plain GameObject as we please.
            */
            if (isPrefabInstance)
            {
                EditorGUILayout.HelpBox(Game2DWaterKitStyles.NewPrefabWorkflowMessage, MessageType.Info);
            }
#else
            if (isPrefabInstance)
            {
                if (GUILayout.Button("Unlink Prefab"))
                {
#if UNITY_2018_2_OR_NEWER
                    GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(go) as GameObject;
#else
                    GameObject prefab = PrefabUtility.GetPrefabParent(go) as GameObject;
#endif
                    PrefabUtility.DisconnectPrefabInstance(go);
                    UnityEngine.Material prefabMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;
                    if (material != null && material == prefabMaterial)
                    {
                        bool usePrefabMaterial = EditorUtility.DisplayDialog("Use same prefab's material?",
                    "Do you still want to use the prefab's material?",
                    "Yes",
                    "No, create water's own material");

                        if (!usePrefabMaterial)
                        {
                            UnityEngine.Material duplicateMaterial = new UnityEngine.Material(material);
                            if (waterNoiseTexture != null)
                            {
                                Texture duplicateWaterNoiseTexture = Instantiate<Texture>(waterNoiseTexture);
                                duplicateMaterial.SetTexture("_NoiseTexture", duplicateWaterNoiseTexture);
                            }
                            go.GetComponent<MeshRenderer>().sharedMaterial = duplicateMaterial;
                        }
                    }
                }
            }

            if (isPrefabInstanceDisconnected)
            {
                if (GUILayout.Button("Relink Prefab"))
                {
                    PrefabUtility.ReconnectToLastPrefab(go);
#if UNITY_2018_2_OR_NEWER
                    GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(go) as GameObject;
#else
                    GameObject prefab = PrefabUtility.GetPrefabParent(go) as GameObject;
#endif
                    UnityEngine.Material prefabMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;

                    if (prefabMaterial != null && material != prefabMaterial)
                    {
                        bool usePrefabMaterial = EditorUtility.DisplayDialog("Use prefab's material?",
                        "Do you want to use the prefab's material?",
                        "Yes",
                        "No, continue to use the current water material");

                        if (usePrefabMaterial)
                        {
                            go.GetComponent<MeshRenderer>().sharedMaterial = prefabMaterial;
                        }
                        else
                        {
                            if (!materialAssetAlreadyExist)
                            {
                                string fileName = GetValidAssetFileName(prefabsPath, go.name, ".mat", typeof(UnityEngine.Material));

                                if (!textureAssetAlreadyExist)
                                {
                                    string noiseTexturePath = prefabsPath + fileName + "_noiseTexture.asset";
                                    AssetDatabase.CreateAsset(waterNoiseTexture, noiseTexturePath);
                                }

                                string materialPath = prefabsPath + fileName + ".mat";
                                AssetDatabase.CreateAsset(material, materialPath);
                            }
                        }
                    }
                }
            }
#endif
        }

        protected System.Action<Arg> CreateDelegate<T, Arg>(T instance, string methodName)
        {
            return (System.Action<Arg>)System.Delegate.CreateDelegate(typeof(System.Action<Arg>), instance, methodName);
        }

        protected System.Action<Arg1, Arg2> CreateDelegate<T, Arg1, Arg2>(T instance, string methodName)
        {
            return (System.Action<Arg1, Arg2>)System.Delegate.CreateDelegate(typeof(System.Action<Arg1, Arg2>), instance, methodName);
        }

        protected System.Action CreateDelegate<T>(T instance, string methodName)
        {
            return (System.Action)System.Delegate.CreateDelegate(typeof(System.Action), instance, methodName);
        }

        protected System.Func<Arg> CreatePropertyGetterDelegate<T, Arg>(T instance, string propertyName)
        {
            return (System.Func<Arg>)System.Delegate.CreateDelegate(typeof(System.Func<Arg>), instance, "get_" + propertyName);
        }
    }
}
