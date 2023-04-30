namespace Game2DWaterKit.Ripples.Effects
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;

    public class WaterRipplesParticleEffect
    {
        #region Variables
        private readonly Transform _poolRootParent;

        private bool _isActive;
        private ParticleSystem _particleSystem;
        private bool _canExpandPool;
        private int _poolSize;
        private Vector3 _spawnOffset;
        private UnityEvent _stopAction;

        private Transform _poolRoot;
        private List<ParticleSystem> _pool;
        private int _activeParticleSystemsCount;
        private int _firstActiveParticleSystemIndex;
        private int _nextParticleSystemToActivateIndex;
        #endregion

        public WaterRipplesParticleEffect(WaterRipplesParticleEffectParameters parameters, Transform poolParent)
        {
            _isActive = parameters.IsActive;
            _particleSystem = parameters.ParticleSystem;
            _spawnOffset = parameters.SpawnOffset;
            _stopAction = parameters.StopAction;
            _poolSize = parameters.PoolSize;
            _canExpandPool = parameters.CanExpandPool;

            _poolRootParent = poolParent;

            if (_isActive)
                CreatePool();
        }

        #region Properties
        public bool IsActive { get { return _isActive; } set { SetActive(value); } }
        public bool IsPoolCreated { get { return _poolRoot != null; } }
        public ParticleSystem ParticleSystem { get { return _particleSystem; } set { ChangeParticleSystem(value); } }
        public bool CanExpandPool { get { return _canExpandPool; } set { _canExpandPool = value; } }
        public int PoolSize { get { return _poolSize; } set { _poolSize = Mathf.Clamp(value, 0, int.MaxValue); } }
        public Vector3 SpawnOffset { get { return _spawnOffset; } set { _spawnOffset = value; } }
        public UnityEvent StopAction { get { return _stopAction; } set { _stopAction = value; } }
        #endregion

        #region Methods

        internal void Update()
        {
            if (IsPoolCreated && _activeParticleSystemsCount > 0)
            {
                if (_poolSize > _pool.Count)
                    ExpandPool(_poolSize);

                ParticleSystem firstActiveAudioSource = _pool[_firstActiveParticleSystemIndex];
                if (!firstActiveAudioSource.IsAlive(true))
                {
                    if (_stopAction != null)
                        _stopAction.Invoke();

                    firstActiveAudioSource.gameObject.SetActive(false);
                    _firstActiveParticleSystemIndex = (_firstActiveParticleSystemIndex + 1 < _pool.Count) ? _firstActiveParticleSystemIndex + 1 : 0;
                    _activeParticleSystemsCount--;
                }
            }
        }

        internal void PlayParticleEffect(Vector3 position)
        {
            if (!_isActive || !IsPoolCreated)
                return;

            if (_activeParticleSystemsCount == _poolSize)
            {
                if (!_canExpandPool)
                    return;

                _nextParticleSystemToActivateIndex = _poolSize;
                ExpandPool(newPoolSize: _poolSize * 2);
            }

            ParticleSystem newlyActivatedParticleSystem = _pool[_nextParticleSystemToActivateIndex];
            newlyActivatedParticleSystem.transform.position = position + _spawnOffset;
            newlyActivatedParticleSystem.gameObject.SetActive(true);
            newlyActivatedParticleSystem.Play(true);

            _activeParticleSystemsCount++;
            _nextParticleSystemToActivateIndex = (_nextParticleSystemToActivateIndex + 1 < _pool.Count) ? _nextParticleSystemToActivateIndex + 1 : 0;
        }

        private void SetActive(bool isActive)
        {
            if (_isActive == isActive)
                return;

            _isActive = isActive;

            if (IsPoolCreated)
                _poolRoot.gameObject.SetActive(_isActive);
            else if (_isActive)
                CreatePool();
        }

        private void CreatePool()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            if (!_isActive || IsPoolCreated || _particleSystem == null || _poolSize < 1)
                return;

            _poolRoot = new GameObject("Particle Effects Pool").transform;
            _poolRoot.parent = _poolRootParent;

            _pool = new List<ParticleSystem>(_poolSize);
            for (int i = 0; i < _poolSize; i++)
            {
                _pool.Add(CreateNewParticleSystem());
            }
        }

        private void DestroyPool()
        {
            if (!IsPoolCreated)
                return;

            GameObject.Destroy(_poolRoot.gameObject);
            _poolRoot = null;
            _pool = null;

            _firstActiveParticleSystemIndex = 0;
            _nextParticleSystemToActivateIndex = 0;
            _activeParticleSystemsCount = 0;
        }

        private void ExpandPool(int newPoolSize)
        {
            if (!IsPoolCreated)
                return;

            _poolSize = newPoolSize;

            for (int i = _pool.Count, imax = newPoolSize; i < imax; i++)
            {
                _pool.Add(CreateNewParticleSystem());
            }
        }

        private void ChangeParticleSystem(ParticleSystem particleSystem)
        {
            if (_particleSystem == particleSystem)
                return;

            _particleSystem = particleSystem;

            DestroyPool();
            CreatePool();
        }

        private ParticleSystem CreateNewParticleSystem()
        {
            GameObject particleSystemGameObject = _particleSystem.gameObject;
            Vector3 position = Vector3.zero;
            Quaternion rotation = particleSystemGameObject.transform.rotation;
            GameObject particleSystemInstanceGameObject = GameObject.Instantiate(particleSystemGameObject, position, rotation, _poolRoot);
            particleSystemInstanceGameObject.SetActive(false);

            return particleSystemInstanceGameObject.GetComponent<ParticleSystem>();
        }

        #endregion

        #region Editor Only Methods
        #if UNITY_EDITOR
        internal void Validate(WaterRipplesParticleEffectParameters parameters)
        {
            SpawnOffset = parameters.SpawnOffset;
            StopAction = parameters.StopAction;
            CanExpandPool = parameters.CanExpandPool;
            ParticleSystem = parameters.ParticleSystem;
            PoolSize = parameters.PoolSize;
            IsActive = parameters.IsActive;
        }
        #endif
        #endregion
    }

    public struct WaterRipplesParticleEffectParameters
    {
        public bool IsActive;
        public bool CanExpandPool;
        public ParticleSystem ParticleSystem;
        public int PoolSize;
        public Vector3 SpawnOffset;
        public UnityEvent StopAction;
    }
}
