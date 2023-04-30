namespace Game2DWaterKit
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;

    [CanEditMultipleObjects, CustomEditor(typeof(Game2DWaterfall))]
    internal class Game2DWaterfallInspector : Game2DWaterKitInspector
    {
        private static bool _isWaterfallSimulationModeActive;

        private ReorderableList _affectedWaterObjectsList;

        private System.Action<float>[] _ripplesModulePhysicsUpdateDelegates;
        private System.Action<float>[][] _simulationModulePhysicsUpdateDelegates;
        private System.Action[][] _simulationModuleResetSimulationDelegates;
        private System.Action[][] _meshModuleUpdateMeshDelegates;
        private bool _updateSimulationModeDelegates;

        private static string[] _edgesNames = new[] { "Top Edge", "Bottom Edge" };

        protected override void Initiliaze()
        {
            InitializeSimulationModeDelegates();
        }

        protected override void DrawProperties()
        {
            _isWaterfallSimulationModeActive = _isInSimulationMode && _isSimulationModeOwnedByWaterfallEditor;

            BeginPropertiesGroup(true);
            DrawSizeProperty();
            DrawTopBottomEdgesRelativeLength();
            EndPropertiesGroup();

            if (!_isEditingMeshMask)
            {
                if (!_isWaterfallSimulationModeActive)
                {
                    DrawPropertiesFadeGroup("Refraction Properties", DrawRefractionProperties);
                    DrawPropertiesFadeGroup("Rendering Properties", DrawRenderingProperties);
                    DrawPropertiesFadeGroup("Affected Water Objects", DrawAffectedWaterObjectsProperties, true, false, false, serializedObject.FindProperty("_isRipplesModuleActive"));
                    DrawPrefabUtilityProperties();
                }
                else DrawPropertiesGroup(DrawAffectedWaterObjectsProperties, true, "Affected Water Objects", serializedObject.FindProperty("_isRipplesModuleActive"));
            }
        }

        protected override void DrawSceneView() {}

        private void DrawTopBottomEdgesRelativeLength()
        {
            var topBottomEdgesRelativeWidthProperty = serializedObject.FindProperty("_meshModuleTopBottomEdgesRelativeLength");

            var isTopEdgeSelected = topBottomEdgesRelativeWidthProperty.vector2Value.y == 1f;
            var relativeWidth = topBottomEdgesRelativeWidthProperty.vector2Value.x;

            EditorGUI.BeginChangeCheck();

            var rect = EditorGUILayout.GetControlRect();
            var xMax = rect.xMax;

            rect.xMax = rect.xMin + 110f;
            EditorGUIUtility.labelWidth = 90f;
            EditorGUI.LabelField(rect, "Relative Length Of");

            rect.xMin = rect.xMax + 3f;
            rect.xMax = rect.xMin + 95f;
            isTopEdgeSelected = EditorGUI.Popup(rect, isTopEdgeSelected ? 0 : 1, _edgesNames) == 0;

            rect.xMin = rect.xMax + 3f;
            rect.xMax = xMax;
            relativeWidth = EditorGUI.Slider(rect, relativeWidth, 0f, 1f);

            if (EditorGUI.EndChangeCheck())
                topBottomEdgesRelativeWidthProperty.vector2Value = new Vector2(relativeWidth, isTopEdgeSelected ? 1f : 0f);
        }

        private void DrawRefractionProperties()
        {
            bool hasDifferentMaterials = _materialProperty.hasMultipleDifferentValues;
            bool hasRefraction = false;

            if (!hasDifferentMaterials)
            {
                var material = _materialProperty.objectReferenceValue as UnityEngine.Material;
                hasRefraction = material.IsKeywordEnabled("Waterfall2D_Refraction");
            }
            else EditorGUILayout.HelpBox(Game2DWaterKitStyles.CantMultiEditBecauseUsingDifferentMaterialMessage, MessageType.Info);

            if (!hasDifferentMaterials && !hasRefraction)
                EditorGUILayout.HelpBox(Game2DWaterKitStyles.DisabledRefractionMessage, MessageType.Info);

            EditorGUI.BeginDisabledGroup(!hasRefraction);

            BeginPropertiesGroup(false);
            DrawProperty("_renderingModuleRefractionCullingMask", Game2DWaterKitStyles.RefractionReflectionMaskPropertyLabel);
            EndPropertiesGroup();

            DrawRenderTextureProperties("_renderingModuleRefraction");

            EditorGUI.EndDisabledGroup();
        }

        private void DrawRenderingProperties()
        {
            var material = _materialProperty.objectReferenceValue as UnityEngine.Material;
            bool isRefractionEnabled = material.IsKeywordEnabled("Waterfall2D_Refraction");
            DrawRenderingModuleProperties(isRefractionEnabled);
        }

        private void DrawAffectedWaterObjectsProperties()
        {
            DrawSimulationPreviewProperties(true, DrawSimulationPreviewStopRestartButtons);

            var affectedWaterObjects = serializedObject.FindProperty("_ripplesModuleAffectedWaterObjects");

            if (_affectedWaterObjectsList == null)
            {
                _affectedWaterObjectsList = new ReorderableList(serializedObject, affectedWaterObjects, false, true, true, true);
                _affectedWaterObjectsList.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "Water Objects List", EditorStyles.miniBoldLabel); };
                _affectedWaterObjectsList.drawElementCallback = (rect, index, active, focused) => { EditorGUI.PropertyField(rect, affectedWaterObjects.GetArrayElementAtIndex(index), true); };
                _affectedWaterObjectsList.onChangedCallback = (l) => _updateSimulationModeDelegates = true;
                AffectedWaterObjectProperty.OnChanged = () => _updateSimulationModeDelegates = true;
            }

            if (!_isWaterfallSimulationModeActive)
            {
                BeginPropertiesGroup();
                DrawProperty("_ripplesModuleUpdateWhenOffscreen", Game2DWaterKitStyles.WaterfallAffectedWaterObjectRippleUpdateWhenOffscreenPropertyLabel);
                EndPropertiesGroup();
            }

            DrawTimeIntervalProperties("_ripplesModule");

            BeginPropertiesGroup();
            _affectedWaterObjectsList.elementHeight = affectedWaterObjects.arraySize > 0 ? EditorGUIUtility.singleLineHeight * 5f + 14f : 18f;
            _affectedWaterObjectsList.DoLayoutList();
            EndPropertiesGroup();
        }

        protected override void IterateSimulationPreview()
        {
            if (_updateSimulationModeDelegates)
                RestartSimulationPreview();

            float deltaTime = Game2DWaterKitSimulationPreviewMode.TimeStep;

            for (int i = 0, imax = targets.Length; i < imax; i++)
            {
                var waterfallObject = targets[i] as Game2DWaterfall;
                _ripplesModulePhysicsUpdateDelegates[i](deltaTime);

                var affectedWaterObjects = waterfallObject.RipplesModule.AffectedWaterObjects;

                var simulationModuleFixedUpdateDelegates = _simulationModulePhysicsUpdateDelegates[i];
                var meshModuleUpdateMeshDelegates = _meshModuleUpdateMeshDelegates[i];

                for (int j = 0, jmax = affectedWaterObjects.Count; j < jmax; j++)
                {
                    var affectedWaterObject = affectedWaterObjects[j].waterObject;
                    if(affectedWaterObject != null)
                    {
                        simulationModuleFixedUpdateDelegates[j](deltaTime);
                        meshModuleUpdateMeshDelegates[j]();
                    }
                }
            }
        }

        protected override void RestartSimulationPreview()
        {
            for (int i = 0, imax = targets.Length; i < imax; i++)
            {
                var simulationModuleResetSimulationDelegates = _simulationModuleResetSimulationDelegates[i];
                var meshModuleUpdateMeshDelegates = _meshModuleUpdateMeshDelegates[i];
                for (int j = 0, jmax = simulationModuleResetSimulationDelegates.Length; j < jmax; j++)
                {
                    if (simulationModuleResetSimulationDelegates[j] != null)
                    {
                        simulationModuleResetSimulationDelegates[j]();
                        meshModuleUpdateMeshDelegates[j]();
                    }
                }
            }

            if (_updateSimulationModeDelegates)
                InitializeSimulationModeDelegates();
        }

        private void InitializeSimulationModeDelegates()
        {
            int targetCount = targets.Length;

            _ripplesModulePhysicsUpdateDelegates = new System.Action<float>[targetCount];
            _simulationModulePhysicsUpdateDelegates = new System.Action<float>[targetCount][];
            _simulationModuleResetSimulationDelegates = new System.Action[targetCount][];
            _meshModuleUpdateMeshDelegates = new System.Action[targetCount][];

            for (int i = 0, imax = targetCount; i < imax; i++)
            {
                var waterfallRipplesModule = (targets[i] as Game2DWaterfall).RipplesModule;

                _ripplesModulePhysicsUpdateDelegates[i] = CreateDelegate<Ripples.WaterfallRipplesModule, float>(waterfallRipplesModule, "PhysicsUpdate");
                
                var affectedWaterObjects = waterfallRipplesModule.AffectedWaterObjects;
                int affectedWaterObjectCount = affectedWaterObjects.Count;

                var simulationModuleFixedUpdateDelegates = new System.Action<float>[affectedWaterObjectCount];
                var simulationModuleResetSimulationDelegates = new System.Action[affectedWaterObjectCount];
                var meshModuleUpdateMeshDelegates = new System.Action[affectedWaterObjectCount];

                for (int j = 0, jmax = affectedWaterObjectCount; j < jmax; j++)
                {
                    var affectedWaterObject = affectedWaterObjects[j].waterObject;

                    if (affectedWaterObject == null)
                        continue;

                    if (!affectedWaterObject.IsInitialized)
                        affectedWaterObject.InitializeModules();

                    var simulationModule = affectedWaterObject.SimulationModule;
                    var meshModule = affectedWaterObject.MeshModule;

                    simulationModuleFixedUpdateDelegates[j] = CreateDelegate<Simulation.WaterSimulationModule, float>(simulationModule, "PhysicsUpdate");
                    simulationModuleResetSimulationDelegates[j] = CreateDelegate<Simulation.WaterSimulationModule>(simulationModule, "ResetSimulation");
                    meshModuleUpdateMeshDelegates[j] = CreateDelegate<Mesh.WaterMeshModule>(meshModule, "UpdateMesh");
                }

                _simulationModulePhysicsUpdateDelegates[i] = simulationModuleFixedUpdateDelegates;
                _simulationModuleResetSimulationDelegates[i] = simulationModuleResetSimulationDelegates;
                _meshModuleUpdateMeshDelegates[i] = meshModuleUpdateMeshDelegates;
            }

            _updateSimulationModeDelegates = false;
        }

        [CustomPropertyDrawer(typeof(Ripples.WaterfallAffectedWaterObjet))]
        public class AffectedWaterObjectProperty : PropertyDrawer
        {
            public static System.Action OnChanged;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                float singleLineHeight = EditorGUIUtility.singleLineHeight;

                position.height = singleLineHeight;

                EditorGUI.BeginProperty(position, label, property);

                EditorGUI.BeginChangeCheck();

                position.y += 7f;
                DrawProperty(position, property.FindPropertyRelative("waterObject"), Game2DWaterKitStyles.WaterfallAffectedWaterObjectPropertyLabel);

                if (EditorGUI.EndChangeCheck() && OnChanged != null)
                    OnChanged.Invoke();

                position.y += singleLineHeight + 1f;
                DrawProperty(position, property.FindPropertyRelative("minimumDisturbance"), Game2DWaterKitStyles.MinimumDisturbancePropertyLabel);

                position.y += singleLineHeight + 1f;
                DrawProperty(position, property.FindPropertyRelative("maximumDisturbance"), Game2DWaterKitStyles.MaximumDisturbancePropertyLabel);

                position.y += singleLineHeight + 1f;
                DrawProperty(position, property.FindPropertyRelative("spread"), Game2DWaterKitStyles.WaterfallAffectedWaterObjectRippleSpreadPropertyLabel);

                position.y += singleLineHeight + 1f;
                float xMax = position.xMax;

                var smoothRipplesProperty = property.FindPropertyRelative("smoothRipples");
                SetEditorGUIUtilityLabelWidth(smoothRipplesProperty, Game2DWaterKitStyles.SmoothRipplesPropertyLabel);
                position.width = EditorGUIUtility.labelWidth + 16f;
                EditorGUI.PropertyField(position, smoothRipplesProperty, Game2DWaterKitStyles.SmoothRipplesPropertyLabel.Content);

                position.x += position.width;
                position.xMax = xMax;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("smoothingFactor"), GUIContent.none);

                EditorGUI.EndProperty();
            }
        }
    }

}