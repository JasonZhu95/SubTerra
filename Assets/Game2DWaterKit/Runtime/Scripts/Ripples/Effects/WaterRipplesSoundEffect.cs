namespace Game2DWaterKit.Ripples.Effects
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Audio;

    public class WaterRipplesSoundEffect
    {
        #region Variables
        private readonly Transform _poolRootParent;

        private bool _isActive;
        private AudioClip _audioClip;
        private GameObject _audioSourcePrefab;
        private bool _canExpandPool;
        private int _poolSize;
        private bool _isUsingConstantAudioPitch;
        private float _audioPitch;
        private float _maximumAudioPitch;
        private float _minimumAudioPitch;
        private float _audioVolume;
        private AudioMixerGroup _output;

        private Transform _poolRoot;
        private List<AudioSource> _pool;
        private int _activeAudioSourcesCount;
        private int _firstActiveAudioSourceIndex;
        private int _nextAudioSourceToActivateIndex;
        #endregion

        public WaterRipplesSoundEffect(WaterRipplesSoundEffectParameters parameters, Transform poolParent)
        {
            _isActive = parameters.IsActive;
            _audioClip = parameters.AudioClip;
            _isUsingConstantAudioPitch = parameters.UseConstantAudioPitch;
            _audioPitch = parameters.AudioPitch;
            _minimumAudioPitch = parameters.MinimumAudioPitch;
            _maximumAudioPitch = parameters.MaximumAudioPitch;
            _audioVolume = parameters.AudioVolume;
            _poolSize = parameters.PoolSize;
            _canExpandPool = parameters.CanExpandPool;
            _output = parameters.output;
            _audioSourcePrefab = parameters.audioSourcePrefab;

            _poolRootParent = poolParent;

            if (_isActive)
                CreatePool();
        }

        #region Properties
        public bool IsActive { get { return _isActive; } set { SetActive(value); } }
        private bool IsPoolCreated { get { return _poolRoot != null; } }
        public AudioClip AudioClip { get { return _audioClip; } set { ChangeAudioClip(value); } }
        public float AudioVolume { get { return _audioVolume; } set { ChangeAudioVolume(Mathf.Clamp01(value)); } }
        public bool CanExpandPool { get { return _canExpandPool; } set { _canExpandPool = value; } }
        public int PoolSize { get { return _poolSize; } set { _poolSize = Mathf.Clamp(value, 0, int.MaxValue); } }
        public float AudioPitch { get { return _audioPitch; } set { _audioPitch = Mathf.Clamp(value, -3f, 3f); } }
        public float MaximumAudioPitch { get { return _maximumAudioPitch; } set { _maximumAudioPitch = Mathf.Clamp(value, -3f, 3f); } }
        public float MinimumAudioPitch { get { return _minimumAudioPitch; } set { _minimumAudioPitch = Mathf.Clamp(value, -3f, 3f); } }
        public bool IsUsingConstantAudioPitch { get { return _isUsingConstantAudioPitch; } set { _isUsingConstantAudioPitch = value; } }
        public AudioMixerGroup Output { get { return _output; } set { _output = value; } }
        #endregion

        #region Methods

        internal void Update()
        {
            if (IsPoolCreated && _activeAudioSourcesCount > 0)
            {
                if (_poolSize > _pool.Count)
                    ExpandPool(_poolSize);

                AudioSource firstActiveAudioSource = _pool[_firstActiveAudioSourceIndex];
                if (!firstActiveAudioSource.isPlaying)
                {
                    firstActiveAudioSource.gameObject.SetActive(false);
                    _firstActiveAudioSourceIndex = (_firstActiveAudioSourceIndex + 1 < _pool.Count) ? _firstActiveAudioSourceIndex + 1 : 0;
                    _activeAudioSourcesCount--;
                }
            }
        }

        internal void PlaySoundEffect(Vector3 position, float disturbanceFactor)
        {
            if (!_isActive || !IsPoolCreated)
                return;

            if (_activeAudioSourcesCount == _poolSize)
            {
                if (!_canExpandPool)
                    return;

                _nextAudioSourceToActivateIndex = _poolSize;
                ExpandPool(newPoolSize: _poolSize * 2);
            }

            AudioSource newlyActivatedAudioSource = _pool[_nextAudioSourceToActivateIndex];
            newlyActivatedAudioSource.transform.position = position;
            newlyActivatedAudioSource.gameObject.SetActive(true);
            newlyActivatedAudioSource.pitch = _isUsingConstantAudioPitch ? _audioPitch : Mathf.Lerp(_minimumAudioPitch, _maximumAudioPitch, 1f - disturbanceFactor);
            newlyActivatedAudioSource.outputAudioMixerGroup = newlyActivatedAudioSource.outputAudioMixerGroup ?? _output;
            newlyActivatedAudioSource.Play();

            _activeAudioSourcesCount++;
            _nextAudioSourceToActivateIndex = (_nextAudioSourceToActivateIndex + 1 < _pool.Count) ? _nextAudioSourceToActivateIndex + 1 : 0;
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

            if (!_isActive || IsPoolCreated || _audioClip == null || _poolSize < 1)
                return;

            _poolRoot = new GameObject("Sound Effects").transform;
            _poolRoot.parent = _poolRootParent;

            _pool = new List<AudioSource>(_poolSize);
            for (int i = 0; i < _poolSize; i++)
            {
                _pool.Add(CreateNewAudioSource());
            }
        }

        private void DestroyPool()
        {
            if (!IsPoolCreated)
                return;

            GameObject.Destroy(_poolRoot.gameObject);
            _poolRoot = null;
            _pool = null;

            _firstActiveAudioSourceIndex = 0;
            _nextAudioSourceToActivateIndex = 0;
            _activeAudioSourcesCount = 0;
        }

        private void ExpandPool(int newPoolSize)
        {
            if (!IsPoolCreated)
                return;

            _poolSize = newPoolSize;

            for (int i = _pool.Count, imax = newPoolSize; i < imax; i++)
            {
                _pool.Add(CreateNewAudioSource());
            }
        }

        private void ChangeAudioClip(AudioClip audioClip)
        {
            if (_audioClip == audioClip)
                return;

            _audioClip = audioClip;

            if (IsPoolCreated)
            {
                for (int i = 0; i < _poolSize; i++)
                {
                    _pool[i].clip = _audioClip;
                }
            }
            else if (_isActive)
                CreatePool();
        }

        private void ChangeAudioVolume(float audioVolume)
        {
            if (_audioVolume == audioVolume)
                return;

            _audioVolume = audioVolume;

            if (IsPoolCreated)
            {
                for (int i = 0; i < _poolSize; i++)
                {
                    _pool[i].volume = _audioVolume;
                }
            }
            else if (_isActive)
                CreatePool();
        }

        private AudioSource CreateNewAudioSource()
        {
            GameObject audioSourceGameObject;
            if (_audioSourcePrefab != null)
                audioSourceGameObject = GameObject.Instantiate(_audioSourcePrefab);
            else
                audioSourceGameObject = new GameObject("Sound Effect", typeof(AudioSource));

            audioSourceGameObject.transform.parent = _poolRoot;
            audioSourceGameObject.SetActive(false);

            AudioSource audioSource = audioSourceGameObject.GetComponent<AudioSource>();
            audioSource.clip = _audioClip;
            audioSource.volume = _audioVolume;
            return audioSource;
        }

        #endregion

        #region Editor Only Methods
        #if UNITY_EDITOR
        public void Validate(WaterRipplesSoundEffectParameters parameters)
        {
            PoolSize = parameters.PoolSize;
            CanExpandPool = parameters.CanExpandPool;
            IsActive = parameters.IsActive;
            AudioClip = parameters.AudioClip;
            AudioVolume = parameters.AudioVolume;
            Output = parameters.output;
            IsUsingConstantAudioPitch = parameters.UseConstantAudioPitch;
            AudioPitch = parameters.AudioPitch;
            MinimumAudioPitch = parameters.MinimumAudioPitch;
            MaximumAudioPitch = parameters.MaximumAudioPitch;
            _audioSourcePrefab = parameters.audioSourcePrefab;
        }
        #endif
        #endregion
    }

    public struct WaterRipplesSoundEffectParameters
    {
        public bool IsActive;
        public AudioClip AudioClip;
        public float AudioPitch;
        public bool UseConstantAudioPitch;
        public float MaximumAudioPitch;
        public float MinimumAudioPitch;
        public float AudioVolume;
        public bool CanExpandPool;
        public int PoolSize;
        public AudioMixerGroup output;
        public GameObject audioSourcePrefab;
    }

}
