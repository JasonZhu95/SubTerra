namespace Game2DWaterKit.Rendering
{
    using Game2DWaterKit.Material;
    using Game2DWaterKit.Utils;
    using Game2DWaterKit.Main;
    using Game2DWaterKit.Rendering.Mask;
    using UnityEngine;

    public class WaterRenderingModule : RenderingModule
    {
        private static SimpleFixedSizeList<Vector2> _clipeePoints;
        private static readonly Color _clearColor = Color.clear;

        private Game2DWater _waterObject;

        private WaterRenderingVisibleArea _wholeWaterVisibleArea;
        private WaterRenderingVisibleArea _surfaceVisibleArea;
        private WaterRenderingVisibleArea _surfaceBelowSubmergeLevelVisibleArea;

        private WaterRenderingCameraFrustum _renderingCameraFrustum;

#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
        UnityEngine.Rendering.ScriptableRenderContext _renderingContext;
#endif

        public WaterRenderingModule(Game2DWater waterObject, WaterRenderingModuleParameters parameters)
            : base(parameters.RenderPixelLights, parameters.FarClipPlane, parameters.AllowMSAA, parameters.AllowHDR, parameters.SortingLayerID, parameters.SortingOrder, parameters.MeshMaskParameters)
        {
            _waterObject = waterObject;

            ReflectionZOffset = parameters.ReflectionZOffset;
            ReflectionYOffset = parameters.ReflectionYOffset;

            Refraction = new WaterRenderingMode(parameters.RefractionParameters, isReflectionMode: false);
            RefractionPartiallySubmergedObjects = new WaterRenderingMode(parameters.RefractionPartiallySubmergedObjectsParameters, isReflectionMode: false);
            Reflection = new WaterRenderingMode(parameters.ReflectionParameters, isReflectionMode: true);
            ReflectionPartiallySubmergedObjects = new WaterRenderingMode(parameters.ReflectionPartiallySubmergedObjectsParameters, isReflectionMode: true);
        }

        #region Properties
        public WaterRenderingMode Refraction { get; private set; }
        public WaterRenderingMode RefractionPartiallySubmergedObjects { get; private set; }
        public WaterRenderingMode Reflection { get; private set; }
        public WaterRenderingMode ReflectionPartiallySubmergedObjects { get; private set; }
        public float ReflectionZOffset { get; set; }
        public float ReflectionYOffset { get; set; }
        #endregion

        #region Methods

        override internal void Initialize()
        {
            _mainModule = _waterObject.MainModule;
            _meshModule = _waterObject.MeshModule;
            _materialModule = _waterObject.MaterialModule;

            _wholeWaterVisibleArea = new WaterRenderingVisibleArea(_mainModule);
            _surfaceVisibleArea = new WaterRenderingVisibleArea(_mainModule);
            _surfaceBelowSubmergeLevelVisibleArea = new WaterRenderingVisibleArea(_mainModule);
            _renderingCameraFrustum = new WaterRenderingCameraFrustum(_mainModule);

            if(_clipeePoints == null)
                _clipeePoints = new SimpleFixedSizeList<Vector2>(8);

            base.Initialize();
        }

        internal override bool IsVisibleToRenderingCamera(RenderingCameraInformation renderingCameraInformation)
        {
            var renderingCameraCullingMask = renderingCameraInformation.CurrentCamera.cullingMask;
            if (renderingCameraCullingMask != (renderingCameraCullingMask | (1 << _mainModule.GameobjectLayer)))
                return false;

            bool isValidWaterSize = _mainModule.Width > 0f && _mainModule.Height > 0f;
            if (!isValidWaterSize)
                return false;

            _renderingCameraFrustum.Setup(renderingCameraInformation);

            Vector2 aabbMin = _renderingCameraFrustum.AABBMin;
            Vector2 aabbMax = _renderingCameraFrustum.AABBMax;
            Vector2 aabbExtents = (aabbMax - aabbMin) * 0.5f;
            Vector2 aabbCenter = (aabbMax + aabbMin) * 0.5f;

            if (Mathf.Abs(aabbCenter.x) > (_mainModule.Width * 0.5f + aabbExtents.x))
                return false;

            if (Mathf.Abs(aabbCenter.y) > (_mainModule.Height * 0.5f + aabbExtents.y))
                return false;

            return true;
        }

#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
        internal override void Render(UnityEngine.Rendering.ScriptableRenderContext context, RenderingCameraInformation renderingCameraInformation)
#else
        internal override void Render(RenderingCameraInformation renderingCameraInformation)
#endif
        {
            var materialModule = _materialModule as WaterMaterialModule;

            bool renderRefraction = materialModule.IsRefractionEnabled;
            bool renderReflection = materialModule.IsReflectionEnabled;
            bool fakePerspective = materialModule.IsFakePerspectiveEnabled;

            if (!(renderReflection || renderRefraction || fakePerspective))
                return;

            var mainModule = _mainModule as WaterMainModule;

            var largeWaterAreaManager = mainModule.LargeWaterAreaManager;
            bool isConnectedToLargeWaterArea = largeWaterAreaManager != null;

            if (isConnectedToLargeWaterArea && largeWaterAreaManager.HasAlreadyRenderedCurrentFrame(renderingCameraInformation.CurrentCamera))
            {
                GetCurrentFrameRenderInformationFromLargeWaterAreaManager(largeWaterAreaManager, renderRefraction, renderReflection, fakePerspective);
                return;
            }

            ComputeVisibleAreas(!isConnectedToLargeWaterArea ? _meshModule.BoundsLocalSpace : largeWaterAreaManager.GetWaterObjectsBoundsRelativeToSpecifiedWaterObject(mainModule));

#if !GAME_2D_WATER_KIT_LWRP && !GAME_2D_WATER_KIT_URP
            int pixelLightCount = QualitySettings.pixelLightCount;
            if (!_renderPixelLights)
                QualitySettings.pixelLightCount = 0;
#endif

            _meshModule.SetRendererActive(false);

            Color backgroundColor = renderingCameraInformation.CurrentCamera.backgroundColor;
            backgroundColor.a = 1f;

            bool isUsingOpaqueRenderQueue = _materialModule.RenderQueue == 2000;

            if ((renderRefraction || fakePerspective) && !isUsingOpaqueRenderQueue)
            {
                var refractionRenderingProperties = renderRefraction ? _wholeWaterVisibleArea.RefractionProperties : _surfaceVisibleArea.RefractionProperties;

                Vector3 maskPosition = refractionRenderingProperties.Position;
                maskPosition.z += refractionRenderingProperties.NearClipPlane + 0.001f;

                SetupRefractionMask(maskPosition);
            }
#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
            _renderingContext = context;
#endif

            if (!fakePerspective)
            {
                if (renderRefraction)
                {
                    if(!isUsingOpaqueRenderQueue)
                        SetRefractionMaskLayer(Refraction.GetValidRefractionMaskLayer());

                    ExecuteRendering(Refraction, _g2dwCamera, _wholeWaterVisibleArea, backgroundColor, _allowHDR, _allowMSAA);
                    materialModule.SetRefractionRenderTexture(Refraction.RenderTexture);
                }

                if (renderReflection)
                {
                    _refractionMask.SetActive(false);

                    ExecuteRendering(Reflection, _g2dwCamera, _wholeWaterVisibleArea, _clearColor, _allowHDR, _allowMSAA);
                    materialModule.SetReflectionRenderTexture(Reflection.RenderTexture);
                }
            }
            else
            {
                if (renderRefraction)
                {
                    int extraLayersToIgnoreMask = RefractionPartiallySubmergedObjects.CullingMask;

                    if (!isUsingOpaqueRenderQueue)
                        SetRefractionMaskLayer(Refraction.GetValidRefractionMaskLayer(extraLayersToIgnoreMask));

                    ExecuteRendering(Refraction, _g2dwCamera, _wholeWaterVisibleArea, backgroundColor, _allowHDR, _allowMSAA, extraLayersToIgnoreMask);
                    materialModule.SetRefractionRenderTexture(Refraction.RenderTexture);

                    if (!isUsingOpaqueRenderQueue)
                        SetRefractionMaskLayer(RefractionPartiallySubmergedObjects.GetValidRefractionMaskLayer());

                    ExecuteRendering(RefractionPartiallySubmergedObjects, _g2dwCamera, _wholeWaterVisibleArea, _clearColor, _allowHDR, _allowMSAA);
                    materialModule.SetRefractionPartiallySubmergedObjectsRenderTexture(RefractionPartiallySubmergedObjects.RenderTexture);
                }
                else
                {
                    if (_surfaceVisibleArea.IsValid)
                    {
                        if (!isUsingOpaqueRenderQueue)
                            SetRefractionMaskLayer(RefractionPartiallySubmergedObjects.GetValidRefractionMaskLayer());

                        ExecuteRendering(RefractionPartiallySubmergedObjects, _g2dwCamera, _surfaceVisibleArea, _clearColor, _allowHDR, _allowMSAA);
                        materialModule.SetRefractionPartiallySubmergedObjectsRenderTexture(RefractionPartiallySubmergedObjects.RenderTexture);
                    }
                }

                if (renderReflection)
                {
                    _refractionMask.SetActive(false);

                    ExecuteRendering(Reflection, _g2dwCamera, _surfaceVisibleArea, _clearColor, _allowHDR, _allowMSAA, ReflectionPartiallySubmergedObjects.CullingMask);
                    materialModule.SetReflectionRenderTexture(Reflection.RenderTexture);

                    ExecuteRendering(ReflectionPartiallySubmergedObjects, _g2dwCamera, _surfaceBelowSubmergeLevelVisibleArea, _clearColor, _allowHDR, _allowMSAA);
                    materialModule.SetReflectionPartiallySubmergedObjectsRenderTexture(ReflectionPartiallySubmergedObjects.RenderTexture);
                }
            }

            _refractionMask.SetActive(false);

#if !GAME_2D_WATER_KIT_LWRP && !GAME_2D_WATER_KIT_URP
            QualitySettings.pixelLightCount = pixelLightCount;
#endif

            _meshModule.SetRendererActive(true);

            if (fakePerspective && !renderRefraction)
            {
                var worldToVisibleAreaMatrix = Matrix4x4.TRS(_surfaceVisibleArea.RefractionProperties.Position, _surfaceVisibleArea.RefractionProperties.Rotation, new Vector3(1f, 1f, -1f)).inverse;
                var projectionMatrix = _surfaceVisibleArea.RefractionProperties.ProjectionMatrix;
                materialModule.SetWaterMatrix(projectionMatrix * worldToVisibleAreaMatrix * _mainModule.LocalToWorldMatrix);
                
                if (isConnectedToLargeWaterArea)
                    SetCurrentFrameRenderInformationToLargeWaterAreaManager(largeWaterAreaManager, renderRefraction, renderReflection, fakePerspective, projectionMatrix, worldToVisibleAreaMatrix, renderingCameraInformation.CurrentCamera);
            }
            else
            {
                var worldToVisibleAreaMatrix = Matrix4x4.TRS(_wholeWaterVisibleArea.RefractionProperties.Position, _wholeWaterVisibleArea.RefractionProperties.Rotation, new Vector3(1f, 1f, -1f)).inverse;
                var projectionMatrix = _wholeWaterVisibleArea.RefractionProperties.ProjectionMatrix;
                materialModule.SetWaterMatrix(projectionMatrix * worldToVisibleAreaMatrix * _mainModule.LocalToWorldMatrix);
                
                if (isConnectedToLargeWaterArea)
                    SetCurrentFrameRenderInformationToLargeWaterAreaManager(largeWaterAreaManager, renderRefraction, renderReflection, fakePerspective, projectionMatrix, worldToVisibleAreaMatrix, renderingCameraInformation.CurrentCamera);
            }

            if (fakePerspective)
            {
                var waterBottomEdgeLocalSpace = _mainModule.Height * -0.5f;
                var meshHeightInverse = 1f / _meshModule.BoundsLocalSpace.size.y;

                materialModule.SetReflectionFakePerspectiveLowerLimit((_surfaceVisibleArea.FrustumBottomEdgeLocalSpace - waterBottomEdgeLocalSpace) * meshHeightInverse);
                materialModule.SetReflectionFakePerspectiveUpperLimit((_surfaceVisibleArea.FrustumTopEdgeLocalSpace - waterBottomEdgeLocalSpace) * meshHeightInverse);
                materialModule.SetReflectionFakePerspectivePartiallySubmergedObjectsUpperLimit((_surfaceBelowSubmergeLevelVisibleArea.FrustumTopEdgeLocalSpace - waterBottomEdgeLocalSpace) * meshHeightInverse);
            }

            materialModule.SetReflectionLowerLimit(_mainModule.Height * 0.5f);
            materialModule.ValidateMaterialPropertyBlock();
        }

        private void ExecuteRendering(WaterRenderingMode renderingMode, Camera camera, WaterRenderingVisibleArea visibleArea, Color backgroundColor, bool hdr, bool msaa, int extraLayersToIgnoreMask = 0)
        {
#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
            renderingMode.Render(_renderingContext, camera, visibleArea, backgroundColor, hdr, msaa, extraLayersToIgnoreMask);
#else
            renderingMode.Render(camera, visibleArea, backgroundColor, hdr, msaa, extraLayersToIgnoreMask);
#endif
        }

        private void ComputeVisibleAreas(Bounds waterBounds)
        {
            var materialModule = _materialModule as WaterMaterialModule;

            bool isReflectionEnabled = materialModule.IsReflectionEnabled;

            _clipeePoints.Clear();
            _clipeePoints.Add(_renderingCameraFrustum.TopLeft);
            _clipeePoints.Add(_renderingCameraFrustum.TopRight);
            _clipeePoints.Add(_renderingCameraFrustum.BottomRight);
            _clipeePoints.Add(_renderingCameraFrustum.BottomLeft);

            bool isRenderingCameraFullyContainedInWaterBox = true;

            //Finding visible Water Area

            Vector2 waterBoundsMin = waterBounds.min;
            Vector2 waterBoundsMax = waterBounds.max;

            //Clip camera frustrum against water box edges to find the visible water area
            //top edge
            isRenderingCameraFullyContainedInWaterBox &= WaterUtility.ClipPointsAgainstAABBEdge(_clipeePoints, isBottomOrTopEdge: true, isBottomOrLeftEdge: false, edgePosition: waterBoundsMax.y);
            //right edge
            isRenderingCameraFullyContainedInWaterBox &= WaterUtility.ClipPointsAgainstAABBEdge(_clipeePoints, isBottomOrTopEdge: false, isBottomOrLeftEdge: false, edgePosition: waterBoundsMax.x);
            //bottom edge
            isRenderingCameraFullyContainedInWaterBox &= WaterUtility.ClipPointsAgainstAABBEdge(_clipeePoints, isBottomOrTopEdge: true, isBottomOrLeftEdge: true, edgePosition: waterBoundsMin.y);
            //left edge
            isRenderingCameraFullyContainedInWaterBox &= WaterUtility.ClipPointsAgainstAABBEdge(_clipeePoints, isBottomOrTopEdge: false, isBottomOrLeftEdge: true, edgePosition: waterBoundsMin.x);

            bool isFakePerspectiveEnabled = materialModule.IsFakePerspectiveEnabled;

            bool computePixelSize = !Refraction.RenderTextureUseFixedSize || !(isFakePerspectiveEnabled ? RefractionPartiallySubmergedObjects.RenderTextureUseFixedSize : Reflection.RenderTextureUseFixedSize);
            _wholeWaterVisibleArea.UpdateArea(_clipeePoints, _renderingCameraFrustum, isRenderingCameraFullyContainedInWaterBox, computePixelSize, _farClipPlane, true, isReflectionEnabled && !isFakePerspectiveEnabled, reflectionYOffset: ReflectionYOffset, reflectionZOffset: ReflectionZOffset, reflectionAxis: -waterBoundsMin.y, reflectionFrustumHeightScalingFactor: Reflection.ViewingFrustumHeightScalingFactor);

            if (isFakePerspectiveEnabled)
            {
                float waterBoxHeight = waterBoundsMax.y - waterBoundsMin.y;

                bool computeRefractionProperties = !materialModule.IsRefractionEnabled;

                float surfaceLevel = waterBoundsMin.y + waterBoxHeight * materialModule.GetSurfaceLevelNormalized();
                float submergeLevel = waterBoundsMin.y + waterBoxHeight * materialModule.GetSubmergeLevelNormalized();

                //Finding visible Surface Area
                isRenderingCameraFullyContainedInWaterBox &= WaterUtility.ClipPointsAgainstAABBEdge(_clipeePoints, isBottomOrTopEdge: true, isBottomOrLeftEdge: true, edgePosition: surfaceLevel);
                _surfaceVisibleArea.UpdateArea(_clipeePoints, _renderingCameraFrustum, isRenderingCameraFullyContainedInWaterBox, !Reflection.RenderTextureUseFixedSize, _farClipPlane, computeRefractionProperties, isReflectionEnabled, reflectionYOffset: ReflectionYOffset, reflectionZOffset: ReflectionZOffset, reflectionAxis: waterBoundsMax.y, reflectionFrustumHeightScalingFactor: Reflection.ViewingFrustumHeightScalingFactor);

                //Finding visible Surface Area below submerge level
                isRenderingCameraFullyContainedInWaterBox &= WaterUtility.ClipPointsAgainstAABBEdge(_clipeePoints, isBottomOrTopEdge: true, isBottomOrLeftEdge: false, edgePosition: submergeLevel);
                _surfaceBelowSubmergeLevelVisibleArea.UpdateArea(_clipeePoints, _renderingCameraFrustum, isRenderingCameraFullyContainedInWaterBox, !ReflectionPartiallySubmergedObjects.RenderTextureUseFixedSize, _farClipPlane, false, isReflectionEnabled, reflectionYOffset: ReflectionYOffset, reflectionZOffset: ReflectionZOffset, reflectionAxis: submergeLevel, reflectionFrustumHeightScalingFactor: ReflectionPartiallySubmergedObjects.ViewingFrustumHeightScalingFactor);
            }
        }

        private void GetCurrentFrameRenderInformationFromLargeWaterAreaManager(LargeWaterAreaManager largeWaterAreaManager, bool renderRefraction, bool renderReflection, bool fakePerspective)
        {
            var materialModule = _materialModule as WaterMaterialModule;

            if (!fakePerspective)
            {
                if (renderRefraction)
                    materialModule.SetRefractionRenderTexture(largeWaterAreaManager.RefractionRenderTexture);

                if (renderReflection)
                    materialModule.SetReflectionRenderTexture(largeWaterAreaManager.ReflectionRenderTexture);
            }
            else
            {
                if (renderRefraction)
                    materialModule.SetRefractionRenderTexture(largeWaterAreaManager.RefractionRenderTexture);

                if (fakePerspective)
                    materialModule.SetRefractionPartiallySubmergedObjectsRenderTexture(largeWaterAreaManager.RefractionPartiallySubmergedObjectsRenderTexture);

                if (renderReflection)
                {
                    materialModule.SetReflectionRenderTexture(largeWaterAreaManager.ReflectionRenderTexture);
                    materialModule.SetReflectionPartiallySubmergedObjectsRenderTexture(largeWaterAreaManager.ReflectionPartiallySubmergedObjectsRenderTexture);
                }
            }

            materialModule.SetWaterMatrix(largeWaterAreaManager.ProjectionMatrix * largeWaterAreaManager.WorlToVisibleAreaMatrix * _mainModule.LocalToWorldMatrix);
            materialModule.SetReflectionLowerLimit(_mainModule.Height * 0.5f);
            materialModule.ValidateMaterialPropertyBlock();
        }

        private void SetCurrentFrameRenderInformationToLargeWaterAreaManager(LargeWaterAreaManager largeWaterAreaManager, bool renderRefraction, bool renderReflection, bool fakePerspective, Matrix4x4 projectionMatrix, Matrix4x4 worldToVisibleAreaMatrix, Camera currentRenderingCamera)
        {
            if (!fakePerspective)
            {
                if (renderRefraction)
                    largeWaterAreaManager.RefractionRenderTexture = Refraction.RenderTexture;

                if (renderReflection)
                    largeWaterAreaManager.ReflectionRenderTexture = Reflection.RenderTexture;
            }
            else
            {
                if (renderRefraction)
                    largeWaterAreaManager.RefractionRenderTexture = Refraction.RenderTexture;

                if (fakePerspective)
                    largeWaterAreaManager.RefractionPartiallySubmergedObjectsRenderTexture = RefractionPartiallySubmergedObjects.RenderTexture;

                if (renderReflection)
                {
                    largeWaterAreaManager.ReflectionRenderTexture = Reflection.RenderTexture;
                    largeWaterAreaManager.ReflectionPartiallySubmergedObjectsRenderTexture = ReflectionPartiallySubmergedObjects.RenderTexture;
                }
            }

            largeWaterAreaManager.WorlToVisibleAreaMatrix = worldToVisibleAreaMatrix;
            largeWaterAreaManager.ProjectionMatrix = projectionMatrix;
            largeWaterAreaManager.MarkCurrentFrameAsRendered(currentRenderingCamera);
        }

        #endregion

        #region Editor Only Methods

#if UNITY_EDITOR

        internal void Validate(WaterRenderingModuleParameters parameters)
        {
            Refraction.Validate(parameters.RefractionParameters);
            RefractionPartiallySubmergedObjects.Validate(parameters.RefractionPartiallySubmergedObjectsParameters);
            Reflection.Validate(parameters.ReflectionParameters);
            ReflectionPartiallySubmergedObjects.Validate(parameters.ReflectionPartiallySubmergedObjectsParameters);

            RenderPixelLights = parameters.RenderPixelLights;
            FarClipPlane = parameters.FarClipPlane;
            AllowMSAA = parameters.AllowMSAA;
            AllowHDR = parameters.AllowHDR;
            SortingLayerID = parameters.SortingLayerID;
            SortingOrder = parameters.SortingOrder;
            ReflectionZOffset = parameters.ReflectionZOffset;
            ReflectionYOffset = parameters.ReflectionYOffset;

            MeshMask.Validate(parameters.MeshMaskParameters);
        }

#endif

        #endregion
    }

    public struct WaterRenderingModuleParameters
    {
        public WaterRenderingModeParameters RefractionParameters;
        public WaterRenderingModeParameters RefractionPartiallySubmergedObjectsParameters;
        public WaterRenderingModeParameters ReflectionParameters;
        public WaterRenderingModeParameters ReflectionPartiallySubmergedObjectsParameters;
        public float ReflectionZOffset;
        public float ReflectionYOffset;
        public float FarClipPlane;
        public bool RenderPixelLights;
        public bool AllowMSAA;
        public bool AllowHDR;
        public int SortingLayerID;
        public int SortingOrder;
        public MeshMaskParameters MeshMaskParameters;
    }
}
