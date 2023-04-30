namespace Game2DWaterKit.Demo
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D),typeof(CircleCollider2D),typeof(Animator))]
    public class Player : MonoBehaviour
    {
        public float onLandSpeed;
        public Vector2 underwaterSpeed;
        public float jumpHeight;
        public float jumpDistance;
        public float waterSurfaceLevel;
        public ParticleSystem airBubblesEffect;

        [Header("Animations")]
        public float eyeBlinkMinInterval;
        public float eyeBlinkMaxInterval;

        [HideInInspector] public PlayerUnderwaterEvent OnPlayerUnderwater;

        #region Private Variables

        private float _jumpInitialVerticalVelocity;
        private bool _jump;
        private float _horizontalMove;
        private float _verticalMove;
        private Rigidbody2D _playerRigidBody;
        private int _jumpsCount;
        private float _currentHorizontalMovementVelocity;
        private float _currentVerticalMovementVelocity;
        private float _nextEyeBlink;

        private Animator _animator;
        private int _walkAnimHash = Animator.StringToHash("speed");
        private int _jumpAnimHash = Animator.StringToHash("jump");
        private int _swimAnimHash = Animator.StringToHash("swim");
        private int _eyeBlinkAnimHash = Animator.StringToHash("eyeBlink");

        private bool _isPlayerUnderWater;

        private float _playerCircleColliderRadius;

        #endregion

        private void Awake()
        {
            _playerRigidBody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();

            _playerCircleColliderRadius = GetComponent<CircleCollider2D>().radius;

            ComputePhysicsParameters();
        }

        private void Update()
        {
            bool isPlayerUnderwater = (transform.position.y - waterSurfaceLevel) < _playerCircleColliderRadius;

            _horizontalMove = Input.GetAxisRaw("Horizontal") * (isPlayerUnderwater ? underwaterSpeed.x : onLandSpeed);
            _animator.SetFloat(_walkAnimHash, Mathf.Abs(_horizontalMove) > 0f ? 1f : 0f);

            _verticalMove = isPlayerUnderwater ? Input.GetAxisRaw("Vertical") * underwaterSpeed.y : 0f;

            if (!isPlayerUnderwater && Input.GetButtonDown("Jump"))
            {
                _jump = true;
                _animator.SetTrigger(_jumpAnimHash);
            }

            if (_isPlayerUnderWater != isPlayerUnderwater)
            {
                _isPlayerUnderWater = isPlayerUnderwater;

                if (isPlayerUnderwater)
                    airBubblesEffect.Play();
                else
                    airBubblesEffect.Stop();

                _animator.SetBool(_swimAnimHash, isPlayerUnderwater);

                if (OnPlayerUnderwater != null)
                    OnPlayerUnderwater.Invoke(_isPlayerUnderWater);
            }
            
            if(Time.time > _nextEyeBlink)
            {
                _animator.SetTrigger(_eyeBlinkAnimHash);
                _nextEyeBlink = Time.time + Random.Range(eyeBlinkMinInterval, eyeBlinkMaxInterval);
            }

            CheckPlayerSpriteOrientation();
        }

        private void FixedUpdate()
        {
            Vector2 playerVelocity = _playerRigidBody.velocity;

            playerVelocity.x = _horizontalMove;
            playerVelocity.y += _verticalMove;

            if (_jump)
            {
                playerVelocity.y = _jumpInitialVerticalVelocity;
                _jump = false;
            }

            _playerRigidBody.velocity = playerVelocity;
        }

        private void CheckPlayerSpriteOrientation()
        {
            bool isPlayerMovingLeft = _horizontalMove < 0f;
            bool isPlayerFacingLeft = transform.localScale.x < 0f;

            if (_horizontalMove != 0f && (isPlayerFacingLeft != isPlayerMovingLeft))
                transform.localScale = isPlayerMovingLeft ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
        }

        private void ComputePhysicsParameters()
        {
            float halfJumpDistance = jumpDistance * 0.5f;
            _jumpInitialVerticalVelocity = 2f * jumpHeight * onLandSpeed / halfJumpDistance;
            float gravity = -_jumpInitialVerticalVelocity * onLandSpeed / halfJumpDistance;
            if(_playerRigidBody != null)
                _playerRigidBody.gravityScale = gravity / Physics2D.gravity.y;
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
           ComputePhysicsParameters();
        }
        #endif

        [System.Serializable]
        public class PlayerUnderwaterEvent : UnityEngine.Events.UnityEvent<bool> { }
    }
}
