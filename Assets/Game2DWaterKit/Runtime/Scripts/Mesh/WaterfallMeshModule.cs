namespace Game2DWaterKit.Mesh
{
    using System.Collections.Generic;
    using UnityEngine;

    public class WaterfallMeshModule : MeshModule
    {
        private Game2DWaterfall _waterfallObject;
        private Vector2 _topBottomEdgesRelativeLength;
        private List<Vector4> _uvs;

        public WaterfallMeshModule(Game2DWaterfall waterfallObject, Vector2 topBottomEdgesRelativeLength)
        {
            _waterfallObject = waterfallObject;
            _topBottomEdgesRelativeLength = topBottomEdgesRelativeLength;
        }

        public List<Vector4> Uvs { get { return _uvs; } }

        internal Vector2 TopBottomEdgesRelativeLength { get { return _topBottomEdgesRelativeLength; } }

        internal override void Initialize()
        {
            _mainModule = _waterfallObject.MainModule;

            base.Initialize();
        }

        protected override void RecomputeMesh()
        {
            Vector2 waterfallSize = new Vector2(_mainModule.Width, _mainModule.Height);

            var vertices = new Vector3[4];
            var triangles = new int[6];
            var uvs = new List<Vector4>(4);

            float topEdgeLength = waterfallSize.x * (_topBottomEdgesRelativeLength.y == 1f ? _topBottomEdgesRelativeLength.x : 1f);
            float bottomEdgeLength = waterfallSize.x * (_topBottomEdgesRelativeLength.y == 0f ? _topBottomEdgesRelativeLength.x : 1f);
            float d = (waterfallSize.x - topEdgeLength) * 0.5f;
            float sideEdgesLength = Mathf.Sqrt(waterfallSize.y * waterfallSize.y + d * d);

            vertices[0] = new Vector3(-topEdgeLength * 0.5f, waterfallSize.y * 0.5f);
            vertices[1] = new Vector3(topEdgeLength * 0.5f, waterfallSize.y * 0.5f);
            vertices[2] = new Vector3(bottomEdgeLength * 0.5f, -waterfallSize.y * 0.5f);
            vertices[3] = new Vector3(-bottomEdgeLength * 0.5f, -waterfallSize.y * 0.5f);

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;

            uvs.Add(new Vector4(0f, 1f, topEdgeLength, sideEdgesLength));
            uvs.Add(new Vector4(1f, 1f, topEdgeLength, sideEdgesLength));
            uvs.Add(new Vector4(1f, 0f, bottomEdgeLength, sideEdgesLength));
            uvs.Add(new Vector4(0f, 0f, bottomEdgeLength, sideEdgesLength));

            Mesh.Clear(false);
            Mesh.vertices = _vertices = vertices;
            _uvs = uvs;
            Mesh.SetUVs(0, uvs);
            Mesh.triangles = triangles;
            Mesh.RecalculateNormals();

            Mesh.bounds = _bounds = new Bounds(Vector3.zero, new Vector3(_mainModule.Width, _mainModule.Height));

            base.RecomputeMesh();
        }

        protected override void UpdateMesh()
        {
            Mesh.SetUVs(0, _uvs);

            //Updating mesh bounds
            Vector3 center = Vector3.zero;
            Vector3 size = new Vector3(_mainModule.Width, _mainModule.Height, 0f);
            _bounds = new Bounds(center, size);

            base.UpdateMesh();
        }

#if UNITY_EDITOR
        internal void Validate(Vector2 topBottomEdgesRelativeLength)
        {
            if (_topBottomEdgesRelativeLength != topBottomEdgesRelativeLength)
            {
                _topBottomEdgesRelativeLength = topBottomEdgesRelativeLength;
                _recomputeMeshData = true;
            }

            if (_recomputeMeshData)
                RecomputeMesh();
        }
#endif
    }
}
