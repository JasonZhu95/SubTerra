namespace Game2DWaterKit.Rendering.Mask
{
    using Game2DWaterKit.Main;
    using Game2DWaterKit.Rendering;
    using Game2DWaterKit.Material;
    using Game2DWaterKit.Utils;
    using UnityEngine;
    using UnityEngine.Rendering;
    using System.Collections.Generic;

    public class MeshMask
    {
        #region Variables
        private MainModule _mainModule;
        private RenderingModule _renderingModule;
        private MaterialModule _materialModule;

        private bool _isActive;
        private bool _arePositionAndSizeLocked;
        private int _subdivisions;
        private Vector3 _position;
        private Vector3 _size;
        private readonly List<ControlPoint> _controlPoints;
        private Vector3[] _vertices;

        private Mesh _mesh;
        private MeshRenderer _maskBefore;
        private MeshRenderer _maskAfter;
        #endregion

        #region Properties
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_maskBefore != null)
                {
                    _maskBefore.gameObject.SetActive(value);
                    _maskAfter.gameObject.SetActive(value);
                }
                _isActive = value;
            }
        }
        public bool ArePositionAndSizeLocked { get { return _arePositionAndSizeLocked; } }
        public Vector3 Position { get { return _position; } }
        public Vector3 Size { get { return _size; } }
        public Vector3[] Vertices { get { return _vertices; } }
        public List<ControlPoint> ControlPoints { get { return _controlPoints; } }
        public int Subdivisions { get { return _subdivisions; } }
        #endregion

        public MeshMask(MeshMaskParameters parameters)
        {
            _isActive = parameters.IsActive;
            _position = parameters.Position;
            _size = parameters.Size;
            _arePositionAndSizeLocked = parameters.ArePositionAndSizeLocked;
            _controlPoints = parameters.ControlPoints;
            _subdivisions = parameters.Subdivisions;
        }

        #region Methods
        internal void Initialize(MainModule mainModule, RenderingModule renderingModule, MaterialModule materialModule)
        {
            _mainModule = mainModule;
            _renderingModule = renderingModule;
            _materialModule = materialModule;

            _mesh = new Mesh();
            _mesh.hideFlags = HideFlags.HideAndDontSave;

            _maskBefore = CreateMaskObject(isBeforeMask: true);
            _maskAfter = CreateMaskObject(isBeforeMask: false);

            SetupMesh();

#if !UNITY_EDITOR
            SetupMaterials();

            _maskBefore.gameObject.SetActive(_isActive);
            _maskAfter.gameObject.SetActive(_isActive);
#endif

#if UNITY_EDITOR
            _materialModule.OnRenderQueueChange += UpdateSortingProperties;
#endif

            _mainModule.OnGameobjectLayerChange += UpdateMaskObjectsLayers;
        }

#if UNITY_EDITOR
        internal void Update()
        {
            if (_maskBefore.sharedMaterial == null)
                SetupMaterials();

            if (_maskBefore.gameObject.activeSelf != _isActive)
            {
                _maskBefore.gameObject.SetActive(_isActive);
                _maskAfter.gameObject.SetActive(_isActive);
            }
        }
#endif

        internal void UpdatePositionAndScale()
        {
            if (!_arePositionAndSizeLocked)
            {
                float width = _mainModule.Width;
                float height = _mainModule.Height;
                Vector3 scale = _mainModule.Scale;

                width *= scale.x;
                height *= scale.y;

                Vector3 position = _mainModule.Position;
                Vector3 size = new Vector3(width, height, 1f);

                _maskBefore.transform.localScale = size;
                _maskAfter.transform.localScale = size;
                _maskBefore.transform.position = position;
                _maskAfter.transform.position = position;

                _position = position;
                _size = size;
            }
        }

        internal void UpdateSortingProperties()
        {
            if (_maskBefore.sharedMaterial == null)
                return;

            var sortingLayer = _renderingModule.SortingLayerID;
            var orderInLayer = _renderingModule.SortingOrder;
            var renderQueue = _materialModule.RenderQueue;

            _maskBefore.sortingLayerID = _maskAfter.sortingLayerID = sortingLayer;
            _maskBefore.sortingOrder = orderInLayer - 1;
            _maskAfter.sortingOrder = orderInLayer + 1;
            _maskBefore.sharedMaterial.renderQueue = _maskAfter.sharedMaterial.renderQueue = renderQueue;
        }

        internal void Cleanup()
        {
            DestroyMaskObject(_maskBefore);
            DestroyMaskObject(_maskAfter);
        }

        private MeshRenderer CreateMaskObject(bool isBeforeMask)
        {
            var maskGameObject = new GameObject(_mainModule.Transform.name + (isBeforeMask ? " Before-Mask" : " After-Mask"));
            maskGameObject.hideFlags = HideFlags.HideAndDontSave;
            maskGameObject.layer = _mainModule.GameobjectLayer;

            maskGameObject.SetActive(false);

            maskGameObject.transform.position = _arePositionAndSizeLocked ? _position : _mainModule.Position;
            maskGameObject.transform.localScale = _arePositionAndSizeLocked ? _size : new Vector3(_mainModule.Width, _mainModule.Height, 1f);
            maskGameObject.transform.rotation = _mainModule.Rotation;

            maskGameObject.AddComponent<MeshFilter>().sharedMesh = _mesh;

            return maskGameObject.AddComponent<MeshRenderer>();
        }

        private void SetupMaterials()
        {
            _maskBefore.sharedMaterial = CreateMaskMaterial(isBeforeMask: true);
            _maskAfter.sharedMaterial = CreateMaskMaterial(isBeforeMask: false);

            UpdateSortingProperties();
        }

        private Material CreateMaskMaterial(bool isBeforeMask)
        {
            var material = new Material(Shader.Find("Hidden/Game2DWaterKit-MeshMask"));
            material.hideFlags = HideFlags.HideAndDontSave;
            material.SetInt("_StencilOperation", (int)(isBeforeMask ? StencilOp.IncrementSaturate : StencilOp.DecrementSaturate));

            return material;
        }

        private void UpdateMaskObjectsLayers()
        {
            _maskBefore.gameObject.layer = _mainModule.GameobjectLayer;
            _maskAfter.gameObject.layer = _mainModule.GameobjectLayer;
        }

        private void DestroyMaskObject(MeshRenderer mask)
        {
            if (mask != null)
            {
                WaterUtility.SafeDestroyObject(mask.GetComponent<MeshFilter>().sharedMesh);
                WaterUtility.SafeDestroyObject(mask.sharedMaterial);
                WaterUtility.SafeDestroyObject(mask.gameObject);
            }
        }

        private void SetupMesh()
        {
            if (_controlPoints.Count < 2)
                return;

            var vertices = ComputeVertices();
            var triangles = WaterTriangulator.Triangulate(vertices);

            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;

#if UNITY_EDITOR
            bool markNoLongerReadable = false;
#else
            bool markNoLongerReadable = true;
#endif

            _mesh.RecalculateBounds();
            _mesh.UploadMeshData(markNoLongerReadable);

            _vertices = vertices;
        }

        private Vector3[] ComputeVertices()
        {
            var vertices = new List<Vector3>();

            int vertexCount = 0;

            for (int i = 0, imax = _controlPoints.Count; i < imax; i++)
            {
                var currentSegment = _controlPoints[i];
                var nextSegment = _controlPoints[i + 1 < imax ? i + 1 : 0];

                for (int j = 0, jmax = _subdivisions + 1; j < jmax; j++)
                {
                    float t = j / (float)jmax;

                    var vertexPosition = (1f - t) * (1f - t) * (1f - t) * currentSegment.anchorPointPosition + t * t * t * nextSegment.anchorPointPosition + 3f * t * (1f - t) * (1f - t) * currentSegment.secondHandlePosition + 3f * (1f - t) * t * t * nextSegment.firstHandlePosition;

                    if(vertexCount > 1 && WaterUtility.AreColinear(vertices[vertexCount - 2], vertices[vertexCount - 1], vertexPosition))
                    {
                        vertices[vertexCount - 1] = vertexPosition;
                    }
                    else
                    {
                        vertices.Add(vertexPosition);
                        vertexCount++;
                    }
                }
            }

            if (WaterUtility.AreColinear(vertices[vertexCount - 2], vertices[vertexCount - 1], vertices[0]))
                vertices.RemoveAt(vertexCount - 1);

            return vertices.ToArray();
        }

#if UNITY_EDITOR
        internal void Validate(MeshMaskParameters parameters)
        {
            _isActive = parameters.IsActive;
            _arePositionAndSizeLocked = parameters.ArePositionAndSizeLocked;
            _subdivisions = Mathf.Clamp(parameters.Subdivisions, 0, int.MaxValue);
            _position = parameters.Position;
            _size = parameters.Size;

            UpdatePositionAndScale();

            if (!Application.isPlaying)
            {
                SetupMesh();

                var position = _arePositionAndSizeLocked ? _position : _mainModule.Position;

                _maskBefore.transform.position = position;
                _maskAfter.transform.position = position;

                _maskBefore.transform.localScale = _maskAfter.transform.localScale = _arePositionAndSizeLocked ? _size : new Vector3(_mainModule.Width, _mainModule.Height, 1f);
            }
        }
#endif
        #endregion

        [System.Serializable]
        public struct ControlPoint
        {
            public Vector2 anchorPointPosition;
            public Vector2 firstHandlePosition;
            public Vector2 secondHandlePosition;
            public ControlPointType type;

            public ControlPoint(Vector2 anchorPointPosition, Vector2 firstHandlePosition, Vector2 secondHandlePosition, ControlPointType type = ControlPointType.Cusp)
            {
                this.anchorPointPosition = anchorPointPosition;
                this.firstHandlePosition = firstHandlePosition;
                this.secondHandlePosition = secondHandlePosition;
                this.type = type;
            }

            public enum ControlPointType
            {
                Cusp,
                Smooth,
                Symetric
            }
        }
    }

    public struct MeshMaskParameters
    {
        public bool IsActive;
        public Vector3 Position;
        public Vector3 Size;
        public bool ArePositionAndSizeLocked;
        public int Subdivisions;
        public List<MeshMask.ControlPoint> ControlPoints;
    }
}
