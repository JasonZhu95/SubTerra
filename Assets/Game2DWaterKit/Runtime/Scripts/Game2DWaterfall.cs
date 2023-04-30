namespace Game2DWaterKit
{
    using Game2DWaterKit.Main;
    using Game2DWaterKit.Mesh;
    using Game2DWaterKit.Material;
    using Game2DWaterKit.Rendering;
    using Game2DWaterKit.Ripples;
    using Game2DWaterKit.Animation;
    using Game2DWaterKit.AttachedComponents;

    using System.Collections.Generic;

    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    [AddComponentMenu("Game 2D Water Kit/Waterfall")]
    [ExecuteInEditMode, RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class Game2DWaterfall : Game2DWaterKitObject
    {
        #region Variables

        #region Serialized Variables
        [SerializeField] private bool _renderingModuleRefractionRenderTextureUseFixedSize = false;
        [SerializeField] private int _renderingModuleRefractionRenderTextureFixedSize = 256;
        [SerializeField, Range(0f, 1f)] private float _renderingModuleRefractionRenderTextureResizeFactor = 1.0f;
        [SerializeField] private LayerMask _renderingModuleRefractionCullingMask = -1;
        [SerializeField] private FilterMode _renderingModuleRefractionRenderTextureFilterMode = FilterMode.Bilinear;

        [SerializeField] private int _renderingModuleSortingLayerID = 0;
        [SerializeField] private int _renderingModuleSortingOrder = 0;
        [SerializeField] private float _renderingModuleFarClipPlane = 1000f;
        [SerializeField] private bool _renderingModuleAllowMSAA = false;
        [SerializeField] private bool _renderingModuleAllowHDR = false;
        [SerializeField] private bool _renderingModuleRenderPixelLights = true;

        [SerializeField] private bool _isRipplesModuleActive = false;
        [SerializeField] private bool _ripplesModuleUpdateWhenOffscreen = true;
        [SerializeField] private bool _ripplesModuleRandomizeTimeInterval = false;
        [SerializeField] private float _ripplesModuleTimeInterval = 1f;
        [SerializeField] private float _ripplesModuleMinimumTimeInterval = 0.5f;
        [SerializeField] private float _ripplesModuleMaximumTimeInterval = 1.0f;
        [SerializeField] private List<WaterfallAffectedWaterObjet> _ripplesModuleAffectedWaterObjects = new List<WaterfallAffectedWaterObjet>();
        
        [SerializeField] private Vector2 _meshModuleTopBottomEdgesRelativeLength = new Vector2(1f, 0f); // x = relative width (0->1) , y == 0 => bottom , y == 1 => top
        #endregion

        private WaterfallMainModule _mainModule;
        private WaterfallRenderingModule _renderingModule;
        private WaterfallMeshModule _meshModule;
        private WaterfallMaterialModule _materialModule;
        private WaterfallRipplesModule _ripplesModule;
        private WaterfallAnimationModule _animationModule;
        private WaterfallAttachedComponentsModule _attachedComponentsModule;
        #endregion

        #region Properties
        public WaterfallMainModule MainModule { get { return _mainModule; } }
        public WaterfallRenderingModule RenderingModule { get { return _renderingModule; } }
        public WaterfallMeshModule MeshModule { get { return _meshModule; } }
        public WaterfallMaterialModule MaterialModule { get { return _materialModule; } }
        public WaterfallRipplesModule RipplesModule { get { return _ripplesModule; } }
        public WaterfallAnimationModule AnimationModule { get { return _animationModule; } }
        public WaterfallAttachedComponentsModule AttachedComponentsModule { get { return _attachedComponentsModule; } }
        #endregion

        public override void InitializeModules()
        {
            if (_areModulesInitialized)
                return;

            _mainModule = new WaterfallMainModule(this, _size);
            _renderingModule = new WaterfallRenderingModule(this, GetRenderingModuleParameters());
            _meshModule = new WaterfallMeshModule(this, _meshModuleTopBottomEdgesRelativeLength);
            _materialModule = new WaterfallMaterialModule(this);
            _ripplesModule = new WaterfallRipplesModule(this, GetRipplesModuleParameters());
            _animationModule = new WaterfallAnimationModule(this);
            _attachedComponentsModule = new WaterfallAttachedComponentsModule(this);
            
            _mainModule.Initialize();
            _meshModule.Initialize();
            _materialModule.Initialize();
            _renderingModule.Initialize();
            _ripplesModule.Initialize();
            _animationModule.Initialze();
            _attachedComponentsModule.Initialize();

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
                _animationModule.SyncAnimatableVariables(_size);

            _animationModule.Update();
            _mainModule.Update();
            _meshModule.Update();

            _renderingModule.Update();
        }

        protected override void PhysicsUpdate()
        {
            _ripplesModule.PhysicsUpdate(Time.fixedDeltaTime * Game2DWaterKitObject.TimeScale);
        }

        protected override void Cleanup()
        {
            if(_areModulesInitialized)
                _renderingModule.MeshMask.Cleanup();

            _areModulesInitialized = false;
        }

        private WaterfallRipplesModuleParameters GetRipplesModuleParameters()
        {
            return new WaterfallRipplesModuleParameters
            {
                IsActive = _isRipplesModuleActive,
                UpdateWhenOffscreen = _ripplesModuleUpdateWhenOffscreen,
                AffectedWaterObjets = _ripplesModuleAffectedWaterObjects,
                TimeInterval = _ripplesModuleTimeInterval,
                RandomizeTimeInterval = _ripplesModuleRandomizeTimeInterval,
                MinimumTimeInterval = _ripplesModuleMinimumTimeInterval,
                MaximumTimeInterval = _ripplesModuleMaximumTimeInterval
            };
        }

        private WaterfallRenderingModuleParameters GetRenderingModuleParameters()
        {
            return new WaterfallRenderingModuleParameters
            {
                RefractionParameters = new WaterRenderingModeParameters
                {
                    TextuerResizingFactor = _renderingModuleRefractionRenderTextureResizeFactor,
                    ViewingFrustumHeightScalingFactor = 1f,
                    CullingMask = _renderingModuleRefractionCullingMask,
                    FilterMode = _renderingModuleRefractionRenderTextureFilterMode,
                    TextureFixedSize = _renderingModuleRefractionRenderTextureFixedSize,
                    TextureUseFixedSize = _renderingModuleRefractionRenderTextureUseFixedSize
                },
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

#if UNITY_EDITOR

        protected override void ValidateProperties()
        {
            if (!_areModulesInitialized)
                return;

            _mainModule.Validate(_size);
            _meshModule.Validate(_meshModuleTopBottomEdgesRelativeLength);
            _renderingModule.Validate(GetRenderingModuleParameters());
            _ripplesModule.Validate(GetRipplesModuleParameters());
            _attachedComponentsModule.Validate();
        }

        protected override void ResetProperties()
        {
            _size = Vector2.one;

            _renderingModuleRefractionRenderTextureUseFixedSize = false;
            _renderingModuleRefractionRenderTextureFixedSize = 256;
            _renderingModuleRefractionRenderTextureResizeFactor = 1.0f;
            _renderingModuleRefractionCullingMask = -1;
            _renderingModuleRefractionRenderTextureFilterMode = FilterMode.Bilinear;

            _renderingModuleSortingLayerID = 0;
            _renderingModuleSortingOrder = 0;
            _renderingModuleFarClipPlane = 100f;
            _renderingModuleAllowMSAA = false;
            _renderingModuleAllowHDR = false;
            _renderingModuleRenderPixelLights = true;

            _isRipplesModuleActive = false;
            _ripplesModuleUpdateWhenOffscreen = true;
            _ripplesModuleRandomizeTimeInterval = false;
            _ripplesModuleTimeInterval = 1f;
            _ripplesModuleMinimumTimeInterval = 0.5f;
            _ripplesModuleMaximumTimeInterval = 1.0f;
            _ripplesModuleAffectedWaterObjects = new List<WaterfallAffectedWaterObjet>();

            _meshModuleTopBottomEdgesRelativeLength = new Vector2(1f, 0f);

            //Reset modules!
            _areModulesInitialized = false;

            DeactivateObjectRendering();
            InitializeModules();
            ActivateObjectRendering();
        }

        // Add menu item to create Game2D Water GameObject.
        [MenuItem("GameObject/2D Object/Game2D Water Kit/Waterfall Object", false, 10)]
        private static void CreateWaterObject(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Waterfall", typeof(Game2DWaterfall));
            var spawnPosition = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            spawnPosition.z = 0f;
            go.transform.position = spawnPosition;
            go.layer = LayerMask.NameToLayer("Water");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

#endif

    }

}