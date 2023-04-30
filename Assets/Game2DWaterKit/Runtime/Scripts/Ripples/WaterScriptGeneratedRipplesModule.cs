namespace Game2DWaterKit.Ripples
{
    using Game2DWaterKit.Mesh;
    using Game2DWaterKit.Ripples.Effects;
    using Game2DWaterKit.Simulation;
    using Game2DWaterKit.Main;
    using UnityEngine;

    public class WaterScriptGeneratedRipplesModule
    {
        #region Variables
        private readonly Transform _ripplesEffectsRoot;
        private readonly WaterRipplesParticleEffect _particleEffect;
        private readonly WaterRipplesSoundEffect _soundEffect;

        private float _minimumDisturbance;
        private float _maximumDisturbance;

        private Game2DWater _waterObject;
        private WaterMainModule _mainModule;
        private WaterMeshModule _meshModule;
        private WaterSimulationModule _simulationModule;
        #endregion

        public WaterScriptGeneratedRipplesModule(Game2DWater waterObject, WaterScriptGeneratedRipplesModuleParameters parameters, Transform ripplesEffectsRootParent)
        {
            _waterObject = waterObject;

            _minimumDisturbance = parameters.MinimumDisturbance;
            _maximumDisturbance = parameters.MaximumDisturbance;

            _ripplesEffectsRoot = CreateRipplesEffectsRoot(ripplesEffectsRootParent);

            _particleEffect = new WaterRipplesParticleEffect(parameters.ParticleEffectParameters, _ripplesEffectsRoot);
            _soundEffect = new WaterRipplesSoundEffect(parameters.SoundEffectParameters, _ripplesEffectsRoot);
        }

        #region Properties
        public WaterRipplesParticleEffect ParticleEffect { get { return _particleEffect; } }
        public WaterRipplesSoundEffect SoundEffect { get { return _soundEffect; } }
        public float MaximumDisturbance { get { return _maximumDisturbance; } set { _maximumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public float MinimumDisturbance { get { return _minimumDisturbance; } set { _minimumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue); } }
        #endregion

        #region Methods

        /// <summary>
        /// Generate a ripple at a particular position.
        /// </summary>
        /// <param name="position">Ripple position.</param>
        /// <param name="disturbanceFactor">Range: [0..1]: The disturbance is linearly interpolated between the minimum disturbance and the maximum disturbance by this factor.</param>
        /// <param name="pullWaterDown">Pull water down or up?</param>
        /// <param name="playSoundEffect">Play the sound effect.</param>
        /// <param name="playParticleEffect">Play the particle effect.</param>
        /// <param name="smoothRipple">Disturb neighbor vertices to create a smoother ripple (wave).</param>
        /// <param name="smoothingFactor">Range: [0..1]: The amount of disturbance to apply to neighbor vertices.</param>
        public void GenerateRipple(Vector2 position, float disturbanceFactor, bool pullWaterDown, bool playSoundEffect, bool playParticleEffect, bool smoothRipple, float smoothingFactor = 0.5f)
        {
            float xPosition = _mainModule.TransformPointWorldToLocal(position).x;

            float leftBoundary = _simulationModule.LeftBoundary;
            float rightBoundary = _simulationModule.RightBoundary;
            int surfaceVertexCount = _meshModule.SurfaceVerticesCount;
            int leftMostSurfaceVertexIndex = _simulationModule.IsUsingCustomBoundaries ? 1 : 0;
            int rightMostSurfaceVertexIndex = _simulationModule.IsUsingCustomBoundaries ? surfaceVertexCount - 2 : surfaceVertexCount - 1;

            if (xPosition < leftBoundary || xPosition > rightBoundary)
                return;

            float disturbance = (pullWaterDown ? -1f : 1f) * Mathf.Lerp(_minimumDisturbance, _maximumDisturbance, Mathf.Clamp01(disturbanceFactor));

            float subdivisionsPerUnit = (surfaceVertexCount - (_simulationModule.IsUsingCustomBoundaries ? 3 : 1)) / (rightBoundary - leftBoundary);
            float delta = (xPosition - leftBoundary) * subdivisionsPerUnit;
            int nearestVertexIndex = leftMostSurfaceVertexIndex + Mathf.RoundToInt(delta);

            _simulationModule.DisturbSurfaceVertex(nearestVertexIndex, disturbance);

            if (smoothRipple)
            {
                smoothingFactor = Mathf.Clamp01(smoothingFactor);
                float smoothedDisturbance = disturbance * smoothingFactor;

                int previousNearestIndex = nearestVertexIndex - 1;
                if (previousNearestIndex >= leftMostSurfaceVertexIndex)
                    _simulationModule.DisturbSurfaceVertex(previousNearestIndex, smoothedDisturbance);

                int nextNearestIndex = nearestVertexIndex + 1;
                if (nextNearestIndex <= rightMostSurfaceVertexIndex)
                    _simulationModule.DisturbSurfaceVertex(nextNearestIndex, smoothedDisturbance);
            }

            Vector3 spawnPosition = _mainModule.TransformPointLocalToWorld(new Vector3(xPosition, _mainModule.Height * 0.5f));

            if (playParticleEffect)
                _particleEffect.PlayParticleEffect(spawnPosition);

            if (playSoundEffect)
                _soundEffect.PlaySoundEffect(spawnPosition, disturbanceFactor);
        }

        internal void Initialize()
        {
            _meshModule = _waterObject.MeshModule;
            _mainModule = _waterObject.MainModule;
            _simulationModule = _waterObject.SimulationModule;
        }

        internal void Update()
        {

#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            _soundEffect.Update();
            _particleEffect.Update();
        }

        private static Transform CreateRipplesEffectsRoot(Transform parent)
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
                return null;
            #endif
            var root = new GameObject("ScriptGeneratedRipplesEffects").transform;
            root.parent = parent;
            return root;
        }

        #endregion

        #region Editor Only Methods
        #if UNITY_EDITOR
        internal void Validate(WaterScriptGeneratedRipplesModuleParameters parameters)
        {
            MinimumDisturbance = parameters.MinimumDisturbance;
            MaximumDisturbance = parameters.MaximumDisturbance;

            _particleEffect.Validate(parameters.ParticleEffectParameters);
            _soundEffect.Validate(parameters.SoundEffectParameters);
        }
        #endif
        #endregion
    }

    public struct WaterScriptGeneratedRipplesModuleParameters
    {
        public float MaximumDisturbance;
        public float MinimumDisturbance;
        public WaterRipplesParticleEffectParameters ParticleEffectParameters;
        public WaterRipplesSoundEffectParameters SoundEffectParameters;
    }
}
