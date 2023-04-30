namespace Game2DWaterKit.Animation
{
    using Game2DWaterKit.Mesh;
    using UnityEngine;

    public class WaterfallAnimationModule : AnimationModule
    {
        private Game2DWaterfall _waterfallObject;
        private WaterfallMeshModule _meshModule;

        public WaterfallAnimationModule(Game2DWaterfall waterfallObject)
        {
            _waterfallObject = waterfallObject;
        }

        #region Methods

        public void AnimateWaterfallSize(Vector2 targetSize, float duration, WaterAnimationConstraint constraint, WaterAnimationWrapMode wrapMode = WaterAnimationWrapMode.Once)
        {
            AnimateSize(targetSize, duration, constraint, wrapMode);
        }

        internal void Initialze()
        {
            _mainModule = _waterfallObject.MainModule;
            _meshModule = _waterfallObject.MeshModule;

            _meshModule.OnRecomputeMesh += ResetCachedVariables;
            ResetCachedVariables();
        }

        internal void SyncAnimatableVariables(Vector2 waterSize)
        {
            //Water size could be animated in Unity animation system,
            //so we make sure to reflect any changes to its value
            _mainModule.SetSize(waterSize);
        }

        protected override void UpdateSize(Vector2 newSize)
        {
            var vertices = _meshModule.Vertices;
            var uvs = _meshModule.Uvs;

            var waterfallSize = new Vector2(_mainModule.Width, _mainModule.Height);
            var topBottomEdgesRelativeLength = _meshModule.TopBottomEdgesRelativeLength;

            float topEdgeLength = waterfallSize.x * (topBottomEdgesRelativeLength.y == 1f ? topBottomEdgesRelativeLength.x : 1f);
            float bottomEdgeLength = waterfallSize.x * (topBottomEdgesRelativeLength.y == 0f ? topBottomEdgesRelativeLength.x : 1f);
            float d = (waterfallSize.x - topEdgeLength) * 0.5f;
            float sideEdgesLength = Mathf.Sqrt(waterfallSize.y * waterfallSize.y + d * d);

            vertices[0] = new Vector3(-topEdgeLength * 0.5f, waterfallSize.y * 0.5f);
            vertices[1] = new Vector3(topEdgeLength * 0.5f, waterfallSize.y * 0.5f);
            vertices[2] = new Vector3(bottomEdgeLength * 0.5f, -waterfallSize.y * 0.5f);
            vertices[3] = new Vector3(-bottomEdgeLength * 0.5f, -waterfallSize.y * 0.5f);

            uvs[0] = new Vector4(0f, 1f, topEdgeLength, sideEdgesLength);
            uvs[1] = new Vector4(1f, 1f, topEdgeLength, sideEdgesLength);
            uvs[2] = new Vector4(1f, 0f, bottomEdgeLength, sideEdgesLength);
            uvs[3] = new Vector4(0f, 0f, bottomEdgeLength, sideEdgesLength);

            _mainModule.SetSize(newSize);
            _meshModule.UpdateMeshData();

            _lastSize = newSize;
        }

        private void ResetCachedVariables()
        {
            _lastSize = new Vector2(_mainModule.Width, _mainModule.Height);
        }

        #endregion
    }
}