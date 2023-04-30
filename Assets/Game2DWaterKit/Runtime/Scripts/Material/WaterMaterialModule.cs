namespace Game2DWaterKit.Material
{
    using Game2DWaterKit.Mesh;
    using UnityEngine;

    public class WaterMaterialModule : MaterialModule
    {
        #region Variables

        private static readonly string _refractionKeyword = "Water2D_Refraction";
        private static readonly string _reflectionKeyword = "Water2D_Reflection";
        private static readonly string _fakePerspectiveKeyword = "Water2D_FakePerspective";
        private static readonly string _gradientColorKeyword = "Water2D_ColorGradient";
        private static readonly string _hasAbsoluteSurfaceThicknessKeyword = "Water2D_SurfaceHasAbsoluteThickness";

        private readonly int _refractionRenderTextureID;
        private readonly int _reflectionRenderTextureID;
        private readonly int _refractionPartiallySubmergedObjectsRenderTextureID;
        private readonly int _reflectionPartiallySubmergedObjectsRenderTextureID;
        private readonly int _waterMatrixID;
        private readonly int _surfaceLevelID;
        private readonly int _surfaceSubmergeLevelID;
        private readonly int _waterReflectionLowerLimitID;
        private readonly int _waterReflectionFakePerspectiveLowerLimitID;
        private readonly int _waterReflectionFakePerspectiveUpperLimitID;
        private readonly int _waterReflectionFakePerspectivePartiallySubmergedObjectsUpperLimitID;
        private readonly int _waterSolidColorID;
        private readonly int _waterGradientStartColorID;
        private readonly int _waterGradientEndColorID;
        private readonly int _waterSurfaceColorID;

        private bool _isRefractionEnabled;
        private bool _isReflectionEnabled;
        private bool _isFakePerspectiveEnabled;
        private bool _isUsingGradientColor;
        private bool _hasAbsoluteSurfaceThickness;

        private Game2DWater _waterObject;

        internal System.Action OnSurfaceThicknessOrSubmergeLevelChanged;
        #endregion

        public WaterMaterialModule(Game2DWater waterObject)
        {
            _waterObject = waterObject;

            _refractionRenderTextureID = Shader.PropertyToID("_RefractionTexture");
            _refractionPartiallySubmergedObjectsRenderTextureID = Shader.PropertyToID("_RefractionTexturePartiallySubmergedObjects");
            _reflectionRenderTextureID = Shader.PropertyToID("_ReflectionTexture");
            _reflectionPartiallySubmergedObjectsRenderTextureID = Shader.PropertyToID("_ReflectionTexturePartiallySubmergedObjects");
            _waterMatrixID = Shader.PropertyToID("_WaterMVP");
            _surfaceLevelID = Shader.PropertyToID("_SurfaceLevel");
            _surfaceSubmergeLevelID = Shader.PropertyToID("_SubmergeLevel");
            _waterReflectionLowerLimitID = Shader.PropertyToID("_ReflectionLowerLimit");
            _waterReflectionFakePerspectiveLowerLimitID = Shader.PropertyToID("_ReflectionFakePerspectiveLowerLimit");
            _waterReflectionFakePerspectiveUpperLimitID = Shader.PropertyToID("_ReflectionFakePerspectiveUpperLimit");
            _waterReflectionFakePerspectivePartiallySubmergedObjectsUpperLimitID = Shader.PropertyToID("_ReflectionFakePerspectivePartiallySubmergedObjectsUpperLimit");
            _waterSolidColorID = Shader.PropertyToID("_WaterColor");
            _waterGradientStartColorID = Shader.PropertyToID("_WaterColorGradientStart");
            _waterGradientEndColorID = Shader.PropertyToID("_WaterColorGradientEnd");
            _waterSurfaceColorID = Shader.PropertyToID("_SurfaceColor");

#if GAME_2D_WATER_KIT_LWRP
            _defaultMaterialShaderName = "Game2DWaterKit/Lightweight Render Pipeline/Unlit/Water";
#elif GAME_2D_WATER_KIT_URP
            _defaultMaterialShaderName = "Game2DWaterKit/Universal Render Pipeline/Unlit/Water";
#else
            _defaultMaterialShaderName = "Game2DWaterKit/Built-in Render Pipeline/Unlit/Water";
#endif
        }

        #region Properties

        public bool IsUsingGradientColor
        {
            get
            {
                #if UNITY_EDITOR
                _isUsingGradientColor = Material.IsKeywordEnabled(_gradientColorKeyword);
                #endif
                return _isUsingGradientColor;
            }
        }

        public Color SolidColor { get { return Material.GetColor(_waterSolidColorID); } set { Material.SetColor(_waterSolidColorID, value); } }

        public Color GradientStartColor { get { return Material.GetColor(_waterGradientStartColorID); } set { Material.SetColor(_waterGradientStartColorID, value); } }

        public Color GradientEndColor { get { return Material.GetColor(_waterGradientEndColorID); } set { Material.SetColor(_waterGradientEndColorID, value); } }

        public Color SurfaceColor { get { return Material.GetColor(_waterSurfaceColorID); } set { Material.SetColor(_waterSurfaceColorID, value); } }

        public bool IsFakePerspectiveEnabled
        {
            get
            {
                #if UNITY_EDITOR
                _isFakePerspectiveEnabled = Material.IsKeywordEnabled(_fakePerspectiveKeyword);
                #endif
                return _isFakePerspectiveEnabled;
            }
            set
            {
                _isFakePerspectiveEnabled = value;

                if (value)
                    Material.EnableKeyword(_fakePerspectiveKeyword);
                else
                    Material.DisableKeyword(_fakePerspectiveKeyword);
            }
        }

        public bool IsReflectionEnabled
        {
            get
            {
                #if UNITY_EDITOR
                _isReflectionEnabled = Material.IsKeywordEnabled(_reflectionKeyword);
                _isRendererModuleResponsibleForUpdatingMaterialPropertyBlock = _isRefractionEnabled || _isReflectionEnabled;
                #endif
                return _isReflectionEnabled;
            }
            set
            {
                _isReflectionEnabled = value;

                if (value)
                    Material.EnableKeyword(_reflectionKeyword);
                else
                    Material.DisableKeyword(_reflectionKeyword);
            }
        }

        public bool IsRefractionEnabled
        {
            get
            {
                #if UNITY_EDITOR
                _isRefractionEnabled = Material.IsKeywordEnabled(_refractionKeyword);
                _isRendererModuleResponsibleForUpdatingMaterialPropertyBlock = _isRefractionEnabled || _isReflectionEnabled;
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

        public float SubmergeLevel
        {
            get
            {
                float submergeLevel;

#if UNITY_EDITOR
                submergeLevel = Material.HasProperty(_surfaceSubmergeLevelID) ? Material.GetFloat(_surfaceSubmergeLevelID) : 0f;
#else
                submergeLevel = Material.GetFloat(_surfaceSubmergeLevelID);
#endif

                if (!HasAbsoluteSurfaceThickness)
                {
                    float surfaceThickness = SurfaceThickness;
                    return (submergeLevel - 1f + surfaceThickness) / surfaceThickness;
                }
                else return submergeLevel;
            }

            set
            {
                if (!HasAbsoluteSurfaceThickness)
                {
                    float surfaceThickness = SurfaceThickness;
                    Material.SetFloat(_surfaceSubmergeLevelID, 1f - surfaceThickness + Mathf.Clamp01(value) * surfaceThickness);
                }
                else Material.SetFloat(_surfaceSubmergeLevelID, Mathf.Clamp01(value));

                if (OnSurfaceThicknessOrSubmergeLevelChanged != null)
                    OnSurfaceThicknessOrSubmergeLevelChanged.Invoke();
            }
        }

        public float SurfaceThickness
        {
            get
            {
#if UNITY_EDITOR
                if (HasAbsoluteSurfaceThickness)
                    return Material.HasProperty(_surfaceLevelID) ? Material.GetFloat(_surfaceLevelID) : 0f;
                else
                    return 1.0f - (Material.HasProperty(_surfaceLevelID) ? Material.GetFloat(_surfaceLevelID) : 0f);
#else
                if (HasAbsoluteSurfaceThickness)
                    return Material.GetFloat(_surfaceLevelID);
                else
                    return 1.0f - Material.GetFloat(_surfaceLevelID);
#endif
            }
            set
            {
                if(HasAbsoluteSurfaceThickness)
                    Material.SetFloat(_surfaceLevelID, value);
                else
                    Material.SetFloat(_surfaceLevelID, 1f - Mathf.Clamp01(value));

                if (OnSurfaceThicknessOrSubmergeLevelChanged != null)
                    OnSurfaceThicknessOrSubmergeLevelChanged.Invoke();
            }
        }

        public bool HasAbsoluteSurfaceThickness
        {
            get
            {
#if UNITY_EDITOR
                _hasAbsoluteSurfaceThickness = Material.IsKeywordEnabled(_hasAbsoluteSurfaceThicknessKeyword);
#endif
                return _hasAbsoluteSurfaceThickness;
            }
            set
            {
                _hasAbsoluteSurfaceThickness = value;

                if (value)
                    Material.EnableKeyword(_hasAbsoluteSurfaceThicknessKeyword);
                else
                    Material.DisableKeyword(_hasAbsoluteSurfaceThicknessKeyword);
            }
        }

#endregion

#region Methods
        override internal void Initialize()
        {
            _mainModule = _waterObject.MainModule;
            _meshModule = _waterObject.MeshModule;

            base.Initialize();

            _isRefractionEnabled = Material.IsKeywordEnabled(_refractionKeyword);
            _isReflectionEnabled = Material.IsKeywordEnabled(_reflectionKeyword);
            _isFakePerspectiveEnabled = Material.IsKeywordEnabled(_fakePerspectiveKeyword);
            _isUsingGradientColor = Material.IsKeywordEnabled(_gradientColorKeyword);
            _hasAbsoluteSurfaceThickness = Material.IsKeywordEnabled(_hasAbsoluteSurfaceThicknessKeyword);

            _isRendererModuleResponsibleForUpdatingMaterialPropertyBlock = _isRefractionEnabled || _isReflectionEnabled;

            UpdateAspectRatioAndSize();
        }

        public float TransformPointWorldSpaceToSurfaceThickness(Vector3 positionWorldSpace)
        {
            if (HasAbsoluteSurfaceThickness)
                return _mainModule.Height * 0.5f - _mainModule.TransformPointWorldToLocal(positionWorldSpace).y;
            else
                return 1f - Mathf.Clamp01(TransformPointWorldSpaceToWaterSpaceRelativeYAxis(positionWorldSpace));
        }

        public float TransformPointWorldSpaceToSubmergeLevel(Vector3 positionWorldSpace)
        {
            float surfaceSubmergeLevelThickness;

            if (HasAbsoluteSurfaceThickness)
                surfaceSubmergeLevelThickness = _mainModule.Height * 0.5f - _mainModule.TransformPointWorldToLocal(positionWorldSpace).y;
            else
                surfaceSubmergeLevelThickness = 1f - Mathf.Clamp01(TransformPointWorldSpaceToWaterSpaceRelativeYAxis(positionWorldSpace));

            return 1f - Mathf.Clamp01(surfaceSubmergeLevelThickness / SurfaceThickness);
        }

        public float GetSurfaceLevelNormalized()
        {
            float surfaceLevel;

            if (HasAbsoluteSurfaceThickness)
            {
                float absoluteSurfaceThickness = Material.HasProperty(_surfaceLevelID) ? Material.GetFloat(_surfaceLevelID) : 0f;
                float waterHeight = _mainModule.Height;

                surfaceLevel = (waterHeight - absoluteSurfaceThickness) / waterHeight;
            }
            else surfaceLevel = Material.HasProperty(_surfaceLevelID) ? Material.GetFloat(_surfaceLevelID) : 0f;

            return Mathf.Clamp01(surfaceLevel);
        }

        public float GetSubmergeLevelNormalized()
        {
#if UNITY_EDITOR
            float submergeLevel = Material.HasProperty(_surfaceSubmergeLevelID) ? Material.GetFloat(_surfaceSubmergeLevelID) : 0f;
#else
            float submergeLevel = Material.GetFloat(_surfaceSubmergeLevelID);
#endif

            if (HasAbsoluteSurfaceThickness)
            {
                float surfaceLevelRelative = GetSurfaceLevelNormalized();
                submergeLevel = surfaceLevelRelative + (1f - surfaceLevelRelative) * submergeLevel;
            }

            return Mathf.Clamp01(submergeLevel);
        }

        private float TransformPointWorldSpaceToWaterSpaceRelativeYAxis(Vector3 positionWorldSpace)
        {
            Vector2 positionWaterSpace = _mainModule.TransformPointWorldToLocal(positionWorldSpace);
            float waterHeight = _mainModule.Height;

            return (positionWaterSpace.y + waterHeight * 0.5f) / waterHeight;
        }

        internal void SetRefractionRenderTexture(RenderTexture renderTexture)
        {
            if (renderTexture != null)
                _materialPropertyBlock.SetTexture(_refractionRenderTextureID, renderTexture);
        }

        internal void SetRefractionPartiallySubmergedObjectsRenderTexture(RenderTexture renderTexture)
        {
            if (renderTexture != null)
                _materialPropertyBlock.SetTexture(_refractionPartiallySubmergedObjectsRenderTextureID, renderTexture);
        }

        internal void SetReflectionRenderTexture(RenderTexture renderTexture)
        {
            if (renderTexture != null)
                _materialPropertyBlock.SetTexture(_reflectionRenderTextureID, renderTexture);
        }

        internal void SetReflectionPartiallySubmergedObjectsRenderTexture(RenderTexture renderTexture)
        {
            if (renderTexture != null)
                _materialPropertyBlock.SetTexture(_reflectionPartiallySubmergedObjectsRenderTextureID, renderTexture);
        }

        internal void SetReflectionLowerLimit(float lowerLimit)
        {
            _materialPropertyBlock.SetFloat(_waterReflectionLowerLimitID, lowerLimit);
        }

        internal void SetReflectionFakePerspectiveUpperLimit(float upperLimit)
        {
            _materialPropertyBlock.SetFloat(_waterReflectionFakePerspectiveUpperLimitID, upperLimit);
        }

        internal void SetReflectionFakePerspectiveLowerLimit(float lowerLimit)
        {
            _materialPropertyBlock.SetFloat(_waterReflectionFakePerspectiveLowerLimitID, lowerLimit);
        }

        internal void SetReflectionFakePerspectivePartiallySubmergedObjectsUpperLimit(float upperLimit)
        {
            _materialPropertyBlock.SetFloat(_waterReflectionFakePerspectivePartiallySubmergedObjectsUpperLimitID, upperLimit);
        }

        internal void SetWaterMatrix(Matrix4x4 matrix)
        {
            _materialPropertyBlock.SetMatrix(_waterMatrixID, matrix);
        }

#endregion
    }
}
