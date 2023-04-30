namespace Game2DWaterKit
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Serialization;
    using Game2DWaterKit.Animation;
    using Game2DWaterKit.AttachedComponents;
    using Game2DWaterKit.Material;
    using Game2DWaterKit.Mesh;
    using Game2DWaterKit.Rendering;
    using Game2DWaterKit.Ripples;
    using Game2DWaterKit.Ripples.Effects;
    using Game2DWaterKit.Simulation;
    using Game2DWaterKit.Main;
    using Game2DWaterKit.Utils;
    using UnityEngine.Audio;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    [AddComponentMenu("Game 2D Water Kit/Water")]
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    [ExecuteInEditMode]
    public partial class Game2DWater : Game2DWaterKitObject
    {
        #region Variables

        #region Serialized Variables

        #region Water Mesh Serialized Variables
        [SerializeField] private int subdivisionsCountPerUnit = 3;
        #endregion

        #region Water Simulation Serialized Variables
        [SerializeField, Range(0f, 1f)] private float damping = 0.05f;
        [SerializeField] private float stiffness = 60f;
        [SerializeField] private float spread = 60f;
        [SerializeField] private bool useCustomBoundaries = false;
        [SerializeField] private float firstCustomBoundary = 0.5f;
        [SerializeField] private float secondCustomBoundary = -0.5f;
        [SerializeField] private bool waterSimulationAreSineWavesActive = false;
        [SerializeField] private List<WaterSimulationSineWaveParameters> waterSimulationSineWavesParameters = new List<WaterSimulationSineWaveParameters>();
        [SerializeField] private float maximumdDynamicWaveDisturbance = 1f;
        [SerializeField] private bool shouldClampDynamicWaveDisturbance = false;
        [SerializeField] private bool canWavesAffectRigidbodies = false;
        [SerializeField, Range(0f, 2f)] private float wavesStrengthOnRigidbodies = 0.6f;
        #endregion

        #region Water Collision Ripples Serialized Variables
        [SerializeField] private bool activateOnCollisionOnWaterEnterRipples = true;
        [SerializeField] private bool activateOnCollisionOnWaterExitRipples = true;
        //Disturbance Properties
        [FormerlySerializedAs("minimumDisturbance"), SerializeField] private float onCollisionRipplesMinimumDisturbance = 0.1f;
        [FormerlySerializedAs("maximumDisturbance"), SerializeField] private float onCollisionRipplesMaximumDisturbance = 0.75f;
        [FormerlySerializedAs("velocityMultiplier"), SerializeField] private float onCollisionRipplesVelocityMultiplier = 0.12f;
        //Collision Properties
        [SerializeField] private bool onCollisionRipplesIgnoreTriggers = false;
        [FormerlySerializedAs("collisionMask"), SerializeField] private LayerMask onCollisionRipplesCollisionMask = ~(1 << 4);
        [FormerlySerializedAs("collisionMinimumDepth"), SerializeField] private float onCollisionRipplesCollisionMinimumDepth = -10f;
        [FormerlySerializedAs("collisionMaximumDepth"), SerializeField] private float onCollisionRipplesCollisionMaximumDepth = 10f;
        [FormerlySerializedAs("collisionRaycastMaxDistance"), SerializeField] private float onCollisionRipplesCollisionRaycastMaxDistance = 0.5f;
        //On Water Move Collisions Properties
        [SerializeField] private bool activateOnCollisionOnWaterMoveRipples = false;
        [SerializeField] private float onCollisionOnWaterMoveRipplesMaximumDisturbance = 0.1f;
        [SerializeField] private float onCollisionOnWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance = 6f;
        [SerializeField, Range(0f, 1f)] private float onCollisionOnWaterMoveRipplesDisturbanceSmoothFactor = 0.8f;
        //Particle Effect Properties (On Water Enter)
        [FormerlySerializedAs("activateOnCollisionSplashParticleEffect"), SerializeField] private bool onCollisionRipplesActivateOnWaterEnterParticleEffect = false;
        [FormerlySerializedAs("onCollisionSplashParticleEffect"), SerializeField] private ParticleSystem onCollisionRipplesOnWaterEnterParticleEffect = null;
        [FormerlySerializedAs("onCollisionSplashParticleEffectSpawnOffset"), SerializeField] private Vector3 onCollisionRipplesOnWaterEnterParticleEffectSpawnOffset = Vector3.zero;
        [SerializeField] private UnityEvent onCollisionRipplesOnWaterEnterParticleEffectStopAction = new UnityEvent();
        [FormerlySerializedAs("onCollisionSplashParticleEffectPoolSize"), SerializeField] private int onCollisionRipplesOnWaterEnterParticleEffectPoolSize = 10;
        [SerializeField] private bool onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary = true;
        //Sound Effect Properties (On Water Enter)
        [SerializeField] private bool onCollisionRipplesActivateOnWaterEnterSoundEffect = true;
        [FormerlySerializedAs("splashAudioClip"), SerializeField] private AudioClip onCollisionRipplesOnWaterEnterAudioClip = null;
        [FormerlySerializedAs("useConstantAudioPitch"), SerializeField] private bool onCollisionRipplesUseConstantOnWaterEnterAudioPitch = false;
        [FormerlySerializedAs("audioPitch"), SerializeField, Range(-3f, 3f)] private float onCollisionRipplesOnWaterEnterAudioPitch = 1f;
        [FormerlySerializedAs("minimumAudioPitch"), SerializeField, Range(-3f, 3f)] private float onCollisionRipplesOnWaterEnterMinimumAudioPitch = 0.75f;
        [FormerlySerializedAs("maximumAudioPitch"), SerializeField, Range(-3f, 3f)] private float onCollisionRipplesOnWaterEnterMaximumAudioPitch = 1.25f;
        [SerializeField, Range(0f, 1f)] private float onCollisionRipplesOnWaterEnterAudioVolume = 1.0f;
        [SerializeField] private int onCollisionRipplesOnWaterEnterSoundEffectPoolSize = 10;
        [SerializeField] private bool onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary = true;
        [SerializeField] private AudioMixerGroup onCollisionRipplesOnWaterEnterSoundEffectOutput = null;
        [SerializeField] private GameObject onCollisionRipplesOnWaterEnterSoundEffectAudioSourcePrefab = null;
        //Particle Effect Properties (On Water Exit)
        [SerializeField] private bool onCollisionRipplesActivateOnWaterExitParticleEffect = false;
        [SerializeField] private ParticleSystem onCollisionRipplesOnWaterExitParticleEffect = null;
        [SerializeField] private Vector3 onCollisionRipplesOnWaterExitParticleEffectSpawnOffset = Vector3.zero;
        [SerializeField] private UnityEvent onCollisionRipplesOnWaterExitParticleEffectStopAction = new UnityEvent();
        [SerializeField] private int onCollisionRipplesOnWaterExitParticleEffectPoolSize = 10;
        [SerializeField] private bool onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary = true;
        //Sound Effect Properties (On Water Exit)
        [SerializeField] private bool onCollisionRipplesActivateOnWaterExitSoundEffect = false;
        [SerializeField] private AudioClip onCollisionRipplesOnWaterExitAudioClip = null;
        [SerializeField] private bool onCollisionRipplesUseConstantOnWaterExitAudioPitch = false;
        [SerializeField, Range(-3f, 3f)] private float onCollisionRipplesOnWaterExitAudioPitch = 1f;
        [SerializeField, Range(-3f, 3f)] private float onCollisionRipplesOnWaterExitMinimumAudioPitch = 0.75f;
        [SerializeField, Range(-3f, 3f)] private float onCollisionRipplesOnWaterExitMaximumAudioPitch = 1.25f;
        [SerializeField, Range(0f, 1f)] private float onCollisionRipplesOnWaterExitAudioVolume = 1.0f;
        [SerializeField] private int onCollisionRipplesOnWaterExitSoundEffectPoolSize = 10;
        [SerializeField] private bool onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary = true;
        [SerializeField] private AudioMixerGroup onCollisionRipplesOnWaterExitSoundEffectOutput = null;
        [SerializeField] private GameObject onCollisionRipplesOnWaterExitSoundEffectAudioSourcePrefab = null;
        //Events
        [SerializeField] private UnityEvent onWaterEnter = new UnityEvent();
        [SerializeField] private UnityEvent onWaterExit = new UnityEvent();
        #endregion

        #region Water Constant Ripples Serialized Variables
        [SerializeField] private bool activateConstantRipples = false;
        [SerializeField] private bool constantRipplesUpdateWhenOffscreen = false;
        //Disturbance Properties
        [SerializeField] private bool constantRipplesRandomizeDisturbance = false;
        [SerializeField] private bool constantRipplesSmoothDisturbance = false;
        [SerializeField, Range(0f, 1f)] private float constantRipplesSmoothFactor = 0.5f;
        [SerializeField] private float constantRipplesDisturbance = 0.10f;
        [SerializeField] private float constantRipplesMinimumDisturbance = 0.08f;
        [SerializeField] private float constantRipplesMaximumDisturbance = 0.12f;
        //Interval Properties
        [SerializeField, FormerlySerializedAs("constantRipplesRandomizeInterval")] private bool constantRipplesRandomizeTimeInterval = false;
        [SerializeField, FormerlySerializedAs("constantRipplesInterval")] private float constantRipplesTimeInterval = 1f;
        [SerializeField, FormerlySerializedAs("constantRipplesMinimumInterval")] private float constantRipplesMinimumTimeInterval = 0.75f;
        [SerializeField, FormerlySerializedAs("constantRipplesMaximumInterval")] private float constantRipplesMaximumTimeInterval = 1.25f;
        //Ripple Source Positions Properties
        [SerializeField] private bool constantRipplesRandomizeRipplesSourcesPositions = false;
        [SerializeField] private int constantRipplesRandomizeRipplesSourcesCount = 3;
        [SerializeField] private bool constantRipplesAllowDuplicateRipplesSourcesPositions = false;
        [SerializeField] private List<float> constantRipplesSourcePositions = new List<float>();
        //Sound Effect Properties
        [SerializeField] private bool constantRipplesActivateSoundEffect = false;
        [SerializeField] private bool constantRipplesUseConstantAudioPitch = false;
        [SerializeField] private AudioClip constantRipplesAudioClip = null;
        [SerializeField, Range(-3f, 3f)] private float constantRipplesAudioPitch = 1f;
        [SerializeField, Range(-3f, 3f)] private float constantRipplesMinimumAudioPitch = 0.75f;
        [SerializeField, Range(-3f, 3f)] private float constantRipplesMaximumAudioPitch = 1.25f;
        [SerializeField] private int constantRipplesSoundEffectPoolSize = 10;
        [SerializeField] private bool constantRipplesSoundEffectPoolExpandIfNecessary = true;
        [SerializeField, Range(0f, 1f)] private float constantRipplesAudioVolume = 1.0f;
        [SerializeField] private AudioMixerGroup constantRipplesSoundEffectOutput = null;
        [SerializeField] private GameObject constantRipplesSoundEffectAudioSourcePrefab = null;
        //Particle Effect Properties
        [FormerlySerializedAs("activateConstantSplashParticleEffect"), SerializeField] private bool constantRipplesActivateParticleEffect = false;
        [FormerlySerializedAs("constantSplashParticleEffect"), SerializeField] private ParticleSystem constantRipplesParticleEffect = null;
        [FormerlySerializedAs("constantSplashParticleEffectSpawnOffset"), SerializeField] private Vector3 constantRipplesParticleEffectSpawnOffset = Vector3.zero;
        [SerializeField] private UnityEvent constantRipplesParticleEffectStopAction = new UnityEvent();
        [FormerlySerializedAs("constantSplashParticleEffectPoolSize"), SerializeField] private int constantRipplesParticleEffectPoolSize = 10;
        [SerializeField] private bool constantRipplesParticleEffectPoolExpandIfNecessary = true;
        #endregion

        #region Water Script Generated Ripples Serialized Variables
        //Disturbance Properties
        [SerializeField] private float scriptGeneratedRipplesMinimumDisturbance = 0.1f;
        [SerializeField] private float scriptGeneratedRipplesMaximumDisturbance = 0.75f;
        //Sound Effect Properties
        [SerializeField] private bool scriptGeneratedRipplesActivateSoundEffect = false;
        [SerializeField] private AudioClip scriptGeneratedRipplesAudioClip = null;
        [SerializeField] private bool scriptGeneratedRipplesUseConstantAudioPitch = false;
        [SerializeField, Range(-3f, 3f)] private float scriptGeneratedRipplesAudioPitch = 1f;
        [SerializeField, Range(-3f, 3f)] private float scriptGeneratedRipplesMinimumAudioPitch = 0.75f;
        [SerializeField, Range(-3f, 3f)] private float scriptGeneratedRipplesMaximumAudioPitch = 1.25f;
        [SerializeField, Range(0f, 1f)] private float scriptGeneratedRipplesAudioVolume = 1.0f;
        [SerializeField] private int scriptGeneratedRipplesSoundEffectPoolSize = 10;
        [SerializeField] private bool scriptGeneratedRipplesSoundEffectPoolExpandIfNecessary = true;
        [SerializeField] private AudioMixerGroup scriptGeneratedRipplesSoundEffectOutput = null;
        [SerializeField] private GameObject scriptGeneratedRipplesSoundEffectAudioSourcePrefab = null;
        //Particle Effect Properties
        [SerializeField] private bool scriptGeneratedRipplesActivateParticleEffect = false;
        [SerializeField] private ParticleSystem scriptGeneratedRipplesParticleEffect = null;
        [SerializeField] private Vector3 scriptGeneratedRipplesParticleEffectSpawnOffset = Vector3.zero;
        [SerializeField] private UnityEvent scriptGeneratedRipplesParticleEffectStopAction = new UnityEvent();
        [SerializeField] private int scriptGeneratedRipplesParticleEffectPoolSize = 10;
        [SerializeField] private bool scriptGeneratedRipplesParticleEffectPoolExpandIfNecessary = true;
        #endregion

        #region Water Rendering Serialized Variables
        //Refraction Properties
        [SerializeField, Range(0f, 1f)] private float refractionRenderTextureResizeFactor = 1f;
        [SerializeField] private LayerMask refractionCullingMask = -1;
        [SerializeField] private LayerMask refractionPartiallySubmergedObjectsCullingMask = 0;
        [SerializeField] private FilterMode refractionRenderTextureFilterMode = FilterMode.Bilinear;
        [SerializeField] private bool refractionRenderTextureUseFixedSize = false;
        [SerializeField] private int refractionRenderTextureFixedSize = 256;
        //Reflection Properties
        [SerializeField, Range(0f, 1f)] private float reflectionRenderTextureResizeFactor = 1f;
        [SerializeField] private float reflectionViewingFrustumHeightScalingFactor = 1f;
        [SerializeField] private float reflectionPartiallySubmergedObjectsViewingFrustumHeightScalingFactor = 1f;
        [SerializeField] private LayerMask reflectionCullingMask = -1;
        [SerializeField] private LayerMask reflectionPartiallySubmergedObjectsCullingMask = 0;
        [SerializeField] private FilterMode reflectionRenderTextureFilterMode = FilterMode.Bilinear;
        [SerializeField] private bool reflectionRenderTextureUseFixedSize = false;
        [SerializeField] private int reflectionRenderTextureFixedSize = 256;
        [SerializeField] private float reflectionZOffset = 0f;
        [SerializeField] private float reflectionYOffset = 0f;
        //Shared Properties
        [SerializeField, FormerlySerializedAs("sortingLayerID")] private int _renderingModuleSortingLayerID = 0;
        [SerializeField, FormerlySerializedAs("sortingOrder")] private int _renderingModuleSortingOrder = 0;
        [SerializeField, FormerlySerializedAs("farClipPlane")] private float _renderingModuleFarClipPlane = 1000f;
        [SerializeField, FormerlySerializedAs("renderPixelLights")] private bool _renderingModuleRenderPixelLights = true;
        [SerializeField, FormerlySerializedAs("allowMSAA")] private bool _renderingModuleAllowMSAA = false;
        [SerializeField, FormerlySerializedAs("allowHDR")] private bool _renderingModuleAllowHDR = false;
        #endregion

        #region Water Buoyancy Serialized Variables
        [SerializeField, Range(0f, 1f)] private float buoyancyEffectorSurfaceLevel = 0.02f;
        [SerializeField] private WaterAttachedComponentsModule.BuoyancyEffector2DSurfaceLevelLocation buoyancyEffectorSurfaceLevelLocation = WaterAttachedComponentsModule.BuoyancyEffector2DSurfaceLevelLocation.Custom;
        [SerializeField] private WaterAttachedComponentsModule.BoxCollider2DTopEdgeLocation boxColliderTopEdgeLocation = WaterAttachedComponentsModule.BoxCollider2DTopEdgeLocation.WaterTopEdge;
        #endregion

        #endregion

        private Transform _ripplesEffectsRoot;

        private WaterMainModule _mainModule;
        private WaterRenderingModule _renderingModule;
        private WaterSimulationModule _simulationModule;
        private WaterAnimationModule _animationModule;
        private WaterCollisionRipplesModule _onCollisonRipplesModule;
        private WaterConstantRipplesModule _constantRipplesModule;
        private WaterScriptGeneratedRipplesModule _scriptGeneratedRipplesModule;
        private WaterAttachedComponentsModule _attachedComponentsModule;
        private WaterWaterfallRipplesModule _waterfallRipplesModule;
        private WaterMeshModule _meshModule;
        private WaterMaterialModule _materialModule;
        #endregion

        #region Properties

        public WaterMainModule MainModule { get { return _mainModule; } }
        public WaterRenderingModule RenderingModule { get { return _renderingModule; } }
        public WaterMeshModule MeshModule { get { return _meshModule; } }
        public WaterMaterialModule MaterialModule { get { return _materialModule; } }
        public WaterSimulationModule SimulationModule { get { return _simulationModule; } }
        public WaterAnimationModule AnimationModule { get { return _animationModule; } }
        public WaterCollisionRipplesModule OnCollisonRipplesModule { get { return _onCollisonRipplesModule; } }
        public WaterConstantRipplesModule ConstantRipplesModule { get { return _constantRipplesModule; } }
        public WaterScriptGeneratedRipplesModule ScriptGeneratedRipplesModule { get { return _scriptGeneratedRipplesModule; } }
        public WaterWaterfallRipplesModule WaterfallRipplesModule { get { return _waterfallRipplesModule; } }
        public WaterAttachedComponentsModule AttachedComponentsModule { get { return _attachedComponentsModule; } }

        #endregion

        #region Methods

        public override void InitializeModules()
        {
            if (_areModulesInitialized)
                return;

            _mainModule = new WaterMainModule(this, _size);
            _renderingModule = new WaterRenderingModule(this, GetRenderingModuleParameters());
            _simulationModule = new WaterSimulationModule(this, GetSimulationModuleParameters());
            _meshModule = new WaterMeshModule(this, subdivisionsCountPerUnit);
            _materialModule = new WaterMaterialModule(this);
            _animationModule = new WaterAnimationModule(this);
            _onCollisonRipplesModule = new WaterCollisionRipplesModule(this, GetCollisionRipplesModuleParameters(), GetRipplesEffectsPoolsRoot());
            _constantRipplesModule = new WaterConstantRipplesModule(this, GetConstantRipplesModuleParameters(), GetRipplesEffectsPoolsRoot());
            _scriptGeneratedRipplesModule = new WaterScriptGeneratedRipplesModule(this, GetScriptGeneratedRipplesModuleParameters(), GetRipplesEffectsPoolsRoot());
            _waterfallRipplesModule = new WaterWaterfallRipplesModule(this);
            _attachedComponentsModule = new WaterAttachedComponentsModule(this, buoyancyEffectorSurfaceLevel, buoyancyEffectorSurfaceLevelLocation, boxColliderTopEdgeLocation);

            _mainModule.Initialize();
            _simulationModule.Initialize();
            _meshModule.Initialize();
            _materialModule.Initialize();
            _renderingModule.Initialize();
            _attachedComponentsModule.Initialize();
            _constantRipplesModule.Initialze();
            _onCollisonRipplesModule.Initialize();
            _scriptGeneratedRipplesModule.Initialize();
            _waterfallRipplesModule.Initialize();
            _animationModule.Initialze();

            _areModulesInitialized = true;
        }

        protected override void ActivateObjectRendering()
        {
            _renderingModule.SetActive(true);
        }

        protected override void DeactivateObjectRendering()
        {
            _renderingModule.SetActive(false);
        }

        protected override void RegularUpdate()
        {
            if (_attachedComponentsModule.HasAnimatorAttached)
                _animationModule.SyncAnimatableVariables(_size, firstCustomBoundary, secondCustomBoundary);

            _animationModule.Update();
            _mainModule.Update();

            _constantRipplesModule.Update();
            _scriptGeneratedRipplesModule.Update();
            _onCollisonRipplesModule.Update();

            _meshModule.Update();
            _attachedComponentsModule.Update();

            _renderingModule.Update();
        }

        protected override void PhysicsUpdate()
        {
            float deltaTime = Time.fixedDeltaTime * Game2DWaterKitObject.TimeScale;

            _constantRipplesModule.PhysicsUpdate(deltaTime);
            _onCollisonRipplesModule.PhysicsUpdate();

            if (!_simulationModule.IsControlledByLargeWaterAreaManager)
                _simulationModule.PhysicsUpdate(deltaTime);
        }

        protected override void Cleanup()
        {
            if (_ripplesEffectsRoot != null)
                WaterUtility.SafeDestroyObject(_ripplesEffectsRoot.gameObject);
            
            if(_areModulesInitialized)
                _renderingModule.MeshMask.Cleanup();

            _areModulesInitialized = false;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            _onCollisonRipplesModule.ResolveCollision(collider, isObjectEnteringWater: true);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            _onCollisonRipplesModule.ResolveCollision(collider, isObjectEnteringWater: false);
        }

        private Transform GetRipplesEffectsPoolsRoot()
        {
#if UNITY_EDITOR
            //We don't need to spawn ripples effects objects (audioSources and particleSystems) in edit mode
            if (!Application.isPlaying)
                return null;
#endif

            if (_ripplesEffectsRoot == null)
            {
                var ripplesEffectsRootGO = new GameObject("Ripples Effects For Water " + GetInstanceID());
                ripplesEffectsRootGO.hideFlags = HideFlags.HideInHierarchy;
                _ripplesEffectsRoot = ripplesEffectsRootGO.transform;
            }
            return _ripplesEffectsRoot;
        }

        private WaterRenderingModuleParameters GetRenderingModuleParameters()
        {
            return new WaterRenderingModuleParameters
            {
                RefractionParameters = new WaterRenderingModeParameters
                {
                    TextuerResizingFactor = refractionRenderTextureResizeFactor,
                    TextureUseFixedSize = refractionRenderTextureUseFixedSize,
                    TextureFixedSize = refractionRenderTextureFixedSize,
                    ViewingFrustumHeightScalingFactor = 1f,
                    CullingMask = refractionCullingMask,
                    FilterMode = refractionRenderTextureFilterMode
                },
                RefractionPartiallySubmergedObjectsParameters = new WaterRenderingModeParameters
                {
                    TextuerResizingFactor = refractionRenderTextureResizeFactor,
                    TextureUseFixedSize = refractionRenderTextureUseFixedSize,
                    TextureFixedSize = refractionRenderTextureFixedSize,
                    ViewingFrustumHeightScalingFactor = 1f,
                    CullingMask = refractionPartiallySubmergedObjectsCullingMask,
                    FilterMode = refractionRenderTextureFilterMode
                },
                ReflectionParameters = new WaterRenderingModeParameters
                {
                    TextuerResizingFactor = reflectionRenderTextureResizeFactor,
                    TextureUseFixedSize = reflectionRenderTextureUseFixedSize,
                    TextureFixedSize = reflectionRenderTextureFixedSize,
                    ViewingFrustumHeightScalingFactor = reflectionViewingFrustumHeightScalingFactor,
                    CullingMask = reflectionCullingMask,
                    FilterMode = reflectionRenderTextureFilterMode
                },
                ReflectionPartiallySubmergedObjectsParameters = new WaterRenderingModeParameters
                {
                    TextuerResizingFactor = reflectionRenderTextureResizeFactor,
                    TextureUseFixedSize = reflectionRenderTextureUseFixedSize,
                    TextureFixedSize = reflectionRenderTextureFixedSize,
                    ViewingFrustumHeightScalingFactor = reflectionPartiallySubmergedObjectsViewingFrustumHeightScalingFactor,
                    CullingMask = reflectionPartiallySubmergedObjectsCullingMask,
                    FilterMode = reflectionRenderTextureFilterMode
                },
                ReflectionZOffset = reflectionZOffset,
                ReflectionYOffset = reflectionYOffset,
                FarClipPlane = _renderingModuleFarClipPlane,
                RenderPixelLights = _renderingModuleRenderPixelLights,
                AllowMSAA = _renderingModuleAllowMSAA,
                AllowHDR = _renderingModuleAllowHDR,
                SortingOrder = _renderingModuleSortingOrder,
                SortingLayerID = _renderingModuleSortingLayerID,
                MeshMaskParameters = new Rendering.Mask.MeshMaskParameters
                {
                    IsActive = _meshMaskIsActive,
                    Position = _meshMaskPosition,
                    Size = _meshMaskSize,
                    ControlPoints = _meshMaskControlPoints,
                    Subdivisions = _meshMaskSubdivisions,
                    ArePositionAndSizeLocked = _meshMaskArePositionAndSizeLocked
                }
            };
        }

        private WaterSimulationModuleParameters GetSimulationModuleParameters()
        {
            return new WaterSimulationModuleParameters
            {
                Damping = damping,
                Spread = spread,
                Stiffness = stiffness,
                FirstCustomBoundary = firstCustomBoundary,
                SecondCustomBoundary = secondCustomBoundary,
                IsUsingCustomBoundaries = useCustomBoundaries,
                MaximumDynamicWavesDisturbance = maximumdDynamicWaveDisturbance,
                LimitDynamicWavesDisturbance = shouldClampDynamicWaveDisturbance,
                AreSineWavesActive = waterSimulationAreSineWavesActive,
                SineWavesParameters = waterSimulationSineWavesParameters,
                CanWavesAffectRigidbodies = canWavesAffectRigidbodies,
                WavesStrengthOnRigidbodies = wavesStrengthOnRigidbodies
            };
        }

        private WaterScriptGeneratedRipplesModuleParameters GetScriptGeneratedRipplesModuleParameters()
        {
            return new WaterScriptGeneratedRipplesModuleParameters
            {
                MinimumDisturbance = scriptGeneratedRipplesMinimumDisturbance,
                MaximumDisturbance = scriptGeneratedRipplesMaximumDisturbance,
                SoundEffectParameters = new WaterRipplesSoundEffectParameters
                {
                    IsActive = scriptGeneratedRipplesActivateSoundEffect,
                    AudioClip = scriptGeneratedRipplesAudioClip,
                    UseConstantAudioPitch = scriptGeneratedRipplesUseConstantAudioPitch,
                    AudioPitch = scriptGeneratedRipplesAudioPitch,
                    MinimumAudioPitch = scriptGeneratedRipplesMinimumAudioPitch,
                    MaximumAudioPitch = scriptGeneratedRipplesMaximumAudioPitch,
                    AudioVolume = scriptGeneratedRipplesAudioVolume,
                    PoolSize = scriptGeneratedRipplesSoundEffectPoolSize,
                    CanExpandPool = scriptGeneratedRipplesSoundEffectPoolExpandIfNecessary,
                    output = scriptGeneratedRipplesSoundEffectOutput,
                    audioSourcePrefab = scriptGeneratedRipplesSoundEffectAudioSourcePrefab
                },
                ParticleEffectParameters = new WaterRipplesParticleEffectParameters
                {
                    IsActive = scriptGeneratedRipplesActivateParticleEffect,
                    ParticleSystem = scriptGeneratedRipplesParticleEffect,
                    SpawnOffset = scriptGeneratedRipplesParticleEffectSpawnOffset,
                    StopAction = scriptGeneratedRipplesParticleEffectStopAction,
                    PoolSize = scriptGeneratedRipplesParticleEffectPoolSize,
                    CanExpandPool = scriptGeneratedRipplesParticleEffectPoolExpandIfNecessary
                }
            };
        }

        private WaterConstantRipplesModuleParameters GetConstantRipplesModuleParameters()
        {
            return new WaterConstantRipplesModuleParameters
            {
                IsActive = activateConstantRipples,
                UpdateWhenOffscreen = constantRipplesUpdateWhenOffscreen,
                RandomizeDisturbance = constantRipplesRandomizeDisturbance,
                SmoothDisturbance = constantRipplesSmoothDisturbance,
                SmoothFactor = constantRipplesSmoothFactor,
                Disturbance = constantRipplesDisturbance,
                MinimumDisturbance = constantRipplesMinimumDisturbance,
                MaximumDisturbance = constantRipplesMaximumDisturbance,
                RandomizeInterval = constantRipplesRandomizeTimeInterval,
                Interval = constantRipplesTimeInterval,
                MinimumInterval = constantRipplesMinimumTimeInterval,
                MaximumInterval = constantRipplesMaximumTimeInterval,
                RandomizeRipplesSourcesPositions = constantRipplesRandomizeRipplesSourcesPositions,
                RandomizeRipplesSourcesCount = constantRipplesRandomizeRipplesSourcesCount,
                AllowDuplicateRipplesSourcesPositions = constantRipplesAllowDuplicateRipplesSourcesPositions,
                SourcePositions = constantRipplesSourcePositions,
                SoundEffectParameters = new WaterRipplesSoundEffectParameters
                {
                    IsActive = constantRipplesActivateSoundEffect,
                    AudioClip = constantRipplesAudioClip,
                    UseConstantAudioPitch = constantRipplesUseConstantAudioPitch,
                    AudioPitch = constantRipplesAudioPitch,
                    MinimumAudioPitch = constantRipplesMinimumAudioPitch,
                    MaximumAudioPitch = constantRipplesMaximumAudioPitch,
                    AudioVolume = constantRipplesAudioVolume,
                    PoolSize = constantRipplesSoundEffectPoolSize,
                    CanExpandPool = constantRipplesSoundEffectPoolExpandIfNecessary,
                    output = constantRipplesSoundEffectOutput,
                    audioSourcePrefab = constantRipplesSoundEffectAudioSourcePrefab
                },
                ParticleEffectParameters = new WaterRipplesParticleEffectParameters
                {
                    IsActive = constantRipplesActivateParticleEffect,
                    ParticleSystem = constantRipplesParticleEffect,
                    SpawnOffset = constantRipplesParticleEffectSpawnOffset,
                    StopAction = constantRipplesParticleEffectStopAction,
                    PoolSize = constantRipplesParticleEffectPoolSize,
                    CanExpandPool = constantRipplesParticleEffectPoolExpandIfNecessary
                }
            };
        }

        private WaterCollisionRipplesModuleParameters GetCollisionRipplesModuleParameters()
        {
            return new WaterCollisionRipplesModuleParameters
            {
                ActivateOnWaterEnterRipples = activateOnCollisionOnWaterEnterRipples,
                ActivateOnWaterExitRipples = activateOnCollisionOnWaterExitRipples,
                ActivateOnWaterMoveRipples = activateOnCollisionOnWaterMoveRipples,
                CollisionIgnoreTriggers = onCollisionRipplesIgnoreTriggers,
                MinimumDisturbance = onCollisionRipplesMinimumDisturbance,
                MaximumDisturbance = onCollisionRipplesMaximumDisturbance,
                VelocityMultiplier = onCollisionRipplesVelocityMultiplier,
                OnWaterMoveRipplesMaximumDisturbance = onCollisionOnWaterMoveRipplesMaximumDisturbance,
                OnWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance = onCollisionOnWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance,
                OnWaterMoveRipplesDisturbanceSmoothFactor = onCollisionOnWaterMoveRipplesDisturbanceSmoothFactor,
                CollisionMask = onCollisionRipplesCollisionMask,
                CollisionMinimumDepth = onCollisionRipplesCollisionMinimumDepth,
                CollisionMaximumDepth = onCollisionRipplesCollisionMaximumDepth,
                CollisionRaycastMaxDistance = onCollisionRipplesCollisionRaycastMaxDistance,
                OnWaterEnter = onWaterEnter,
                OnWaterExit = onWaterExit,
                WaterEnterSoundEffectParameters = new WaterRipplesSoundEffectParameters
                {
                    IsActive = onCollisionRipplesActivateOnWaterEnterSoundEffect,
                    AudioClip = onCollisionRipplesOnWaterEnterAudioClip,
                    UseConstantAudioPitch = onCollisionRipplesUseConstantOnWaterEnterAudioPitch,
                    AudioPitch = onCollisionRipplesOnWaterEnterAudioPitch,
                    MinimumAudioPitch = onCollisionRipplesOnWaterEnterMinimumAudioPitch,
                    MaximumAudioPitch = onCollisionRipplesOnWaterEnterMaximumAudioPitch,
                    AudioVolume = onCollisionRipplesOnWaterEnterAudioVolume,
                    PoolSize = onCollisionRipplesOnWaterEnterSoundEffectPoolSize,
                    CanExpandPool = onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary,
                    output = onCollisionRipplesOnWaterEnterSoundEffectOutput,
                    audioSourcePrefab = onCollisionRipplesOnWaterEnterSoundEffectAudioSourcePrefab
                },
                WaterEnterParticleEffectParameters = new WaterRipplesParticleEffectParameters
                {
                    IsActive = onCollisionRipplesActivateOnWaterEnterParticleEffect,
                    ParticleSystem = onCollisionRipplesOnWaterEnterParticleEffect,
                    SpawnOffset = onCollisionRipplesOnWaterEnterParticleEffectSpawnOffset,
                    StopAction = onCollisionRipplesOnWaterEnterParticleEffectStopAction,
                    PoolSize = onCollisionRipplesOnWaterEnterParticleEffectPoolSize,
                    CanExpandPool = onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary
                },
                WaterExitSoundEffectParameters = new WaterRipplesSoundEffectParameters
                {
                    IsActive = onCollisionRipplesActivateOnWaterExitSoundEffect,
                    AudioClip = onCollisionRipplesOnWaterExitAudioClip,
                    UseConstantAudioPitch = onCollisionRipplesUseConstantOnWaterExitAudioPitch,
                    AudioPitch = onCollisionRipplesOnWaterExitAudioPitch,
                    MinimumAudioPitch = onCollisionRipplesOnWaterExitMinimumAudioPitch,
                    MaximumAudioPitch = onCollisionRipplesOnWaterExitMaximumAudioPitch,
                    AudioVolume = onCollisionRipplesOnWaterExitAudioVolume,
                    PoolSize = onCollisionRipplesOnWaterExitSoundEffectPoolSize,
                    CanExpandPool = onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary,
                    output = onCollisionRipplesOnWaterExitSoundEffectOutput,
                    audioSourcePrefab = onCollisionRipplesOnWaterExitSoundEffectAudioSourcePrefab
                },
                WaterExitParticleEffectParameters = new WaterRipplesParticleEffectParameters
                {
                    IsActive = onCollisionRipplesActivateOnWaterExitParticleEffect,
                    ParticleSystem = onCollisionRipplesOnWaterExitParticleEffect,
                    SpawnOffset = onCollisionRipplesOnWaterExitParticleEffectSpawnOffset,
                    StopAction = onCollisionRipplesOnWaterExitParticleEffectStopAction,
                    PoolSize = onCollisionRipplesOnWaterExitParticleEffectPoolSize,
                    CanExpandPool = onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary
                }
            };
        }

        #endregion

        #region Editor Only Methods

#if UNITY_EDITOR

        // Add menu item to create Game2D Water GameObject.
        [MenuItem("GameObject/2D Object/Game2D Water Kit/Water Object", false, 10)]
        private static void CreateWaterObject(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Water", typeof(Game2DWater), typeof(BoxCollider2D), typeof(BuoyancyEffector2D));
            var spawnPosition = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            spawnPosition.z = 0f;
            go.transform.position = spawnPosition;
            go.layer = LayerMask.NameToLayer("Water");
            var boxCollider2D = go.GetComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = boxCollider2D.usedByEffector = true;
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        protected override void ValidateProperties()
        {
            if (_areModulesInitialized)
            {
                _mainModule.Validate(_size);
                _meshModule.Validate(subdivisionsCountPerUnit);
                _simulationModule.Validate(GetSimulationModuleParameters());
                _renderingModule.Validate(GetRenderingModuleParameters());
                _onCollisonRipplesModule.Validate(GetCollisionRipplesModuleParameters());
                _constantRipplesModule.Validate(GetConstantRipplesModuleParameters());
                _scriptGeneratedRipplesModule.Validate(GetScriptGeneratedRipplesModuleParameters());
                _attachedComponentsModule.Validate(buoyancyEffectorSurfaceLevel, buoyancyEffectorSurfaceLevelLocation, boxColliderTopEdgeLocation);
            }
        }

        protected override void ResetProperties()
        {
            //Reset serialized variables to default values

            #region Water Mesh Serialized Variables
            _size = Vector2.one;
            subdivisionsCountPerUnit = 3;
            #endregion

            #region Water Simulation Serialized Variables
            damping = 0.05f;
            stiffness = 60f;
            spread = 60f;
            useCustomBoundaries = false;
            firstCustomBoundary = 0.5f;
            secondCustomBoundary = -0.5f;
            waterSimulationAreSineWavesActive = false;
            waterSimulationSineWavesParameters = new List<WaterSimulationSineWaveParameters>();
            maximumdDynamicWaveDisturbance = 1f;
            shouldClampDynamicWaveDisturbance = false;
            canWavesAffectRigidbodies = false;
            wavesStrengthOnRigidbodies = 0.6f;
            #endregion

            #region Water Collision Ripples Serialized Variables
            activateOnCollisionOnWaterEnterRipples = true;
            activateOnCollisionOnWaterExitRipples = true;
            activateOnCollisionOnWaterMoveRipples = false;
            onCollisionRipplesIgnoreTriggers = false;
            //Disturbance Properties
            onCollisionRipplesMinimumDisturbance = 0.1f;
            onCollisionRipplesMaximumDisturbance = 0.75f;
            onCollisionRipplesVelocityMultiplier = 0.12f;
            //Collision Properties
            onCollisionRipplesCollisionMask = ~(1 << 4);
            onCollisionRipplesCollisionMinimumDepth = -10f;
            onCollisionRipplesCollisionMaximumDepth = 10f;
            onCollisionRipplesCollisionRaycastMaxDistance = 0.5f;
            //Particle Effect Properties (On Water Enter)
            onCollisionRipplesActivateOnWaterEnterParticleEffect = false;
            onCollisionRipplesOnWaterEnterParticleEffect = null;
            onCollisionRipplesOnWaterEnterParticleEffectSpawnOffset = Vector3.zero;
            onCollisionRipplesOnWaterEnterParticleEffectStopAction = new UnityEvent();
            onCollisionRipplesOnWaterEnterParticleEffectPoolSize = 10;
            onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary = true;
            //Sound Effect Properties (On Water Enter)
            onCollisionRipplesActivateOnWaterEnterSoundEffect = true;
            onCollisionRipplesOnWaterEnterAudioClip = null;
            onCollisionRipplesUseConstantOnWaterEnterAudioPitch = false;
            onCollisionRipplesOnWaterEnterAudioPitch = 1f;
            onCollisionRipplesOnWaterEnterMinimumAudioPitch = 0.75f;
            onCollisionRipplesOnWaterEnterMaximumAudioPitch = 1.25f;
            onCollisionRipplesOnWaterEnterAudioVolume = 1.0f;
            onCollisionRipplesOnWaterEnterSoundEffectPoolSize = 10;
            onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary = true;
            onCollisionRipplesOnWaterEnterSoundEffectOutput = null;
            //Particle Effect Properties (On Water Exit)
            onCollisionRipplesActivateOnWaterExitParticleEffect = false;
            onCollisionRipplesOnWaterExitParticleEffect = null;
            onCollisionRipplesOnWaterExitParticleEffectSpawnOffset = Vector3.zero;
            onCollisionRipplesOnWaterExitParticleEffectStopAction = new UnityEvent();
            onCollisionRipplesOnWaterExitParticleEffectPoolSize = 10;
            onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary = true;
            //Sound Effect Properties (On Water Exit)
            onCollisionRipplesActivateOnWaterExitSoundEffect = false;
            onCollisionRipplesOnWaterExitAudioClip = null;
            onCollisionRipplesUseConstantOnWaterExitAudioPitch = false;
            onCollisionRipplesOnWaterExitAudioPitch = 1f;
            onCollisionRipplesOnWaterExitMinimumAudioPitch = 0.75f;
            onCollisionRipplesOnWaterExitMaximumAudioPitch = 1.25f;
            onCollisionRipplesOnWaterExitAudioVolume = 1.0f;
            onCollisionRipplesOnWaterExitSoundEffectPoolSize = 10;
            onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary = true;
            onCollisionRipplesOnWaterExitSoundEffectOutput = null;
            //Events
            onWaterEnter = new UnityEvent();
            onWaterExit = new UnityEvent();
            // On Water Move Ripples
            onCollisionOnWaterMoveRipplesMaximumDisturbance = 0.1f;
            onCollisionOnWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance = 6f;
            onCollisionOnWaterMoveRipplesDisturbanceSmoothFactor = 0.8f;
            #endregion

            #region Water Constant Ripples Serialized Variables
            activateConstantRipples = false;
            constantRipplesUpdateWhenOffscreen = false;
            //Disturbance Properties
            constantRipplesRandomizeDisturbance = false;
            constantRipplesSmoothDisturbance = false;
            constantRipplesSmoothFactor = 0.5f;
            constantRipplesDisturbance = 0.10f;
            constantRipplesMinimumDisturbance = 0.08f;
            constantRipplesMaximumDisturbance = 0.12f;
            //Interval Properties
            constantRipplesRandomizeTimeInterval = false;
            constantRipplesTimeInterval = 1f;
            constantRipplesMinimumTimeInterval = 0.75f;
            constantRipplesMaximumTimeInterval = 1.25f;
            //Ripple Source Positions Properties
            constantRipplesRandomizeRipplesSourcesPositions = false;
            constantRipplesRandomizeRipplesSourcesCount = 3;
            constantRipplesAllowDuplicateRipplesSourcesPositions = false;
            constantRipplesSourcePositions = new List<float>();
            //Sound Effect Properties
            constantRipplesActivateSoundEffect = false;
            constantRipplesUseConstantAudioPitch = false;
            constantRipplesAudioClip = null;
            constantRipplesAudioPitch = 1f;
            constantRipplesMinimumAudioPitch = 0.75f;
            constantRipplesMaximumAudioPitch = 1.25f;
            constantRipplesSoundEffectPoolSize = 10;
            constantRipplesSoundEffectPoolExpandIfNecessary = true;
            constantRipplesAudioVolume = 1.0f;
            constantRipplesSoundEffectOutput = null;
            //Particle Effect Properties
            constantRipplesActivateParticleEffect = false;
            constantRipplesParticleEffect = null;
            constantRipplesParticleEffectSpawnOffset = Vector3.zero;
            constantRipplesParticleEffectStopAction = new UnityEvent();
            constantRipplesParticleEffectPoolSize = 10;
            constantRipplesParticleEffectPoolExpandIfNecessary = true;
            #endregion

            #region Water Script Generated Ripples Serialized Variables
            //Disturbance Properties
            scriptGeneratedRipplesMinimumDisturbance = 0.1f;
            scriptGeneratedRipplesMaximumDisturbance = 0.75f;
            //Sound Effect Properties
            scriptGeneratedRipplesActivateSoundEffect = false;
            scriptGeneratedRipplesAudioClip = null;
            scriptGeneratedRipplesUseConstantAudioPitch = false;
            scriptGeneratedRipplesAudioPitch = 1f;
            scriptGeneratedRipplesMinimumAudioPitch = 0.75f;
            scriptGeneratedRipplesMaximumAudioPitch = 1.25f;
            scriptGeneratedRipplesAudioVolume = 1.0f;
            scriptGeneratedRipplesSoundEffectPoolSize = 10;
            scriptGeneratedRipplesSoundEffectPoolExpandIfNecessary = true;
            scriptGeneratedRipplesSoundEffectOutput = null;
            //Particle Effect Properties
            scriptGeneratedRipplesActivateParticleEffect = false;
            scriptGeneratedRipplesParticleEffect = null;
            scriptGeneratedRipplesParticleEffectSpawnOffset = Vector3.zero;
            scriptGeneratedRipplesParticleEffectStopAction = new UnityEvent();
            scriptGeneratedRipplesParticleEffectPoolSize = 10;
            scriptGeneratedRipplesParticleEffectPoolExpandIfNecessary = true;
            #endregion

            #region Water Rendering Serialized Variables
            //Refraction Properties
            refractionRenderTextureResizeFactor = 1f;
            refractionRenderTextureUseFixedSize = false;
            refractionRenderTextureFixedSize = 256;
            refractionCullingMask = -1;
            refractionPartiallySubmergedObjectsCullingMask = 0;
            refractionRenderTextureFilterMode = FilterMode.Bilinear;
            //Reflection Properties
            reflectionRenderTextureResizeFactor = 1f;
            reflectionRenderTextureUseFixedSize = false;
            reflectionRenderTextureFixedSize = 256;
            reflectionCullingMask = -1;
            reflectionPartiallySubmergedObjectsCullingMask = 0;
            reflectionRenderTextureFilterMode = FilterMode.Bilinear;
            reflectionZOffset = 0f;
            reflectionYOffset = 0f;
            //Shared Properties
            _renderingModuleSortingLayerID = 0;
            _renderingModuleSortingOrder = 0;
            _renderingModuleFarClipPlane = 100f;
            _renderingModuleRenderPixelLights = true;
            _renderingModuleAllowMSAA = false;
            _renderingModuleAllowHDR = false;
            #endregion

            #region Attached components Serialized Variables
            buoyancyEffectorSurfaceLevel = 0.02f;
            buoyancyEffectorSurfaceLevelLocation = WaterAttachedComponentsModule.BuoyancyEffector2DSurfaceLevelLocation.Custom;
            boxColliderTopEdgeLocation = WaterAttachedComponentsModule.BoxCollider2DTopEdgeLocation.WaterTopEdge;
            #endregion

            //Cleanup instantiated objects (Cameras used for rendering "refraction" and "reflection", ripples particle effect and sound effect pooled objects)
            Cleanup();

            //Reset modules!
            _areModulesInitialized = false;

            DeactivateObjectRendering();
            InitializeModules();
            ActivateObjectRendering();
        }
#endif

        #endregion
    }
}
