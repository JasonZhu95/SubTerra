namespace Game2DWaterKit.Simulation
{
    using Game2DWaterKit.Main;
    using Game2DWaterKit.Mesh;
    using Game2DWaterKit.AttachedComponents;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class WaterSimulationModule
    {
        private const float updateSimulationMinimumAbsoluteVelocityThreshold = 0.001f;

        private Game2DWater _waterObject;
        private WaterMainModule _mainModule;
        private WaterMeshModule _meshModule;
        private WaterAttachedComponentsModule _attachedComponentsModule;

        private float _damping;
        private float _stiffness;
        private float _stiffnessSquareRoot;
        private float _spread;
        private float _firstCustomBoundary;
        private float _secondCustomBoundary;
        private bool _isUsingCustomBoundaries;
        private float _maximumdDynamicWavesDisturbance;
        private bool _limitDynamicWavesDisturbance;
        private bool _canWavesAffectRigidbodies;
        private float _wavesStrengthOnRigidbodies;

        private float _leftCustomBoundary;
        private float _rightCustomBoundary;
        private float _surfaceHeighestPointDeltaHeight;
        private float[] _dynamicWavesVelocities;
        private float[] _dynamicWavesPositions;
        private float[] _sinesWavesPositions;
        private bool _shouldUpdateDynamicWavesSimulation;

        private bool _areSineWavesActive;
        private List<WaterSimulationSineWaveParameters> _sineWavesParameters;
        private float _sineWavesSimulationElapsedTime;
        private HashSet<Collider2D> _inWaterColliders = new HashSet<Collider2D>();
        private Queue<Collider2D> _destroyedCollidersToRemoveFromInWaterCollidersSet = new Queue<Collider2D>();

        public WaterSimulationModule(Game2DWater waterObject, WaterSimulationModuleParameters parameters)
        {
            _waterObject = waterObject;

            _damping = parameters.Damping;
            _stiffness = parameters.Stiffness;
            _spread = parameters.Spread;
            _firstCustomBoundary = parameters.FirstCustomBoundary;
            _secondCustomBoundary = parameters.SecondCustomBoundary;
            _isUsingCustomBoundaries = parameters.IsUsingCustomBoundaries;
            _maximumdDynamicWavesDisturbance = parameters.MaximumDynamicWavesDisturbance;
            _limitDynamicWavesDisturbance = parameters.LimitDynamicWavesDisturbance;
            _areSineWavesActive = parameters.AreSineWavesActive;
            _sineWavesParameters = parameters.SineWavesParameters;
            _canWavesAffectRigidbodies = parameters.CanWavesAffectRigidbodies;
            _wavesStrengthOnRigidbodies = parameters.WavesStrengthOnRigidbodies;

            _stiffnessSquareRoot = Mathf.Sqrt(_stiffness);
            _leftCustomBoundary = Mathf.Min(_firstCustomBoundary, _secondCustomBoundary);
            _rightCustomBoundary = Mathf.Max(_firstCustomBoundary, _secondCustomBoundary);
        }

        #region Properties
        public float Damping { get { return _damping; } set { _damping = Mathf.Clamp01(value); } }
        public float Spread { get { return _spread; } set { _spread = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public float Stiffness
        {
            get { return _stiffness; }

            set
            {
                _stiffness = Mathf.Clamp(value, 0f, float.MaxValue);
                _stiffnessSquareRoot = Mathf.Sqrt(_stiffness);
            }
        }
        public bool IsUsingCustomBoundaries
        {
            get { return _isUsingCustomBoundaries; }
            set
            {
                if (_isUsingCustomBoundaries == value)
                    return;

                _isUsingCustomBoundaries = value;
                _meshModule.RecomputeMeshData();
            }
        }
        public float FirstCustomBoundary
        {
            get { return _firstCustomBoundary; }
            set
            {
                float halfWaterWidth = _mainModule.Width * 0.5f;
                _firstCustomBoundary = Mathf.Clamp(value, -halfWaterWidth, halfWaterWidth);

                _leftCustomBoundary = Mathf.Min(_firstCustomBoundary, _secondCustomBoundary);
                _rightCustomBoundary = Mathf.Max(_firstCustomBoundary, _secondCustomBoundary);
            }
        }
        public float SecondCustomBoundary
        {
            get { return _secondCustomBoundary; }
            set
            {
                float halfWaterWidth = _mainModule.Width * 0.5f;
                _secondCustomBoundary = Mathf.Clamp(value, -halfWaterWidth, halfWaterWidth);

                _leftCustomBoundary = Mathf.Min(_firstCustomBoundary, _secondCustomBoundary);
                _rightCustomBoundary = Mathf.Max(_firstCustomBoundary, _secondCustomBoundary);
            }
        }
        public float LeftBoundary { get { return _isUsingCustomBoundaries ? LeftCustomBoundary : _mainModule.Width * -0.5f; } }
        public float RightBoundary { get { return _isUsingCustomBoundaries ? RightCustomBoundary : _mainModule.Width * 0.5f; } }
        public float MaximumDynamicWavesDisturbance { get { return _maximumdDynamicWavesDisturbance; } set { _maximumdDynamicWavesDisturbance = value; } }
        public bool LimitDynamicWavesDisturbance { get { return _limitDynamicWavesDisturbance; } set { _limitDynamicWavesDisturbance = value; } }
        public bool AreSineWavesActive { get { return _areSineWavesActive; } set { _areSineWavesActive = value; } }
        public List<WaterSimulationSineWaveParameters> SineWavesParameters { get { return _sineWavesParameters; } set { _sineWavesParameters = value; } }
        public bool CanWavesAffectRigidbodies { get { return _canWavesAffectRigidbodies; } set { _canWavesAffectRigidbodies = value; } }
        public float WavesStrengthOnRigidbodies { get { return _wavesStrengthOnRigidbodies; } set { _wavesStrengthOnRigidbodies = value; } }
        internal int SineWavesOffset { get; set; }
        internal WaterMainModule MainModule { get { return _mainModule; } }
        internal float[] DynamicWavesVelocities { get { return _dynamicWavesVelocities; } }
        internal float LeftCustomBoundary { get { return Mathf.Clamp(_leftCustomBoundary, -_mainModule.Width * 0.5f, _mainModule.Width * 0.5f); } }
        internal float RightCustomBoundary { get { return Mathf.Clamp(_rightCustomBoundary, -_mainModule.Width * 0.5f, _mainModule.Width * 0.5f); } }
        internal bool IsControlledByLargeWaterAreaManager { get; set; }
        internal WaterSimulationModule NextWaterSimulationModule { get; set; }
        internal WaterSimulationModule PreviousWaterSimulationModule { get; set; }
        internal float SurfaceHeighestPoint { get { return _mainModule.Height * 0.5f + _surfaceHeighestPointDeltaHeight; } }
        private float LastSurfaceVertexDynamicWavesPositionHeight { get { return _dynamicWavesPositions[_meshModule.SurfaceVerticesCount - 1]; } }
        private float SecondSurfaceVertexDynamicWavesPositionHeight { get { return _dynamicWavesPositions[1]; } }
        private bool ShouldUpdateDynamicWavesSimulation
        {
            get
            {
                if (_shouldUpdateDynamicWavesSimulation)
                    return true;

                if (IsControlledByLargeWaterAreaManager)
                {
                    if (PreviousWaterSimulationModule != null && PreviousWaterSimulationModule._shouldUpdateDynamicWavesSimulation)
                        return true;

                    if (NextWaterSimulationModule != null && NextWaterSimulationModule._shouldUpdateDynamicWavesSimulation)
                        return true;
                }

                return false;
            }
        }
        #endregion

        #region Methods

        public void ResetSineWavesSimulation()
        {
            _sineWavesSimulationElapsedTime = 0f;

            if (_sinesWavesPositions == null)
                return;

            for (int i = 0, imax = _sinesWavesPositions.Length; i < imax; i++)
            {
                _sinesWavesPositions[i] = 0f;
            }
        }

        internal void Initialize()
        {
            _mainModule = _waterObject.MainModule;
            _meshModule = _waterObject.MeshModule;
            _attachedComponentsModule = _waterObject.AttachedComponentsModule;

            ResetSimulation();
            _meshModule.OnRecomputeMesh += ResetSimulation;
            _surfaceHeighestPointDeltaHeight = 0f;
        }

        internal void PhysicsUpdate(float deltaTime)
        {
            bool updatedSimulation = _areSineWavesActive || ShouldUpdateDynamicWavesSimulation;

            if (ShouldUpdateDynamicWavesSimulation)
                IterateDynamicWavesSimulation(deltaTime);

            if (_areSineWavesActive)
                IterateSineWavesSimulation(deltaTime);

            if (updatedSimulation)
            {
                UpdateMeshVertices();

                if (_canWavesAffectRigidbodies)
                    AffectRigidbodies();
            }
        }

        internal void ResetSimulation()
        {
            _surfaceHeighestPointDeltaHeight = 0f;

            if(!IsControlledByLargeWaterAreaManager)
                _sineWavesSimulationElapsedTime = 0f;

            if (_dynamicWavesVelocities == null || _dynamicWavesVelocities.Length != _meshModule.SurfaceVerticesCount)
            {
                var surfaceVertexCount = _meshModule.SurfaceVerticesCount;

                _dynamicWavesVelocities = new float[surfaceVertexCount];
                _dynamicWavesPositions = new float[surfaceVertexCount];
                _sinesWavesPositions = new float[surfaceVertexCount];
            }

            float waterPositionOfRest = _mainModule.Height * 0.5f;
            var vertices = _meshModule.Vertices;
            for (int i = 0, imax = _dynamicWavesVelocities.Length; i < imax; i++)
            {
                _dynamicWavesVelocities[i] = 0f;
                _sinesWavesPositions[i] = 0f;
                _dynamicWavesPositions[i] = 0f;

                vertices[i].y = waterPositionOfRest;
            }

            _meshModule.UpdateMeshData();
        }

        internal void DisturbSurfaceVertex(int surfaceVertexIndex, float distubance)
        {
            if (_limitDynamicWavesDisturbance)
            {
                float minVelocity = -_maximumdDynamicWavesDisturbance * _stiffnessSquareRoot;
                float maxVelocity = _maximumdDynamicWavesDisturbance * _stiffnessSquareRoot;
                float currentVelocity = _dynamicWavesVelocities[surfaceVertexIndex];

                _dynamicWavesVelocities[surfaceVertexIndex] = Mathf.Clamp(currentVelocity + distubance * _stiffnessSquareRoot, minVelocity, maxVelocity);
            }
            else _dynamicWavesVelocities[surfaceVertexIndex] += distubance * _stiffnessSquareRoot;

            _shouldUpdateDynamicWavesSimulation = true;
        }

        internal HashSet<Collider2D> GetInWaterColliders()
        {
            if (IsControlledByLargeWaterAreaManager)
                return _mainModule.LargeWaterAreaManager.InWaterColliders;
            else
                return _inWaterColliders;
        }

        private void IterateDynamicWavesSimulation(float deltaTime)
        {
            int surfaceVerticesCount = _meshModule.SurfaceVerticesCount;

            int startSurfaceVertexIndex = _isUsingCustomBoundaries ? 1 : 0;
            int endSurfaceVertexIndex = _isUsingCustomBoundaries ? surfaceVerticesCount - 2 : surfaceVerticesCount - 1;

            float firstSurfaceVertexHeight;
            float endSurfaceVertexHeight;

            if (!IsControlledByLargeWaterAreaManager || _isUsingCustomBoundaries)
            {
                firstSurfaceVertexHeight = _dynamicWavesPositions[startSurfaceVertexIndex];
                endSurfaceVertexHeight = _dynamicWavesPositions[endSurfaceVertexIndex];
            }
            else
            {
                if (PreviousWaterSimulationModule != null)
                {
                    firstSurfaceVertexHeight = PreviousWaterSimulationModule.LastSurfaceVertexDynamicWavesPositionHeight;
                    _dynamicWavesPositions[0] = firstSurfaceVertexHeight;
                    startSurfaceVertexIndex++;
                }
                else
                {
                    firstSurfaceVertexHeight = _dynamicWavesPositions[startSurfaceVertexIndex];
                }

                if (NextWaterSimulationModule != null)
                    endSurfaceVertexHeight = NextWaterSimulationModule.SecondSurfaceVertexDynamicWavesPositionHeight;
                else
                    endSurfaceVertexHeight = _dynamicWavesPositions[endSurfaceVertexIndex];
            }

            float dampingFactor = _damping * 2f * _stiffnessSquareRoot;
            float spreadFactor = _spread * _meshModule.SubdivisionsPerUnit;

            float currentVertexPosition = _dynamicWavesPositions[startSurfaceVertexIndex];
            float previousVertexPosition = firstSurfaceVertexHeight;
            float nextVertexPosition;

            _shouldUpdateDynamicWavesSimulation = false;

            int iterations = 1 + Mathf.FloorToInt(_meshModule.SubdivisionsPerUnit / 11);
            deltaTime /= iterations;

            for (int iter = 0; iter < iterations; iter++)
            {
                for (int i = startSurfaceVertexIndex; i <= endSurfaceVertexIndex; i++)
                {
                    nextVertexPosition = i + 1 <= endSurfaceVertexIndex ? _dynamicWavesPositions[i + 1] : endSurfaceVertexHeight;

                    float velocity = _dynamicWavesVelocities[i];
                    float restoringForce = -_stiffness * currentVertexPosition;
                    float dampingForce = -dampingFactor * velocity;
                    float spreadForce = spreadFactor * (previousVertexPosition - currentVertexPosition + nextVertexPosition - currentVertexPosition);

                    previousVertexPosition = currentVertexPosition;

                    float forces = restoringForce + dampingForce + spreadForce;

                    velocity += forces * deltaTime;
                    currentVertexPosition += velocity * deltaTime;

                    _dynamicWavesVelocities[i] = velocity;
                    _dynamicWavesPositions[i] = currentVertexPosition;

                    currentVertexPosition = nextVertexPosition;

                    _shouldUpdateDynamicWavesSimulation |= velocity < -updateSimulationMinimumAbsoluteVelocityThreshold || velocity > updateSimulationMinimumAbsoluteVelocityThreshold;
                }
            }
        }

        private void IterateSineWavesSimulation(float deltaTime)
        {
            if (_sineWavesParameters == null || _sineWavesParameters.Count < 1)
                return;

            _sineWavesSimulationElapsedTime += deltaTime;

            float subdivisionLength = 1f / _meshModule.SubdivisionsPerUnit;

            int sineWavesOffset = IsControlledByLargeWaterAreaManager ? SineWavesOffset : 0;

            for (int i = 0, imax = _sineWavesParameters.Count; i < imax; i++)
            {
                var currentSineWave = _sineWavesParameters[i];

                float cst = 2f * Mathf.PI / Mathf.Clamp(currentSineWave.Length, 0.001f, float.MaxValue);

                for (int j = sineWavesOffset, jmax = _sinesWavesPositions.Length + sineWavesOffset; j < jmax; j++)
                {
                    int currentVertexIndex = j - sineWavesOffset;

                    if (i == 0)
                        _sinesWavesPositions[currentVertexIndex] = 0f;

                    var x = j * subdivisionLength;

                    _sinesWavesPositions[currentVertexIndex] += currentSineWave.Amplitude * Mathf.Sin((x + currentSineWave.Offset + _sineWavesSimulationElapsedTime * currentSineWave.Velocity) * cst) / imax;
                }
            }
        }

        private void AffectRigidbodies()
        {
            var inWaterColliders = GetInWaterColliders();

            if (inWaterColliders.Count > 0)
            {
                float waterRestPosition = _mainModule.Height * 0.5f;
                float waterBuoyancyLevel = _mainModule.Height * (0.5f - _waterObject.AttachedComponentsModule.BuoyancyEffectorSurfaceLevel);
                float offset = waterRestPosition - waterBuoyancyLevel;

                float leftBoundary = LeftBoundary;
                int surfaceVertexCount = _meshModule.SurfaceVerticesCount;
                int leftMostSurfaceVertexIndex = _isUsingCustomBoundaries ? 1 : 0;
                int rightMostSurfaceVertexIndex = _isUsingCustomBoundaries ? surfaceVertexCount - 2 : surfaceVertexCount - 1;
                float subdivisionsPerUnit = _meshModule.SurfaceVerticesCount / _mainModule.Width;
                float cellWidth = 1f / subdivisionsPerUnit;

                float density = _attachedComponentsModule.BuoyancyEffector.density;
                Vector2 gravity = Physics2D.gravity;

                Vector3[] vertices = _meshModule.Vertices;

                foreach (var collider in inWaterColliders)
                {
                    if (collider == null)
                    {
                        _destroyedCollidersToRemoveFromInWaterCollidersSet.Enqueue(collider);
                        continue;
                    }

                    Bounds colliderBounds = collider.bounds;
                    Vector2 colliderBoundsCenterWaterSpace = _mainModule.TransformPointWorldToLocal(colliderBounds.center);
                    Vector2 colliderBoundsExtents = colliderBounds.extents;

                    float yMaxColliderBounds = colliderBoundsCenterWaterSpace.y + colliderBoundsExtents.y;
                    float yMinColliderBounds = colliderBoundsCenterWaterSpace.y - colliderBoundsExtents.y;

                    if (yMaxColliderBounds > waterBuoyancyLevel)
                    {
                        int nearestStartVertexIndex = Mathf.Clamp(Mathf.RoundToInt((colliderBoundsCenterWaterSpace.x - colliderBoundsExtents.x - leftBoundary) * subdivisionsPerUnit), leftMostSurfaceVertexIndex, rightMostSurfaceVertexIndex);
                        int nearestEndVertexIndex = Mathf.Clamp(Mathf.RoundToInt((colliderBoundsCenterWaterSpace.x + colliderBoundsExtents.x - leftBoundary) * subdivisionsPerUnit), leftMostSurfaceVertexIndex, rightMostSurfaceVertexIndex);

                        Vector2 centroid = Vector2.zero;
                        float totalArea = 0f;

                        for (int i = nearestStartVertexIndex; i < nearestEndVertexIndex; i++)
                        {
                            Vector2 currentSurfaceVertexPosition = vertices[i];

                            currentSurfaceVertexPosition.y -= offset;

                            if (currentSurfaceVertexPosition.y > yMaxColliderBounds)
                                currentSurfaceVertexPosition.y = yMaxColliderBounds;
                            else if (currentSurfaceVertexPosition.y < yMinColliderBounds)
                                currentSurfaceVertexPosition.y = yMinColliderBounds;

                            float cellHeight = currentSurfaceVertexPosition.y - waterBuoyancyLevel;

                            Vector2 cellCenter = currentSurfaceVertexPosition + 0.5f * new Vector2(cellWidth, -cellHeight);
                            float cellArea = cellHeight * cellWidth;

                            totalArea += cellArea;

                            if (Mathf.Abs(totalArea) > 0f)
                                centroid += (cellArea / totalArea) * (cellCenter - centroid);
                        }

                        var attachedRigidbody = collider.attachedRigidbody;

                        Vector2 force = (_wavesStrengthOnRigidbodies * totalArea * density * attachedRigidbody.gravityScale) * -gravity;

                        attachedRigidbody.AddForceAtPosition(force, _mainModule.TransformPointLocalToWorld(centroid));
                    }
                }

                while (_destroyedCollidersToRemoveFromInWaterCollidersSet.Count > 0)
                    inWaterColliders.Remove(_destroyedCollidersToRemoveFromInWaterCollidersSet.Dequeue());
            }
        }

        private void UpdateMeshVertices()
        {
            var vertices = _meshModule.Vertices;

            float waterRestPosition = MainModule.Height * 0.5f;
            float heighestPoint = float.MinValue;

            for (int i = 0, imax = _dynamicWavesVelocities.Length; i < imax; i++)
            {
                var vertexHeight = waterRestPosition + _dynamicWavesPositions[i] + _sinesWavesPositions[i];

                vertices[i].y = vertexHeight;

                if (vertexHeight > heighestPoint)
                    heighestPoint = vertexHeight;
            }

            _surfaceHeighestPointDeltaHeight = heighestPoint - _mainModule.Height * 0.5f;

            _meshModule.UpdateMeshData();
        }

        #endregion

        #region Editor Only Methods
#if UNITY_EDITOR
        internal void Validate(WaterSimulationModuleParameters parameters)
        {
            bool recomputeMesh = parameters.FirstCustomBoundary != FirstCustomBoundary || parameters.SecondCustomBoundary != SecondCustomBoundary;

            Damping = parameters.Damping;
            Stiffness = parameters.Stiffness;
            Spread = parameters.Spread;
            FirstCustomBoundary = parameters.FirstCustomBoundary;
            SecondCustomBoundary = parameters.SecondCustomBoundary;
            IsUsingCustomBoundaries = parameters.IsUsingCustomBoundaries;
            MaximumDynamicWavesDisturbance = parameters.MaximumDynamicWavesDisturbance;
            LimitDynamicWavesDisturbance = parameters.LimitDynamicWavesDisturbance;
            AreSineWavesActive = parameters.AreSineWavesActive;
            SineWavesParameters = parameters.SineWavesParameters;
            CanWavesAffectRigidbodies = parameters.CanWavesAffectRigidbodies;
            WavesStrengthOnRigidbodies = parameters.WavesStrengthOnRigidbodies;

            if (recomputeMesh)
                _meshModule.RecomputeMeshData();
        }
#endif
        #endregion
    }

    public struct WaterSimulationModuleParameters
    {
        public float Damping;
        public float Stiffness;
        public float Spread;
        public float FirstCustomBoundary;
        public float SecondCustomBoundary;
        public bool IsUsingCustomBoundaries;
        public float MaximumDynamicWavesDisturbance;
        public bool LimitDynamicWavesDisturbance;
        public bool AreSineWavesActive;
        public List<WaterSimulationSineWaveParameters> SineWavesParameters;
        public bool CanWavesAffectRigidbodies;
        public float WavesStrengthOnRigidbodies;
    }

    [System.Serializable]
    public struct WaterSimulationSineWaveParameters
    {
        public bool IsRandom;
        public float Amplitude;
        public float Length;
        public float Velocity;
        public float Offset;
    }
}
