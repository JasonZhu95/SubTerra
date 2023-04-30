namespace Game2DWaterKit.Material
{
    using Game2DWaterKit.Main;
    using Game2DWaterKit.Mesh;
    using UnityEngine;

    public abstract class MaterialModule
    {
        private bool _isInitialized;

        private readonly int _aspectRatioID;
        private readonly int _sizeID;

        protected MainModule _mainModule;
        protected MeshModule _meshModule;
        protected Material _material;
        protected MaterialPropertyBlock _materialPropertyBlock;
        protected string _defaultMaterialShaderName;
        protected bool _isRendererModuleResponsibleForUpdatingMaterialPropertyBlock;

#if UNITY_EDITOR
        internal event System.Action OnRenderQueueChange;
        private int _previousRenderQueue;
#endif

        protected MaterialModule()
        {
            _aspectRatioID = Shader.PropertyToID("_AspectRatio");
            _sizeID = Shader.PropertyToID("_Size");
        }

        internal int RenderQueue
        {
            get
            {
                int renderQueue = _material.renderQueue;

#if UNITY_EDITOR
                if(renderQueue != _previousRenderQueue)
                {
                    _previousRenderQueue = renderQueue;
                    if (OnRenderQueueChange != null)
                        OnRenderQueueChange.Invoke();
                }
#endif

                return renderQueue;
            }
        }

        public Material Material
        {
            get
            {
#if UNITY_EDITOR
                CheckMaterial();
#endif
                return _material;
            }
        }

        virtual internal void Initialize()
        {
            CheckMaterial();

            _materialPropertyBlock = new MaterialPropertyBlock();
            _meshModule.MeshRenderer.GetPropertyBlock(_materialPropertyBlock);

            _isInitialized = true;
        }

        internal void UpdateAspectRatioAndSize()
        {
            if (!_isInitialized)
                return;

            float halfWidth = _mainModule.Width * 0.5f;
            float halfHeight = _mainModule.Height * 0.5f;

            Vector2 min = _mainModule.TransformPointLocalToWorldNoRotation(new Vector3(-halfWidth, -halfHeight));
            Vector2 max = _mainModule.TransformPointLocalToWorldNoRotation(new Vector3(halfWidth, halfHeight));

            float aspect = (max.x - min.x) / (max.y - min.y);
            float width = max.x - min.x;
            float height = max.y - min.y;

            _materialPropertyBlock.SetVector(_aspectRatioID, new Vector4(aspect, 1f, 1f, 1f / aspect));
            _materialPropertyBlock.SetVector(_sizeID, new Vector4(width, height, 1f / width, 1f / height));

            if (!_isRendererModuleResponsibleForUpdatingMaterialPropertyBlock)
                ValidateMaterialPropertyBlock();
        }

        internal void ValidateMaterialPropertyBlock()
        {
            _meshModule.MeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }

        private void CheckMaterial()
        {
            _material = _meshModule.MeshRenderer.sharedMaterial;
            if (_material == null)
            {
                _material = new Material(Shader.Find(_defaultMaterialShaderName));
                _material.name = _defaultMaterialShaderName.Contains("Waterfall") ? "Waterfall Material" : "Water Material";
                _meshModule.MeshRenderer.sharedMaterial = _material;
            }
        }

    }
}