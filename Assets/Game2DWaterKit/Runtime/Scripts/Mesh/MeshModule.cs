namespace Game2DWaterKit.Mesh
{
    using Game2DWaterKit.Main;
    using UnityEngine;
    using System;

    public abstract class MeshModule
    {
        protected MainModule _mainModule;

        protected MeshRenderer _meshRenderer;
        protected MeshFilter _meshFilter;
        protected Mesh _mesh;
        protected Bounds _bounds;

        protected bool _updateMeshData;
        protected bool _recomputeMeshData;
        protected Vector3[] _vertices;

        internal event Action OnRecomputeMesh;

        public Vector3[] Vertices { get { return _vertices; } }
        internal Mesh Mesh
        {
            get
            {
#if UNITY_EDITOR
                _mesh = _meshFilter.sharedMesh;
#endif
                if (_mesh == null)
                {
                    _mesh = new Mesh();
                    _mesh.MarkDynamic();
                    _meshFilter.sharedMesh = _mesh;
                }
                return _mesh;
            }
        }
        internal MeshRenderer MeshRenderer { get { return _meshRenderer; } }
        internal MeshFilter MeshFilter { get { return _meshFilter; } }
        internal Bounds BoundsLocalSpace { get { return _bounds; } }

        internal void SetRendererActive(bool active)
        {
            _meshRenderer.enabled = active;
        }

        protected virtual void RecomputeMesh()
        {
            if (OnRecomputeMesh != null)
                OnRecomputeMesh.Invoke();

            _updateMeshData = false;
            _recomputeMeshData = false;
        }

        protected virtual void UpdateMesh()
        {
            var mesh = Mesh;

            mesh.vertices = _vertices;
            mesh.bounds = _bounds;

            _updateMeshData = false;
        }

        internal void UpdateMeshData()
        {
            _updateMeshData = true;
        }

        internal void RecomputeMeshData()
        {
            _recomputeMeshData = true;
        }

        internal void Update()
        {
            if (_recomputeMeshData)
            {
                RecomputeMesh();
                return;
            }

            if (!_mainModule.IsVisible)
                return;

            if (_updateMeshData)
                UpdateMesh();
        }

        virtual internal void Initialize()
        {
            _meshRenderer = _mainModule.Transform.GetComponent<MeshRenderer>();
            _meshFilter = _mainModule.Transform.GetComponent<MeshFilter>();
            //We set the meshFilter sharedMesh to null to make sure that this water/waterfall object 
            //will get its own unique mesh in the next call to Mesh property, as it's undesirable that two water objects
            //refer to and operate on the same mesh (as this might happen when cloning water/waterfall objects)
            _meshFilter.sharedMesh = null;

            RecomputeMesh();
        }
    }
}