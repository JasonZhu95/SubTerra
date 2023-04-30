namespace Game2DWaterKit.Ripples
{
    using Game2DWaterKit.Mesh;
    using Game2DWaterKit.Ripples.Effects;
    using Game2DWaterKit.Simulation;
    using Game2DWaterKit.Main;
    using Game2DWaterKit.AttachedComponents;
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;

    public class WaterCollisionRipplesModule
    {
        #region Variables
        private readonly Transform _onCollisionRipplesEffectsPoolsRoot;
        private readonly WaterRipplesParticleEffect _onWaterEnterRipplesParticleEffect;
        private readonly WaterRipplesParticleEffect _onWaterExitRipplesParticleEffect;
        private readonly WaterRipplesSoundEffect _onWaterEnterRipplesSoundEffect;
        private readonly WaterRipplesSoundEffect _onWaterExitRipplesSoundEffect;

        private bool _isOnWaterEnterRipplesActive;
        private bool _isOnWaterExitRipplesActive;
        private bool _isOnWaterMoveRipplesActive;
        private bool _collisionIgnoreTriggers;
        private float _minimumDisturbance;
        private float _maximumDisturbance;
        private float _velocityMultiplier;
        private float _onWaterMoveRipplesMaximumDisturbance;
        private float _onWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance;
        private float _onWaterMoveRipplesDisturbanceSmoothFactor;
        private LayerMask _collisionMask;
        private float _collisionMinimumDepth;
        private float _collisionMaximumDepth;
        private float _collisionRaycastMaximumDistance;
        private UnityEvent _onWaterEnter;
        private UnityEvent _onWaterExit;
        private static OnWaterEnterExitEvent _onWaterEnterExit = new OnWaterEnterExitEvent();

        private Game2DWater _waterObject;
        private WaterMeshModule _meshModule;
        private WaterMainModule _mainModule;
        private WaterSimulationModule _simulationModule;
        private WaterAttachedComponentsModule _attachedComponentsModule;

        private RaycastHit2D[] _raycastResults = new RaycastHit2D[8];
        #endregion

        public WaterCollisionRipplesModule(Game2DWater waterObject, WaterCollisionRipplesModuleParameters parameters, Transform ripplesEffectsPoolsRootParent)
        {
            _waterObject = waterObject;

            _isOnWaterEnterRipplesActive = parameters.ActivateOnWaterEnterRipples;
            _isOnWaterExitRipplesActive = parameters.ActivateOnWaterExitRipples;
            _isOnWaterMoveRipplesActive = parameters.ActivateOnWaterMoveRipples;
            _collisionIgnoreTriggers = parameters.CollisionIgnoreTriggers;
            _minimumDisturbance = parameters.MinimumDisturbance;
            _maximumDisturbance = parameters.MaximumDisturbance;
            _velocityMultiplier = parameters.VelocityMultiplier;
            _onWaterMoveRipplesMaximumDisturbance = parameters.OnWaterMoveRipplesMaximumDisturbance;
            _onWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance = parameters.OnWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance;
            _onWaterMoveRipplesDisturbanceSmoothFactor = parameters.OnWaterMoveRipplesDisturbanceSmoothFactor;
            _collisionMask = parameters.CollisionMask;
            _collisionMinimumDepth = parameters.CollisionMinimumDepth;
            _collisionMaximumDepth = parameters.CollisionMaximumDepth;
            _collisionRaycastMaximumDistance = parameters.CollisionRaycastMaxDistance;
            _onWaterEnter = parameters.OnWaterEnter;
            _onWaterExit = parameters.OnWaterExit;

            _onCollisionRipplesEffectsPoolsRoot = CreateRipplesEffectsPoolsRoot(ripplesEffectsPoolsRootParent);

            _onWaterEnterRipplesParticleEffect = new WaterRipplesParticleEffect(parameters.WaterEnterParticleEffectParameters, _onCollisionRipplesEffectsPoolsRoot);
            _onWaterExitRipplesParticleEffect = new WaterRipplesParticleEffect(parameters.WaterExitParticleEffectParameters, _onCollisionRipplesEffectsPoolsRoot);
            _onWaterEnterRipplesSoundEffect = new WaterRipplesSoundEffect(parameters.WaterEnterSoundEffectParameters, _onCollisionRipplesEffectsPoolsRoot);
            _onWaterExitRipplesSoundEffect = new WaterRipplesSoundEffect(parameters.WaterExitSoundEffectParameters, _onCollisionRipplesEffectsPoolsRoot);
        }

        #region Properties
        public bool IsOnWaterEnterRipplesActive { get { return _isOnWaterEnterRipplesActive; } set { _isOnWaterEnterRipplesActive = value; } }
        public bool IsOnWaterExitRipplesActive { get { return _isOnWaterExitRipplesActive; } set { _isOnWaterExitRipplesActive = value; } }
        public bool IsOnWaterMoveRipplesActive { get { return _isOnWaterMoveRipplesActive; } set { _isOnWaterMoveRipplesActive = value; } }
        public bool CollisionIgnoreTriggers { get { return _collisionIgnoreTriggers; } set { _collisionIgnoreTriggers = value; } }
        public LayerMask CollisionMask { get { return _collisionMask; } set { _collisionMask = value; } }
        public float CollisionMaximumDepth { get { return _collisionMaximumDepth; } set { _collisionMaximumDepth = value; } }
        public float CollisionMinimumDepth { get { return _collisionMinimumDepth; } set { _collisionMinimumDepth = value; } }
        public float CollisionRaycastMaximumDistance { get { return _collisionRaycastMaximumDistance; } set { _collisionRaycastMaximumDistance = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public float MaximumDisturbance { get { return _maximumDisturbance; } set { _maximumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public float MinimumDisturbance { get { return _minimumDisturbance; } set { _minimumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public float MinimumVelocityToCauseMaximumDisturbance { get { return _maximumDisturbance / _velocityMultiplier; } set { if (value != 0f) VelocityMultiplier = _maximumDisturbance / value; } }
        public float OnWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance { get { return _onWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance; } set { _onWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public float OnWaterMoveRipplesDisturbanceSmoothFactor { get { return _onWaterMoveRipplesDisturbanceSmoothFactor; } set { _onWaterMoveRipplesDisturbanceSmoothFactor = Mathf.Clamp01(value); } }
        public float OnWaterMoveRipplesMaximumDisturbance { get { return _onWaterMoveRipplesMaximumDisturbance; } set { _onWaterMoveRipplesMaximumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public UnityEvent OnWaterEnter { get { return _onWaterEnter; } set { _onWaterEnter = value; } }
        public UnityEvent OnWaterExit { get { return _onWaterExit; } set { _onWaterExit = value; } }
        public static OnWaterEnterExitEvent OnWaterEnterExit { get { return _onWaterEnterExit; } set { _onWaterEnterExit = value; } }
        public float VelocityMultiplier { get { return _velocityMultiplier; } set { _velocityMultiplier = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public WaterRipplesParticleEffect OnWaterEnterRipplesParticleEffect { get { return _onWaterEnterRipplesParticleEffect; } }
        public WaterRipplesParticleEffect OnWaterExitRipplesParticleEffect { get { return _onWaterExitRipplesParticleEffect; } }
        public WaterRipplesSoundEffect OnWaterEnterRipplesSoundEffect { get { return _onWaterEnterRipplesSoundEffect; } }
        public WaterRipplesSoundEffect OnWaterExitRipplesSoundEffect { get { return _onWaterExitRipplesSoundEffect; } }
        #endregion

        #region Methods

        internal void Initialize()
        {
            _mainModule = _waterObject.MainModule;
            _meshModule = _waterObject.MeshModule;
            _simulationModule = _waterObject.SimulationModule;
            _attachedComponentsModule = _waterObject.AttachedComponentsModule;
        }

        internal bool ResolveCollision(Collider2D objectCollider, bool isObjectEnteringWater)
        {
            if (_collisionMask != (_collisionMask | (1 << objectCollider.gameObject.layer)))
                return false;

            if (_collisionIgnoreTriggers && objectCollider.isTrigger)
                return false;

            if (!_simulationModule.IsControlledByLargeWaterAreaManager)
            {
                if (isObjectEnteringWater)
                    _simulationModule.GetInWaterColliders().Add(objectCollider);
                else
                    _simulationModule.GetInWaterColliders().Remove(objectCollider);
            }

            if ((isObjectEnteringWater && _isOnWaterEnterRipplesActive) || (!isObjectEnteringWater && _isOnWaterExitRipplesActive))
                CreateWaterEnterExitRipples(objectCollider, isObjectEnteringWater);

            return true;
        }

        internal void Update()
        {

#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            _onWaterEnterRipplesSoundEffect.Update();
            _onWaterEnterRipplesParticleEffect.Update();
            _onWaterExitRipplesSoundEffect.Update();
            _onWaterExitRipplesParticleEffect.Update();
        }

        internal void PhysicsUpdate()
        {
            if (_isOnWaterMoveRipplesActive)
                CreateWaterMoveRipples();
        }

        private int GetObjectColliderIndexInRaycastResults(Collider2D objectCollider, int hitCount)
        {
            for (int i = 0, imax = hitCount; i < imax; i++)
            {
                if (_raycastResults[i].collider == objectCollider)
                    return i;
            }

            return -1;
        }

        private void CreateWaterEnterExitRipples(Collider2D objectCollider, bool isObjectEnteringWater)
        {
            Vector3[] vertices = _meshModule.Vertices;
            Vector3 raycastDirection = _mainModule.UpDirection;

            int surfaceVerticesCount = _meshModule.SurfaceVerticesCount;
            int subdivisionsPerUnit = _meshModule.SubdivisionsPerUnit;

            Bounds objectColliderBounds = objectCollider.bounds;
            float minColliderBoundsX = _mainModule.TransformPointWorldToLocal(objectColliderBounds.min).x;
            float maxColliderBoundsX = _mainModule.TransformPointWorldToLocal(objectColliderBounds.max).x;

            int firstSurfaceVertexIndex = _simulationModule.IsUsingCustomBoundaries ? 1 : 0;
            int lastSurfaceVertexIndex = _simulationModule.IsUsingCustomBoundaries ? surfaceVerticesCount - 2 : surfaceVerticesCount - 1;
            float firstSurfaceVertexPosition = vertices[firstSurfaceVertexIndex].x;
            int startIndex = Mathf.Clamp(Mathf.RoundToInt((minColliderBoundsX - firstSurfaceVertexPosition) * subdivisionsPerUnit), firstSurfaceVertexIndex, lastSurfaceVertexIndex);
            int endIndex = Mathf.Clamp(Mathf.RoundToInt((maxColliderBoundsX - firstSurfaceVertexPosition) * subdivisionsPerUnit), firstSurfaceVertexIndex, lastSurfaceVertexIndex);

            int hitPointCount = 0;
            float hitPointsVelocitiesSum = 0f;
            Vector2 hitPointsPositionsSum = new Vector2(0f, 0f);

            bool queriesStartInColliders = Physics2D.queriesStartInColliders;
            Physics2D.queriesStartInColliders = true;

            float raycastOriginYPos;

            var boxCollider = _attachedComponentsModule.BoxCollider;
            if (boxCollider != null && _attachedComponentsModule.BoxColliderTopEdgeLocation != WaterAttachedComponentsModule.BoxCollider2DTopEdgeLocation.WaterTopEdge)
                raycastOriginYPos = boxCollider.offset.y + boxCollider.size.y * 0.5f;
            else
                raycastOriginYPos = _mainModule.Height * 0.5f;

            for (int surfaceVertexIndex = startIndex; surfaceVertexIndex <= endIndex; surfaceVertexIndex++)
            {
                Vector2 surfaceVertexPosition = _mainModule.TransformPointLocalToWorld(new Vector3(vertices[surfaceVertexIndex].x, raycastOriginYPos));

                var hitCount = Physics2D.RaycastNonAlloc(surfaceVertexPosition, raycastDirection, _raycastResults, _collisionRaycastMaximumDistance, _collisionMask, _collisionMinimumDepth, _collisionMaximumDepth);

                if (hitCount > 0)
                {
                    int index = GetObjectColliderIndexInRaycastResults(objectCollider, hitCount);

                    if (index < 0)
                        continue;

                    RaycastHit2D hit = _raycastResults[index];

                    float velocity = hit.rigidbody.GetPointVelocity(hit.point).y * _velocityMultiplier;
                    velocity = Mathf.Clamp(Mathf.Abs(velocity), _minimumDisturbance, _maximumDisturbance);
                    _simulationModule.DisturbSurfaceVertex(surfaceVertexIndex, velocity * (isObjectEnteringWater ? -1f : 1f));
                    hitPointsVelocitiesSum += velocity;
                    hitPointsPositionsSum += hit.point;
                    hitPointCount++;
                }
            }

            Physics2D.queriesStartInColliders = queriesStartInColliders;

            if (hitPointCount > 0)
            {
                Vector2 hitPointsPositionsMean = hitPointsPositionsSum / hitPointCount;
                Vector3 spawnPosition = new Vector3(hitPointsPositionsMean.x, hitPointsPositionsMean.y, _mainModule.Position.z);

                float hitPointsVelocitiesMean = hitPointsVelocitiesSum / hitPointCount;
                float disturbanceFactor = Mathf.InverseLerp(_minimumDisturbance, _maximumDisturbance, hitPointsVelocitiesMean);

                if (isObjectEnteringWater)
                {
                    OnWaterEnterRipplesParticleEffect.PlayParticleEffect(spawnPosition);
                    OnWaterEnterRipplesSoundEffect.PlaySoundEffect(spawnPosition, disturbanceFactor);

                    if (_onWaterEnter != null)
                        _onWaterEnter.Invoke();

                    if(_onWaterEnterExit != null)
                        _onWaterEnterExit.Invoke(_waterObject, objectCollider, true);
                }
                else
                {
                    OnWaterExitRipplesParticleEffect.PlayParticleEffect(spawnPosition);
                    OnWaterExitRipplesSoundEffect.PlaySoundEffect(spawnPosition, disturbanceFactor);

                    if (_onWaterExit != null)
                        _onWaterExit.Invoke();

                    if (_onWaterEnterExit != null)
                        _onWaterEnterExit.Invoke(_waterObject, objectCollider, false);
                }
            }
        }

        private void CreateWaterMoveRipples()
        {
            var inWaterColliders = _simulationModule.GetInWaterColliders();

            if (inWaterColliders.Count > 0)
            {
                float waterRestPosition = _attachedComponentsModule.BuoyancyEffector == null ? _mainModule.Height * 0.5f : _mainModule.Height * (0.5f - _attachedComponentsModule.BuoyancyEffectorSurfaceLevel);
                float leftBoundary = _simulationModule.LeftBoundary;
                float rightBoundary = _simulationModule.RightBoundary;
                int surfaceVertexCount = _meshModule.SurfaceVerticesCount;
                int leftMostSurfaceVertexIndex = _simulationModule.IsUsingCustomBoundaries ? 1 : 0;
                int rightMostSurfaceVertexIndex = _simulationModule.IsUsingCustomBoundaries ? surfaceVertexCount - 2 : surfaceVertexCount - 1;
                float subdivisionsPerUnit = _meshModule.SurfaceVerticesCount / _mainModule.Width;

                foreach (var collider in inWaterColliders)
                {
                    Bounds colliderBounds = collider.bounds;
                    Vector2 colliderBoundsCenterWaterSpace = _mainModule.TransformPointWorldToLocal(colliderBounds.center);
                    Vector2 colliderBoundsExtents = colliderBounds.extents;

                    float xVelocity = collider.attachedRigidbody.velocity.x;

                    if ((colliderBoundsCenterWaterSpace.y + colliderBoundsExtents.y) > waterRestPosition)
                    {
                        if (Mathf.Abs(xVelocity) < 0.001f)
                            continue;

                        float xDisturbanceSourcePosition = colliderBoundsCenterWaterSpace.x + colliderBoundsExtents.x * Mathf.Sign(xVelocity);

                        if (xDisturbanceSourcePosition >= leftBoundary && xDisturbanceSourcePosition <= rightBoundary)
                        {
                            int nearestVertexIndex = Mathf.Clamp(Mathf.RoundToInt((xDisturbanceSourcePosition - leftBoundary) * subdivisionsPerUnit), leftMostSurfaceVertexIndex, rightMostSurfaceVertexIndex);
                            int previousNearestVertexIndex = nearestVertexIndex - 1;
                            int nextNearestVertexIndex = nearestVertexIndex + 1;

                            float disturbanceStrength = Mathf.Lerp(0f, _onWaterMoveRipplesMaximumDisturbance, Mathf.Abs(xVelocity) / _onWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance);
                            float nearestVertexDisturbanceStrength = disturbanceStrength / (_onWaterMoveRipplesDisturbanceSmoothFactor * 2f + 1f);
                            float neighborsDisturbanceStrength = (disturbanceStrength - nearestVertexDisturbanceStrength) * 0.5f;

                            _simulationModule.DisturbSurfaceVertex(nearestVertexIndex, nearestVertexDisturbanceStrength);

                            if (previousNearestVertexIndex >= leftMostSurfaceVertexIndex)
                                _simulationModule.DisturbSurfaceVertex(previousNearestVertexIndex, neighborsDisturbanceStrength);

                            if (nextNearestVertexIndex <= rightMostSurfaceVertexIndex)
                                _simulationModule.DisturbSurfaceVertex(nextNearestVertexIndex, neighborsDisturbanceStrength);
                        }
                    }
                }
            }
        }

        private static Transform CreateRipplesEffectsPoolsRoot(Transform parent)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return null;
#endif
            var root = new GameObject("OnCollisionRipplesEffects").transform;
            root.parent = parent;
            return root;
        }

        #endregion

        #region Editor Only Methods
#if UNITY_EDITOR
        internal void Validate(WaterCollisionRipplesModuleParameters parameters)
        {
            IsOnWaterEnterRipplesActive = parameters.ActivateOnWaterEnterRipples;
            IsOnWaterExitRipplesActive = parameters.ActivateOnWaterExitRipples;
            IsOnWaterMoveRipplesActive = parameters.ActivateOnWaterMoveRipples;
            CollisionIgnoreTriggers = parameters.CollisionIgnoreTriggers;
            MinimumDisturbance = parameters.MinimumDisturbance;
            MaximumDisturbance = parameters.MaximumDisturbance;
            VelocityMultiplier = parameters.VelocityMultiplier;
            OnWaterMoveRipplesMaximumDisturbance = parameters.OnWaterMoveRipplesMaximumDisturbance;
            OnWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance = parameters.OnWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance;
            OnWaterMoveRipplesDisturbanceSmoothFactor = parameters.OnWaterMoveRipplesDisturbanceSmoothFactor;
            CollisionMask = parameters.CollisionMask;
            CollisionMinimumDepth = parameters.CollisionMinimumDepth;
            CollisionMaximumDepth = parameters.CollisionMaximumDepth;
            CollisionRaycastMaximumDistance = parameters.CollisionRaycastMaxDistance;
            OnWaterEnter = parameters.OnWaterEnter;
            OnWaterExit = parameters.OnWaterExit;

            OnWaterEnterRipplesParticleEffect.Validate(parameters.WaterEnterParticleEffectParameters);
            OnWaterEnterRipplesSoundEffect.Validate(parameters.WaterEnterSoundEffectParameters);
            OnWaterExitRipplesParticleEffect.Validate(parameters.WaterExitParticleEffectParameters);
            OnWaterExitRipplesSoundEffect.Validate(parameters.WaterExitSoundEffectParameters);
        }
#endif
        #endregion

        public class OnWaterEnterExitEvent : UnityEvent<Game2DWater, Collider2D, bool> { }
    }

    public struct WaterCollisionRipplesModuleParameters
    {
        public bool ActivateOnWaterEnterRipples;
        public bool ActivateOnWaterExitRipples;
        public bool ActivateOnWaterMoveRipples;
        public bool CollisionIgnoreTriggers;
        public float MinimumDisturbance;
        public float MaximumDisturbance;
        public float VelocityMultiplier;
        public LayerMask CollisionMask;
        public float CollisionMinimumDepth;
        public float CollisionMaximumDepth;
        public float CollisionRaycastMaxDistance;
        public float OnWaterMoveRipplesMaximumDisturbance;
        public float OnWaterMoveRipplesMinimumVelocityToCauseMaximumDisturbance;
        public float OnWaterMoveRipplesDisturbanceSmoothFactor;
        public UnityEvent OnWaterEnter;
        public UnityEvent OnWaterExit;
        public WaterRipplesParticleEffectParameters WaterEnterParticleEffectParameters;
        public WaterRipplesSoundEffectParameters WaterEnterSoundEffectParameters;
        public WaterRipplesParticleEffectParameters WaterExitParticleEffectParameters;
        public WaterRipplesSoundEffectParameters WaterExitSoundEffectParameters;
    }

}
