namespace Game2DWaterKit.Ripples
{
    using Game2DWaterKit.Main;
    using Game2DWaterKit.Mesh;
    using Game2DWaterKit.Simulation;
    using UnityEngine;

    using System.Collections.Generic;

    public class WaterWaterfallRipplesModule
    {
        #region Variables
        private Game2DWater _waterObject;
        private WaterMainModule _mainModule;
        private WaterMeshModule _meshModule;
        private WaterSimulationModule _simulationModule;

        private List<int> _affectedVerticesIndices = new List<int>();
        #endregion

        public WaterWaterfallRipplesModule(Game2DWater waterObject)
        {
            _waterObject = waterObject;
        }

        #region Methods

        internal void Initialize()
        {
            _mainModule = _waterObject.MainModule;
            _meshModule = _waterObject.MeshModule;
            _simulationModule = _waterObject.SimulationModule;
        }

        internal void CreateRipples(Vector2 left, Vector2 right, float disturbance, float spread, bool smooth, float smoothingFactor = 0.5f)
        {
            Vector2 leftVertexPos = _mainModule.TransformPointWorldToLocal(left);
            Vector2 rightVertexPos = _mainModule.TransformPointWorldToLocal(right);

            float halfWaterHeight = _mainModule.Height * 0.5f;

            if (leftVertexPos.y > halfWaterHeight)
                return;

            int leftVertexIndex;
            int rightVertexIndex;

            float leftBoundary = _simulationModule.LeftBoundary;
            float rightBoundary = _simulationModule.RightBoundary;
            int surfaceVerticesCount = _meshModule.SurfaceVerticesCount;
            int startIndex = _simulationModule.IsUsingCustomBoundaries ? 1 : 0;
            int endIndex = _simulationModule.IsUsingCustomBoundaries ? surfaceVerticesCount - 2 : surfaceVerticesCount - 1;

            if (leftVertexPos.x > leftBoundary)
            {
                leftVertexIndex = startIndex + Mathf.RoundToInt((leftVertexPos.x - leftBoundary) * _meshModule.SubdivisionsPerUnit);
                if (leftVertexIndex > endIndex)
                    leftVertexIndex = endIndex;
            }
            else leftVertexIndex = startIndex;

            if (rightVertexPos.x < rightBoundary)
            {
                rightVertexIndex = startIndex + Mathf.RoundToInt((rightVertexPos.x - leftBoundary) * _meshModule.SubdivisionsPerUnit);
                if (rightVertexIndex < startIndex)
                    rightVertexIndex = startIndex;
            }
            else rightVertexIndex = endIndex;

            if (leftVertexIndex == rightVertexIndex)
                return;

            var vertices = _meshModule.Vertices;

            _affectedVerticesIndices.Clear();

            int affectedVerticesCount = Mathf.CeilToInt(Mathf.Abs(rightVertexIndex - leftVertexIndex) * spread);

            for (int i = 0; i < affectedVerticesCount + 1;)
            {
                int vertexIndex = Random.Range(leftVertexIndex, rightVertexIndex + 1);

                if (!_affectedVerticesIndices.Contains(vertexIndex))
                {
                    _affectedVerticesIndices.Add(vertexIndex);
                    i++;
                }
            }

            for (int i = 0; i < affectedVerticesCount; i++)
            {
                int vertexIndex = _affectedVerticesIndices[i];

                _simulationModule.DisturbSurfaceVertex(vertexIndex, -disturbance);
                if (smooth)
                {
                    smoothingFactor = Mathf.Clamp01(smoothingFactor);
                    float smoothedDisturbance = disturbance * smoothingFactor;

                    int previousNearestIndex = vertexIndex - 1;
                    if (previousNearestIndex >= startIndex)
                        _simulationModule.DisturbSurfaceVertex(previousNearestIndex, -smoothedDisturbance);

                    int nextNearestIndex = vertexIndex + 1;
                    if (nextNearestIndex <= endIndex)
                        _simulationModule.DisturbSurfaceVertex(nextNearestIndex, -smoothedDisturbance);
                }
            }
        }

        #endregion
    }
}
