namespace Game2DWaterKit.Main
{
    using UnityEngine;

    public class WaterMainModule : MainModule
    {
        private Game2DWater _waterObject;

        public WaterMainModule(Game2DWater waterObject, Vector2 waterSize)
        {
            _waterObject = waterObject;
            _transform = waterObject.transform;
            _size = waterSize;
        }

        #region Properties

        public Vector2 WaterSize { get { return _size; } }
        internal Game2DWater WaterObject { get { return _waterObject; } }
        internal LargeWaterAreaManager LargeWaterAreaManager { get; set; }
        #endregion

        #region Methods

        internal void Initialize()
        {
            _materialModule = _waterObject.MaterialModule;
            _meshModule = _waterObject.MeshModule;
            _meshMask = _waterObject.RenderingModule.MeshMask;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                IsVisible = true;
            }
#endif
            UpdateCachedTransformInformation();

            _gameobjectLayer = _transform.gameObject.layer;
        }

        public float GetWaterHeightAtSpcificPoint(float xPosition)
        {
            var meshModule = _waterObject.MeshModule;
            var simulationModule = _waterObject.SimulationModule;
            var mainModule = _waterObject.MainModule;

            int surfaceVertexCount = meshModule.SurfaceVerticesCount;
            int leftMostSurfaceVertexIndex = simulationModule.IsUsingCustomBoundaries ? 1 : 0;
            int rightMostSurfaceVertexIndex = simulationModule.IsUsingCustomBoundaries ? surfaceVertexCount - 2 : surfaceVertexCount - 1;
            float subdivisionsPerUnit = meshModule.SurfaceVerticesCount / mainModule.Width;

            int nearestSurfaceVertexIndex = Mathf.Clamp(Mathf.RoundToInt((mainModule.TransformPointWorldToLocal(new Vector3(xPosition, 0f)).x - simulationModule.LeftBoundary) * subdivisionsPerUnit), leftMostSurfaceVertexIndex, rightMostSurfaceVertexIndex);

            return meshModule.Vertices[nearestSurfaceVertexIndex].y + mainModule.Height * 0.5f;
        }

        [System.Obsolete("Please use SetSize(Vector2, bool) instead.")]
        public void SetWaterSize(Vector2 newWaterSize, bool recomputeMesh = false)
        {
            SetSize(newWaterSize, recomputeMesh);
        }

        #endregion
    }
}
