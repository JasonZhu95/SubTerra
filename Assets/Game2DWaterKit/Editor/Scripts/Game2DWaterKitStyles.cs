namespace Game2DWaterKit
{
    using UnityEngine;
    using UnityEditor;

    public abstract partial class Game2DWaterKitInspector : Editor
    {

        public static class Game2DWaterKitStyles
        {
            public static bool IsInitialized;

            #region GUI Styles
            public static GUIStyle HelpBoxStyle;
            public static GUIStyle GroupBoxStyle;
            public static GUIStyle BoldFoldoutStyle;
            public static GUIStyle ButtonStyle;
            #endregion

            #region Icon Buttons Labels
            public static GUIContent PreviewIconOffButtonLabel;
            public static GUIContent PreviewIconOnButtonLabel;
            public static GUIContent EditSizeIconOffButtonLabel;
            public static GUIContent EditSizeIconOnButtonLabel;
            public static GUIContent RunSimulationButtonLabel;
            public static GUIContent StopSimulationButtonLabel;
            public static GUIContent ContinueSimulationButtonLabel;
            public static GUIContent PreviewSimulationOnButtonLabel;
            public static GUIContent PreviewSimulationOffButtonLabel;
            public static GUIContent RestartSimulationButtonLabel;
            #endregion

            #region Properties Labels
            public static Game2DWaterKitPropertyLabel SizePropertyLabel;
            public static Game2DWaterKitPropertyLabel SubdivisionsPerUnitPropertyLabel;
            public static Game2DWaterKitPropertyLabel UseEdgeColliderPropertyLabel;
            public static Game2DWaterKitPropertyLabel UseBuoyancyEffectorPropertyLabel;
            public static Game2DWaterKitPropertyLabel WaveStiffnessPropertyLabel;
            public static Game2DWaterKitPropertyLabel WaveDampingPropertyLabel;
            public static Game2DWaterKitPropertyLabel WaveSpreadPropertyLabel;
            public static Game2DWaterKitPropertyLabel WaveShouldClampDisturbancePropertyLabel;
            public static Game2DWaterKitPropertyLabel WaveUseCustomBoundariesPropertyLabel;
            public static Game2DWaterKitPropertyLabel WaveFirstCustomBoundaryPropertyLabel;
            public static Game2DWaterKitPropertyLabel WaveSecondCustomBoundaryPropertyLabel;
            public static Game2DWaterKitPropertyLabel BuoyancyEffectorSurfaceLevelPropertyLabel;
            public static Game2DWaterKitPropertyLabel BuoyancyEffectorSurfaceLevelLocationPropertyLabel;
            public static Game2DWaterKitPropertyLabel CanWavesAffectRigidbodies;
            public static Game2DWaterKitPropertyLabel WavesStrengthOnRigidbodiesLabel;
            public static Game2DWaterKitPropertyLabel ActivateOnCollisionOnWaterEnterRipples;
            public static Game2DWaterKitPropertyLabel ActivateOnCollisionOnWaterExitRipples;
            public static Game2DWaterKitPropertyLabel ActivateOnCollisionOnWaterMoveRipples;
            public static Game2DWaterKitPropertyLabel OnCollisionRipplesRaycastMaskPropertyLabel;
            public static Game2DWaterKitPropertyLabel OnCollisionRipplesMatchBoxColliderTopEdgePropertyLabel;
            public static Game2DWaterKitPropertyLabel OnCollisionRipplesRaycastMinimumDepthPropertyLabel;
            public static Game2DWaterKitPropertyLabel OnCollisionRipplesRaycastMaximumDepthPropertyLabel;
            public static Game2DWaterKitPropertyLabel OnCollisionRipplesRaycastMaximumDistancePropertyLabel;
            public static Game2DWaterKitPropertyLabel OnCollisionRipplesOnWaterEnterExitMinimumVelocityPropertyLabel;
            public static Game2DWaterKitPropertyLabel OnCollisionRipplesOnWaterMoveMinimumVelocityPropertyLabel;
            public static Game2DWaterKitPropertyLabel OnCollisionRipplesOnWaterMoveSmoothFactorPropertyLabel;
            public static Game2DWaterKitPropertyLabel OnCollisionRipplesOnWaterMoveMaximumDisturbancePropertyLabel;
            public static Game2DWaterKitPropertyLabel OnCollisionRipplesIgnoreTriggersPropertyLabel;
            public static Game2DWaterKitPropertyLabel OnCollisionRipplesOnWaterEnterEventPropertyLabel;
            public static Game2DWaterKitPropertyLabel OnCollisionRipplesOnWaterExitEventPropertyLabel;
            public static Game2DWaterKitPropertyLabel ConstantRipplesUpdateWhenOffscreenPropertyLabel;
            public static Game2DWaterKitPropertyLabel ConstantRipplesRandomizeDisturbancePropertyLabel;
            public static Game2DWaterKitPropertyLabel ConstantRipplesSourcesPositionsPropertyLabel;
            public static Game2DWaterKitPropertyLabel ConstantRipplesRandomizeSourcesPositionsPropertyLabel;
            public static Game2DWaterKitPropertyLabel ConstantRipplesRandomSourceCountPropertyLabel;
            public static Game2DWaterKitPropertyLabel ConstantRipplesAllowDuplicateSourcesPositionsPropertyLabel;
            public static Game2DWaterKitPropertyLabel ConstantRipplesEditSourcesPositionsButtonLabel;
            public static Game2DWaterKitPropertyLabel ScriptGeneratedRipplesDisturbanceFactorPropertyLabel;
            public static Game2DWaterKitPropertyLabel TimeIntervalPropertyLabel;
            public static Game2DWaterKitPropertyLabel RandomizeTimeIntervalPropertyLabel;
            public static Game2DWaterKitPropertyLabel MinimumTimeIntervalPropertyLabel;
            public static Game2DWaterKitPropertyLabel MaximumTimeIntervalPropertyLabel;
            public static Game2DWaterKitPropertyLabel SmoothRipplesPropertyLabel;
            public static Game2DWaterKitPropertyLabel SmoothingFactorPropertyLabel;
            public static Game2DWaterKitPropertyLabel DisturbancePropertyLabel;
            public static Game2DWaterKitPropertyLabel MinimumDisturbancePropertyLabel;
            public static Game2DWaterKitPropertyLabel MaximumDisturbancePropertyLabel;
            public static Game2DWaterKitPropertyLabel SoundEffectAudioClipPropertyLabel;
            public static Game2DWaterKitPropertyLabel SoundEffectPoolSizePropertyLabel;
            public static Game2DWaterKitPropertyLabel SoundEffectPoolCanExpandPropertyLabel;
            public static Game2DWaterKitPropertyLabel SoundEffectUseConstantPitchPropertyLabel;
            public static Game2DWaterKitPropertyLabel SoundEffectPitchPropertyLabel;
            public static Game2DWaterKitPropertyLabel SoundEffectMinimumPitchPropertyLabel;
            public static Game2DWaterKitPropertyLabel SoundEffectMaximumPitchPropertyLabel;
            public static Game2DWaterKitPropertyLabel SoundEffectVolumePropertyLabel;
            public static Game2DWaterKitPropertyLabel SoundEffectOutputPropertyLabel;
            public static Game2DWaterKitPropertyLabel SoundEffectAudioSourcePrefabPropertyLabel;
            public static Game2DWaterKitPropertyLabel ParticleEffectParticleSystemPropertyLabel;
            public static Game2DWaterKitPropertyLabel ParticleEffectPoolSizePropertyLabel;
            public static Game2DWaterKitPropertyLabel ParticleEffectPoolCanExpandPropertyLabel;
            public static Game2DWaterKitPropertyLabel ParticleEffectSpawnOffsetPropertyLabel;
            public static Game2DWaterKitPropertyLabel ParticleEffectStopActionPropertyLabel;
            public static Game2DWaterKitPropertyLabel RefractionReflectionMaskPropertyLabel;
            public static Game2DWaterKitPropertyLabel RefractionReflectionPartiallySubmergedObjectsMaskPropertyLabel;
            public static Game2DWaterKitPropertyLabel ReflectionPartiallySubmergedObjectsViewingFrustumHeightScalingFactorPropertyLabel;
            public static Game2DWaterKitPropertyLabel ReflectionOtherObjectsViewingFrustumHeightScalingFactorPropertyLabel;
            public static Game2DWaterKitPropertyLabel ReflectionViewingFrustumHeightScalingFactorPropertyLabel;
            public static Game2DWaterKitPropertyLabel ReflectionZOffsetPropertyLabel;
            public static Game2DWaterKitPropertyLabel ReflectionYOffsetPropertyLabel;
            public static Game2DWaterKitPropertyLabel RenderTextureFixedSizePropertyLabel;
            public static Game2DWaterKitPropertyLabel RenderTextureResizingFactorPropertyLabel;
            public static Game2DWaterKitPropertyLabel RenderTextureUseFixedSizePropertyLabel;
            public static Game2DWaterKitPropertyLabel RenderTextureFilterModePropertyLabel;
            public static Game2DWaterKitPropertyLabel MaterialPropertyLabel;
            public static Game2DWaterKitPropertyLabel FarClipPlanePropertyLabel;
            public static Game2DWaterKitPropertyLabel AllowMSAAPropertyLabel;
            public static Game2DWaterKitPropertyLabel AllowHDRPropertyLabel;
            public static Game2DWaterKitPropertyLabel RenderPixelLightsPropertyLabel;
            public static Game2DWaterKitPropertyLabel SortingLayerPropertyLabel;
            public static Game2DWaterKitPropertyLabel SortingOrderInLayerPropertyLabel;

            public static Game2DWaterKitPropertyLabel MeshMaskSubdivisionsPropertyLabel;
            public static Game2DWaterKitPropertyLabel MeshMaskArePositionAndSizeLockedPropertyLabel;

            public static Game2DWaterKitPropertyLabel WaterfallAffectedWaterObjectPropertyLabel;
            public static Game2DWaterKitPropertyLabel WaterfallAffectedWaterObjectRippleSpreadPropertyLabel;
            public static Game2DWaterKitPropertyLabel WaterfallAffectedWaterObjectRippleUpdateWhenOffscreenPropertyLabel;

            public static Game2DWaterKitPropertyLabel SimulationSineWaveAmplitudePropertyLabel;
            public static Game2DWaterKitPropertyLabel SimulationSineWaveLengthPropertyLabel;
            public static Game2DWaterKitPropertyLabel SimulationSineWaveVelocityPropertyLabel;
            public static Game2DWaterKitPropertyLabel SimulationSineWaveOffsetPropertyLabel;

            public static Game2DWaterKitPropertyLabel SimulationModeTargetFrameratePropertyLabel;
            public static Game2DWaterKitPropertyLabel SimulationModeTimeStepPropertyLabel;
            public static Game2DWaterKitPropertyLabel SimulationModeOnCollisionRipplesRegionPropertyLabel;
            public static Game2DWaterKitPropertyLabel SimulationModeScriptGeneratedRipplesSourcePositionropertyLabel;
            #endregion

            #region Messages
            public static readonly string CantMultiEditBecauseUsingDifferentMaterialMessage = "Can't multi-edit these properties when the objects share different materials!";
            public static readonly string DisabledReflectionMessage = "Reflection properties are disabled. \"Reflection\" can be activated in the material inspector.";
            public static readonly string DisabledRefractionMessage = "Refraction properties are disabled. \"Refraction\" can be activated in the material inspector.";
            #endregion

            #region Sceneview Handles Colors
            public static Color BuoyancySurfaceLevelPreviewColor;
            public static Color BoxColliderPreviewColor;
            public static Color WaterSubdivisionsPreviewColor;
            public static Color ConstantRipplesSourcesColorAdd;
            public static Color ConstantRipplesSourcesColorRemove;
            public static Color CustomBoundariesPreviewColor;
            // Simulation Mode
            public static Color OnCollisionRipplesSimulationRegionBoundariesColor;
            public static Color ScriptGeneratedRipplesSimulationRegionBoundariesColor;
            // Mesh Mask Tool
            public static Color MeshMaskToolInsertControlPointColor;
            public static Color MeshMaskToolInsertControlPointHoveredColor;
            public static Color MeshMaskToolRemoveControlPointColor;
            public static Color MeshMaskToolRemoveControlPointHoveredColor;
            public static Color MeshMaskToolDefaultControlPointColor;
            public static Color MeshMaskToolDefaultControlPointHoveredColor;
            public static Color MeshMaskToolDefaultControlPointSelectedColor;
            public static Color MeshMaskToolDefaultControlPointDisabledColor;
            public static Color MeshMaskToolSegmentColor;
            public static Color MeshMaskToolSegmentHoveredColor;
            public static Color MeshMaskToolSegmentSelectedColor;
            public static Color MeshMaskToolHandleColor;
            public static Color MeshMaskToolHandleDefaultHoveredColor;
            public static Color MeshMaskToolHandleDefaultSelectedColor;
            public static Color MeshMaskToolEdgeColliderOutlinePreviewColor;
            public static Color MeshMaskToolPolygonColliderOutlinePreviewColor;
            public static Color MeshMaskToolObjectOutlinePreviewColor;
            public static Color MeshMaskToolMaskOutlinePreviewColor;
            public static Color MeshMaskToolDefaultEdgeColliderPointColor;
            public static Color MeshMaskToolDefaultEdgeColliderPointSelectedColor;
            #endregion

            #region Misc
            public static float MinimumLabelWidth = 150f;
            public static readonly string[] WavesToPreviewType = new[] { "Dynamic Waves - On Collision Ripples", "Dynamic Waves - Constant Ripples", "Dynamic Waves - Script-Genrated Ripples", "Sine Waves"};
            public static readonly string NewPrefabWorkflowMessage = "As of Unity 2018.3, disconnecting (unlinking) and relinking a Prefab instance are no longer supported. Alternatively, you can now unpack a Prefab instance if you want to entirely remove its link to its Prefab asset and thus be able to restructure the resulting plain GameObject as you please.";
            public static readonly string SimulationModuleWavePropertiesMessage = "These properties are shared between all types of ripples.";
            private static readonly GUIContent _tempLabel = new GUIContent();
            #endregion

            public static void Initialize()
            {
                UpdateHandlesColors();

                #region GUI Styles
                HelpBoxStyle = new GUIStyle(EditorStyles.helpBox);
                GroupBoxStyle = new GUIStyle("GroupBox");
                ButtonStyle = new GUIStyle("button");
                #endregion

                #region Icon Buttons Labels
                BoldFoldoutStyle = new GUIStyle(EditorStyles.foldout);
                BoldFoldoutStyle.fontStyle = FontStyle.Bold;

                string prefix = EditorGUIUtility.isProSkin ? "d_" : string.Empty;

                PreviewIconOffButtonLabel = GetButtonIconContent(prefix + "btn_preview_off", "Preview On");
                PreviewIconOnButtonLabel = GetButtonIconContent(prefix + "btn_preview_on", "Preview Off");

                EditSizeIconOffButtonLabel = GetButtonIconContent(prefix + "btn_resize_off", "Edit Size On");
                EditSizeIconOnButtonLabel = GetButtonIconContent(prefix + "btn_resize_on", "Edit Size Off");

                PreviewSimulationOffButtonLabel = GetButtonIconContent(prefix + "btn_start_off", "Enter Simulation Mode", "Enter Simulation Mode");
                PreviewSimulationOnButtonLabel = GetButtonIconContent(prefix + "btn_stop_on", "Quit Simulation Mode", "Quit Simulation Mode");
                RestartSimulationButtonLabel = GetButtonIconContent(prefix + "btn_restart_off", "Restart Simulation", "Restart Simulation");
                StopSimulationButtonLabel = GetButtonIconContent(prefix + "btn_stop_off", "Stop Simulation", "Stop Simulation");
                RunSimulationButtonLabel = GetButtonIconContent(prefix + "btn_start_off", "Run Simulation");
                #endregion

                #region Properties Labels
                SizePropertyLabel = CreatePropertyLabel("Size", "Sets the object width/height");
                SubdivisionsPerUnitPropertyLabel = CreatePropertyLabel("Subdivisions Per Unit", "Sets the number of water’s surface vertices within one unit.");
                UseEdgeColliderPropertyLabel = CreatePropertyLabel("Use Edge Collider", "Adds/Removes an EdgeCollider2D component that limits the water boundaries (left, right and bottom edges). The water script takes care of updating the edge collider points.");
                UseBuoyancyEffectorPropertyLabel = CreatePropertyLabel("Use Buoyancy Effector", "Adds/Removes a Buoyancy Effector 2D component");

                WaveStiffnessPropertyLabel = CreatePropertyLabel("Stiffness", "Controls the frequency of wave vibration. A low value will make waves oscillate slowly, while a high value will make waves oscillate quickly.");
                WaveDampingPropertyLabel = CreatePropertyLabel("Damping", "Controls how fast the waves decay. A low value will make waves oscillate for a long time, while a high value will make waves oscillate for a short time.");
                WaveShouldClampDisturbancePropertyLabel = CreatePropertyLabel("Limit maximum disturbance to", "Limit the maximum disturbance caused by different types of ripples. For instance, when two ripples happen to disturb the same surface vertex, the sum of the disturbance caused by those two ripples are added together and applied to that vertex. When this property is enabled, the resultant disturbance is clamped to this maximum disturbance value. So the applied disturbance to the surface vertex is never greater than this maximum value.");
                WaveSpreadPropertyLabel = CreatePropertyLabel("Spread", "Controls how fast the waves spread.");
                WaveUseCustomBoundariesPropertyLabel = CreatePropertyLabel("Use Custom Boundaries", "Enable/Disable using custom wave boundaries. When waves reach a boundary, they bounce back.");
                WaveFirstCustomBoundaryPropertyLabel = CreatePropertyLabel("First Boundary", "The location of the first boundary.");
                WaveSecondCustomBoundaryPropertyLabel = CreatePropertyLabel("Second Boundary", "The location of the second boundary.");
                BuoyancyEffectorSurfaceLevelPropertyLabel = CreatePropertyLabel("Buoyancy Level", "Sets the surface location of the buoyancy fluid. When an object is above this line, no buoyancy forces are applied. When an object is intersecting or completely below this line, buoyancy forces are applied.");
                BuoyancyEffectorSurfaceLevelLocationPropertyLabel = CreatePropertyLabel("Match Buoyancy Level To", null);
                CanWavesAffectRigidbodies = CreatePropertyLabel("Waves Can Affect Rigidbodies", "Controls whether or not floating rigidbodies follow the undulations of water.");
                WavesStrengthOnRigidbodiesLabel = CreatePropertyLabel("Strength", "Controls the strength of the force to apply to rigidbodies floating on water.");

                ActivateOnCollisionOnWaterEnterRipples = CreatePropertyLabel("A Rigidbody Enters The Water", null);
                ActivateOnCollisionOnWaterExitRipples = CreatePropertyLabel("A Rigidbody Exits The Water", null);
                ActivateOnCollisionOnWaterMoveRipples = CreatePropertyLabel("A Rigidbody Moves In Water", null);
                OnCollisionRipplesRaycastMaskPropertyLabel = CreatePropertyLabel("Collision Mask", "Only objects on these layers will disturb the water’s surface and will trigger the OnWaterEnter and the OnWaterExit events when they get into or out of the water.");
                OnCollisionRipplesRaycastMinimumDepthPropertyLabel = CreatePropertyLabel("Minimum Depth", "Only objects with Z coordinate (depth) greater than or equal to this value will disturb the water’s surface.");
                OnCollisionRipplesRaycastMaximumDepthPropertyLabel = CreatePropertyLabel("Maximum Depth", "Only objects with Z coordinate (depth) less than or equal to this value will disturb the water’s surface.");
                OnCollisionRipplesRaycastMaximumDistancePropertyLabel = CreatePropertyLabel("Maximum Distance", "The maximum distance from the water's surface over which to check for collisions (Default: 0.5)");
                OnCollisionRipplesOnWaterEnterExitMinimumVelocityPropertyLabel = CreatePropertyLabel("Minimum Velocity", "Sets the minimum velocity that a rigidbody falling into water should have to cause the maximum disturbance to the water's surface.");
                OnCollisionRipplesIgnoreTriggersPropertyLabel = CreatePropertyLabel("Ignore Triggers", "Controls whether or not a collider that is marked as \"Is Trigger\" can disturb the water’s surface and trigger the OnWaterEnter and the OnWaterExit events when it gets into or out of the water.");
                OnCollisionRipplesOnWaterEnterEventPropertyLabel = CreatePropertyLabel("On Water Enter", "Event that is triggered when a rigidbody falls into water.");
                OnCollisionRipplesOnWaterExitEventPropertyLabel = CreatePropertyLabel("On Water Exit", "Event that is triggered when a rigidbody gets out of the water.");
                OnCollisionRipplesMatchBoxColliderTopEdgePropertyLabel = CreatePropertyLabel("Match Box Collider's Top Edge To", null);
                OnCollisionRipplesOnWaterMoveMaximumDisturbancePropertyLabel = CreatePropertyLabel("Maximum Disturbance", "Sets the maximum displacement of the water’s surface.");
                OnCollisionRipplesOnWaterMoveSmoothFactorPropertyLabel = CreatePropertyLabel("Smoothing Factor", "The relative amount of disturbance to apply to neighbor surface vertices to create a smoother ripple.");
                OnCollisionRipplesOnWaterMoveMinimumVelocityPropertyLabel = CreatePropertyLabel("Minimum Velocity", "Sets the minimum velocity that a rigidbody moving in water should have to cause the maximum disturbance to the water's surface.");

                ScriptGeneratedRipplesDisturbanceFactorPropertyLabel = CreatePropertyLabel("Disturbance Factor", "Range: [0..1]: The disturbance is linearly interpolated between the minimum disturbance and the maximum disturbance by this factor.");

                TimeIntervalPropertyLabel = CreatePropertyLabel("Time Interval", "Generate ripples at regular time interval (expressed in seconds).");
                RandomizeTimeIntervalPropertyLabel = CreatePropertyLabel("Randomize Time Inteval", "Randomize the time interval.");
                MinimumTimeIntervalPropertyLabel = CreatePropertyLabel("Minimum Time Interval", "The minimum time interval.");
                MaximumTimeIntervalPropertyLabel = CreatePropertyLabel("Maximum Time Interval", "The maximum time interval");
                DisturbancePropertyLabel = CreatePropertyLabel("Disturbance", "Sets the displacement of the water’s surface.");
                MinimumDisturbancePropertyLabel = CreatePropertyLabel("Minimum Disturbance", "Sets the minimum displacement of the water’s surface.");
                MaximumDisturbancePropertyLabel = CreatePropertyLabel("Maximum Disturbance", "Sets the maximum displacement of the water’s surface.");
                SmoothRipplesPropertyLabel = CreatePropertyLabel("Smooth Ripples", "Disturb neighbor surface vertices to create a smoother ripple.");
                SmoothingFactorPropertyLabel = CreatePropertyLabel("Smoothing Factor", "The relative amount of disturbance to apply to neighbor surface vertices.");

                SoundEffectAudioClipPropertyLabel = CreatePropertyLabel("Audio Clip", "The AudioClip asset to play.");
                SoundEffectPoolSizePropertyLabel = CreatePropertyLabel("Pool Size", "Sets the number of audio source objects that will be created and pooled when the game starts.");
                SoundEffectPoolCanExpandPropertyLabel = CreatePropertyLabel("Can Expand", "Enables/Disables increasing the number of pooled audio source objects at runtime if needed.");
                SoundEffectUseConstantPitchPropertyLabel = CreatePropertyLabel("Constant Pitch", "Apply constant audio clip playback speed.");
                SoundEffectPitchPropertyLabel = CreatePropertyLabel("Pitch", "Apply constant audio clip playback speed.");
                SoundEffectMinimumPitchPropertyLabel = CreatePropertyLabel("Minimum Pitch", "Sets the audio clip’s minimum playback speed. (when ‘Constant Pitch’ is toggled off)");
                SoundEffectMaximumPitchPropertyLabel = CreatePropertyLabel("Maximum Pitch", "Sets the audio clip’s maximum playback speed. (when constant pitch is toggled off)");
                SoundEffectVolumePropertyLabel = CreatePropertyLabel("Volume", "Sets the audio clip’s volume.");
                SoundEffectOutputPropertyLabel = CreatePropertyLabel("Output", "Sets the target group to which the AudioSource should route its signal");
                SoundEffectAudioSourcePrefabPropertyLabel = CreatePropertyLabel("Audio Source Prefab", "Sets a prefab from which audio sources will be instantiated");

                ParticleEffectParticleSystemPropertyLabel = CreatePropertyLabel("Particle System", "Sets the particle effect system to play.");
                ParticleEffectPoolSizePropertyLabel = CreatePropertyLabel("Pool Size", "Sets the number of particle system objects that will be created and pooled when the game starts.");
                ParticleEffectPoolCanExpandPropertyLabel = CreatePropertyLabel("Can Expand", "Enables/Disables increasing the number of pooled particle system objects at runtime if needed.");
                ParticleEffectSpawnOffsetPropertyLabel = CreatePropertyLabel("Spawn Offset", "Shifts the particle system spawn position.");
                ParticleEffectStopActionPropertyLabel = CreatePropertyLabel("Stop Action", "This UnityEvent is triggered when the particle system finishes playing.");

                ConstantRipplesUpdateWhenOffscreenPropertyLabel = CreatePropertyLabel("Continue creating ripples when off-screen", "Continue creating ripples even when the water object is not visible to any camera in the scene.");
                ConstantRipplesRandomizeDisturbancePropertyLabel = CreatePropertyLabel("Randomize Disturbance", "Randomize the disturbance (displacement) of the water's surface.");
                ConstantRipplesSourcesPositionsPropertyLabel = CreatePropertyLabel("Ripples Sources Positions (X-Axis)", "Ripples sources positions list.");
                ConstantRipplesRandomizeSourcesPositionsPropertyLabel = CreatePropertyLabel("Randomize Positions", "Randomize constant ripples sources positions. When checked, random surface vertices are disturbed each time the constant ripples are generated.");
                ConstantRipplesRandomSourceCountPropertyLabel = CreatePropertyLabel("Ripples Source Count", "When Randomize Positions is checked, this sets the number of random surface vertices to disturb when generating constant ripples.");
                ConstantRipplesAllowDuplicateSourcesPositionsPropertyLabel = CreatePropertyLabel("Allow Duplicate Positions", "Allow generating multiple ripples in the same position and at the same time.");
                ConstantRipplesEditSourcesPositionsButtonLabel = CreatePropertyLabel("Edit Positions", "Add/Remove ripples sources positions in the sceneview.");

                RefractionReflectionMaskPropertyLabel = CreatePropertyLabel("Objects To Render", "Only objects on these layers will be rendered by the water camera.");
                RefractionReflectionPartiallySubmergedObjectsMaskPropertyLabel = CreatePropertyLabel("Partially Submerged Objects", "Objects on these layers will be rendered as partially submerged into water when they intersect the submerge level.");
                ReflectionPartiallySubmergedObjectsViewingFrustumHeightScalingFactorPropertyLabel = CreatePropertyLabel("Partially Submerged Objects", "Sets how much to scale the partially submerged objects reflection camera viewing frustum height. The default viewing frustum height is equal to the distance between the surface level and the submerge level.");
                ReflectionOtherObjectsViewingFrustumHeightScalingFactorPropertyLabel = CreatePropertyLabel("Other Objects", "Sets how much to scale the reflection camera viewing frustum height when rendering other objects (all objects specified in ‘Objects to render’ layers except those specified in ‘Partially Submerged Objects’ layers). The default viewing frustum height for the reflection camera is equal to the surface thickness.");
                ReflectionViewingFrustumHeightScalingFactorPropertyLabel = CreatePropertyLabel("Height Scaling Factor", "Sets how much to scale the reflection camera viewing frustum height.");
                ReflectionZOffsetPropertyLabel = CreatePropertyLabel("Z-Offset", "Controls where to start rendering the reflection relative to the water object z-position.");
                ReflectionYOffsetPropertyLabel = CreatePropertyLabel("Y-Offset", "Controls how much to offset the position of the reflection camera along the y-axis.");

                RenderTextureFixedSizePropertyLabel = CreatePropertyLabel("Size", "Sets the render texture size.");
                RenderTextureResizingFactorPropertyLabel = CreatePropertyLabel("Resizing Factor", "Specifies how much the RenderTexture is resized. The \"normal\" (before resizing) RenderTexture size is equal to the water visible area size");
                RenderTextureUseFixedSizePropertyLabel = CreatePropertyLabel("Use Fixed Size", "Use a fixed render texture size.");
                RenderTextureFilterModePropertyLabel = CreatePropertyLabel("Filter Mode", "Sets the render texture filter mode.");
                MaterialPropertyLabel = CreatePropertyLabel("Material", null);
                FarClipPlanePropertyLabel = CreatePropertyLabel("Far Clip Plane", "Sets the furthest point relative to the water that will be included in the refraction/reflection rendering.");
                AllowMSAAPropertyLabel = CreatePropertyLabel("Allow MSAA", "Allow multi-sample anti-aliasing rendering.");
                AllowHDRPropertyLabel = CreatePropertyLabel("Allow HDR", "Allow high dynamic range rendering.");
                RenderPixelLightsPropertyLabel = CreatePropertyLabel("Render Pixel Lights", "Controls whether or not the rendered objects will be affected by pixel lights. Disabling this property could increase performance at the expense of visual fidelity.");
                SortingLayerPropertyLabel = CreatePropertyLabel("Sorting Layer", "The name of the mesh renderer sorting layer.");
                SortingOrderInLayerPropertyLabel = CreatePropertyLabel("Order In Layer", "The order within the sorting layer.");

                MeshMaskSubdivisionsPropertyLabel = CreatePropertyLabel("Subdivisions", "Sets how many time to split a segment into subsegments.");
                MeshMaskArePositionAndSizeLockedPropertyLabel = CreatePropertyLabel("Lock Position And Size", "When this property is enabled, the mesh mask's position and size are no longer updated to match the water object's position and size.");

                WaterfallAffectedWaterObjectPropertyLabel = CreatePropertyLabel("Water Object", "The water object to disturb.");
                WaterfallAffectedWaterObjectRippleSpreadPropertyLabel = CreatePropertyLabel("Spread", "Controls the weight of the disturbance. Setting this property to 1 (full weight), the waterfall disturbs all the water surface vertices it overlaps.");
                WaterfallAffectedWaterObjectRippleUpdateWhenOffscreenPropertyLabel = CreatePropertyLabel("Continue creating ripples when off-screen", "Continue creating ripples even when the waterfall object is not visible to any camera in the scene.");

                SimulationSineWaveAmplitudePropertyLabel = CreatePropertyLabel("Amplitude", "The height from the water's rest position (top edge) to the wave crest.");
                SimulationSineWaveLengthPropertyLabel = CreatePropertyLabel("Length", "The crest-to-crest distance between sine waves in units.");
                SimulationSineWaveVelocityPropertyLabel = CreatePropertyLabel("Velocity", "The distance the crest moves in units per second.");
                SimulationSineWaveOffsetPropertyLabel = CreatePropertyLabel("Offset", "The starting phase of the sine wave.");

                SimulationModeTargetFrameratePropertyLabel = CreatePropertyLabel("Target Framerate", "Sets the target number of simulation iterations per second.");
                SimulationModeTimeStepPropertyLabel = CreatePropertyLabel("Timestep", "The interval in seconds at which the simulation updates.");
                SimulationModeOnCollisionRipplesRegionPropertyLabel = CreatePropertyLabel("Region", "Specifies the region where to simulate collision forces.");
                SimulationModeScriptGeneratedRipplesSourcePositionropertyLabel = CreatePropertyLabel("Position", "Sets the position where to create the ripple.");
                #endregion

                IsInitialized = true;
            }

            private static GUIContent GetButtonIconContent(string iconName, string tooltip, string text = null)
            {
                return new GUIContent
                {
                    image = (Texture)Resources.Load("Images/" + iconName),
                    text = text,
                    tooltip = tooltip
                };
            }

            public static void UpdateHandlesColors()
            {
                BuoyancySurfaceLevelPreviewColor = GetColorFromEditorPrefs("BuoyancySurfaceLevelPreviewColor", Color.cyan);
                BoxColliderPreviewColor = GetColorFromEditorPrefs("BoxColliderPreviewColor", Color.green);
                WaterSubdivisionsPreviewColor = GetColorFromEditorPrefs("WaterSubdivisionsPreviewColor", new Color(0.89f, 0.259f, 0.204f));
                ConstantRipplesSourcesColorAdd = GetColorFromEditorPrefs("ConstantRipplesSourcesColorAdd", Color.green);
                ConstantRipplesSourcesColorRemove = GetColorFromEditorPrefs("ConstantRipplesSourcesColorRemove", Color.red);
                CustomBoundariesPreviewColor = GetColorFromEditorPrefs("CustomBoundariesPreviewColor", Color.yellow);
                // Simulation Mode
                OnCollisionRipplesSimulationRegionBoundariesColor = GetColorFromEditorPrefs("OnCollisionRipplesSimulationRegionBoundariesColor", Color.cyan);
                ScriptGeneratedRipplesSimulationRegionBoundariesColor = GetColorFromEditorPrefs("ScriptGeneratedRipplesSimulationRegionBoundariesColor", Color.green);
                // Mesh Mask Tool
                MeshMaskToolInsertControlPointColor = GetColorFromEditorPrefs("MeshMaskToolInsertControlPointColor", new Color(0f, 1f, 0f, 0.6f));
                MeshMaskToolInsertControlPointHoveredColor = GetColorFromEditorPrefs("MeshMaskToolInsertControlPointHoveredColor", new Color(0f, 1f, 0f, 1f));
                MeshMaskToolRemoveControlPointColor = GetColorFromEditorPrefs("MeshMaskToolRemoveControlPointColor", new Color(1f, 0f, 0f, 0.6f));
                MeshMaskToolRemoveControlPointHoveredColor = GetColorFromEditorPrefs("MeshMaskToolRemoveControlPointHoveredColor", new Color(1f, 0f, 0f, 1f));
                MeshMaskToolDefaultControlPointColor = GetColorFromEditorPrefs("MeshMaskToolDefaultControlPointColor", new Color(0.8f, 0f, 0.2f, 0.6f));
                MeshMaskToolDefaultControlPointHoveredColor = GetColorFromEditorPrefs("MeshMaskToolDefaultControlPointHoveredColor", new Color(0.8f, 0f, 0.2f, 1f));
                MeshMaskToolDefaultControlPointSelectedColor = GetColorFromEditorPrefs("MeshMaskToolDefaultControlPointSelectedColor", new Color(0.9f, 0.4f, 0.2f, 1f));
                MeshMaskToolDefaultControlPointDisabledColor = GetColorFromEditorPrefs("MeshMaskToolDefaultControlPointDisabledColor", new Color(0.5f, 0.5f, 0.5f, 1f));
                MeshMaskToolSegmentColor = GetColorFromEditorPrefs("MeshMaskToolSegmentColor", new Color(1f, 1f, 1f, 0.6f));
                MeshMaskToolSegmentHoveredColor = GetColorFromEditorPrefs("MeshMaskToolSegmentHoveredColor", new Color(1f, 1f, 1f, 1f));
                MeshMaskToolSegmentSelectedColor = GetColorFromEditorPrefs("MeshMaskToolSegmentSelectedColor", new Color(1f, 0.2f, 0.4f, 1f));
                MeshMaskToolHandleColor = GetColorFromEditorPrefs("MeshMaskToolHandleColor", new Color(1f, 0.92f, 0.016f, 0.6f));
                MeshMaskToolHandleDefaultHoveredColor = GetColorFromEditorPrefs("MeshMaskToolHandleDefaultHoveredColor", new Color(1f, 0.92f, 0.016f, 1f));
                MeshMaskToolHandleDefaultSelectedColor = GetColorFromEditorPrefs("MeshMaskToolHandleDefaultSelectedColor", new Color(0.9f, 0.4f, 0.2f, 1f));

                MeshMaskToolEdgeColliderOutlinePreviewColor = GetColorFromEditorPrefs("MeshMaskToolEdgeColliderOutlinePreviewColor", Color.green);
                MeshMaskToolPolygonColliderOutlinePreviewColor = GetColorFromEditorPrefs("MeshMaskToolPolygonColliderOutlinePreviewColor", Color.yellow);
                MeshMaskToolObjectOutlinePreviewColor = GetColorFromEditorPrefs("MeshMaskToolObjectOutlinePreviewColor", Color.cyan);
                MeshMaskToolMaskOutlinePreviewColor = GetColorFromEditorPrefs("MeshMaskToolMaskOutlinePreviewColor", Color.red);
                MeshMaskToolDefaultEdgeColliderPointColor = GetColorFromEditorPrefs("MeshMaskToolDefaultEdgeColliderPointColor", new Color(1f, 0.92f, 0.016f, 0.6f));
                MeshMaskToolDefaultEdgeColliderPointSelectedColor = GetColorFromEditorPrefs("MeshMaskToolDefaultEdgeColliderPointSelectedColor", Color.green);
                
                SceneView.RepaintAll();
            }

            public static void ResetDefaultColors() 
            {
                SetColorInEditorPrefs("BuoyancySurfaceLevelPreviewColor", Color.cyan);
                SetColorInEditorPrefs("BoxColliderPreviewColor", Color.green);
                SetColorInEditorPrefs("WaterSubdivisionsPreviewColor", new Color(0.89f, 0.259f, 0.204f));
                SetColorInEditorPrefs("ConstantRipplesSourcesColorAdd", Color.green);
                SetColorInEditorPrefs("ConstantRipplesSourcesColorRemove", Color.red);
                SetColorInEditorPrefs("CustomBoundariesPreviewColor", Color.yellow);
                // Simulation Mode
                SetColorInEditorPrefs("OnCollisionRipplesSimulationRegionBoundariesColor", Color.cyan);
                SetColorInEditorPrefs("ScriptGeneratedRipplesSimulationRegionBoundariesColor", Color.green);
                // Mesh Mask Tool
                SetColorInEditorPrefs("MeshMaskToolInsertControlPointColor", new Color(0f, 1f, 0f, 0.6f));
                SetColorInEditorPrefs("MeshMaskToolInsertControlPointHoveredColor", new Color(0f, 1f, 0f, 1f));
                SetColorInEditorPrefs("MeshMaskToolRemoveControlPointColor", new Color(1f, 0f, 0f, 0.6f));
                SetColorInEditorPrefs("MeshMaskToolRemoveControlPointHoveredColor", new Color(1f, 0f, 0f, 1f));
                SetColorInEditorPrefs("MeshMaskToolDefaultControlPointColor", new Color(0.8f, 0f, 0.2f, 0.6f));
                SetColorInEditorPrefs("MeshMaskToolDefaultControlPointHoveredColor", new Color(0.8f, 0f, 0.2f, 1f));
                SetColorInEditorPrefs("MeshMaskToolDefaultControlPointSelectedColor", new Color(0.9f, 0.4f, 0.2f, 1f));
                SetColorInEditorPrefs("MeshMaskToolDefaultControlPointDisabledColor", new Color(0.5f, 0.5f, 0.5f, 1f));
                SetColorInEditorPrefs("MeshMaskToolSegmentColor", new Color(1f, 1f, 1f, 0.6f));
                SetColorInEditorPrefs("MeshMaskToolSegmentHoveredColor", new Color(1f, 1f, 1f, 1f));
                SetColorInEditorPrefs("MeshMaskToolSegmentSelectedColor", new Color(1f, 0.2f, 0.4f, 1f));
                SetColorInEditorPrefs("MeshMaskToolHandleColor", new Color(1f, 0.92f, 0.016f, 0.6f));
                SetColorInEditorPrefs("MeshMaskToolHandleDefaultHoveredColor", new Color(1f, 0.92f, 0.016f, 1f));
                SetColorInEditorPrefs("MeshMaskToolHandleDefaultSelectedColor", new Color(0.9f, 0.4f, 0.2f, 1f));

                SetColorInEditorPrefs("MeshMaskToolEdgeColliderOutlinePreviewColor", Color.green);
                SetColorInEditorPrefs("MeshMaskToolPolygonColliderOutlinePreviewColor", Color.yellow);
                SetColorInEditorPrefs("MeshMaskToolObjectOutlinePreviewColor", Color.cyan);
                SetColorInEditorPrefs("MeshMaskToolMaskOutlinePreviewColor", Color.red);
                SetColorInEditorPrefs("MeshMaskToolDefaultEdgeColliderPointColor", new Color(1f, 0.92f, 0.016f, 0.6f));
                SetColorInEditorPrefs("MeshMaskToolDefaultEdgeColliderPointSelectedColor", Color.green);

                SceneView.RepaintAll();
            }

            public static Color GetColorFromEditorPrefs(string key, Color defaultColor)
            {
                return JsonUtility.FromJson<Color>(EditorPrefs.GetString(key, JsonUtility.ToJson(defaultColor)));
            }

            public static void SetColorInEditorPrefs(string key, Color color)
            {
                EditorPrefs.SetString(key, JsonUtility.ToJson(color));
            }

            public static Game2DWaterKitPropertyLabel CreatePropertyLabel(string text, string tooltip)
            {
                const float labelRightMargin = 4f;

                var labelContent = new Game2DWaterKitPropertyLabel();

                labelContent.Content = new GUIContent(text, tooltip);
                labelContent.WidthRegular = EditorStyles.label.CalcSize(labelContent.Content).x + labelRightMargin;
                labelContent.WidthBold = EditorStyles.boldLabel.CalcSize(labelContent.Content).x + labelRightMargin;

                return labelContent;
            }

            public static GUIContent GetTempLabel(string text, string tooltip = null, Texture image = null)
            {
                _tempLabel.text = text;
                _tempLabel.tooltip = tooltip;
                _tempLabel.image = image;

                return _tempLabel;
            }

            public class Game2DWaterKitPropertyLabel
            {
                public GUIContent Content;
                public float WidthRegular;
                public float WidthBold;
            }
        }

    }
}