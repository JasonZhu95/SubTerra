namespace Game2DWaterKit.Main
{
    using UnityEngine;

    public class WaterfallMainModule : MainModule
    {
        private Game2DWaterfall _waterfallObject;

        public WaterfallMainModule(Game2DWaterfall waterfallObject, Vector2 waterfallSize)
        {
            _waterfallObject = waterfallObject;
            _transform = waterfallObject.transform;
            _size = waterfallSize;
        }

        #region Properties

        public Vector2 WaterfallSize { get { return _size; } }

        #endregion

        internal void Initialize()
        {
            _materialModule = _waterfallObject.MaterialModule;
            _meshModule = _waterfallObject.MeshModule;
            _meshMask = _waterfallObject.RenderingModule.MeshMask;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                IsVisible = true;
            }
#endif
            UpdateCachedTransformInformation();

            _gameobjectLayer = _transform.gameObject.layer;
        }
    }
}
