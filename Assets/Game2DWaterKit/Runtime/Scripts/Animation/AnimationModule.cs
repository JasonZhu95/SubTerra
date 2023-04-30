namespace Game2DWaterKit.Animation
{
    using Game2DWaterKit.Main;
    using UnityEngine;

    public abstract class AnimationModule
    {
        protected MainModule _mainModule;
        protected Vector2 _lastSize;

        private bool _isAnimatingSize;
        private Vector2 _animationTargetSize;
        private Vector2 _animationInitialSize;
        private WaterAnimationConstraint _animationConstraints;
        private WaterAnimationWrapMode _animationWrapMode;
        private float _animationDuration;
        private float _animationElapsedTime;

        public bool IsAnimating { get { return _isAnimatingSize; } }

        #region Methods

        protected void AnimateSize(Vector2 targetSize, float duration, WaterAnimationConstraint constraint, WaterAnimationWrapMode wrapMode = WaterAnimationWrapMode.Once)
        {
            if (targetSize.x <= 0f)
                targetSize.x = 0.001f;
            if (targetSize.y <= 0f)
                targetSize.y = 0.001f;

            _animationTargetSize = targetSize;
            _animationInitialSize = new Vector2(_mainModule.Width, _mainModule.Height);
            _animationDuration = Mathf.Clamp(duration, 0.001f, float.MaxValue);
            _animationConstraints = constraint;
            _animationWrapMode = wrapMode;
            _animationElapsedTime = 0f;
            _isAnimatingSize = true;
        }

        public void StopAnimation()
        {
            _isAnimatingSize = false;
        }

        internal virtual void Update()
        {
            Vector2 size = new Vector2(_mainModule.Width, _mainModule.Height);

            if (_isAnimatingSize)
            {
                _animationElapsedTime += Time.deltaTime * Game2DWaterKitObject.TimeScale;
                float t = _animationElapsedTime / _animationDuration;
                size.x = Mathf.SmoothStep(_animationInitialSize.x, _animationTargetSize.x, t);
                size.y = Mathf.SmoothStep(_animationInitialSize.y, _animationTargetSize.y, t);

                const float threshold = 0.005f;
                _isAnimatingSize = (Mathf.Abs(size.y - _animationTargetSize.y) > threshold) || (Mathf.Abs(size.x - _animationTargetSize.x) > threshold);

                if (!_isAnimatingSize)
                {
                    switch (_animationWrapMode)
                    {
                        case WaterAnimationWrapMode.Once:
                            size = _animationTargetSize;
                            break;
                        case WaterAnimationWrapMode.Loop:
                            _isAnimatingSize = true;
                            size = _animationInitialSize;
                            break;
                        case WaterAnimationWrapMode.PingPong:
                            _isAnimatingSize = true;
                            size = _animationTargetSize;
                            _animationTargetSize = _animationInitialSize;
                            _animationInitialSize = size;
                            break;
                    }

                    _animationElapsedTime = 0f;
                }

                ApplyAnimationConstraints(size);
            }

            if (size != _lastSize)
                UpdateSize(size);
        }

        private void ApplyAnimationConstraints(Vector2 newSize)
        {
            float xFactor = 0f;
            float yFactor = 0f;

            bool hasBottomConstraint = HasConstraintDefined(WaterAnimationConstraint.Bottom);
            bool hasVerticalConstraint = hasBottomConstraint || HasConstraintDefined(WaterAnimationConstraint.Top);
            if (hasVerticalConstraint)
            {
                yFactor = hasBottomConstraint ? 1f : -1f;
            }

            bool hasLeftConstraint = HasConstraintDefined(WaterAnimationConstraint.Left);
            bool hasHorizontalConstraint = hasLeftConstraint || HasConstraintDefined(WaterAnimationConstraint.Right);
            if (hasHorizontalConstraint)
            {
                xFactor = hasLeftConstraint ? 1f : -1f;
            }

            // Calculating new water/waterfall position = currentPosition ± deltaChange * 0.5f
            // we're working here in local space so the current water/waterfall position is always equal to (0,0)
            // the newPosition = ± deltaChange * 0.5f
            // ± : the sign depends on the defined constraints
            float x = ((newSize.x - _lastSize.x) * 0.5f) * xFactor;
            float y = ((newSize.y - _lastSize.y) * 0.5f) * yFactor;

            _mainModule.Position = _mainModule.Transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(x, y));
        }

        private bool HasConstraintDefined(WaterAnimationConstraint constraint)
        {
            return (_animationConstraints & constraint) == constraint;
        }

        protected abstract void UpdateSize(Vector2 newWaterSize);

        #endregion
    }

    [System.Flags]
    public enum WaterAnimationConstraint
    {
        None = 0,
        Top = 1 << 0,
        Bottom = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        TopLeft = Top | Left,
        TopRight = Top | Right,
        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right
    }

    public enum WaterAnimationWrapMode
    {
        Once,
        Loop,
        PingPong
    }
}