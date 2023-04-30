namespace Game2DWaterKit.Rendering
{
    using Game2DWaterKit.Main;
    using Game2DWaterKit.Mesh;
    using Game2DWaterKit.Rendering.Mask;
    using Game2DWaterKit.Material;
    using Game2DWaterKit.Utils;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class RenderingModule
    {
        protected static Camera _g2dwCamera;
        protected static WaterRenderingRefractionMask _refractionMask;

        private static bool _resortRenderingModulesList;
        private static List<RenderingModule> _renderingModulesList = new List<RenderingModule>();
        private static RenderingCameraInformation _renderingCameraInformation = new RenderingCameraInformation();
        private static int _timeID = Shader.PropertyToID("_G2DWK_Frame_Time");
        private static float _elapsedTime = 0f;

        protected MainModule _mainModule;
        protected MeshModule _meshModule;
        protected MaterialModule _materialModule;

        protected bool _renderPixelLights;
        protected float _farClipPlane;
        protected bool _allowMSAA;
        protected bool _allowHDR;
        protected int _sortingLayerID;
        protected int _sortingOrder;

        protected MeshMask _meshMask;

        protected RenderingModule(bool renderPixelLights, float farClipPlane, bool msaa, bool hdr, int sortingLayerID, int sortingOrder, MeshMaskParameters meshMaskParameters)
        {
            _renderPixelLights = renderPixelLights;
            _farClipPlane = farClipPlane;
            _allowMSAA = msaa;
            _allowHDR = hdr;
            _sortingLayerID = sortingLayerID;
            _sortingOrder = sortingOrder;
            _meshMask = new MeshMask(meshMaskParameters);
        }

        #region Properties
        public bool AllowHDR { get { return _allowHDR; } set { _allowHDR = value; } }
        public bool AllowMSAA { get { return _allowMSAA; } set { _allowMSAA = value; } }
        public float FarClipPlane { get { return _farClipPlane; } set { _farClipPlane = Mathf.Clamp(value, 0.001f, float.MaxValue); } }
        public bool RenderPixelLights { get { return _renderPixelLights; } set { _renderPixelLights = value; } }
        public int SortingLayerID
        {
            get { return _sortingLayerID; }
            set
            {
                if (_meshModule != null && _meshModule.MeshRenderer.sortingLayerID != value)
                {
                    _sortingLayerID = value;
                    _meshModule.MeshRenderer.sortingLayerID = _sortingLayerID;
                    ResortRenderingModulesList();
                    _meshMask.UpdateSortingProperties();
                }
            }
        }
        public int SortingOrder
        {
            get { return _sortingOrder; }
            set
            {
                if (_meshModule != null && _meshModule.MeshRenderer.sortingOrder != value)
                {
                    _sortingOrder = value;
                    _meshModule.MeshRenderer.sortingOrder = _sortingOrder;
                    ResortRenderingModulesList();
                    _meshMask.UpdateSortingProperties();
                }
            }
        }
        public MeshMask MeshMask { get { return _meshMask; } }
        #endregion

        internal abstract bool IsVisibleToRenderingCamera(RenderingCameraInformation renderingCameraInformation);
#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
        internal abstract void Render(UnityEngine.Rendering.ScriptableRenderContext context, RenderingCameraInformation renderingCameraInformation);
#else
        internal abstract void Render(RenderingCameraInformation renderingCameraInformation);
#endif

        virtual internal void Initialize()
        {
            _meshModule.MeshRenderer.sortingOrder = _sortingOrder;
            _meshModule.MeshRenderer.sortingLayerID = _sortingLayerID;

            _meshMask.Initialize(_mainModule, this, _materialModule);
        }

        internal void Update()
        {
#if UNITY_EDITOR
            _meshMask.Update();
#endif

            _mainModule.IsVisible = false;
        }

        protected void SetupRefractionMask(Vector3 position)
        {
            _refractionMask.transform.SetPositionAndRotation(position, _mainModule.Rotation);
            _refractionMask.SetupRenderingProperties(_materialModule.RenderQueue, _sortingLayerID, _sortingOrder);
        }

        protected void SetRefractionMaskLayer(int refractionMaskLayer)
        {
            if (refractionMaskLayer != -1)
            {
                _refractionMask.gameObject.layer = refractionMaskLayer;
                _refractionMask.SetActive(true);
            }
            else _refractionMask.SetActive(false);
        }

        internal void SetActive(bool isActive)
        {
            if (isActive)
                AddToRenderingModulesList(this);
            else
                RemoveFromRenderingModulesList(this);
        }

#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
        private static System.Action<UnityEngine.Rendering.ScriptableRenderContext, Camera> _renderObjectsDelegate = new System.Action<UnityEngine.Rendering.ScriptableRenderContext, Camera>(RenderObjects);
        private static void OnBeginFrameRendering(UnityEngine.Rendering.ScriptableRenderContext context, Camera[] cameras)
        {
            UnityEngine.Rendering.RenderPipelineManager.beginCameraRendering += _renderObjectsDelegate;
        }
        private static void OnEndFrameRendering(UnityEngine.Rendering.ScriptableRenderContext context, Camera[] cameras)
        {
            UnityEngine.Rendering.RenderPipelineManager.beginCameraRendering -= _renderObjectsDelegate;
        }

        private static void RenderObjects(UnityEngine.Rendering.ScriptableRenderContext context, Camera currentCamera)
#else
        private static void RenderObjects(Camera currentCamera)
#endif
        {
            if (currentCamera == _g2dwCamera)
                return;

            if (_resortRenderingModulesList)
                DoRenderingModulesListResort();

            SetupRenderingCameraInformation(currentCamera);

            if (_g2dwCamera == null)
                CreateCamera();

            if (_refractionMask == null)
                CreateRefractionMask();

            for (int i = 0, imax = _renderingModulesList.Count; i < imax; i++)
            {
                var currentRenderingModule = _renderingModulesList[i];

                bool isVisible = currentRenderingModule.IsVisibleToRenderingCamera(_renderingCameraInformation);

                currentRenderingModule._mainModule.IsVisible |= isVisible;

                if (isVisible)
                {
#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
                    currentRenderingModule.Render(context, _renderingCameraInformation);
#else
                    currentRenderingModule.Render(_renderingCameraInformation);
#endif
                }
            }
        }

        private static void SetupRenderingCameraInformation(Camera currentRenderingCamera)
        {
            _renderingCameraInformation.CurrentCamera = currentRenderingCamera;
            _renderingCameraInformation.Position = currentRenderingCamera.transform.position;
            Vector3 rotationEulerAngles = currentRenderingCamera.transform.rotation.eulerAngles;
            _renderingCameraInformation.RotationEulerAngles = rotationEulerAngles;
            _renderingCameraInformation.ForwardDirection = currentRenderingCamera.transform.forward;
            _renderingCameraInformation.IsIsometric = currentRenderingCamera.orthographic && ((rotationEulerAngles.x > 0.5f && rotationEulerAngles.x < 359.5f) || (rotationEulerAngles.y > 0.5f && rotationEulerAngles.y < 359.5f));
            float nearClipPlane = currentRenderingCamera.nearClipPlane;
            _renderingCameraInformation.nearFrustumPlaneTopLeft = currentRenderingCamera.ViewportToWorldPoint(new Vector3(0f, 1f, nearClipPlane));
            _renderingCameraInformation.nearFrustumPlaneTopRight = currentRenderingCamera.ViewportToWorldPoint(new Vector3(1f, 1f, nearClipPlane));
            _renderingCameraInformation.nearFrustumPlaneBottomLeft = currentRenderingCamera.ViewportToWorldPoint(new Vector3(0f, 0f, nearClipPlane));
            _renderingCameraInformation.nearFrustumPlaneBottomRight = currentRenderingCamera.ViewportToWorldPoint(new Vector3(1f, 0f, nearClipPlane));
        }

        internal static void AddToRenderingModulesList(RenderingModule renderingModule)
        {
            _renderingModulesList.Add(renderingModule);
            _resortRenderingModulesList = true;

            renderingModule._mainModule.OnDepthChange += ResortRenderingModulesList;
#if UNITY_EDITOR
            renderingModule._materialModule.OnRenderQueueChange += ResortRenderingModulesList;
#endif

            if (_renderingModulesList.Count == 1)
            {
#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
            UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering += OnBeginFrameRendering;
            UnityEngine.Rendering.RenderPipelineManager.endFrameRendering += OnEndFrameRendering;
#else
                Camera.onPreCull += RenderObjects;
#endif

                Game2DWaterKitObject.FrameUpdate += OnFrameUpdate;

                Game2DWaterKitObject.AllWaterKitObjectsDestroyed -= OnAllWaterKitObjectsDestroyed;
                Game2DWaterKitObject.AllWaterKitObjectsDestroyed += OnAllWaterKitObjectsDestroyed;
            }
        }

        internal static void RemoveFromRenderingModulesList(RenderingModule renderingModule)
        {
            _renderingModulesList.Remove(renderingModule);
            _resortRenderingModulesList = true;

            renderingModule._mainModule.OnDepthChange -= ResortRenderingModulesList;
#if UNITY_EDITOR
            renderingModule._materialModule.OnRenderQueueChange -= ResortRenderingModulesList;
#endif

            if (_renderingModulesList.Count == 0)
            {
#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
                UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering -= OnBeginFrameRendering;
                UnityEngine.Rendering.RenderPipelineManager.endFrameRendering -= OnEndFrameRendering;
#else
                Camera.onPreCull -= RenderObjects;
#endif

                Game2DWaterKitObject.FrameUpdate -= OnFrameUpdate;
            }
        }

        private static void OnAllWaterKitObjectsDestroyed()
        {
            if (_g2dwCamera != null)
                WaterUtility.SafeDestroyObject(_g2dwCamera.gameObject);

            if (_refractionMask != null)
                WaterUtility.SafeDestroyObject(_refractionMask.gameObject);
        }

        // Called in LateUpdate (Just before starting to render the current frame)
        private static void OnFrameUpdate()
        {
            _elapsedTime += Time.deltaTime * Game2DWaterKitObject.TimeScale;

#if UNITY_EDITOR
            float currentTime = (Application.isPlaying ? _elapsedTime : Time.realtimeSinceStartup);
#else
            float currentTime = _elapsedTime;
#endif
            Shader.SetGlobalVector(_timeID, new Vector4(currentTime * 0.05f, currentTime, currentTime * 2f, currentTime * 3f));
        }

        private static void ResortRenderingModulesList()
        {
            _resortRenderingModulesList = true;
        }

        private static void DoRenderingModulesListResort()
        {
            _renderingModulesList.Sort(RenderingModulesSortComparer);
            _resortRenderingModulesList = false;
        }

        private static int RenderingModulesSortComparer(RenderingModule a, RenderingModule b)
        {
            bool aIsOpaque = a._materialModule.RenderQueue <= 2500;
            bool bIsOpaque = b._materialModule.RenderQueue <= 2500;

            if (aIsOpaque == bIsOpaque)
            {
                if (aIsOpaque) // Both renderers are opaque
                {
                    int dc = CompareDepth(a, b);
                    if (dc == 0)
                        return CompareSortingLayers(a, b);
                    else
                        return dc;
                }
                else // Both renderers are transparent
                {
                    int sc = CompareSortingLayers(a, b);
                    if (sc == 0)
                        return CompareDepth(a, b);
                    else
                        return sc;
                }
            }
            else // One renderer is opaque and the other is transparent
            {
                int dc = CompareDepth(a, b);
                if (dc == 0)
                    return aIsOpaque ? -1 : 1;
                else
                    return dc;
            }
        }

        private static int CompareDepth(RenderingModule a, RenderingModule b)
        {
            float aDepth = a._mainModule.Depth;
            float bDepth = b._mainModule.Depth;

            if (aDepth == bDepth)
                return 0;
            else
                return aDepth < bDepth ? 1 : -1;
        }

        private static int CompareSortingLayers(RenderingModule a, RenderingModule b)
        {
            int aLayerOrder = SortingLayer.GetLayerValueFromID(a._sortingLayerID);
            int bLayerOrder = SortingLayer.GetLayerValueFromID(b._sortingLayerID);

            if (aLayerOrder == bLayerOrder)
            {
                int aOrderInLayer = a._sortingOrder;
                int bOrderInLayer = b._sortingOrder;

                if (aOrderInLayer == bOrderInLayer)
                    return 0;
                else
                    return aOrderInLayer < bOrderInLayer ? -1 : 1;
            }
            else return aLayerOrder < bLayerOrder ? -1 : 1;
        }

        private static void CreateCamera()
        {
            var cameraGO = new GameObject("G2DWK Camera");

            cameraGO.hideFlags = HideFlags.HideAndDontSave;
            cameraGO.SetActive(false);

            _g2dwCamera = cameraGO.AddComponent<Camera>();
            _g2dwCamera.enabled = false;
            _g2dwCamera.clearFlags = CameraClearFlags.SolidColor;
        }

        private static void CreateRefractionMask()
        {
            var refractionMaskGo = new GameObject("G2DWK RefractionMask");
            refractionMaskGo.hideFlags = HideFlags.HideAndDontSave;
            refractionMaskGo.AddComponent<MeshRenderer>();
            refractionMaskGo.AddComponent<MeshFilter>();

            _refractionMask = refractionMaskGo.AddComponent<WaterRenderingRefractionMask>();
            _refractionMask.SetActive(false);
        }
    }

    internal class RenderingCameraInformation
    {
        internal Camera CurrentCamera { get; set; }
        internal Vector3 Position { get; set; }
        internal Vector3 RotationEulerAngles { get; set; }
        internal Vector3 nearFrustumPlaneTopLeft { get; set; }
        internal Vector3 nearFrustumPlaneTopRight { get; set; }
        internal Vector3 nearFrustumPlaneBottomLeft { get; set; }
        internal Vector3 nearFrustumPlaneBottomRight { get; set; }
        internal Vector3 ForwardDirection { get; set; }
        internal bool IsIsometric { get; set; }
    }

}