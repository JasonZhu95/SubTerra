namespace Game2DWaterKit.Animation
{
    using Game2DWaterKit.Simulation;
    using Game2DWaterKit.Mesh;
    using UnityEngine;

    public class WaterAnimationModule : AnimationModule
    {
        private Game2DWater _waterObject;

        private WaterSimulationModule _simulationModule;
        private WaterMeshModule _meshModule;

        private float _lastLeftCustomBoundary;
        private float _lastRightCustomBoundary;

        public WaterAnimationModule(Game2DWater waterObject)
        {
            _waterObject = waterObject;
        }

        #region Methods

        public void AnimateWaterSize(Vector2 targetSize, float duration, WaterAnimationConstraint constraint, WaterAnimationWrapMode wrapMode = WaterAnimationWrapMode.Once)
        {
            AnimateSize(targetSize, duration, constraint, wrapMode);
        }

        internal void Initialze()
        {
            _mainModule = _waterObject.MainModule;
            _meshModule = _waterObject.MeshModule;
            _simulationModule = _waterObject.SimulationModule;

            _meshModule.OnRecomputeMesh += ResetCachedVariables;
            ResetCachedVariables();
        }

        internal override void Update()
        {
            base.Update();

            if (_simulationModule.IsUsingCustomBoundaries && (_simulationModule.LeftCustomBoundary != _lastLeftCustomBoundary || _simulationModule.RightCustomBoundary != _lastRightCustomBoundary))
                UpdateMeshCustomBoundaries();
        }

        internal void SyncAnimatableVariables(Vector2 waterSize, float firstCustomBoundary, float secondCustomBoundary)
        {
            //These variables could be animated in Unity animation system,
            //so we make sure to reflect any changes to their values into their respective modules
            _mainModule.SetSize(waterSize);
            _simulationModule.FirstCustomBoundary = firstCustomBoundary;
            _simulationModule.SecondCustomBoundary = secondCustomBoundary;
        }

        protected override void UpdateSize(Vector2 newWaterSize)
        {
            int surfaceVerticesCount = _meshModule.SurfaceVerticesCount;
            var vertices = _meshModule.Vertices;

            if (_simulationModule.IsUsingCustomBoundaries)
            {
                if(newWaterSize.x != _lastSize.x)
                {
                    int firstSurfaceVertexIndex = 0; //topLeft
                    int lastSurfaceVertexIndex = surfaceVerticesCount - 1; //topRight

                    int firstBottomVertexIndex = surfaceVerticesCount; //bottomLeft
                    int lastBottomVertexIndex = surfaceVerticesCount * 2 - 1; //bottomRight

                    float halfWidth = newWaterSize.x * 0.5f;
                    vertices[firstSurfaceVertexIndex].x = vertices[firstBottomVertexIndex].x = -halfWidth;
                    vertices[lastSurfaceVertexIndex].x = vertices[lastBottomVertexIndex].x = halfWidth;
                }

                if(newWaterSize.y != _lastSize.y)
                {
                    float halfDeltaHeight = (newWaterSize.y - _lastSize.y) * 0.5f;
                    for (int surfaceVertexIndex = 0; surfaceVertexIndex < surfaceVerticesCount; surfaceVertexIndex++)
                    {
                        int bottomVertexIndex = surfaceVertexIndex + surfaceVerticesCount;

                        vertices[surfaceVertexIndex].y += halfDeltaHeight;
                        vertices[bottomVertexIndex].y -= halfDeltaHeight;
                    }
                }
            }
            else
            {
                float halfDeltaHeight = (newWaterSize.y - _lastSize.y) * 0.5f;
                float columnWidth = newWaterSize.x / (surfaceVerticesCount - 1);

                float xPos = -newWaterSize.x * 0.5f;
                for (int surfaceVertexIndex = 0; surfaceVertexIndex < surfaceVerticesCount; surfaceVertexIndex++)
                {
                    int bottomVertexIndex = surfaceVertexIndex + surfaceVerticesCount;

                    vertices[surfaceVertexIndex].x = vertices[bottomVertexIndex].x = xPos;
                    vertices[surfaceVertexIndex].y += halfDeltaHeight;
                    vertices[bottomVertexIndex].y -= halfDeltaHeight;

                    xPos += columnWidth;
                }
            }

            _mainModule.SetSize(newWaterSize);
            _meshModule.UpdateMeshData();

            _lastSize = newWaterSize;
        }

        private void UpdateMeshCustomBoundaries()
        {
            float newLeftCustomBoundary = _simulationModule.LeftCustomBoundary;
            float newRightCustomBoundary = _simulationModule.RightCustomBoundary;

            int surfaceVerticesCount = _meshModule.SurfaceVerticesCount;
            var vertices = _meshModule.Vertices;
            float columnWidth = (newRightCustomBoundary - newLeftCustomBoundary) / (surfaceVerticesCount - 3);

            float xPos = newLeftCustomBoundary;
            for (int surfaceVertexIndex = 1, max = surfaceVerticesCount - 1; surfaceVertexIndex < max; surfaceVertexIndex++)
            {
                int bottomVertexIndex = surfaceVertexIndex + surfaceVerticesCount;
                vertices[surfaceVertexIndex].x = vertices[bottomVertexIndex].x = xPos;

                xPos += columnWidth;
            }

            _lastLeftCustomBoundary = newLeftCustomBoundary;
            _lastRightCustomBoundary = newRightCustomBoundary;
            _meshModule.UpdateMeshData();
        }

        private void ResetCachedVariables()
        {
            _lastSize = new Vector2(_mainModule.Width, _mainModule.Height);
            _lastLeftCustomBoundary = _simulationModule.LeftCustomBoundary;
            _lastRightCustomBoundary = _simulationModule.RightCustomBoundary;
        }

        #endregion
    }
}