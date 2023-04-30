namespace Game2DWaterKit.AttachedComponents
{
    using UnityEngine;

    public class WaterfallAttachedComponentsModule
    {
        private Game2DWaterfall _waterfallObject;

        public WaterfallAttachedComponentsModule(Game2DWaterfall waterfallObject)
        {
            _waterfallObject = waterfallObject;
        }

        #region Properties
        internal bool HasAnimatorAttached { get; private set; }
        #endregion

        #region Methods

        internal void Initialize()
        {
            HasAnimatorAttached = _waterfallObject.MainModule.Transform.GetComponent<Animator>() != null;
        }

#if UNITY_EDITOR

        internal void Validate()
        {
            HasAnimatorAttached = _waterfallObject.MainModule.Transform.GetComponent<Animator>() != null;
        }

#endif

        #endregion
    }
}
