namespace Game2DWaterKit
{
    using Game2DWaterKit.Main;
    using Game2DWaterKit.Simulation;
    using UnityEngine;
    using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    [AddComponentMenu("Game 2D Water Kit/Large Water Area Manager")]
    public class LargeWaterAreaManager : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera = null;
        [SerializeField] private Game2DWater _waterObject = null;
        [SerializeField] private int _waterObjectCount = 3;
        [SerializeField] private bool _isConstrained = false;
        [SerializeField] private float _constrainedRegionXMin = 0f;
        [SerializeField] private float _constrainedRegionXMax = 0f;

        private WaterSimulationModule _leftMostWaterSimulationModule;
        private WaterSimulationModule _rightMostWaterSimulationModule;
        private float _waterSurfaceHeighestPoint;
        private Bounds _waterObjectsBounds;

        private int _frameCount;
        private int _lastRenderedFrame;
        private Camera _lastRenderedFrameCamera;
        private HashSet<Collider2D> _inWaterColliders = new HashSet<Collider2D>();

        #region Properties

        public Camera MainCamera { get { return _mainCamera; } set { _mainCamera = value; } }
        public Game2DWater WaterObject { get { return _waterObject; } set { _waterObject = value; } }
        public int WaterObjectCount { get { return _waterObjectCount; } set { _waterObjectCount = Mathf.Clamp(value, 2, int.MaxValue); } }
        public bool IsConstrained { get { return _isConstrained; } }
        public float ConstrainedRegionXMin { get { return _constrainedRegionXMin; } }
        public float ConstrainedRegionXMax { get { return _constrainedRegionXMax; } }

        internal Matrix4x4 ProjectionMatrix { get; set; }
        internal Matrix4x4 WorlToVisibleAreaMatrix { get; set; }
        internal RenderTexture ReflectionRenderTexture { get; set; }
        internal RenderTexture RefractionPartiallySubmergedObjectsRenderTexture { get; set; }
        internal RenderTexture ReflectionPartiallySubmergedObjectsRenderTexture { get; set; }
        internal RenderTexture RefractionRenderTexture { get; set; }
        internal HashSet<Collider2D> InWaterColliders { get { return _inWaterColliders; } }

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            if (_waterObject == null)
                return;

            var waterObjectBoxCollider2D = _waterObject.GetComponent<BoxCollider2D>();
            if (waterObjectBoxCollider2D != null)
                waterObjectBoxCollider2D.usedByComposite = true;

            var waterObjectPolygonCollider2D = _waterObject.GetComponent<PolygonCollider2D>();
            if (waterObjectPolygonCollider2D != null)
                waterObjectPolygonCollider2D.usedByComposite = true;

            var waterObjectBuoyancyEffector2D = _waterObject.GetComponent<BuoyancyEffector2D>();
            if(waterObjectBuoyancyEffector2D != null)
            {
                waterObjectBuoyancyEffector2D.enabled = false;

                var buoyancyEffector2D = gameObject.AddComponent<BuoyancyEffector2D>();

                buoyancyEffector2D.surfaceLevel = _waterObject.MainModule.Position.y - transform.position.y + waterObjectBuoyancyEffector2D.surfaceLevel;
                buoyancyEffector2D.density = waterObjectBuoyancyEffector2D.density;
                buoyancyEffector2D.flowAngle = waterObjectBuoyancyEffector2D.flowAngle;
                buoyancyEffector2D.flowMagnitude = waterObjectBuoyancyEffector2D.flowMagnitude;
                buoyancyEffector2D.flowVariation = waterObjectBuoyancyEffector2D.flowVariation;
                buoyancyEffector2D.angularDrag = waterObjectBuoyancyEffector2D.angularDrag;
                buoyancyEffector2D.linearDrag = waterObjectBuoyancyEffector2D.linearDrag;
                buoyancyEffector2D.colliderMask = waterObjectBuoyancyEffector2D.colliderMask;
                buoyancyEffector2D.useColliderMask = waterObjectBuoyancyEffector2D.useColliderMask;

                buoyancyEffector2D.hideFlags |= HideFlags.HideInInspector;
            }

            if (!_isConstrained)
            {
                var rigidbody2D = gameObject.AddComponent<Rigidbody2D>(); //required by the composite collider
                rigidbody2D.isKinematic = true;
                rigidbody2D.hideFlags |= HideFlags.HideInInspector;

                var compositeCollider2D = gameObject.AddComponent<CompositeCollider2D>();
                compositeCollider2D.geometryType = CompositeCollider2D.GeometryType.Polygons;
                compositeCollider2D.isTrigger = true;
                compositeCollider2D.usedByEffector = true;
                compositeCollider2D.hideFlags |= HideFlags.HideInInspector;
            }
            else
            {
                if(waterObjectBoxCollider2D != null)
                {
                    waterObjectBoxCollider2D.enabled = false;

                    var boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
                    boxCollider2D.size = new Vector2(_constrainedRegionXMax - _constrainedRegionXMin, _waterObject.MainModule.Height);
                    boxCollider2D.offset = new Vector2((_constrainedRegionXMin + _constrainedRegionXMax) * 0.5f - transform.position.x, 0f);
                    boxCollider2D.isTrigger = true;
                    boxCollider2D.usedByEffector = true;
                    boxCollider2D.hideFlags |= HideFlags.HideInInspector;
                }
            }
            
            InstantiateWaterObjects();

            _waterSurfaceHeighestPoint = _waterObject.MainModule.Height * 0.5f;
        }

        private void LateUpdate()
        {
            if (_waterObject != null)
                CheckWaterObjectsPositions();
            
            //in the editor, we'll rely on EditorApplication.update callback to increment the frame count
            //because this callback is invoked even when the editor application is paused (see OnValidate () mehod on line 284)
            #if !UNITY_EDITOR
            _frameCount++;
            #endif
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime * Game2DWaterKitObject.TimeScale;

            WaterSimulationModule currentSimulationModule = _leftMostWaterSimulationModule;
            while (currentSimulationModule != null)
            {
                currentSimulationModule.PhysicsUpdate(deltaTime);
                if (currentSimulationModule.SurfaceHeighestPoint > _waterSurfaceHeighestPoint)
                    _waterSurfaceHeighestPoint = currentSimulationModule.SurfaceHeighestPoint;

                currentSimulationModule = currentSimulationModule.NextWaterSimulationModule;
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            var waterObject = GetWaterObjectLocatedAt(collider.transform.position.x);

            if (waterObject != null)
            {
                bool isValidCollision = waterObject.OnCollisonRipplesModule.ResolveCollision(collider, isObjectEnteringWater: true);

                if(isValidCollision)
                    _inWaterColliders.Add(collider);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            var waterObject = GetWaterObjectLocatedAt(collider.transform.position.x);

            if (waterObject != null)
            {
                bool isValidCollision = waterObject.OnCollisonRipplesModule.ResolveCollision(collider, isObjectEnteringWater: false);
                
                if(isValidCollision)
                    _inWaterColliders.Remove(collider);
            }
        }

        #endregion

        #region Methods

        public Game2DWater GetWaterObjectLocatedAt(float xWorldSpacePosition)
        {
            float waterHalfWidth = _waterObject.MainModule.Width * 0.5f;

            WaterSimulationModule currentSimulationModule = _leftMostWaterSimulationModule;
            while (currentSimulationModule != null)
            {
                float waterXPos = currentSimulationModule.MainModule.Position.x;
                if (Mathf.Abs(xWorldSpacePosition - waterXPos) < waterHalfWidth)
                    return currentSimulationModule.MainModule.WaterObject;

                currentSimulationModule = currentSimulationModule.NextWaterSimulationModule;
            }

            return null;
        }

        internal Bounds GetWaterObjectsBoundsRelativeToSpecifiedWaterObject(WaterMainModule currentWaterObject)
        {
            Vector2 waterSize = currentWaterObject.WaterSize;
            Vector2 halfWaterSize = waterSize * 0.5f;

            float leftMostWaterPos = currentWaterObject.TransformPointWorldToLocal(_leftMostWaterSimulationModule.MainModule.Position).x;
            float rightMostWaterPos = leftMostWaterPos + waterSize.x * (_waterObjectCount - 1);

            Vector2 min = new Vector2(-halfWaterSize.x + leftMostWaterPos, -halfWaterSize.y);
            Vector2 max = new Vector2(halfWaterSize.x + rightMostWaterPos, _waterSurfaceHeighestPoint);

            _waterObjectsBounds.SetMinMax(min, max);
            return _waterObjectsBounds;
        }

        internal bool HasAlreadyRenderedCurrentFrame(Camera currentRenderingCamera)
        {
            return _lastRenderedFrame == _frameCount && _lastRenderedFrameCamera == currentRenderingCamera;
        }

        internal void MarkCurrentFrameAsRendered(Camera currentRenderingCamera)
        {
            _lastRenderedFrame = _frameCount;
            _lastRenderedFrameCamera = currentRenderingCamera;
        }

        private void InstantiateWaterObjects()
        {
            var waterSize = _waterObject.MainModule.WaterSize;
            var shouldResizeWater = ValidateWaterSize(ref waterSize);

            if (shouldResizeWater)
                _waterObject.MainModule.SetSize(waterSize, true);

            var waterObject = _waterObject.gameObject;
            var spawnPosition = waterObject.transform.position;
            var spawnRotation = waterObject.transform.rotation;
            var parent = waterObject.transform.parent;
            var waterWidth = waterSize.x;
            var areSineWavesActive = _waterObject.SimulationModule.AreSineWavesActive;

            int surfaceVertexCount;
            if (_waterObject.SimulationModule.IsUsingCustomBoundaries)
                surfaceVertexCount = 4 + Mathf.RoundToInt(_waterObject.MeshModule.SubdivisionsPerUnit * (_waterObject.SimulationModule.RightCustomBoundary - _waterObject.SimulationModule.LeftCustomBoundary));
            else
                surfaceVertexCount = 2 + Mathf.RoundToInt(_waterObject.MeshModule.SubdivisionsPerUnit * waterWidth);

            if (_isConstrained)
            {
                if (spawnPosition.x + waterWidth * (_waterObjectCount - 1) + waterWidth * 0.5f > (_constrainedRegionXMax - 0.001f))
                    spawnPosition.x = _constrainedRegionXMax - waterWidth * (_waterObjectCount - 1) - waterWidth * 0.5f;
                else if (spawnPosition.x - waterWidth * 0.5f < (_constrainedRegionXMin + 0.001f))
                    spawnPosition.x = _constrainedRegionXMin + waterWidth * 0.5f;
                else
                    spawnPosition.x = _constrainedRegionXMin + waterWidth * 0.5f + waterWidth * Mathf.Round(((spawnPosition.x - _constrainedRegionXMin) / waterWidth));
            }
            else spawnPosition.x -= waterWidth;

            _waterObject.MainModule.Position = spawnPosition;
            _waterObject.MainModule.LargeWaterAreaManager = this;
            _leftMostWaterSimulationModule = _waterObject.SimulationModule;
            _leftMostWaterSimulationModule.IsControlledByLargeWaterAreaManager = true;

            WaterSimulationModule previousSimulationModule = _leftMostWaterSimulationModule;
            for (int i = 1; i < _waterObjectCount; i++)
            {
                spawnPosition.x += waterWidth;

                Game2DWater waterObjectClone = Instantiate(waterObject, spawnPosition, spawnRotation, parent).GetComponent<Game2DWater>();

                if (shouldResizeWater)
                    waterObjectClone.MainModule.SetSize(waterSize, true);

                waterObjectClone.MainModule.LargeWaterAreaManager = this;

                WaterSimulationModule currentSimulationModule = waterObjectClone.SimulationModule;

                currentSimulationModule.IsControlledByLargeWaterAreaManager = true;
                currentSimulationModule.PreviousWaterSimulationModule = previousSimulationModule;
                previousSimulationModule.NextWaterSimulationModule = currentSimulationModule;

                if (areSineWavesActive)
                    currentSimulationModule.SineWavesOffset = previousSimulationModule.SineWavesOffset + surfaceVertexCount - 1;

                previousSimulationModule = currentSimulationModule;
            }

            _rightMostWaterSimulationModule = previousSimulationModule;
        }

        private void CheckWaterObjectsPositions()
        {
            float halfWaterWidth = _waterObject.MainModule.Width * 0.5f;

            float xMinWater = _leftMostWaterSimulationModule.MainModule.Position.x - halfWaterWidth;
            float xMaxWater = _rightMostWaterSimulationModule.MainModule.Position.x + halfWaterWidth;
            float xPosCamera = GetCamera().transform.position.x;

            float safeZoneHalfWidth = (_waterObjectCount - 1) * halfWaterWidth;

            float xMinSafeZone, xMaxSafeZone;

            if (_isConstrained)
            {
                xMinSafeZone = Mathf.Min(Mathf.Max(xPosCamera - safeZoneHalfWidth, _constrainedRegionXMin), _constrainedRegionXMax) + 0.001f;
                xMaxSafeZone = Mathf.Max(Mathf.Min(xPosCamera + safeZoneHalfWidth, _constrainedRegionXMax), _constrainedRegionXMin) - 0.001f;
            }
            else
            {
                xMinSafeZone = xPosCamera - safeZoneHalfWidth;
                xMaxSafeZone = xPosCamera + safeZoneHalfWidth;
            }

            if (xMinWater <= xMinSafeZone && xMaxWater >= xMaxSafeZone)
                return;

            if (xMinWater > xMinSafeZone)
                MoveRightMostWaterToLeftMostPosition();
            else if (xMaxWater < xMaxSafeZone)
                MoveLeftMostWaterToRightMostPosition();
        }

        private void MoveLeftMostWaterToRightMostPosition()
        {
            float waterWidth = _leftMostWaterSimulationModule.MainModule.Width;
            Vector3 newPosition = _rightMostWaterSimulationModule.MainModule.Position + new Vector3(waterWidth, 0f, 0f);
            _leftMostWaterSimulationModule.MainModule.Position = newPosition;
            _leftMostWaterSimulationModule.ResetSimulation();

            UpdateWaterObjectsOrder(newLeftMost: _leftMostWaterSimulationModule.NextWaterSimulationModule, newRightMost: _leftMostWaterSimulationModule);
        }

        private void MoveRightMostWaterToLeftMostPosition()
        {
            float waterWidth = _rightMostWaterSimulationModule.MainModule.Width;
            Vector3 newPosition = _leftMostWaterSimulationModule.MainModule.Position - new Vector3(waterWidth, 0f, 0f);
            _rightMostWaterSimulationModule.MainModule.Position = newPosition;
            _rightMostWaterSimulationModule.ResetSimulation();

            UpdateWaterObjectsOrder(newLeftMost: _rightMostWaterSimulationModule, newRightMost: _rightMostWaterSimulationModule.PreviousWaterSimulationModule);
        }

        private void UpdateWaterObjectsOrder(WaterSimulationModule newLeftMost,WaterSimulationModule newRightMost)
        {
            _leftMostWaterSimulationModule.PreviousWaterSimulationModule = _rightMostWaterSimulationModule;
            _rightMostWaterSimulationModule.NextWaterSimulationModule = _leftMostWaterSimulationModule;

            newLeftMost.PreviousWaterSimulationModule = null;
            newRightMost.NextWaterSimulationModule = null;

            _leftMostWaterSimulationModule = newLeftMost;
            _rightMostWaterSimulationModule = newRightMost;

            if (_waterObject.SimulationModule.AreSineWavesActive)
            {
                var offset = _waterObject.MeshModule.SurfaceVerticesCount - 1;

                _leftMostWaterSimulationModule.SineWavesOffset = _leftMostWaterSimulationModule.NextWaterSimulationModule.SineWavesOffset - offset;
                _rightMostWaterSimulationModule.SineWavesOffset = _rightMostWaterSimulationModule.PreviousWaterSimulationModule.SineWavesOffset + offset;
            }
        }

        private bool ValidateWaterSize(ref Vector2 waterSize)
        {
            float initialWaterWidth = waterSize.x;

            var cam = GetCamera();
            float camFrustumWidth = cam.aspect * cam.orthographicSize * 2f;

            waterSize.x = Mathf.Max(camFrustumWidth * 0.75f, waterSize.x);

            if (_isConstrained)
            {
                float regionWidth = _constrainedRegionXMax - _constrainedRegionXMin;
                waterSize.x = regionWidth / (Mathf.RoundToInt(regionWidth / waterSize.x));
            }

            return waterSize.x != initialWaterWidth;
        }

        private Camera GetCamera()
        {
            if (_mainCamera != null)
                return _mainCamera;
            else
                return Camera.main;
        }
        #endregion

        #region Editor Only Methods
#if UNITY_EDITOR

        private void OnValidate()
        {
            _waterObjectCount = Mathf.Clamp(_waterObjectCount, 2, int.MaxValue);

            if (_constrainedRegionXMin > _constrainedRegionXMax)
                _constrainedRegionXMin = _constrainedRegionXMax;
            else if (_constrainedRegionXMax < _constrainedRegionXMin)
                _constrainedRegionXMax = _constrainedRegionXMin;

            //continues to increment frame count even when the editor application is paused
            EditorApplication.update -= IncrementFrameCount;
            EditorApplication.update += IncrementFrameCount;
        }

        private void IncrementFrameCount()
        {
            _frameCount++;
        }

        // Add menu item to create Game2D Water GameObject.
        [MenuItem("GameObject/2D Object/Game2D Water Kit/Large Water Area Manager Object", false, 10)]
        private static void CreateWaterObject(MenuCommand menuCommand)
        {
            GameObject largeWaterAreaManagerGO = new GameObject("Large Water Area", typeof(LargeWaterAreaManager));
            var spawnPosition = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            spawnPosition.z = 0f;
            largeWaterAreaManagerGO.transform.position = spawnPosition;
            largeWaterAreaManagerGO.layer = LayerMask.NameToLayer("Water");

            GameObject waterGO = new GameObject("Water", typeof(Game2DWater), typeof(BoxCollider2D), typeof(BuoyancyEffector2D));
            waterGO.transform.parent = largeWaterAreaManagerGO.transform;
            waterGO.transform.localPosition = Vector3.zero;
            waterGO.layer = LayerMask.NameToLayer("Water");
            var boxCollider2D = waterGO.GetComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = boxCollider2D.usedByEffector = true;

            var largeWaterArea = largeWaterAreaManagerGO.GetComponent<LargeWaterAreaManager>();
            largeWaterArea._waterObject = waterGO.GetComponent<Game2DWater>();
            largeWaterArea._mainCamera = Camera.main;
            
            GameObjectUtility.SetParentAndAlign(largeWaterAreaManagerGO, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(largeWaterAreaManagerGO, "Create " + largeWaterAreaManagerGO.name);
            Selection.activeObject = waterGO;
        }
        #endif
        #endregion
    }
}
