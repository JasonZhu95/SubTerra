namespace Game2DWaterKit.Demo
{
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        private Camera _camera;
        private float _horizontalVelocity;
        private float _verticalVelocity;
        private Vector3 _playerLastFramePosition;

        public Transform player;
        public float smoothTime;
        public Vector2 smoothFollowWindow;
        public float yMin;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            _playerLastFramePosition = player.position;
        }
        private void LateUpdate()
        {
            if (player != null)
            {
                Vector3 playerPosition = player.transform.position;
                Vector3 playerDeltaPosition = playerPosition - _playerLastFramePosition;
                _playerLastFramePosition = player.position;

                Vector3 cameraPosition = _camera.transform.position;

                //x pos
                if (Mathf.Abs(cameraPosition.x - playerPosition.x) > smoothFollowWindow.x)
                    cameraPosition.x = _camera.transform.position.x + playerDeltaPosition.x;
                else
                    cameraPosition.x = Mathf.SmoothDamp(_camera.transform.position.x, player.position.x, ref _horizontalVelocity, smoothTime);
                
                //y pos
                if (Mathf.Abs(cameraPosition.y - playerPosition.y) > smoothFollowWindow.y)
                    cameraPosition.y = _camera.transform.position.y + playerDeltaPosition.y;
                else
                    cameraPosition.y = Mathf.SmoothDamp(_camera.transform.position.y, player.position.y, ref _verticalVelocity, smoothTime);

                if (cameraPosition.y < yMin)
                    cameraPosition.y = yMin;

                _camera.transform.position = cameraPosition;
            }
        }
    }
}
