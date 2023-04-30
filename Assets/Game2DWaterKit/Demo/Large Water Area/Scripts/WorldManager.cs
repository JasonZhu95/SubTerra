namespace Game2DWaterKit.Demo
{
    using UnityEngine;

    public class WorldManager : MonoBehaviour
    {
        private float _lastCameraXPosition;

        public Camera mainCamera;
        public WorldEntities[] entities;

        public Camera Camera { get { return mainCamera ?? Camera.main; } }

        private void Start()
        {
            if (Camera != null)
            {
                _lastCameraXPosition = Camera.transform.position.x;
            }

            InstantiateEntities();
        }

        private void LateUpdate()
        {
            float cameraWidth = (Screen.width / (float)Screen.height) * Camera.orthographicSize * 2f;
            float cameraXPos = Camera.transform.position.x;
            if (Mathf.Approximately(cameraXPos, _lastCameraXPosition))
            {
                return;
            }

            bool isCamMovingLeftToRight = cameraXPos - _lastCameraXPosition > 0f;
            _lastCameraXPosition = cameraXPos;

            for (int i = 0, imax = entities.Length; i < imax; i++)
            {
                entities[i].UpdateEntity(cameraXPos, cameraWidth, isCamMovingLeftToRight);
            }
        }

        private void InstantiateEntities()
        {
            for (int i = 0, imax = entities.Length; i < imax; i++)
            {
                entities[i].InstantiateEntity();
            }
        }

        [System.Serializable]
        public class WorldEntities
        {
            public enum SpawnInterval
            {
                EntityWidth,
                RandomInterval
            }

            public WorldEntity entity;
            public int count;
            public SpawnInterval spawnInterval;
            public float minSpawnInterval;
            public float maxSpawnInterval;

            private WorldEntity[] _entities;
            private float _width;
            private int _leftMostEntityIndex;
            private int _rightMostEntityIndex;

            private float EntitySpawnInterval { get { return spawnInterval == SpawnInterval.EntityWidth ? _width : Random.Range(minSpawnInterval, maxSpawnInterval); } }

            public void InstantiateEntity()
            {
                _width = entity.Width;

                //spawning entities
                _entities = new WorldEntity[count];
                _entities[0] = entity;
                for (int i = 1; i < count; i++)
                {
                    Vector3 spawnPos = entity.transform.position + i * new Vector3(EntitySpawnInterval, 0f, 0f);
                    _entities[i] = Instantiate(entity.gameObject, spawnPos, Quaternion.identity, entity.transform.parent).GetComponent<WorldEntity>();
                }

                _leftMostEntityIndex = 0;
                _rightMostEntityIndex = count - 1;
            }

            public void UpdateEntity(float cameraXPos, float cameraWidth, bool isCameraMovingLeftToRight)
            {
                if (isCameraMovingLeftToRight)
                {
                    WorldEntity leftMostEntity = _entities[_leftMostEntityIndex];
                    if (!IsEntityVisibleToCamera(leftMostEntity.transform.position.x, cameraXPos, cameraWidth))
                    {
                        WorldEntity rightMostEntity = _entities[_rightMostEntityIndex];
                        leftMostEntity.transform.position = rightMostEntity.transform.position + new Vector3(EntitySpawnInterval, 0f, 0f);
                        _leftMostEntityIndex = GetNextIndex(_leftMostEntityIndex);
                        _rightMostEntityIndex = GetNextIndex(_rightMostEntityIndex);

                        InvokeOnRespawnEvent(leftMostEntity);
                    }
                }
                else
                {
                    WorldEntity rightMostEntity = _entities[_rightMostEntityIndex];
                    if (!IsEntityVisibleToCamera(rightMostEntity.transform.position.x, cameraXPos, cameraWidth))
                    {
                        WorldEntity leftMostEntity = _entities[_leftMostEntityIndex];
                        rightMostEntity.transform.position = leftMostEntity.transform.position - new Vector3(EntitySpawnInterval, 0f, 0f);
                        _leftMostEntityIndex = GetPreviousIndex(_leftMostEntityIndex);
                        _rightMostEntityIndex = GetPreviousIndex(_rightMostEntityIndex);

                        InvokeOnRespawnEvent(rightMostEntity);
                    }
                }
            }

            private bool IsEntityVisibleToCamera(float xPos, float cameraXPos, float cameraWidth)
            {
                return Mathf.Abs(xPos - cameraXPos) < (_width + cameraWidth) * 0.5f;
            }

            private void InvokeOnRespawnEvent(WorldEntity entity)
            {
                if (entity.onRespawn != null)
                {
                    entity.onRespawn.Invoke();
                }
            }

            private int GetPreviousIndex(int index)
            {
                return index > 0 ? index - 1 : count - 1;
            }

            private int GetNextIndex(int index)
            {
                return index < count - 1 ? index + 1 : 0;
            }
        }
    }
}