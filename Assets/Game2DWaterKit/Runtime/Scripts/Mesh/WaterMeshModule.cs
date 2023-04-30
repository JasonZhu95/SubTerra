namespace Game2DWaterKit.Mesh
{
    using Game2DWaterKit.Simulation;
    using UnityEngine;

    public class WaterMeshModule : MeshModule
    {
        private Game2DWater _waterObject;

        private WaterSimulationModule _simulationModule;
        private int _subdivisionsPerUnit;
        private int _surfaceVerticesCount;

        public WaterMeshModule(Game2DWater waterObject, int subdivisionsPerUnit)
        {
            _waterObject = waterObject;
            _subdivisionsPerUnit = subdivisionsPerUnit;
        }

        #region Properties
        public int SubdivisionsPerUnit
        {
            get { return _subdivisionsPerUnit; }
            set
            {
                int newValue = Mathf.Clamp(value, 0, int.MaxValue);
                if (_subdivisionsPerUnit != newValue)
                {
                    _subdivisionsPerUnit = newValue;
                    RecomputeMeshData();
                }
            }
        }
        public int SurfaceVerticesCount { get { return _surfaceVerticesCount; } }
        #endregion

        #region Methods

        internal override void Initialize()
        {
            _mainModule = _waterObject.MainModule;
            _simulationModule = _waterObject.SimulationModule;

            base.Initialize();
        }

        protected override void RecomputeMesh()
        {
            if (_simulationModule.IsUsingCustomBoundaries)
                _surfaceVerticesCount = 4 + Mathf.RoundToInt(_subdivisionsPerUnit * (_simulationModule.RightCustomBoundary - _simulationModule.LeftCustomBoundary));
            else
                _surfaceVerticesCount = 2 + Mathf.RoundToInt(_subdivisionsPerUnit * _mainModule.Width);

            Mesh.Clear(keepVertexLayout: false);
            Mesh.vertices = _vertices = ComputeVertices();
            Mesh.uv = ComputeUVs();
            Mesh.triangles = ComputeTriangles();
            Mesh.RecalculateNormals();
            Mesh.bounds = _bounds = new Bounds(Vector3.zero, new Vector3(_mainModule.Width, _mainModule.Height));

            base.RecomputeMesh();
        }

        protected override void UpdateMesh()
        {
            // updating mesh bounds

            float waterSurfaceHeighestPoint = _simulationModule.SurfaceHeighestPoint;
            float waterBottom = _mainModule.Height * -0.5f;
            Vector3 center = new Vector3(0f, (waterSurfaceHeighestPoint + waterBottom) * 0.5f, 0f);
            Vector3 size = new Vector3(_mainModule.Width, waterSurfaceHeighestPoint - waterBottom, 0f);
            _bounds = new Bounds(center, size);

            base.UpdateMesh();
        }

        private Vector3[] ComputeVertices()
        {
            float halfWidth = _mainModule.Width * 0.5f;
            float halfHeight = _mainModule.Height * 0.5f;

            Vector3[] vertices = new Vector3[_surfaceVerticesCount * 2];

            if (_simulationModule.IsUsingCustomBoundaries)
            {
                float columnWidth = (_simulationModule.RightCustomBoundary - _simulationModule.LeftCustomBoundary) / (_surfaceVerticesCount - 3);

                float leftCustomBoundary = _simulationModule.LeftCustomBoundary;
                vertices[0] = new Vector3(-halfWidth, halfHeight); //topLeft
                vertices[_surfaceVerticesCount] = new Vector3(-halfWidth, -halfHeight); //bottomLeft

                //vertices between the left and the right custom boundaries
                for (int i = 0, imax = _surfaceVerticesCount - 2; i < imax; i++)
                {
                    float x = leftCustomBoundary + i * columnWidth;
                    vertices[i + 1] = new Vector3(x, halfHeight);
                    vertices[i + 1 + _surfaceVerticesCount] = new Vector3(x, -halfHeight);
                }

                vertices[_surfaceVerticesCount - 1] = new Vector3(halfWidth, halfHeight); //topRight
                vertices[_surfaceVerticesCount * 2 - 1] = new Vector3(halfWidth, -halfHeight); //bottomRight
            }
            else
            {
                float columnWidth = _mainModule.Width / (_surfaceVerticesCount - 1);

                for (int i = 0, imax = _surfaceVerticesCount; i < imax; i++)
                {
                    float x = -halfWidth + i * columnWidth;
                    vertices[i] = new Vector3(x, halfHeight); //top (surface)
                    vertices[i + _surfaceVerticesCount] = new Vector3(x, -halfHeight); //bottom
                }
            }

            return vertices;
        }

        private Vector2[] ComputeUVs()
        {
            Vector2[] uvs = new Vector2[_surfaceVerticesCount * 2];

            float width = _mainModule.Width;
            for (int i = 0; i < _surfaceVerticesCount; i++)
            {
                float u = (_vertices[i].x / width) + 0.5f;
                uvs[i] = new Vector3(u, 1f); //top (surface)
                uvs[i + _surfaceVerticesCount] = new Vector3(u, 0f); //bottom
            }

            return uvs;
        }

        private int[] ComputeTriangles()
        {
            int[] triangles = new int[6 * (_surfaceVerticesCount - 1)];

            //the water mesh is composed of (surface vertices count - 1) Quads
            //and each quad is represented as two triangles

            for (int i = 0, imax = _surfaceVerticesCount - 1; i < imax; i++)
            {
                //First Triangle: topLeft -> topRight -> bottomLeft
                triangles[i * 6] = i; //currentQuad topLeft vertex
                triangles[i * 6 + 1] = i + 1; //currentQuad topRight vertex
                triangles[i * 6 + 2] = i + _surfaceVerticesCount; //currentQuad bottomLeft vertex

                //Second Triangle: bottomLeft -> topRight -> bottomRight
                triangles[i * 6 + 3] = i + _surfaceVerticesCount; //currentQuad bottomLeft vertex
                triangles[i * 6 + 4] = i + 1; //currentQuad topRight vertex
                triangles[i * 6 + 5] = i + _surfaceVerticesCount + 1; //currentQuad bottomRight vertex
            }

            return triangles;
        }

#if UNITY_EDITOR
        internal void Validate(int subdivisionsPerUnit)
        {
            SubdivisionsPerUnit = subdivisionsPerUnit;

            if (_recomputeMeshData)
                RecomputeMesh();
        }
#endif
        #endregion
    }
}
