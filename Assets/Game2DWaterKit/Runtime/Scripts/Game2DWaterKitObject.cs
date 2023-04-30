namespace Game2DWaterKit
{
    using Game2DWaterKit.Rendering.Mask;
    using UnityEngine;
    using UnityEngine.Serialization;
    using System.Collections.Generic;

    public abstract class Game2DWaterKitObject : MonoBehaviour
    {
        /// <summary>
        /// The scale at which time passes for all water and waterfalls animations and physics simulations.
        /// Water and waterfalls animations and physics simulations are still affected by Unity Time.timeScale
        /// </summary>
        public static float TimeScale = 1f;

        [FormerlySerializedAs("waterSize"), SerializeField] protected Vector2 _size = Vector2.one;
        [SerializeField] protected List<MeshMask.ControlPoint> _meshMaskControlPoints = new List<MeshMask.ControlPoint>()
        {
            new MeshMask.ControlPoint(new Vector2(-0.5f, 0.5f), new Vector2(-0.5f, 0.25f), new Vector2(-0.25f, 0.5f)),
            new MeshMask.ControlPoint(new Vector2(0.5f, 0.5f), new Vector2(0.25f, 0.5f), new Vector2(0.5f, 0.25f)),
            new MeshMask.ControlPoint(new Vector2(0.5f, -0.5f), new Vector2(0.5f, -0.25f), new Vector2(0.25f, -0.5f)),
            new MeshMask.ControlPoint(new Vector2(-0.5f, -0.5f), new Vector2(-0.25f, -0.5f), new Vector2(-0.5f, -0.25f))
        };
        [SerializeField] protected int _meshMaskSubdivisions = 5;
        [SerializeField] protected bool _meshMaskIsActive = false;
        [SerializeField] protected Vector3 _meshMaskPosition = Vector3.zero;
        [SerializeField] protected Vector3 _meshMaskSize = Vector3.one;
        [SerializeField] protected bool _meshMaskArePositionAndSizeLocked = false;

        [System.NonSerialized] protected bool _areModulesInitialized = false;

        private static int _renderedFrameCount;
#if UNITY_EDITOR
        private static int _activeWaterKitObjectCount;
#endif
        private static List<Game2DWaterKitObject> _aliveWaterKitObjects = new List<Game2DWaterKitObject>();

        internal static event System.Action AllWaterKitObjectsDestroyed;
        internal static event System.Action FrameUpdate;

        public bool IsInitialized { get { return _areModulesInitialized; } }

        public abstract void InitializeModules();
        protected abstract void ActivateObjectRendering();
        protected abstract void DeactivateObjectRendering();
        protected abstract void Cleanup();
        protected abstract void RegularUpdate();
        protected abstract void PhysicsUpdate();
#if UNITY_EDITOR
        protected abstract void ValidateProperties();
        protected abstract void ResetProperties();
#endif

        private void OnEnable()
        {
            InitializeModules();
            ActivateObjectRendering();

#if UNITY_EDITOR
            _activeWaterKitObjectCount++;

            if (!Application.isPlaying && _activeWaterKitObjectCount == 1)
                UnityEditor.EditorApplication.update += OnFrameUpdate;
#endif

            if (!_aliveWaterKitObjects.Contains(this))
                _aliveWaterKitObjects.Add(this);
        }

        private void OnDisable()
        {
            DeactivateObjectRendering();

#if UNITY_EDITOR
            _activeWaterKitObjectCount--;

            if (!Application.isPlaying && _activeWaterKitObjectCount == 0)
                UnityEditor.EditorApplication.update -= OnFrameUpdate;

            // We need to cleanup on scripts reload
            // Unfortunately, OnDestroy is not called on scripts reload, so we call it ourselves
            if (!Application.isPlaying)
                OnDestroy();
#endif
        }

        private void LateUpdate()
        {
            RegularUpdate();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            if (_renderedFrameCount != Time.renderedFrameCount)
            {
                OnFrameUpdate();

                _renderedFrameCount = Time.renderedFrameCount;
            }
        }

        private void FixedUpdate()
        {
            PhysicsUpdate();
        }

        private void OnDestroy()
        {
            _aliveWaterKitObjects.Remove(this);

            if (_aliveWaterKitObjects.Count == 0 && AllWaterKitObjectsDestroyed != null)
                AllWaterKitObjectsDestroyed.Invoke();

            Cleanup();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateProperties();
        }

        private void Reset()
        {
            _meshMaskControlPoints = new List<MeshMask.ControlPoint>()
            {
                new MeshMask.ControlPoint(new Vector2(-0.5f, 0.5f), new Vector2(-0.5f, 0.25f), new Vector2(-0.25f, 0.5f)),
                new MeshMask.ControlPoint(new Vector2(0.5f, 0.5f), new Vector2(0.25f, 0.5f), new Vector2(0.5f, 0.25f)),
                new MeshMask.ControlPoint(new Vector2(0.5f, -0.5f), new Vector2(0.5f, -0.25f), new Vector2(0.25f, -0.5f)),
                new MeshMask.ControlPoint(new Vector2(-0.5f, -0.5f), new Vector2(-0.25f, -0.5f), new Vector2(-0.5f, -0.25f))
            };
            _meshMaskSubdivisions = 5;
            _meshMaskIsActive = false;
            _meshMaskPosition = Vector3.zero;
            _meshMaskSize = Vector3.one;
            _meshMaskArePositionAndSizeLocked = false;
            
            ResetProperties();
        }
#endif

        private static void OnFrameUpdate()
        {
            if (FrameUpdate != null)
                FrameUpdate.Invoke();
        }
    }
}