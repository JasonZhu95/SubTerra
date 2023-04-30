namespace Game2DWaterKit.Main
{
    using Game2DWaterKit.Mesh;
    using Game2DWaterKit.Material;
    using Game2DWaterKit.Rendering.Mask;
    using UnityEngine;

    public abstract class MainModule
    {
        #region Variables
        internal event System.Action OnDepthChange;

        protected Transform _transform;
        protected MeshModule _meshModule;
        protected MaterialModule _materialModule;
        protected MeshMask _meshMask;
        protected Vector2 _size;

        private float _zRotation;
        private float _depth;
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _lossyScale;
        private Vector3 _upDirection;
        private Vector3 _forwardDirection;
        private Matrix4x4 _worldToLocalMatrix;
        private Matrix4x4 _localToWorldMatrix;
        protected int _gameobjectLayer;
        internal event System.Action OnGameobjectLayerChange;
        #endregion


        #region Properties
        public float Width { get { return _size.x; } }
        public float Height { get { return _size.y; } }
        public Vector3 Position { get { return _position; } set { _transform.position = _position = value; } }
        public Quaternion Rotation { get { return _rotation; } set { _transform.rotation = _rotation = value; } }
        public Vector3 Scale { get { return _lossyScale; } }
        public Matrix4x4 LocalToWorldMatrix { get { return _localToWorldMatrix; } }
        public Matrix4x4 WorldToLocalMatrix { get { return _worldToLocalMatrix; } }
        internal Transform Transform { get { return _transform; } }
        internal float ZRotation { get { return _zRotation; } }
        internal float Depth { get { return _depth; } }
        internal Vector3 UpDirection { get { return _upDirection; } }
        internal Vector3 ForwardDirection { get { return _forwardDirection; } }
        internal bool IsVisible { get; set; }
        internal int GameobjectLayer { get { return _gameobjectLayer; } }
        #endregion

        #region Methods

        internal Vector3 TransformPointLocalToWorldNoRotation(Vector3 point)
        {
            Vector3 result;
            result.x = point.x * _lossyScale.x + _position.x;
            result.y = point.y * _lossyScale.y + _position.y;
            result.z = point.z * _lossyScale.z + _position.z;
            return result;
        }

        public Vector3 TransformPointLocalToWorld(Vector3 point)
        {
            return _localToWorldMatrix.MultiplyPoint3x4(point);
        }

        public Vector3 TransformPointWorldToLocal(Vector3 point)
        {
            return _worldToLocalMatrix.MultiplyPoint3x4(point);
        }

        public Vector3 TransformVectorLocalToWorld(Vector3 vector)
        {
            return _localToWorldMatrix.MultiplyVector(vector);
        }

        public Vector3 TransformVectorWorldToLocal(Vector3 vector)
        {
            return _worldToLocalMatrix.MultiplyVector(vector);
        }

        public void SetSize(Vector2 newSize, bool recomputeMesh = false)
        {
            if (newSize.x > 0f && newSize.y > 0f)
            {
                _size = newSize;
                if (recomputeMesh)
                    _meshModule.RecomputeMeshData();
                _materialModule.UpdateAspectRatioAndSize();
            }
        }

        protected void UpdateCachedTransformInformation()
        {
            _localToWorldMatrix = _transform.localToWorldMatrix;
            _worldToLocalMatrix = _transform.worldToLocalMatrix;
            _upDirection = _transform.up;
            _forwardDirection = _transform.forward;
            _position = _transform.position;
            _rotation = _transform.rotation;
            _zRotation = _transform.rotation.eulerAngles.z;
            _depth = _position.z;
            if(_lossyScale != _transform.lossyScale)
            {
                _lossyScale = _transform.lossyScale;
                _materialModule.UpdateAspectRatioAndSize();
            }
        }

        internal void Update()
        {
            if (_transform.hasChanged)
            {
                _transform.hasChanged = false;

                float oldDepth = _position.z;

                UpdateCachedTransformInformation();

                _meshMask.UpdatePositionAndScale();

                if (oldDepth != _position.z && OnDepthChange != null)
                    OnDepthChange.Invoke();
            }

            if (_transform.gameObject.layer != _gameobjectLayer)
            {
                _gameobjectLayer = _transform.gameObject.layer;
                if (OnGameobjectLayerChange != null)
                    OnGameobjectLayerChange.Invoke();
            }
        }

#if UNITY_EDITOR
        internal void Validate(Vector2 newSize)
        {
            if (newSize != _size)
                SetSize(newSize, true);

            UpdateCachedTransformInformation();
        }
#endif

        #endregion
    }
}
