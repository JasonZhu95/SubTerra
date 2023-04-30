namespace Game2DWaterKit.AttachedComponents
{
    using Game2DWaterKit.Main;
    using Game2DWaterKit.Material;
    using Game2DWaterKit.Rendering.Mask;
    using System.Collections.Generic;
    using UnityEngine;

    public class WaterAttachedComponentsModule
    {
        private Game2DWater _waterObject;

        private WaterMainModule _mainModule;
        private WaterMaterialModule _materialModule;
        private MeshMask _meshMask;

        private BuoyancyEffector2D _buoyancyEffector;
        private BoxCollider2D _boxCollider;
        private PolygonCollider2D _polygonCollider;
        private EdgeCollider2D _edgeCollider;
        private bool _hasAnimatorAttached;
        private Vector2 _cachedWaterSize;
        private Vector3 _cachedWaterPosition;
        private Vector3 _cachedWaterScale;
        private float _buoyancyEffectorSurfaceLevel;
        private BuoyancyEffector2DSurfaceLevelLocation _buoyancyEffectorSurfaceLevelLocation;
        private BoxCollider2DTopEdgeLocation _boxColliderTopEdgeLocation;
        private Vector2[] _edgeColliderPoints;
        private List<Vector2[]> _polygonColliderPoints;

        public WaterAttachedComponentsModule(Game2DWater waterObject, float buoyancyEffectorSurfaceLevel, BuoyancyEffector2DSurfaceLevelLocation buoyancyEffectorSurfaceLevelLocation, BoxCollider2DTopEdgeLocation boxColliderTopEdgeLocation)
        {
            _waterObject = waterObject;
            _buoyancyEffectorSurfaceLevel = buoyancyEffectorSurfaceLevel;
            _buoyancyEffectorSurfaceLevelLocation = buoyancyEffectorSurfaceLevelLocation;
            _boxColliderTopEdgeLocation = boxColliderTopEdgeLocation;
        }

        #region Properties
        public BoxCollider2D BoxCollider { get { return _boxCollider; } }
        public PolygonCollider2D PolygonCollider { get { return _polygonCollider; } }
        public BuoyancyEffector2D BuoyancyEffector { get { return _buoyancyEffector; } }
        public EdgeCollider2D EdgeCollider { get { return _edgeCollider; } }
        public float BuoyancyEffectorSurfaceLevel
        {
            get { return (-_buoyancyEffector.surfaceLevel / _cachedWaterSize.y) + 0.5f; }
            set
            {
                _buoyancyEffectorSurfaceLevel = Mathf.Clamp01(value);
                if (_buoyancyEffectorSurfaceLevelLocation == BuoyancyEffector2DSurfaceLevelLocation.Custom)
                    UpdateBuoyancySurfaceLevel();
            }
        }
        public BuoyancyEffector2DSurfaceLevelLocation BuoyancyEffectorSurfaceLevelLocation
        {
            get { return _buoyancyEffectorSurfaceLevelLocation; }
            set
            {
                _buoyancyEffectorSurfaceLevelLocation = value;
                UpdateBuoyancySurfaceLevel();
            }
        }
        public BoxCollider2DTopEdgeLocation BoxColliderTopEdgeLocation
        {
            get { return _boxColliderTopEdgeLocation; }
            set
            {
                _boxColliderTopEdgeLocation = value;
                UpdateBoxColliderSizeAndOffset();
            }
        }
        internal bool HasAnimatorAttached { get { return _hasAnimatorAttached; } }
        #endregion

        #region Methods

        internal void Initialize()
        {
            _mainModule = _waterObject.MainModule;
            _materialModule = _waterObject.MaterialModule;
            _meshMask = _waterObject.RenderingModule.MeshMask;

            _hasAnimatorAttached = _mainModule.Transform.GetComponent<Animator>() != null;

            SetupBuoyancyEffector();
            SetupBoxCollider();
            SetupPolygonCollider();
            SetupEdgeCollider();

            _materialModule.OnSurfaceThicknessOrSubmergeLevelChanged += UpdateBoxColliderSizeAndOffset;
            _materialModule.OnSurfaceThicknessOrSubmergeLevelChanged += UpdateBuoyancySurfaceLevel;

            _cachedWaterSize = _mainModule.WaterSize;
            _cachedWaterPosition = _mainModule.Position;
            _cachedWaterScale = _mainModule.Scale;

            ApplyChanges();
        }

        internal void Update()
        {
            if(_mainModule.WaterSize != _cachedWaterSize || _mainModule.Position != _cachedWaterPosition || _mainModule.Scale != _cachedWaterScale)
                ApplyChanges();
        }

        private void SetupBuoyancyEffector()
        {
            _buoyancyEffector = _mainModule.Transform.GetComponent<BuoyancyEffector2D>();
        }

        private void SetupBoxCollider()
        {
            _boxCollider = _mainModule.Transform.GetComponent<BoxCollider2D>();

            if (_boxCollider != null)
            {
                _boxCollider.isTrigger = true;
                _boxCollider.usedByEffector = _buoyancyEffector != null;
            }
        }

        private void SetupEdgeCollider()
        {
            _edgeCollider = _mainModule.Transform.GetComponent<EdgeCollider2D>();

            if (_edgeCollider != null)
                _edgeColliderPoints = _edgeCollider.points;
        }

        private void SetupPolygonCollider()
        {
            _polygonCollider = _mainModule.Transform.GetComponent<PolygonCollider2D>();

            if (_polygonCollider != null)
            {
                _polygonCollider.isTrigger = true;
                _polygonCollider.usedByEffector = _buoyancyEffector != null;

                _polygonColliderPoints = new List<Vector2[]>();
                for (int i = 0, imax = _polygonCollider.pathCount; i < imax; i++)
                {
                    _polygonColliderPoints.Add(_polygonCollider.GetPath(i));
                }
            }
        }

        private void ApplyChanges()
        {
            UpdateBoxColliderSizeAndOffset();
            UpdateEdgeColliderPointsAndOffset();
            UpdatePolygonColliderPointsAndOffset();
            UpdateBuoyancySurfaceLevel();

            _cachedWaterSize = _mainModule.WaterSize;
            _cachedWaterPosition = _mainModule.Position;
            _cachedWaterScale = _mainModule.Scale;
        }

        private void UpdatePolygonColliderPointsAndOffset()
        {
            if (_polygonCollider == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                _polygonColliderPoints = new List<Vector2[]>();
                for (int i = 0, imax = _polygonCollider.pathCount; i < imax; i++)
                {
                    _polygonColliderPoints.Add(_polygonCollider.GetPath(i));
                }
            }
#endif

            if (_meshMask.ArePositionAndSizeLocked)
            {
                var waterScale = _mainModule.Scale;

                if (_cachedWaterScale != waterScale)
                {
                    Vector2 scale;

                    scale.x = _cachedWaterScale.x / waterScale.x;
                    scale.y = _cachedWaterScale.y / waterScale.y;

                    RescalePolygonColliderPoints(scale);
                }

                var offset = (_meshMask.Position - _mainModule.Position);

                offset.x /= waterScale.x;
                offset.y /= waterScale.y;

                _polygonCollider.offset = offset;
            }
            else
            {
                var waterSize = _mainModule.WaterSize;

                if (_cachedWaterSize != waterSize)
                {
                    Vector2 scale;

                    scale.x = waterSize.x / _cachedWaterSize.x;
                    scale.y = waterSize.y / _cachedWaterSize.y;

                    RescalePolygonColliderPoints(scale);

                    _polygonCollider.offset = Vector2.zero;
                }
            }
        }

        private void RescalePolygonColliderPoints(Vector2 scale)
        {
            for (int i = 0, imax = _polygonColliderPoints.Count; i < imax; i++)
            {
                var path = _polygonColliderPoints[i];
                RescalePoints(path, scale);
                _polygonCollider.SetPath(i, path);
            }
        }

        private void UpdateEdgeColliderPointsAndOffset()
        {
            if (_edgeCollider == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                _edgeColliderPoints = _edgeCollider.points;
#endif

            if (_meshMask.IsActive)
            {
                if (_meshMask.ArePositionAndSizeLocked)
                {
                    var waterScale = _mainModule.Scale;

                    if (_cachedWaterScale != waterScale)
                    {
                        Vector2 scale;

                        scale.x = _cachedWaterScale.x / waterScale.x;
                        scale.y = _cachedWaterScale.y / waterScale.y;

                        RescalePoints(_edgeColliderPoints, scale);

                        _edgeCollider.points = _edgeColliderPoints;
                    }

                    var offset = (_meshMask.Position - _mainModule.Position);

                    offset.x /= waterScale.x;
                    offset.y /= waterScale.y;

                    _edgeCollider.offset = offset;
                }
                else
                {
                    var waterSize = _mainModule.WaterSize;

                    if(_cachedWaterSize != waterSize)
                    {
                        Vector2 scale;
                        
                        scale.x = waterSize.x / _cachedWaterSize.x;
                        scale.y = waterSize.y / _cachedWaterSize.y;

                        RescalePoints(_edgeColliderPoints, scale);

                        _edgeCollider.points = _edgeColliderPoints;
                        _edgeCollider.offset = Vector2.zero;
                    }
                }
            }
            else
            {
                var waterSize = _mainModule.WaterSize;

                if (_edgeColliderPoints.Length != 4)
                    _edgeColliderPoints = new Vector2[4];

                Vector2 halfSize = waterSize * 0.5f;
                _edgeColliderPoints[0].x = _edgeColliderPoints[1].x = -halfSize.x;
                _edgeColliderPoints[2].x = _edgeColliderPoints[3].x = halfSize.x;

                _edgeColliderPoints[0].y = _edgeColliderPoints[3].y = halfSize.y;
                _edgeColliderPoints[1].y = _edgeColliderPoints[2].y = -halfSize.y;

                _edgeCollider.points = _edgeColliderPoints;
                _edgeCollider.offset = Vector2.zero;
            }
        }

        private void RescalePoints(Vector2[] points, Vector2 scale)
        {
            for (int i = 0, imax = points.Length; i < imax; i++)
            {
                points[i].x *= scale.x;
                points[i].y *= scale.y;
            }
        }

        private void UpdateBoxColliderSizeAndOffset()
        {
            if (_boxCollider == null)
                return;

            var waterSize = _mainModule.WaterSize;

            Vector2 size, offset;

            switch (_boxColliderTopEdgeLocation)
            {
                case BoxCollider2DTopEdgeLocation.SurfaceLevel:
                    size = new Vector2(_mainModule.Width, waterSize.y * _materialModule.GetSurfaceLevelNormalized());
                    offset = new Vector2(0f, 0.5f * (size.y - waterSize.y));
                    break;
                case BoxCollider2DTopEdgeLocation.SubmergeLevel:
                    size = new Vector2(_mainModule.Width, waterSize.y * _materialModule.GetSubmergeLevelNormalized());
                    offset = new Vector2(0f, 0.5f * (size.y - waterSize.y));
                    break;
                default: // match to water top edge
                    size = waterSize;
                    offset = Vector2.zero;
                    break;
            }

            _boxCollider.size = size;
            _boxCollider.offset = offset;
        }

        private void UpdateBuoyancySurfaceLevel()
        {
            if (_buoyancyEffector == null)
                return;

            var waterSize = _mainModule.WaterSize;

            float buoyancySurfaceLevel;

            switch (_buoyancyEffectorSurfaceLevelLocation)
            {
                case BuoyancyEffector2DSurfaceLevelLocation.WaterTopEdge:
                    buoyancySurfaceLevel = waterSize.y * 0.5f;
                    break;
                case BuoyancyEffector2DSurfaceLevelLocation.SurfaceLevel:
                    buoyancySurfaceLevel = waterSize.y * (_materialModule.GetSurfaceLevelNormalized() - 0.5f);
                    break;
                case BuoyancyEffector2DSurfaceLevelLocation.SubmergeLevel:
                    buoyancySurfaceLevel = waterSize.y * (_materialModule.GetSubmergeLevelNormalized() - 0.5f);
                    break;
                default: // custom
                    buoyancySurfaceLevel = waterSize.y * (0.5f - _buoyancyEffectorSurfaceLevel);
                    break;
            }

            _buoyancyEffector.surfaceLevel = buoyancySurfaceLevel;
        }

        #endregion

        #region Editor Only Methods

#if UNITY_EDITOR

        internal void Validate(float buoyancyEffectorSurfaceLevel, BuoyancyEffector2DSurfaceLevelLocation buoyancyEffectorSurfaceLevelLocation, BoxCollider2DTopEdgeLocation boxColliderTopEdgeLocation)
        {
            _boxColliderTopEdgeLocation = boxColliderTopEdgeLocation;
            _buoyancyEffectorSurfaceLevel = buoyancyEffectorSurfaceLevel;
            _buoyancyEffectorSurfaceLevelLocation = buoyancyEffectorSurfaceLevelLocation;

            SetupBuoyancyEffector();
            SetupBoxCollider();
            SetupPolygonCollider();
            SetupEdgeCollider();

            _hasAnimatorAttached = _mainModule.Transform.GetComponent<Animator>() != null;

            ApplyChanges();
        }

#endif

        #endregion

        #region Enums
        public enum BoxCollider2DTopEdgeLocation
        {
            WaterTopEdge,
            SurfaceLevel,
            SubmergeLevel
        }
        public enum BuoyancyEffector2DSurfaceLevelLocation
        {
            Custom,
            WaterTopEdge,
            SurfaceLevel,
            SubmergeLevel
        }
        #endregion
    }
}
