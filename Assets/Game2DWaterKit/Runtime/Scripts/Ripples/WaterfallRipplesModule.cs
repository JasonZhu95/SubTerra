namespace Game2DWaterKit.Ripples
{
    using Game2DWaterKit.Main;
    using UnityEngine;
    using System.Collections.Generic;

    public class WaterfallRipplesModule
    {
        private Game2DWaterfall _waterfallObject;
        private WaterfallMainModule _mainModule;

        private bool _isActive;
        private bool _updateWhenOffscreen;
        private bool _randomizeTimeInterval;
        private float _timeInterval;
        private float _minimumTimeInterval;
        private float _maximumTimeInterval;
        private List<WaterfallAffectedWaterObjet> _affectedWaterObjets;

        private float _elapsedTime;
        private float _currentInterval;

        #region Properties
        public bool IsActive { get { return _isActive; } set { _isActive = value; } }
        public bool UpdateWhenOffscreen { get { return _updateWhenOffscreen; } set { _updateWhenOffscreen = value; } }
        public List<WaterfallAffectedWaterObjet> AffectedWaterObjects { get { return _affectedWaterObjets; } set { _affectedWaterObjets = value; } }
        public float TimeInterval { get { return _timeInterval; } set { _timeInterval = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public float MaximumTimeInterval { get { return _maximumTimeInterval; } set { _maximumTimeInterval = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public float MinimumTimeInterval { get { return _minimumTimeInterval; } set { _minimumTimeInterval = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public bool RandomizeTimeInterval { get { return _randomizeTimeInterval; } set { _randomizeTimeInterval = value; } }
        #endregion

        public WaterfallRipplesModule(Game2DWaterfall waterfallObject, WaterfallRipplesModuleParameters parameters)
        {
            _waterfallObject = waterfallObject;

            _isActive = parameters.IsActive;
            _updateWhenOffscreen = parameters.UpdateWhenOffscreen;
            _randomizeTimeInterval = parameters.RandomizeTimeInterval;
            _timeInterval = parameters.TimeInterval;
            _minimumTimeInterval = parameters.MinimumTimeInterval;
            _maximumTimeInterval = parameters.MaximumTimeInterval;
            _affectedWaterObjets = parameters.AffectedWaterObjets;
        }

        internal void Initialize()
        {
            _mainModule = _waterfallObject.MainModule;
        }

        internal void PhysicsUpdate(float deltaTime)
        {
            if (!_isActive || _affectedWaterObjets == null || (!_updateWhenOffscreen && !_mainModule.IsVisible))
                return;

            _elapsedTime += deltaTime;

            if (_elapsedTime >= _currentInterval)
            {
                Vector2 halfSize = _mainModule.WaterfallSize * 0.5f;
                Vector2 left = _mainModule.TransformPointLocalToWorld(new Vector2(-halfSize.x, -halfSize.y));
                Vector2 right = _mainModule.TransformPointLocalToWorld(new Vector2(halfSize.x, -halfSize.y));

                for (int i = 0, imax = _affectedWaterObjets.Count; i < imax; i++)
                {
                    var affectedWaterObject = _affectedWaterObjets[i];

                    if (affectedWaterObject != null && affectedWaterObject.waterObject != null)
                    {
                        Game2DWater waterObject = affectedWaterObject.waterObject;
                        float disturbance = Random.Range(affectedWaterObject.minimumDisturbance, affectedWaterObject.maximumDisturbance);
                        bool smooth = affectedWaterObject.smoothRipples;
                        float smoothingFactor = affectedWaterObject.smoothingFactor;
                        float spread = affectedWaterObject.spread;

                        waterObject.WaterfallRipplesModule.CreateRipples(left, right, disturbance, spread, smooth, smoothingFactor);
                    }
                }

                _currentInterval = _randomizeTimeInterval ? Random.Range(_minimumTimeInterval, _maximumTimeInterval) : _timeInterval;
                _elapsedTime = 0f;
            }
        }

#if UNITY_EDITOR
        internal void Validate(WaterfallRipplesModuleParameters parameters)
        {
            IsActive = parameters.IsActive;
            AffectedWaterObjects = parameters.AffectedWaterObjets;
            TimeInterval = parameters.TimeInterval;
            MinimumTimeInterval = parameters.MinimumTimeInterval;
            MaximumTimeInterval = parameters.MaximumTimeInterval;
            RandomizeTimeInterval = parameters.RandomizeTimeInterval;
            UpdateWhenOffscreen = parameters.UpdateWhenOffscreen;
        }
#endif

    }

    public struct WaterfallRipplesModuleParameters
    {
        public bool IsActive;
        public bool UpdateWhenOffscreen;
        public bool RandomizeTimeInterval;
        public float TimeInterval;
        public float MinimumTimeInterval;
        public float MaximumTimeInterval;
        public List<WaterfallAffectedWaterObjet> AffectedWaterObjets;
    }

    [System.Serializable]
    public class WaterfallAffectedWaterObjet
    {
        public Game2DWater waterObject;
        public float minimumDisturbance = 0.1f;
        public float maximumDisturbance = 0.75f;
        public bool smoothRipples = true;
        [Range(0f, 1f)] public float smoothingFactor = 1f;
        [Range(0f, 1f)] public float spread = 0.5f;
    }
}