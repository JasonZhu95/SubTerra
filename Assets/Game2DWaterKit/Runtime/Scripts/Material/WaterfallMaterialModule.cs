namespace Game2DWaterKit.Material
{
    using UnityEngine;

    public class WaterfallMaterialModule : MaterialModule
    {
        private static readonly string _refractionKeyword = "Waterfall2D_Refraction";

        private readonly int _refractionRenderTextureID;
        private readonly int _waterfallMatrixID;

        private bool _isRefractionEnabled;
        private Game2DWaterfall _waterfallObject;

        public WaterfallMaterialModule(Game2DWaterfall waterfallObject)
        {
            _waterfallObject = waterfallObject;

            _refractionRenderTextureID = Shader.PropertyToID("_RefractionTexture");
            _waterfallMatrixID = Shader.PropertyToID("_WaterfallMVP");

#if GAME_2D_WATER_KIT_LWRP
            _defaultMaterialShaderName = "Game2DWaterKit/Lightweight Render Pipeline/Unlit/Waterfall";
#elif GAME_2D_WATER_KIT_URP
            _defaultMaterialShaderName = "Game2DWaterKit/Universal Render Pipeline/Unlit/Waterfall";
#else
            _defaultMaterialShaderName = "Game2DWaterKit/Built-in Render Pipeline/Unlit/Waterfall";
#endif
        }

        #region Properties

        public bool IsRefractionEnabled
        {
            get
            {
                #if UNITY_EDITOR
                _isRefractionEnabled = Material.IsKeywordEnabled(_refractionKeyword);
                _isRendererModuleResponsibleForUpdatingMaterialPropertyBlock = _isRefractionEnabled;
                #endif
                return _isRefractionEnabled;
            }
            set
            {
                _isRefractionEnabled = value;

                if (value)
                    Material.EnableKeyword(_refractionKeyword);
                else
                    Material.DisableKeyword(_refractionKeyword);
            }
        }

        #endregion

        override internal void Initialize()
        {
            _mainModule = _waterfallObject.MainModule;
            _meshModule = _waterfallObject.MeshModule;

            base.Initialize();

            _isRefractionEnabled = Material.IsKeywordEnabled(_refractionKeyword);

            _isRendererModuleResponsibleForUpdatingMaterialPropertyBlock = _isRefractionEnabled;

            UpdateAspectRatioAndSize();
        }

        internal void SetRefractionRenderTexture(RenderTexture renderTexture)
        {
            if (renderTexture != null)
                _materialPropertyBlock.SetTexture(_refractionRenderTextureID, renderTexture);
        }

        internal void SetWaterfallMatrix(Matrix4x4 matrix)
        {
            _materialPropertyBlock.SetMatrix(_waterfallMatrixID, matrix);
        }
    }
}
