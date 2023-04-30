namespace Game2DWaterKit.Demo
{
    using UnityEngine;
    using UnityEngine.Events;

    public class WorldEntity : MonoBehaviour
    {
        private float _width;

        public bool useRendererBoundsWidth;
        public float width;
        public UnityEvent onRespawn;

        private void Awake()
        {
            _width = useRendererBoundsWidth ? GetComponent<Renderer>().bounds.size.x : width;
        }

        public float Width { get { return _width; } }
    }
}
