namespace Game2DWaterKit.Demo
{
    using UnityEngine;

    public class ObjectsManager : MonoBehaviour
    {
        public GameObject[] objects;
        public Vector3 spawnPosition;
        public float destroyXPosition;

        private GameObject currentObject;

        private void Start()
        {
            currentObject = SpawnRandomObject();
        }

        private void Update()
        {
            if(currentObject != null && currentObject.transform.position.x < destroyXPosition)
            {
                currentObject.SetActive(false);
                currentObject = SpawnRandomObject();
            }
        }

        private GameObject SpawnRandomObject()
        {
            if (objects != null && objects.Length > 0)
            {
                var go = objects[Random.Range(0, objects.Length)];
                go.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
                go.SetActive(true);

                return go;
            }

            return null;
        }
    }
}