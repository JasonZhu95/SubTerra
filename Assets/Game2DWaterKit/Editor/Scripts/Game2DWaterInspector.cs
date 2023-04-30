namespace Game2DWaterKit
{
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Reflection;

    [CanEditMultipleObjects, CustomEditor(typeof(Game2DWater))]
    internal class Game2DWaterInspector : Game2DWaterKitInspector
    {
        private static bool _previewBuoyancyEffectorSurfaceLevel;
        private static bool _previewBoxCollider;
        private static bool _previewWaterSubdivisionsPerUnit;
        private static bool _previewPartiallySubmergedObjectsReflectionFrustum;
        private static bool _previewReflectionFrustum;
        private static bool _previewCustomBoundaries;
        private static bool _constantRipplesEditSourcesPositionsInSceneView;

        private static WaveToPreviewType _simulationActiveRippleType;
        private static float _collisionRipplesSimulationRegionMin = -0.05f;
        private static float _collisionRipplesSimulationRegionMax = 0.05f;
        private static float _scriptGeneratedRipplesSimulationPosition = 0f;
        private static float _scriptGeneratedRipplesDisturbanceFactor = 0.5f;
        private static bool _scriptGeneratedRipplesSmoothRipples = true;
        private static float _scriptGeneratedRipplesSmoothingFactor = 0.5f;

        private static bool _isWaterSimulationModeActive;

        System.Action<float>[] _constantRipplesModulePhysicsUpdateDelegates;
        System.Action<float>[] _simulationModulePhysicsUpdateDelegates;
        System.Action[] _simulationModuleResetSimulationDelegates;
        System.Action<int, float>[] _simulationModuleDisturbSurfaceVertexDelegates;
        System.Action[] _meshModuleUpdateMeshDelegates;

        private UnityEditorInternal.ReorderableList _sineWavesParametersList;

        protected override void Initiliaze()
        {
            int targetCount = targets.Length;

            _constantRipplesModulePhysicsUpdateDelegates = new System.Action<float>[targetCount];
            _simulationModulePhysicsUpdateDelegates = new System.Action<float>[targetCount];
            _simulationModuleResetSimulationDelegates = new System.Action[targetCount];
            _simulationModuleDisturbSurfaceVertexDelegates = new System.Action<int, float>[targetCount];
            _meshModuleUpdateMeshDelegates = new System.Action[targetCount];

            for (int i = 0, imax = targets.Length; i < imax; i++)
            {
                var waterObject = targets[i] as Game2DWater;
                var simulationModule = waterObject.SimulationModule;
                var constantRipplesModule = waterObject.ConstantRipplesModule;
                var meshModule = waterObject.MeshModule;

                _constantRipplesModulePhysicsUpdateDelegates[i] = CreateDelegate<Ripples.WaterConstantRipplesModule, float>(constantRipplesModule, "PhysicsUpdate");
                _simulationModulePhysicsUpdateDelegates[i] = CreateDelegate<Simulation.WaterSimulationModule, float>(simulationModule, "PhysicsUpdate");
                _simulationModuleResetSimulationDelegates[i] = CreateDelegate<Simulation.WaterSimulationModule>(simulationModule, "ResetSimulation");
                _simulationModuleDisturbSurfaceVertexDelegates[i] = CreateDelegate<Simulation.WaterSimulationModule, int, float>(simulationModule, "DisturbSurfaceVertex");
                _meshModuleUpdateMeshDelegates[i] = CreateDelegate<Mesh.WaterMeshModule>(meshModule, "UpdateMesh");
            }
        }

        override protected void DrawProperties()
        {
            _isWaterSimulationModeActive = _isInSimulationMode && !_isSimulationModeOwnedByWaterfallEditor;

            BeginPropertiesGroup(true);
            DrawSizeProperty();
            if (!_isEditingMeshMask)
                DrawSubdivisionCountProperty();
            EndPropertiesGroup();

            if (!_isWaterSimulationModeActive && !_isEditingMeshMask)
                DrawPropertiesGroup(DrawEdgeColliderProperty, true);

            if (!_isEditingMeshMask)
            {
                if (_isWaterSimulationModeActive)
                    DrawPropertiesGroup(DrawSimulationProperties, true);
                else
                    DrawPropertiesFadeGroup("Simulation Properties", DrawSimulationProperties);
            }

            if (!_isWaterSimulationModeActive && !_isEditingMeshMask)
            {
                var material = _materialProperty.objectReferenceValue as UnityEngine.Material;
                var hasRefraction = material != null ? material.IsKeywordEnabled("Water2D_Refraction") : false;
                var hasFakePerspective = material != null ? material.IsKeywordEnabled("Water2D_FakePerspective") : false;
                DrawPropertiesFadeGroup(hasFakePerspective && !hasRefraction ? "Fake Perspective Properties" : "Refraction Properties", DrawRefractionProperties);
                DrawPropertiesFadeGroup("Reflection Properties", DrawReflectionProperties);
                DrawPropertiesFadeGroup("Rendering Properties", DrawRenderingProperties);
                DrawPrefabUtilityProperties();
            }
        }

        protected override void DrawSceneView()
        {
            var waterObject = target as Game2DWater;

            if (!waterObject.IsInitialized)
                return;

            if (_previewBuoyancyEffectorSurfaceLevel)
                DrawBuoyancySurfaceLevelPreview(waterObject);

            if (_previewBoxCollider)
                DrawBoxColliderPreview(waterObject);

            if (_previewWaterSubdivisionsPerUnit)
                DrawWaterSubdivisionsPerUnitPreview(waterObject);

            if (_previewCustomBoundaries)
                DrawCustomBoundariesPreview(waterObject);

            bool _previewReflectionViewingFrustums = _previewPartiallySubmergedObjectsReflectionFrustum || _previewReflectionFrustum;
            if (_previewReflectionViewingFrustums)
                DrawReflectionViewingFrustumsPreview(waterObject, _previewReflectionFrustum, _previewPartiallySubmergedObjectsReflectionFrustum);

            if (_constantRipplesEditSourcesPositionsInSceneView)
                DrawConstantRipplesSourcesPositionsAddRemoveHandles(waterObject);

            if (_isInSimulationMode && !_isSimulationModeOwnedByWaterfallEditor && _simulationActiveRippleType == WaveToPreviewType.DynamicWavesOnCollisionRipples)
                DrawOnCollisionRipplesSimulationRegionBoundaries(waterObject);

            if (_isInSimulationMode && !_isSimulationModeOwnedByWaterfallEditor && _simulationActiveRippleType == WaveToPreviewType.DynamicWavesScriptGeneratedRipples)
                DrawScriptGeneratedRipplesSimulationPositionMarker(waterObject);
        }

        private void DrawSubdivisionCountProperty()
        {
            float defaultMinimumLabelWidth = Game2DWaterKitStyles.MinimumLabelWidth;

            var subdivisionsCountPerUnitProperty = serializedObject.FindProperty("subdivisionsCountPerUnit");
            Game2DWaterKitStyles.MinimumLabelWidth = subdivisionsCountPerUnitProperty.prefabOverride ? Game2DWaterKitStyles.SubdivisionsPerUnitPropertyLabel.WidthBold : Game2DWaterKitStyles.SubdivisionsPerUnitPropertyLabel.WidthRegular;
            _previewWaterSubdivisionsPerUnit = DrawPropertyWithPreviewButton(subdivisionsCountPerUnitProperty, Game2DWaterKitStyles.SubdivisionsPerUnitPropertyLabel, _previewWaterSubdivisionsPerUnit);

            Game2DWaterKitStyles.MinimumLabelWidth = defaultMinimumLabelWidth;
        }

        private void DrawEdgeColliderProperty()
        {
            var rect = EditorGUILayout.GetControlRect();
            var xMax = rect.xMax;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = _isMultiEditing;
            SetEditorGUIUtilityLabelWidth(Game2DWaterKitStyles.UseEdgeColliderPropertyLabel.WidthRegular, false);
            rect.xMax -= 27f;
            bool useEdgeCollider = EditorGUI.ToggleLeft(rect, Game2DWaterKitStyles.UseEdgeColliderPropertyLabel.Content, (target as Game2DWater).GetComponent<EdgeCollider2D>() != null);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Game2DWaterKitObject waterObject in targets)
                {
                    if (useEdgeCollider)
                    {
                        waterObject.gameObject.AddComponent<EdgeCollider2D>();
                        typeof(Game2DWaterKitObject).GetMethod("OnValidate", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(waterObject, null);
                    }
                    else DestroyImmediate(waterObject.GetComponent<EdgeCollider2D>(), true);
                }
            }

            rect.xMax = xMax;
            rect.xMin = xMax - 25f;
            _previewEdgeCollider = DrawPreviewButton(rect, _previewEdgeCollider);
        }

        private bool DrawBuoyancyEffectorProperty()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = _isMultiEditing;
            SetEditorGUIUtilityLabelWidth(Game2DWaterKitStyles.UseBuoyancyEffectorPropertyLabel.WidthRegular, false);
            bool useBuoyancyEffector = EditorGUILayout.ToggleLeft(Game2DWaterKitStyles.UseBuoyancyEffectorPropertyLabel.Content, (target as Game2DWater).GetComponent<BuoyancyEffector2D>() != null);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Game2DWaterKitObject waterObject in targets)
                {
                    if (useBuoyancyEffector)
                    {
                        waterObject.gameObject.AddComponent<BuoyancyEffector2D>();
                        typeof(Game2DWaterKitObject).GetMethod("OnValidate", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(waterObject, null);
                    }
                    else DestroyImmediate(waterObject.GetComponent<BuoyancyEffector2D>(), true);
                }
            }

            return useBuoyancyEffector;
        }

        private void DrawSimulationProperties()
        {
            DrawSimulationPreviewProperties(false, DrawSimulationPreviewActiveRipplesTypeSelectionProperties);

            if (!_isWaterSimulationModeActive)
            {
                BeginPropertiesGroup(false, "Buoyancy Effector Properties");

                bool useBuoyancyEffector = DrawBuoyancyEffectorProperty();

                EditorGUI.BeginDisabledGroup(!useBuoyancyEffector);

                var surfaceLevelLocationProperty = serializedObject.FindProperty("buoyancyEffectorSurfaceLevelLocation");
                _previewBuoyancyEffectorSurfaceLevel = DrawPropertyWithPreviewButton(surfaceLevelLocationProperty, Game2DWaterKitStyles.BuoyancyEffectorSurfaceLevelLocationPropertyLabel, _previewBuoyancyEffectorSurfaceLevel);
                if (surfaceLevelLocationProperty.enumValueIndex == 0)
                {
                    var surfaceLevelProperty = serializedObject.FindProperty("buoyancyEffectorSurfaceLevel");
                    DrawProperty(surfaceLevelProperty, Game2DWaterKitStyles.BuoyancyEffectorSurfaceLevelPropertyLabel, false, true);
                }
                if(surfaceLevelLocationProperty.enumValueIndex > 1)
                {
                    var material = _materialProperty.objectReferenceValue as UnityEngine.Material;

                    if (!material.IsKeywordEnabled("Water2D_Surface"))
                        EditorGUILayout.HelpBox("\"Surface\" is not enabled in the material inspector!", MessageType.Warning);
                    else if (!material.IsKeywordEnabled("Water2D_FakePerspective") && surfaceLevelLocationProperty.enumValueIndex == 3)
                        EditorGUILayout.HelpBox("\"Submerge Level\" property is not enabled in the material inspector!", MessageType.Warning);
                }

                EditorGUI.EndDisabledGroup();

                EndPropertiesGroup();

                BeginPropertiesGroup();
                var canRipplesAffectRigidbodiesProperty = serializedObject.FindProperty("canWavesAffectRigidbodies");
                DrawPropertyLeftToggle(canRipplesAffectRigidbodiesProperty, Game2DWaterKitStyles.CanWavesAffectRigidbodies);
                EditorGUI.BeginDisabledGroup(!canRipplesAffectRigidbodiesProperty.boolValue);
                DrawProperty("wavesStrengthOnRigidbodies", Game2DWaterKitStyles.WavesStrengthOnRigidbodiesLabel, false, true);
                EditorGUI.EndDisabledGroup();
                EndPropertiesGroup();
            }

            if (!_isWaterSimulationModeActive)
            {
                DrawPropertiesFadeGroup("Dynamic Waves", DrawSimulationDynamicWavesProperties, false, true, true);
                DrawPropertiesFadeGroup("Sine Waves", DrawSineWavesProperties, false, true, true, serializedObject.FindProperty("waterSimulationAreSineWavesActive"));
            }
            else
            {
                if (_simulationActiveRippleType == WaveToPreviewType.SineWaves)
                {
                    if (DrawPropertiesGroup(DrawSineWavesProperties, false, "Sine Waves", serializedObject.FindProperty("waterSimulationAreSineWavesActive")))
                        RestartSimulationPreview();
                }
                else DrawSimulationDynamicWavesProperties();
            }
        }

        private void DrawSimulationDynamicWavesProperties()
        {
            DrawPropertiesGroup(DrawWaveProperties, false, "Wave Properties");

            if (!_isWaterSimulationModeActive)
                DrawPropertiesFadeGroup("On-Collision Ripples Properties", DrawOnCollisionRipplesProperties, false, true, true);
            else if (_isWaterSimulationModeActive && _simulationActiveRippleType == WaveToPreviewType.DynamicWavesOnCollisionRipples)
                DrawOnCollisionRipplesProperties();

            if (!_isWaterSimulationModeActive)
                DrawPropertiesFadeGroup("Constant Ripples Properties", DrawConstantRipplesProperties, false, true, true, groupToggleProperty: serializedObject.FindProperty("activateConstantRipples"));
            else if (_isWaterSimulationModeActive && _simulationActiveRippleType == WaveToPreviewType.DynamicWavesConstantRipples)
                DrawPropertiesGroup(DrawConstantRipplesProperties, false, "Constant Ripples Properties", groupToggleProperty: serializedObject.FindProperty("activateConstantRipples"));

            if (!_isWaterSimulationModeActive)
                DrawPropertiesFadeGroup("Script-Generated Ripples Properties", DrawScriptGeneratedRipplesProperties, false, true, true);
            else if (_isWaterSimulationModeActive && _simulationActiveRippleType == WaveToPreviewType.DynamicWavesScriptGeneratedRipples)
                DrawPropertiesGroup(DrawScriptGeneratedRipplesProperties, false, "Script-Generated Ripples Properties");
        }

        private void DrawSineWavesProperties()
        {
            var sineWavesParametersListProperty = serializedObject.FindProperty("waterSimulationSineWavesParameters");

            if (_sineWavesParametersList == null)
            {
                _sineWavesParametersList = new UnityEditorInternal.ReorderableList(serializedObject, sineWavesParametersListProperty, true, true, true, true);
                _sineWavesParametersList.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "Sine Waves List", EditorStyles.miniBoldLabel); };
                _sineWavesParametersList.drawElementCallback = (rect, index, active, focused) => { EditorGUI.PropertyField(rect, sineWavesParametersListProperty.GetArrayElementAtIndex(index), true); };
                _sineWavesParametersList.onRemoveCallback = (list) => { UnityEditorInternal.ReorderableList.defaultBehaviours.DoRemoveButton(list); if (sineWavesParametersListProperty.arraySize < 1) RestartSimulationPreview(); };
            }

            _sineWavesParametersList.elementHeight = sineWavesParametersListProperty.arraySize > 0 ? EditorGUIUtility.singleLineHeight * 4f + 14f : 18f;
            _sineWavesParametersList.DoLayoutList();
        }

        private void DrawSimulationPreviewActiveRipplesTypeSelectionProperties()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUIUtility.labelWidth = 109f;
            _simulationActiveRippleType = (WaveToPreviewType)EditorGUILayout.Popup("Wave To Preview:", (int)_simulationActiveRippleType, Game2DWaterKitStyles.WavesToPreviewType);
            if (EditorGUI.EndChangeCheck())
                Game2DWaterKitSimulationPreviewMode.Restart();

            if (_simulationActiveRippleType == WaveToPreviewType.DynamicWavesOnCollisionRipples)
            {
                BeginPropertiesGroup();
                var regionLabel = Game2DWaterKitStyles.SimulationModeOnCollisionRipplesRegionPropertyLabel;
                SetEditorGUIUtilityLabelWidth(regionLabel.WidthRegular, true);
                EditorGUILayout.MinMaxSlider(regionLabel.Content, ref _collisionRipplesSimulationRegionMin, ref _collisionRipplesSimulationRegionMax, -0.5f, 0.5f);
                EditorGUILayout.HelpBox(Game2DWaterKitStyles.SimulationModeOnCollisionRipplesRegionPropertyLabel.Content.tooltip, MessageType.Info);
                EndPropertiesGroup();
            }
            else if (_simulationActiveRippleType == WaveToPreviewType.DynamicWavesScriptGeneratedRipples)
            {
                BeginPropertiesGroup();
                var positionLabel = Game2DWaterKitStyles.SimulationModeScriptGeneratedRipplesSourcePositionropertyLabel;
                SetEditorGUIUtilityLabelWidth(positionLabel.WidthRegular, true);
                _scriptGeneratedRipplesSimulationPosition = GUI.HorizontalSlider(EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(), positionLabel.Content), _scriptGeneratedRipplesSimulationPosition, -0.5f, 0.5f);
                EditorGUILayout.HelpBox(Game2DWaterKitStyles.SimulationModeScriptGeneratedRipplesSourcePositionropertyLabel.Content.tooltip, MessageType.Info);
                EndPropertiesGroup();
            }
        }

        private void DrawWaveProperties()
        {
            if (_isWaterSimulationModeActive)
                EditorGUILayout.HelpBox(Game2DWaterKitStyles.SimulationModuleWavePropertiesMessage, MessageType.Info);

            DrawProperty("stiffness", Game2DWaterKitStyles.WaveStiffnessPropertyLabel);
            DrawProperty("spread", Game2DWaterKitStyles.WaveSpreadPropertyLabel);
            DrawProperty("damping", Game2DWaterKitStyles.WaveDampingPropertyLabel);

            DrawCustomBoundariesProperties();
            DrawLimitMaximumDisturbanceProperties();
        }

        private void DrawCustomBoundariesProperties()
        {
            var useCustomBoundaries = serializedObject.FindProperty("useCustomBoundaries");

            _previewCustomBoundaries = DrawLeftToggleWithPreviewButton(useCustomBoundaries, Game2DWaterKitStyles.WaveUseCustomBoundariesPropertyLabel, _previewCustomBoundaries);

            if (useCustomBoundaries.boolValue)
            {
                DrawProperty("firstCustomBoundary", Game2DWaterKitStyles.WaveFirstCustomBoundaryPropertyLabel);
                DrawProperty("secondCustomBoundary", Game2DWaterKitStyles.WaveSecondCustomBoundaryPropertyLabel);
            }
        }

        private void DrawLimitMaximumDisturbanceProperties()
        {
            var shouldClampWaveDisturbanceProperty = serializedObject.FindProperty("shouldClampDynamicWaveDisturbance");
            var maximumDisturbanceProperty = serializedObject.FindProperty("maximumdDynamicWaveDisturbance");
            var label = Game2DWaterKitStyles.WaveShouldClampDisturbancePropertyLabel;

            var rect = EditorGUILayout.GetControlRect();
            var xMax = rect.xMax;

            rect.width = 14f + (shouldClampWaveDisturbanceProperty.prefabOverride ? label.WidthBold : label.WidthRegular);

            EditorGUI.BeginProperty(rect, label.Content, shouldClampWaveDisturbanceProperty);
            EditorGUI.BeginChangeCheck();
            var shouldClampWaveDisturbancePropertyValue = EditorGUI.ToggleLeft(rect, label.Content, shouldClampWaveDisturbanceProperty.boolValue);
            if (EditorGUI.EndChangeCheck())
                shouldClampWaveDisturbanceProperty.boolValue = shouldClampWaveDisturbancePropertyValue;
            EditorGUI.EndProperty();

            EditorGUI.BeginDisabledGroup(!shouldClampWaveDisturbanceProperty.boolValue);

            rect.xMin = rect.xMax + 4f;
            rect.xMax = xMax;

            EditorGUI.PropertyField(rect, maximumDisturbanceProperty, GUIContent.none);

            EditorGUI.EndDisabledGroup();
        }

        private void DrawOnCollisionRipplesProperties()
        {
            var activateOnCollisionOnWaterEnterRipplesProperty = serializedObject.FindProperty("activateOnCollisionOnWaterEnterRipples");
            var activateOnCollisionOnWaterExitRipplesProperty = serializedObject.FindProperty("activateOnCollisionOnWaterExitRipples");

            if (!_isWaterSimulationModeActive)
            {
                var activateOnCollisionOnWaterMoveRipplesProperty = serializedObject.FindProperty("activateOnCollisionOnWaterMoveRipples");

                BeginPropertiesGroup(false, "When To Create Ripples?");
                DrawPropertyLeftToggle(activateOnCollisionOnWaterEnterRipplesProperty, Game2DWaterKitStyles.ActivateOnCollisionOnWaterEnterRipples);
                DrawPropertyLeftToggle(activateOnCollisionOnWaterExitRipplesProperty, Game2DWaterKitStyles.ActivateOnCollisionOnWaterExitRipples);
                DrawPropertyLeftToggle(activateOnCollisionOnWaterMoveRipplesProperty, Game2DWaterKitStyles.ActivateOnCollisionOnWaterMoveRipples);
                EndPropertiesGroup();

                DrawPropertiesGroup(DrawOnCollisionRipplesCollisionProperties, false, "Collision Properties");

                EditorGUI.BeginDisabledGroup(!(activateOnCollisionOnWaterEnterRipplesProperty.boolValue || activateOnCollisionOnWaterExitRipplesProperty.boolValue));
                DrawPropertiesFadeGroup("On-Water-Enter/Exit Ripples Properties", () => DrawOnCollisionRipplesOnWaterEnterExitProperties(activateOnCollisionOnWaterEnterRipplesProperty, activateOnCollisionOnWaterExitRipplesProperty), false, true, true);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!activateOnCollisionOnWaterMoveRipplesProperty.boolValue);
                DrawPropertiesFadeGroup("On-Water-Move Ripples Properties", DrawOnCollisionRipplesOnWaterMoveRipplesProperties, false, true, true);
                EditorGUI.EndDisabledGroup();
            }
            else DrawPropertiesGroup(() => DrawOnCollisionRipplesOnWaterEnterExitProperties(activateOnCollisionOnWaterEnterRipplesProperty, activateOnCollisionOnWaterExitRipplesProperty), false, "On-Water-Enter/Exit Ripples Properties");
        }

        private void DrawOnCollisionRipplesOnWaterEnterExitProperties(SerializedProperty activateOnCollisionOnWaterEnterRipples, SerializedProperty activateOnCollisionOnWaterExitRipples)
        {
            if (!_isWaterSimulationModeActive)
            {
                DrawPropertiesGroup(DrawOnCollisionRipplesDisturbanceProperties, false, "Disturbance Properties");

                EditorGUI.BeginDisabledGroup(!activateOnCollisionOnWaterEnterRipples.boolValue);
                DrawPropertiesGroup(DrawOnCollisionRipplesOnWaterEnterRipplesProperties, false, "On-Water-Enter Ripples Properties");
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!activateOnCollisionOnWaterExitRipples.boolValue);
                DrawPropertiesGroup(DrawOnCollisionRipplesOnWaterExitRipplesProperties, false, "On-Water-Exit Ripples Properties");
                EditorGUI.EndDisabledGroup();

                string msg = "There's another event, OnWaterEnterExit, that you can subscribe to in code: Game2DWaterKit.Ripples.WaterCollisionRipplesModule.OnWaterEnterExit(Game2DWater waterObject, Collider2D collider, bool isColliderEnteringWater)";

                EditorGUILayout.HelpBox(msg, MessageType.Info);
            }
            else DrawOnCollisionRipplesDisturbanceProperties();
        }

        private void DrawOnCollisionRipplesOnWaterMoveRipplesProperties()
        {
            DrawProperty("onCollisionOnWaterMoveRipplesMaximumDisturbance", Game2DWaterKitStyles.OnCollisionRipplesOnWaterMoveMaximumDisturbancePropertyLabel);

            EditorGUI.indentLevel++;
            DrawProperty("onCollisionOnWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance", Game2DWaterKitStyles.OnCollisionRipplesOnWaterMoveMinimumVelocityPropertyLabel);
            EditorGUILayout.HelpBox("The \"Minimum Velocity\" property defines the minimum velocity that a rigidbody moving in water should have to cause the \"Maximum Disturbance\" to the water surface.", MessageType.Info);
            EditorGUI.indentLevel--;

            DrawProperty("onCollisionOnWaterMoveRipplesDisturbanceSmoothFactor", Game2DWaterKitStyles.OnCollisionRipplesOnWaterMoveSmoothFactorPropertyLabel);
        }

        private void DrawOnCollisionRipplesDisturbanceProperties()
        {
            var minimumDisturbaneProperty = serializedObject.FindProperty("onCollisionRipplesMinimumDisturbance");
            var maximumDisturbaneProperty = serializedObject.FindProperty("onCollisionRipplesMaximumDisturbance");
            var velocityMultiplierProperty = serializedObject.FindProperty("onCollisionRipplesVelocityMultiplier");
          
            float maximumDisturbanceMinimumVelocity = maximumDisturbaneProperty.floatValue / velocityMultiplierProperty.floatValue;

            // Minimum Disturbance Property
            if(_isWaterSimulationModeActive)
            {
                if (DrawPropertyWithActionButton(minimumDisturbaneProperty, Game2DWaterKitStyles.MinimumDisturbancePropertyLabel, Game2DWaterKitStyles.RunSimulationButtonLabel))
                    RunOnCollisionRippleSimulation(minimumDisturbaneProperty.floatValue);
            }
            else DrawProperty(minimumDisturbaneProperty, Game2DWaterKitStyles.MinimumDisturbancePropertyLabel);

            // Maximum Disturbance Property
            EditorGUI.BeginChangeCheck();
            if (_isWaterSimulationModeActive)
            {
                if (DrawPropertyWithActionButton(maximumDisturbaneProperty, Game2DWaterKitStyles.MaximumDisturbancePropertyLabel, Game2DWaterKitStyles.RunSimulationButtonLabel))
                    RunOnCollisionRippleSimulation(maximumDisturbaneProperty.floatValue);
            }
            else DrawProperty(maximumDisturbaneProperty, Game2DWaterKitStyles.MaximumDisturbancePropertyLabel, true);
            if (EditorGUI.EndChangeCheck() && maximumDisturbaneProperty.floatValue != 0f)
                velocityMultiplierProperty.floatValue = maximumDisturbaneProperty.floatValue / maximumDisturbanceMinimumVelocity;

            EditorGUI.indentLevel++;
            
            if (!_isWaterSimulationModeActive)
            {
                // Maximum Disturbance Minimum Velocity Property
                var rect = EditorGUILayout.GetControlRect();
                var label = EditorGUI.BeginProperty(rect, Game2DWaterKitStyles.OnCollisionRipplesOnWaterEnterExitMinimumVelocityPropertyLabel.Content, velocityMultiplierProperty);
                EditorGUI.BeginChangeCheck();
                SetEditorGUIUtilityLabelWidth(velocityMultiplierProperty, Game2DWaterKitStyles.OnCollisionRipplesOnWaterEnterExitMinimumVelocityPropertyLabel);
                maximumDisturbanceMinimumVelocity = EditorGUI.DelayedFloatField(rect, label, maximumDisturbanceMinimumVelocity);
                if (EditorGUI.EndChangeCheck() && maximumDisturbanceMinimumVelocity != 0f)
                {
                    velocityMultiplierProperty.floatValue = maximumDisturbaneProperty.floatValue / maximumDisturbanceMinimumVelocity;
                    Undo.RecordObjects(targets, label.text);
                }
                EditorGUI.EndProperty();

                EditorGUILayout.HelpBox("The \"Minimum Velocity\" property defines the minimum velocity that a rigidbody should have to cause the \"Maximum Disturbance\" to the water surface.", MessageType.Info);
            }

            EditorGUI.indentLevel--;
        }

        private void DrawOnCollisionRipplesCollisionProperties()
        {
            EditorGUILayout.LabelField("Collision Filtering", EditorStyles.boldLabel);

            DrawProperty("onCollisionRipplesCollisionMask", Game2DWaterKitStyles.OnCollisionRipplesRaycastMaskPropertyLabel);
            DrawProperty("onCollisionRipplesCollisionMinimumDepth", Game2DWaterKitStyles.OnCollisionRipplesRaycastMinimumDepthPropertyLabel);
            DrawProperty("onCollisionRipplesCollisionMaximumDepth", Game2DWaterKitStyles.OnCollisionRipplesRaycastMaximumDepthPropertyLabel);
            DrawProperty("onCollisionRipplesCollisionRaycastMaxDistance", Game2DWaterKitStyles.OnCollisionRipplesRaycastMaximumDistancePropertyLabel);
            DrawProperty("onCollisionRipplesIgnoreTriggers", Game2DWaterKitStyles.OnCollisionRipplesIgnoreTriggersPropertyLabel);

            if(!_isMultiEditing && (target as Game2DWaterKitObject).GetComponent<BoxCollider2D>() != null)
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("2D Box Collider", EditorStyles.boldLabel);

                var boxColliderTopEdgeLocation = serializedObject.FindProperty("boxColliderTopEdgeLocation");
                _previewBoxCollider = DrawPropertyWithPreviewButton(boxColliderTopEdgeLocation, Game2DWaterKitStyles.OnCollisionRipplesMatchBoxColliderTopEdgePropertyLabel, _previewBoxCollider);
                if (boxColliderTopEdgeLocation.enumValueIndex > 0)
                {
                    var material = _materialProperty.objectReferenceValue as UnityEngine.Material;

                    if (material != null)
                    {
                        if (!material.IsKeywordEnabled("Water2D_Surface"))
                            EditorGUILayout.HelpBox("\"Surface\" is not enabled in the material inspector!", MessageType.Warning);
                        else if (!material.IsKeywordEnabled("Water2D_FakePerspective") && boxColliderTopEdgeLocation.enumValueIndex == 2)
                            EditorGUILayout.HelpBox("\"Submerge Level\" property is not enabled in the material inspector!", MessageType.Warning);
                    }
                    else EditorGUILayout.HelpBox("The water object's material is missing!", MessageType.Error);
                }
            }
        }

        private void DrawOnCollisionRipplesOnWaterEnterRipplesProperties()
        {
            DrawProperty("onWaterEnter", Game2DWaterKitStyles.OnCollisionRipplesOnWaterEnterEventPropertyLabel);
            DrawRipplesSoundEffectProperties("onCollisionRipples", "OnWaterEnter");
            DrawRipplesParticleEffectProperties("onCollisionRipples", "OnWaterEnter");
        }

        private void DrawOnCollisionRipplesOnWaterExitRipplesProperties()
        {
            DrawProperty("onWaterExit", Game2DWaterKitStyles.OnCollisionRipplesOnWaterExitEventPropertyLabel);
            DrawRipplesSoundEffectProperties("onCollisionRipples", "OnWaterExit");
            DrawRipplesParticleEffectProperties("onCollisionRipples", "OnWaterExit");
        }

        private void DrawConstantRipplesProperties()
        {
            if (_isWaterSimulationModeActive)
                DrawSimulationPreviewStopRestartButtons();

            if (!_isWaterSimulationModeActive)
            {
                BeginPropertiesGroup();
                DrawProperty("constantRipplesUpdateWhenOffscreen", Game2DWaterKitStyles.ConstantRipplesUpdateWhenOffscreenPropertyLabel);
                EndPropertiesGroup();
            }

            DrawTimeIntervalProperties("constantRipples");
            DrawPropertiesGroup(DrawConstantRipplesSourceProperties, false, "Source Properties");
            DrawPropertiesGroup(DrawConstantRipplesDisturbanceProperties, false, "Disturbance Properties");
            
            if (!_isWaterSimulationModeActive)
            {
                DrawRipplesSoundEffectProperties("constantRipples");
                DrawRipplesParticleEffectProperties("constantRipples");
            }
        }

        private void DrawConstantRipplesDisturbanceProperties()
        {
            var randomizeDisturbance = serializedObject.FindProperty("constantRipplesRandomizeDisturbance");

            DrawProperty(randomizeDisturbance, Game2DWaterKitStyles.ConstantRipplesRandomizeDisturbancePropertyLabel);

            if (randomizeDisturbance.boolValue)
            {
                DrawProperty("constantRipplesMinimumDisturbance", Game2DWaterKitStyles.MinimumDisturbancePropertyLabel);
                DrawProperty("constantRipplesMaximumDisturbance", Game2DWaterKitStyles.MaximumDisturbancePropertyLabel);
            }
            else DrawProperty("constantRipplesDisturbance", Game2DWaterKitStyles.DisturbancePropertyLabel);

            var smoothDisturbance = serializedObject.FindProperty("constantRipplesSmoothDisturbance");

            DrawProperty(smoothDisturbance, Game2DWaterKitStyles.SmoothRipplesPropertyLabel);

            if (smoothDisturbance.boolValue)
                DrawProperty("constantRipplesSmoothFactor", Game2DWaterKitStyles.SmoothingFactorPropertyLabel);
        }

        private void DrawConstantRipplesSourceProperties()
        {
            var randomizeRipplesSourcesPositions = serializedObject.FindProperty("constantRipplesRandomizeRipplesSourcesPositions");

            DrawProperty(randomizeRipplesSourcesPositions, Game2DWaterKitStyles.ConstantRipplesRandomizeSourcesPositionsPropertyLabel);

            if (!randomizeRipplesSourcesPositions.boolValue)
            {
                var ripplesSourcesPositions = serializedObject.FindProperty("constantRipplesSourcePositions");

                DrawProperty("constantRipplesAllowDuplicateRipplesSourcesPositions", Game2DWaterKitStyles.ConstantRipplesAllowDuplicateSourcesPositionsPropertyLabel);
                
                EditorGUI.BeginDisabledGroup(_isMultiEditing);
                _constantRipplesEditSourcesPositionsInSceneView = GUILayout.Toggle(_constantRipplesEditSourcesPositionsInSceneView, "Edit Ripples Sources Positions", Game2DWaterKitStyles.ButtonStyle);
                ripplesSourcesPositions.isExpanded |= _constantRipplesEditSourcesPositionsInSceneView;
                EditorGUI.indentLevel++;
                DrawProperty(ripplesSourcesPositions, Game2DWaterKitStyles.ConstantRipplesSourcesPositionsPropertyLabel);
                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                DrawProperty("constantRipplesRandomizeRipplesSourcesCount", Game2DWaterKitStyles.ConstantRipplesRandomSourceCountPropertyLabel);
                _constantRipplesEditSourcesPositionsInSceneView = false;
            }
        }

        private void DrawScriptGeneratedRipplesProperties()
        {
            BeginPropertiesGroup(false, "Disturbance Properties");
            if (_isWaterSimulationModeActive)
            {
                var minimumDisturbaneProperty = serializedObject.FindProperty("scriptGeneratedRipplesMinimumDisturbance");
                var maximumDisturbaneProperty = serializedObject.FindProperty("scriptGeneratedRipplesMaximumDisturbance");

                if (DrawPropertyWithActionButton(minimumDisturbaneProperty, Game2DWaterKitStyles.MinimumDisturbancePropertyLabel, Game2DWaterKitStyles.RunSimulationButtonLabel))
                    RunScriptGeneratedRipplesSimulation(0f, _scriptGeneratedRipplesSmoothRipples, _scriptGeneratedRipplesSmoothingFactor);

                if (DrawPropertyWithActionButton(maximumDisturbaneProperty, Game2DWaterKitStyles.MaximumDisturbancePropertyLabel, Game2DWaterKitStyles.RunSimulationButtonLabel))
                    RunScriptGeneratedRipplesSimulation(1f, _scriptGeneratedRipplesSmoothRipples, _scriptGeneratedRipplesSmoothingFactor);

                BeginPropertiesGroup();
                EditorGUILayout.HelpBox("These properties are part of the GenerateRipple() method parameter list", MessageType.Info);

                if(DrawSliderWithActionButton(ref _scriptGeneratedRipplesDisturbanceFactor, 0f, 1f, Game2DWaterKitStyles.ScriptGeneratedRipplesDisturbanceFactorPropertyLabel, Game2DWaterKitStyles.RunSimulationButtonLabel))
                    RunScriptGeneratedRipplesSimulation(_scriptGeneratedRipplesDisturbanceFactor, _scriptGeneratedRipplesSmoothRipples, _scriptGeneratedRipplesSmoothingFactor);
                EditorGUILayout.HelpBox(Game2DWaterKitStyles.ScriptGeneratedRipplesDisturbanceFactorPropertyLabel.Content.tooltip, MessageType.Info);

                _scriptGeneratedRipplesSmoothRipples = EditorGUILayout.Toggle("Smooth Ripples", _scriptGeneratedRipplesSmoothRipples);
                _scriptGeneratedRipplesSmoothingFactor = EditorGUILayout.Slider("Smoothing Factor", _scriptGeneratedRipplesSmoothingFactor, 0f, 1f);
                EndPropertiesGroup();
            }
            else
            {
                DrawProperty("scriptGeneratedRipplesMinimumDisturbance", Game2DWaterKitStyles.MinimumDisturbancePropertyLabel);
                DrawProperty("scriptGeneratedRipplesMaximumDisturbance", Game2DWaterKitStyles.MaximumDisturbancePropertyLabel);
            }
            EndPropertiesGroup();

            if (!_isWaterSimulationModeActive)
            {
                DrawRipplesSoundEffectProperties("scriptGeneratedRipples");
                DrawRipplesParticleEffectProperties("scriptGeneratedRipples");
            }
        }

        private void DrawRipplesSoundEffectProperties(string propertyName, string subpropertyName = "")
        {
            var activateSoundEffect = serializedObject.FindProperty(propertyName + "Activate" + subpropertyName + "SoundEffect");
            bool hasChangedSoundEffectActiveState = DrawPropertiesFadeGroup("Sound Effect#" + propertyName + subpropertyName, () => DoSoundEffectPropertiesLayout(propertyName, subpropertyName), false, true, true, groupToggleProperty: activateSoundEffect);
            if(hasChangedSoundEffectActiveState && Application.isPlaying)
            {
                var setActiveMethodInfo = typeof(Ripples.Effects.WaterRipplesSoundEffect).GetMethod("SetActive", BindingFlags.NonPublic | BindingFlags.Instance);
                var parameters = new object[] { activateSoundEffect.boolValue };
                foreach (Game2DWater waterObject in targets)
                {
                    if(propertyName == "scriptGeneratedRipples")
                        setActiveMethodInfo.Invoke(waterObject.ScriptGeneratedRipplesModule.SoundEffect, parameters);
                    else if(propertyName == "constantRipples")
                        setActiveMethodInfo.Invoke(waterObject.ConstantRipplesModule.SoundEffect, parameters);
                    else
                    {
                        if(subpropertyName == "OnWaterEnter")
                            setActiveMethodInfo.Invoke(waterObject.OnCollisonRipplesModule.OnWaterEnterRipplesSoundEffect, parameters);
                        else
                            setActiveMethodInfo.Invoke(waterObject.OnCollisonRipplesModule.OnWaterExitRipplesSoundEffect, parameters);
                    }
                }
            }
        }

        private void DoSoundEffectPropertiesLayout(string propertyName, string subpropertyName)
        {
            string fullName = propertyName + subpropertyName;

            EditorGUI.BeginChangeCheck();
            var audioClipProperty = serializedObject.FindProperty(fullName + "AudioClip");
            DrawProperty(audioClipProperty, Game2DWaterKitStyles.SoundEffectAudioClipPropertyLabel);
            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
            {
                var changeAudioClipMethodInfo = typeof(Ripples.Effects.WaterRipplesSoundEffect).GetMethod("ChangeAudioClip", BindingFlags.NonPublic | BindingFlags.Instance);
                var parameters = new object[] { audioClipProperty.objectReferenceValue };
                foreach (Game2DWater waterObject in targets)
                {
                    if (propertyName == "scriptGeneratedRipples")
                        changeAudioClipMethodInfo.Invoke(waterObject.ScriptGeneratedRipplesModule.SoundEffect, parameters);
                    else if (propertyName == "constantRipples")
                        changeAudioClipMethodInfo.Invoke(waterObject.ConstantRipplesModule.SoundEffect, parameters);
                    else
                    {
                        if (subpropertyName == "OnWaterEnter")
                            changeAudioClipMethodInfo.Invoke(waterObject.OnCollisonRipplesModule.OnWaterEnterRipplesSoundEffect, parameters);
                        else
                            changeAudioClipMethodInfo.Invoke(waterObject.OnCollisonRipplesModule.OnWaterExitRipplesSoundEffect, parameters);
                    }
                }
            }

            DrawProperty(fullName + "SoundEffectAudioSourcePrefab", Game2DWaterKitStyles.SoundEffectAudioSourcePrefabPropertyLabel);
            DrawProperty(fullName + "SoundEffectOutput", Game2DWaterKitStyles.SoundEffectOutputPropertyLabel);
            DrawProperty(fullName + "SoundEffectPoolSize", Game2DWaterKitStyles.SoundEffectPoolSizePropertyLabel);
            DrawProperty(fullName + "SoundEffectPoolExpandIfNecessary", Game2DWaterKitStyles.SoundEffectPoolCanExpandPropertyLabel);
            DrawProperty(fullName + "AudioVolume", Game2DWaterKitStyles.SoundEffectVolumePropertyLabel);

            var useConstantAudioPitch = serializedObject.FindProperty(propertyName + "UseConstant" + subpropertyName + "AudioPitch");

            DrawProperty(useConstantAudioPitch, Game2DWaterKitStyles.SoundEffectUseConstantPitchPropertyLabel);

            if (!useConstantAudioPitch.boolValue)
            {
                DrawProperty(fullName + "MinimumAudioPitch", Game2DWaterKitStyles.SoundEffectMinimumPitchPropertyLabel);
                DrawProperty(fullName + "MaximumAudioPitch", Game2DWaterKitStyles.SoundEffectMaximumPitchPropertyLabel);
            }
            else DrawProperty(fullName + "AudioPitch", Game2DWaterKitStyles.SoundEffectPitchPropertyLabel);
        }

        private void DrawRipplesParticleEffectProperties(string propertyName, string subpropertyName = "")
        {
            var activateParticleEffect = serializedObject.FindProperty(propertyName + "Activate" + subpropertyName + "ParticleEffect");
            bool hasChangedParticleEffectActiveState = DrawPropertiesFadeGroup("Particle Effect#" + propertyName + subpropertyName, () => DoParticleEffectPropertiesLayout(propertyName, subpropertyName), false, true, true, groupToggleProperty: activateParticleEffect);
            if (hasChangedParticleEffectActiveState && Application.isPlaying)
            {
                var setActiveMethodInfo = typeof(Ripples.Effects.WaterRipplesParticleEffect).GetMethod("SetActive", BindingFlags.NonPublic | BindingFlags.Instance);
                var parameters = new object[] { activateParticleEffect.boolValue };
                foreach (Game2DWater waterObject in targets)
                {
                    if (propertyName == "scriptGeneratedRipples")
                        setActiveMethodInfo.Invoke(waterObject.ScriptGeneratedRipplesModule.ParticleEffect, parameters);
                    else if (propertyName == "constantRipples")
                        setActiveMethodInfo.Invoke(waterObject.ConstantRipplesModule.ParticleEffect, parameters);
                    else
                    {
                        if (subpropertyName == "OnWaterEnter")
                            setActiveMethodInfo.Invoke(waterObject.OnCollisonRipplesModule.OnWaterEnterRipplesParticleEffect, parameters);
                        else
                            setActiveMethodInfo.Invoke(waterObject.OnCollisonRipplesModule.OnWaterExitRipplesParticleEffect, parameters);
                    }
                }
            }
        }

        private void DoParticleEffectPropertiesLayout(string propertyName, string subpropertyName)
        {
            string fullName = propertyName + subpropertyName;

            var particleEffectSystemProperty = serializedObject.FindProperty(fullName + "ParticleEffect");

            EditorGUI.BeginChangeCheck();
            DrawProperty(particleEffectSystemProperty, Game2DWaterKitStyles.ParticleEffectParticleSystemPropertyLabel);

            ParticleSystem particleSystem = particleEffectSystemProperty.objectReferenceValue as ParticleSystem;
            if (particleSystem != null && particleSystem.main.loop)
                EditorGUILayout.HelpBox("Please make sure the particle system is not looping!", MessageType.Warning);

            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
            {
                var changeParticleSystemMethodInfo = typeof(Ripples.Effects.WaterRipplesParticleEffect).GetMethod("ChangeParticleSystem", BindingFlags.NonPublic | BindingFlags.Instance);
                var parameters = new object[] { particleSystem };
                foreach (Game2DWater waterObject in targets)
                {
                    if (propertyName == "scriptGeneratedRipples")
                        changeParticleSystemMethodInfo.Invoke(waterObject.ScriptGeneratedRipplesModule.ParticleEffect, parameters);
                    else if (propertyName == "constantRipples")
                        changeParticleSystemMethodInfo.Invoke(waterObject.ConstantRipplesModule.ParticleEffect, parameters);
                    else
                    {
                        if (subpropertyName == "OnWaterEnter")
                            changeParticleSystemMethodInfo.Invoke(waterObject.OnCollisonRipplesModule.OnWaterEnterRipplesParticleEffect, parameters);
                        else
                            changeParticleSystemMethodInfo.Invoke(waterObject.OnCollisonRipplesModule.OnWaterExitRipplesParticleEffect, parameters);
                    }
                }
            }

            DrawProperty(fullName + "ParticleEffectPoolSize", Game2DWaterKitStyles.ParticleEffectPoolSizePropertyLabel);
            DrawProperty(fullName + "ParticleEffectPoolExpandIfNecessary", Game2DWaterKitStyles.ParticleEffectPoolCanExpandPropertyLabel);
            DrawProperty(fullName + "ParticleEffectSpawnOffset", Game2DWaterKitStyles.ParticleEffectSpawnOffsetPropertyLabel);
            DrawProperty(fullName + "ParticleEffectStopAction", Game2DWaterKitStyles.ParticleEffectStopActionPropertyLabel);
        }

        private void DrawRefractionProperties()
        {
            bool hasDifferentMaterials = _materialProperty.hasMultipleDifferentValues;
            bool hasRefraction = false;
            bool hasFakePerspective = false;

            if (!hasDifferentMaterials)
            {
                var material = _materialProperty.objectReferenceValue as UnityEngine.Material;
                hasRefraction = material != null ? material.IsKeywordEnabled("Water2D_Refraction") : false;
                hasFakePerspective = material != null ? material.IsKeywordEnabled("Water2D_FakePerspective") : false;
            }
            else EditorGUILayout.HelpBox(Game2DWaterKitStyles.CantMultiEditBecauseUsingDifferentMaterialMessage, MessageType.Info);

            if (!hasDifferentMaterials && !hasRefraction && !hasFakePerspective)
                EditorGUILayout.HelpBox(Game2DWaterKitStyles.DisabledRefractionMessage, MessageType.Info);

            EditorGUI.BeginDisabledGroup(hasDifferentMaterials || (!hasRefraction && !hasFakePerspective));

            BeginPropertiesGroup();
            DrawRenderingCullingMaskProperties("refraction", hasFakePerspective);
            EndPropertiesGroup();

            DrawRenderTextureProperties("refraction");

            EditorGUI.EndDisabledGroup();
        }

        private void DrawReflectionProperties()
        {
            bool hasDifferentMaterials = _materialProperty.hasMultipleDifferentValues;
            bool hasReflection = false;
            bool hasFakePerspective = false;

            if (!hasDifferentMaterials)
            {
                var material = _materialProperty.objectReferenceValue as UnityEngine.Material;
                hasReflection = material != null ? material.IsKeywordEnabled("Water2D_Reflection") : false;
                hasFakePerspective = material != null ? material.IsKeywordEnabled("Water2D_FakePerspective") : false;
            }
            else EditorGUILayout.HelpBox(Game2DWaterKitStyles.CantMultiEditBecauseUsingDifferentMaterialMessage, MessageType.Info);

            if (!hasDifferentMaterials && !hasReflection)
                EditorGUILayout.HelpBox(Game2DWaterKitStyles.DisabledReflectionMessage, MessageType.Info);

            EditorGUI.BeginDisabledGroup(!hasReflection);

            BeginPropertiesGroup();
            DrawRenderingCullingMaskProperties("reflection", hasFakePerspective);
            EndPropertiesGroup();

            BeginPropertiesGroup(false, "View Frustum Properties");
            DrawProperty("reflectionYOffset", Game2DWaterKitStyles.ReflectionYOffsetPropertyLabel);
            DrawProperty("reflectionZOffset", Game2DWaterKitStyles.ReflectionZOffsetPropertyLabel);
            DrawReflectionViewingFrustumHeightPropertiesGroup(hasFakePerspective);
            EndPropertiesGroup();

            DrawRenderTextureProperties("reflection");

            EditorGUI.EndDisabledGroup();
        }

        private void DrawRenderingCullingMaskProperties(string propertyName, bool isFakePerspectiveEnabled)
        {
            float defaultMinimumLabelWidth = Game2DWaterKitStyles.MinimumLabelWidth;

            Game2DWaterKitStyles.MinimumLabelWidth = isFakePerspectiveEnabled ? 190f : 150f;

            var allObjectsToRenderMask = serializedObject.FindProperty(propertyName + "CullingMask");

            //All objects to render Mask
            DrawProperty(allObjectsToRenderMask, Game2DWaterKitStyles.RefractionReflectionMaskPropertyLabel);

            if (isFakePerspectiveEnabled)
            {
                //Partially submerged objects Mask
                var partiallySubmergedObjectsMask = serializedObject.FindProperty(propertyName + "PartiallySubmergedObjectsCullingMask");

                string[] displayedOptions = GetAllLayersNamesInMask(allObjectsToRenderMask.intValue);
                int mask;

                if (displayedOptions.Length == 0)
                {
                    using (new EditorGUI.DisabledGroupScope(true))
                        DrawProperty(allObjectsToRenderMask, Game2DWaterKitStyles.RefractionReflectionPartiallySubmergedObjectsMaskPropertyLabel);

                    mask = 0;
                }
                else
                {
                    var rect = EditorGUILayout.GetControlRect();
                    var partiallySubmergedObjectsLabel = EditorGUI.BeginProperty(rect, Game2DWaterKitStyles.RefractionReflectionPartiallySubmergedObjectsMaskPropertyLabel.Content, partiallySubmergedObjectsMask);
                    SetEditorGUIUtilityLabelWidth(partiallySubmergedObjectsMask, Game2DWaterKitStyles.RefractionReflectionPartiallySubmergedObjectsMaskPropertyLabel);
                    mask = EditorGUI.MaskField(rect, partiallySubmergedObjectsLabel, LayerMaskToConcatenatedLayersMask(partiallySubmergedObjectsMask.intValue, displayedOptions), displayedOptions);
                    EditorGUI.EndProperty();
                }

                partiallySubmergedObjectsMask.intValue = ConcatenatedLayersMaskToLayerMask(mask, displayedOptions);
            }

            Game2DWaterKitStyles.MinimumLabelWidth = defaultMinimumLabelWidth;
        }

        private void DrawReflectionViewingFrustumHeightPropertiesGroup(bool hasFakePerspective)
        {
            var material = _materialProperty.objectReferenceValue as UnityEngine.Material;
            bool isUsingValidShader = material != null && material.HasProperty("_SurfaceLevel");

            if (!isUsingValidShader)
                return;

            float defaultMinimumLabelWidth = Game2DWaterKitStyles.MinimumLabelWidth;

            if (hasFakePerspective)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Height Scaling Factors", EditorStyles.boldLabel);

                _previewPartiallySubmergedObjectsReflectionFrustum = DrawPropertyWithPreviewButton(serializedObject.FindProperty("reflectionPartiallySubmergedObjectsViewingFrustumHeightScalingFactor"), Game2DWaterKitStyles.ReflectionPartiallySubmergedObjectsViewingFrustumHeightScalingFactorPropertyLabel, _previewPartiallySubmergedObjectsReflectionFrustum, isUsingValidShader);
            }

            _previewReflectionFrustum = DrawPropertyWithPreviewButton(serializedObject.FindProperty("reflectionViewingFrustumHeightScalingFactor"), hasFakePerspective ? Game2DWaterKitStyles.ReflectionOtherObjectsViewingFrustumHeightScalingFactorPropertyLabel : Game2DWaterKitStyles.ReflectionViewingFrustumHeightScalingFactorPropertyLabel, _previewReflectionFrustum, isUsingValidShader);

            Game2DWaterKitStyles.MinimumLabelWidth = defaultMinimumLabelWidth;
        }

        private void DrawRenderingProperties()
        {
            var material = _materialProperty.objectReferenceValue as UnityEngine.Material;
            bool hasAnActiveEffect = material != null && material.IsKeywordEnabled("Water2D_Refraction") || material.IsKeywordEnabled("Water2D_Reflection");
            DrawRenderingModuleProperties(hasAnActiveEffect);
        }

        protected override void IterateSimulationPreview()
        {
            float deltaTime = Game2DWaterKitSimulationPreviewMode.TimeStep;

            for (int i = 0, imax = targets.Length; i < imax; i++)
            {
                if (_simulationActiveRippleType == WaveToPreviewType.DynamicWavesConstantRipples)
                    _constantRipplesModulePhysicsUpdateDelegates[i](deltaTime);

                _simulationModulePhysicsUpdateDelegates[i](deltaTime);
                _meshModuleUpdateMeshDelegates[i]();
            }
        }

        protected override void RestartSimulationPreview()
        {
            for (int i = 0, imax = targets.Length; i < imax; i++)
            {
                _simulationModuleResetSimulationDelegates[i]();
                _meshModuleUpdateMeshDelegates[i]();
            }
        }

        private void RunOnCollisionRippleSimulation(float disturbance)
        {
            Game2DWaterKitSimulationPreviewMode.Stop();

            for (int i = 0, imax = targets.Length; i < imax; i++)
            {
                var waterObject = targets[i] as Game2DWater;

                var mainModule = waterObject.MainModule;
                var meshModule = waterObject.MeshModule;
                var simulationModule = waterObject.SimulationModule;

                Vector3[] vertices = meshModule.Vertices;

                float leftBoundary = simulationModule.LeftBoundary;
                float rightBoundary = simulationModule.RightBoundary;

                int surfaceVerticesCount = meshModule.SurfaceVerticesCount;
                float subdivisionsPerUnit = (surfaceVerticesCount - (simulationModule.IsUsingCustomBoundaries ? 3 : 1)) / (rightBoundary - leftBoundary);

                Vector2 size = mainModule.WaterSize;
                float xMinSimulationRegion = _collisionRipplesSimulationRegionMin * size.x;
                float xMaxSimulationRegion = _collisionRipplesSimulationRegionMax * size.x;
                float center = (xMinSimulationRegion + xMaxSimulationRegion) * 0.5f;

                int leftMostSurfaceVertexIndex = simulationModule.IsUsingCustomBoundaries ? 1 : 0;
                int rightMostSurfaceVertexIndex = simulationModule.IsUsingCustomBoundaries ? surfaceVerticesCount - 2 : surfaceVerticesCount - 1;
                int startIndex = Mathf.Clamp(Mathf.RoundToInt((xMinSimulationRegion - leftBoundary) * subdivisionsPerUnit), leftMostSurfaceVertexIndex, rightMostSurfaceVertexIndex);
                int endIndex = Mathf.Clamp(Mathf.RoundToInt((xMaxSimulationRegion - leftBoundary) * subdivisionsPerUnit), leftMostSurfaceVertexIndex, rightMostSurfaceVertexIndex);

                for (int surfaceVertexIndex = startIndex; surfaceVertexIndex <= endIndex; surfaceVertexIndex++)
                {
                    float x = vertices[surfaceVertexIndex].x;

                    float factor = 1.0f - Mathf.Abs(x - center) / (xMaxSimulationRegion - center);

                    _simulationModuleDisturbSurfaceVertexDelegates[i](surfaceVertexIndex, -disturbance * factor);
                }
            }

            Game2DWaterKitSimulationPreviewMode.Start();
        }

        private void RunScriptGeneratedRipplesSimulation(float disturbanceFactor, bool smooth, float smoothingFactor)
        {
            Game2DWaterKitSimulationPreviewMode.Stop();

            for (int i = 0, imax = targets.Length; i < imax; i++)
            {
                var waterObject = targets[i] as Game2DWater;

                var mainModule = waterObject.MainModule;

                Vector2 size = mainModule.WaterSize;
                Vector2 position = mainModule.TransformPointLocalToWorld(new Vector3(size.x * _scriptGeneratedRipplesSimulationPosition, size.y * 0.5f));
                waterObject.ScriptGeneratedRipplesModule.GenerateRipple(position, disturbanceFactor, true, false, false, smooth, smoothingFactor);
            }

            Game2DWaterKitSimulationPreviewMode.Start();
        }

        #region SceneView
        private void DrawConstantRipplesSourcesPositionsAddRemoveHandles(Game2DWater waterObject)
        {
            List<float> ripplesSourcesPositions = waterObject.ConstantRipplesModule.SourcePositions;
            List<int> ripplesSourcesIndices = new List<int>(ripplesSourcesPositions.Count);
            Vector3[] meshVerticesPositions = waterObject.MeshModule.Vertices;
            int surfaceVertexCount = waterObject.MeshModule.SurfaceVerticesCount;

            Vector2 halfWaterSize = waterObject.MainModule.WaterSize * 0.5f;

            float leftBoundary = waterObject.SimulationModule.LeftBoundary;
            float rightBoundary = waterObject.SimulationModule.RightBoundary;
            float subdivisionsPerUnit = (waterObject.SimulationModule.IsUsingCustomBoundaries ? surfaceVertexCount - 3 : surfaceVertexCount - 1) / (rightBoundary - leftBoundary);

            int indexOffset, start, end;

            if (waterObject.SimulationModule.IsUsingCustomBoundaries)
            {
                indexOffset = 1;
                start = 1;
                end = surfaceVertexCount - 2;
            }
            else
            {
                indexOffset = 0;
                start = 0;
                end = surfaceVertexCount - 1;
            }

            bool changeMade = false;
            bool addNewSource = false;
            int index = -1;

            Quaternion handlesRotation = Quaternion.identity;
            float handlesSize = HandleUtility.GetHandleSize(waterObject.MainModule.Position) * 0.035f;
            Handles.CapFunction handlesCap = Handles.DotHandleCap;
            Color handlesColor = Handles.color;

            using (new Handles.DrawingScope(waterObject.MainModule.LocalToWorldMatrix))
            {
                for (int i = 0, maxi = ripplesSourcesPositions.Count; i < maxi; i++)
                {
                    float xPosition = ripplesSourcesPositions[i];
                    if (xPosition < leftBoundary || xPosition > rightBoundary)
                    {
                        Handles.color = Game2DWaterKitStyles.ConstantRipplesSourcesColorRemove;
                        if (Handles.Button(new Vector3(xPosition, halfWaterSize.y), handlesRotation, handlesSize, handlesSize, handlesCap))
                        {
                            changeMade = true;
                            index = i;
                            addNewSource = false;
                        }
                        ripplesSourcesIndices.Add(-1);
                    }
                    else
                    {
                        int nearestIndex = Mathf.RoundToInt((xPosition - leftBoundary) * subdivisionsPerUnit) + indexOffset;
                        ripplesSourcesIndices.Add(nearestIndex);
                    }
                }

                for (int i = start; i <= end; i++)
                {
                    Vector3 pos = meshVerticesPositions[i];

                    bool foundMatch = false;
                    int foundMatchIndex = -1;
                    for (int j = 0, maxj = ripplesSourcesIndices.Count; j < maxj; j++)
                    {
                        if (ripplesSourcesIndices[j] == i)
                        {
                            foundMatch = true;
                            foundMatchIndex = j;
                            break;
                        }
                    }

                    if (foundMatch)
                    {
                        Handles.color = Game2DWaterKitStyles.ConstantRipplesSourcesColorRemove;
                        if (Handles.Button(pos, handlesRotation, handlesSize, handlesSize, handlesCap))
                        {
                            changeMade = true;
                            index = foundMatchIndex;
                            addNewSource = false;
                        }
                    }
                    else
                    {
                        Handles.color = Game2DWaterKitStyles.ConstantRipplesSourcesColorAdd;
                        if (Handles.Button(pos, handlesRotation, handlesSize, handlesSize, handlesCap))
                        {
                            changeMade = true;
                            index = i;
                            addNewSource = true;
                        }
                    }
                }
            }

            Handles.color = handlesColor;

            if (changeMade)
            {
                Undo.RecordObject(waterObject, "editing water ripple source position");
                if (addNewSource)
                    ripplesSourcesPositions.Add(meshVerticesPositions[index].x);
                else
                    ripplesSourcesPositions.RemoveAt(index);

                EditorUtility.SetDirty(waterObject);

                waterObject.ConstantRipplesModule.SourcePositions = ripplesSourcesPositions;
            }
        }

        private void DrawWaterSubdivisionsPerUnitPreview(Game2DWater waterObject)
        {
            Vector3[] vertices = waterObject.MeshModule.Vertices;
            int surfaceVerticesCount = waterObject.MeshModule.SurfaceVerticesCount;

            int start, end;
            if (waterObject.SimulationModule.IsUsingCustomBoundaries)
            {
                start = 1;
                end = surfaceVerticesCount - 2;
            }
            else
            {
                start = 0;
                end = surfaceVerticesCount - 1;
            }

            using (new Handles.DrawingScope(Game2DWaterKitStyles.WaterSubdivisionsPreviewColor, waterObject.MainModule.LocalToWorldMatrix))
            {
                for (int i = start; i <= end; i++)
                {
                    Handles.DrawLine(vertices[i], vertices[i + surfaceVerticesCount]);
                }
            }
        }

        private void DrawCustomBoundariesPreview(Game2DWater waterObject)
        {
            Vector3[] vertices = waterObject.MeshModule.Vertices;
            int surfaceVerticesCount = waterObject.MeshModule.SurfaceVerticesCount;

            int start, end;
            if (waterObject.SimulationModule.IsUsingCustomBoundaries)
            {
                start = 1;
                end = surfaceVerticesCount - 2;
            }
            else
            {
                start = 0;
                end = surfaceVerticesCount - 1;
            }

            using (new Handles.DrawingScope(Game2DWaterKitStyles.CustomBoundariesPreviewColor, waterObject.MainModule.LocalToWorldMatrix))
            {
                Handles.DrawLine(vertices[start], vertices[start + surfaceVerticesCount]);
                Handles.DrawLine(vertices[end], vertices[end + surfaceVerticesCount]);
            }
        }

        private void DrawBuoyancySurfaceLevelPreview(Game2DWater waterObject)
        {
            if (waterObject.GetComponent<BuoyancyEffector2D>() == null)
                return;

            Vector2 halfWaterSize = waterObject.MainModule.WaterSize * 0.5f;
            float y = halfWaterSize.y * (1f - 2f * waterObject.AttachedComponentsModule.BuoyancyEffectorSurfaceLevel);
            Vector3 lineStart = waterObject.MainModule.TransformPointLocalToWorld(new Vector2(-halfWaterSize.x, y));
            Vector3 lineEnd = waterObject.MainModule.TransformPointLocalToWorld(new Vector2(halfWaterSize.x, y));

            Handles.color = Game2DWaterKitStyles.BuoyancySurfaceLevelPreviewColor;
            Handles.DrawLine(lineStart, lineEnd);
            Handles.color = Color.white;
        }

        private void DrawBoxColliderPreview(Game2DWater waterObject)
        {
            var boxCollider = waterObject.GetComponent<BoxCollider2D>();

            using (new Handles.DrawingScope(Game2DWaterKitStyles.BoxColliderPreviewColor, waterObject.MainModule.LocalToWorldMatrix))
            {
                var halfSize = boxCollider.size * 0.5f;
                var offset = boxCollider.offset;

                var topLeft = offset + new Vector2(-halfSize.x, halfSize.y);
                var topRight = offset + new Vector2(halfSize.x, halfSize.y);
                var bottomLeft = offset + new Vector2(-halfSize.x, -halfSize.y);
                var bottomRight = offset + new Vector2(halfSize.x, -halfSize.y);

                Handles.DrawLine(topLeft, topRight);
                Handles.DrawLine(topRight, bottomRight);
                Handles.DrawLine(bottomRight, bottomLeft);
                Handles.DrawLine(bottomLeft, topLeft);
            }
        }

        private void DrawReflectionViewingFrustumsPreview(Game2DWater waterObject, bool nonPartiallySubmergedObjects, bool partiallySubmergedObjects)
        {
            Color defaultHandlesColor = Handles.color;

            Vector2 waterSize = waterObject.MainModule.WaterSize;

            var waterMaterial = waterObject.MaterialModule.Material;

            if (waterMaterial == null || !waterObject.MaterialModule.IsReflectionEnabled)
                return;

            var offset = new Vector2(0f, waterObject.RenderingModule.ReflectionYOffset);

            using (new Handles.DrawingScope(waterObject.transform.localToWorldMatrix))
            {
                if (waterMaterial.IsKeywordEnabled("Water2D_FakePerspective"))
                {
                    float surfaceLevelRelative = waterObject.MaterialModule.GetSurfaceLevelNormalized();
                    float surfaceSubmergeLevelRelative = waterObject.MaterialModule.GetSubmergeLevelNormalized();

                    float surfaceLevel = waterSize.y * (surfaceLevelRelative - 0.5f);
                    float surfaceSubmergeLevel = waterSize.y * (surfaceSubmergeLevelRelative - 0.5f);
                    float scalingFactor = waterObject.RenderingModule.ReflectionPartiallySubmergedObjects.ViewingFrustumHeightScalingFactor;

                    Vector2 bottomLeft = new Vector2(-waterSize.x * 0.5f, surfaceSubmergeLevel);
                    Vector2 size = new Vector2(waterSize.x, 0.5f * (surfaceSubmergeLevel - surfaceLevel) * (1f + scalingFactor));

                    if (partiallySubmergedObjects)
                        DrawRectInSceneView(bottomLeft + offset, size, Color.red);

                    if (nonPartiallySubmergedObjects)
                    {
                        bottomLeft.y = waterSize.y * 0.5f;
                        scalingFactor = waterObject.RenderingModule.Reflection.ViewingFrustumHeightScalingFactor;
                        size.y = 0.5f * (waterSize.y * 0.5f - surfaceLevel) * (1f + scalingFactor);
                        DrawRectInSceneView(bottomLeft + offset, size, Color.green);
                    }
                }
                else
                {
                    float scalingFactor = waterObject.RenderingModule.Reflection.ViewingFrustumHeightScalingFactor;
                    Vector2 position = new Vector2(-0.5f * waterSize.x, 0.5f * waterSize.y);
                    Vector2 size = new Vector2(waterSize.x, 0.5f * waterSize.y * (1f + scalingFactor));
                    DrawRectInSceneView(position + offset, size, Color.green);
                }
            }

            Handles.color = defaultHandlesColor;
        }

        private void DrawOnCollisionRipplesSimulationRegionBoundaries(Game2DWater waterObject)
        {
            using (new Handles.DrawingScope(Game2DWaterKitStyles.OnCollisionRipplesSimulationRegionBoundariesColor, waterObject.MainModule.LocalToWorldMatrix))
            {
                Vector2 size = waterObject.MainModule.WaterSize;

                float leftBoundary = _collisionRipplesSimulationRegionMin * size.x;
                float rightBoundary = _collisionRipplesSimulationRegionMax * size.x;

                float minY = -99999f;
                float maxY = 99999f;

                float z = waterObject.MainModule.Position.z;

                Handles.DrawLine(new Vector3(leftBoundary, minY, z), new Vector3(leftBoundary, maxY, z));
                Handles.DrawLine(new Vector3(rightBoundary, minY, z), new Vector3(rightBoundary, maxY, z));
            }
        }

        private void DrawScriptGeneratedRipplesSimulationPositionMarker(Game2DWater waterObject)
        {
            using (new Handles.DrawingScope(Game2DWaterKitStyles.ScriptGeneratedRipplesSimulationRegionBoundariesColor, waterObject.MainModule.LocalToWorldMatrix))
            {
                Vector2 size = waterObject.MainModule.WaterSize;

                float vertexPositionX = _scriptGeneratedRipplesSimulationPosition * size.x;

                float minY = -99999f;
                float maxY = 99999f;

                float z = waterObject.MainModule.Position.z;

                Handles.DrawLine(new Vector3(vertexPositionX, minY, z), new Vector3(vertexPositionX, maxY, z));
            }
        }
        #endregion

        private enum WaveToPreviewType
        {
            DynamicWavesOnCollisionRipples = 0,
            DynamicWavesConstantRipples = 1,
            DynamicWavesScriptGeneratedRipples = 2,
            SineWaves = 3
        };

        [CustomPropertyDrawer(typeof(Simulation.WaterSimulationSineWaveParameters))]
        public class WaterSimulationSineWaveParametersProperty : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                float singleLineHeight = EditorGUIUtility.singleLineHeight;

                position.height = singleLineHeight;

                EditorGUI.BeginProperty(position, label, property);

                EditorGUIUtility.labelWidth = 73f;

                position.y += 7f;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("Amplitude"), Game2DWaterKitStyles.SimulationSineWaveAmplitudePropertyLabel.Content);

                position.y += singleLineHeight + 1f;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("Length"), Game2DWaterKitStyles.SimulationSineWaveLengthPropertyLabel.Content);

                position.y += singleLineHeight + 1f;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("Velocity"), Game2DWaterKitStyles.SimulationSineWaveVelocityPropertyLabel.Content);

                position.y += singleLineHeight + 1f;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("Offset"), Game2DWaterKitStyles.SimulationSineWaveOffsetPropertyLabel.Content);

                EditorGUI.EndProperty();
            }
        }
    }
}