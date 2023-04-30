namespace Game2DWaterKit
{
    using UnityEngine;
    using UnityEditor;

    public abstract partial class Game2DWaterKitInspector : Editor
    {
        protected static class Game2DWaterKitSimulationPreviewMode
        {
            public static System.Action IterateSimulation;
            public static System.Action RestartSimulation;

            public static float TimeStep { get; set; }
            public static bool IsRunning { get; private set; }
            public static float RelativeAnimationSpeed { get; private set; }

            private static float _previousTime;
            private static float _elapsedTimeSinceLastIteration;
            private static float _elapsedTime;
            private static int _simulatedFrames;

            static Game2DWaterKitSimulationPreviewMode()
            {
                TimeStep = 1 / 30f;
            }

            public static void Start()
            {
                if (IterateSimulation != null && RestartSimulation != null)
                {
                    EditorApplication.update -= Update;
                    EditorApplication.update += Update;

                    _previousTime = Time.realtimeSinceStartup;
                    _elapsedTimeSinceLastIteration = 0f;

                    IsRunning = true;
                }
            }

            public static void Stop()
            {
                EditorApplication.update -= Update;
                if (RestartSimulation != null)
                    RestartSimulation();
                IsRunning = false;
            }

            public static void Restart()
            {
                if (IterateSimulation != null && RestartSimulation != null)
                {
                    if (!IsRunning)
                        Start();

                    RestartSimulation();
                }
            }

            private static void Update()
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    EditorApplication.update -= Update;
                    IsRunning = false;
                }

                float currentTime = Time.realtimeSinceStartup;
                float deltaTime = currentTime - _previousTime;

                _elapsedTimeSinceLastIteration += deltaTime;
                _elapsedTime += deltaTime;

                if (_elapsedTimeSinceLastIteration >= TimeStep)
                {
                    _elapsedTimeSinceLastIteration = 0f;
                    IterateSimulation();
                    _simulatedFrames++;
                }

                if (_elapsedTime >= 0.25f) // Report approximate simulation framerate every 0.25 second
                {
                    float simulatedFramesPerSecond = _simulatedFrames / _elapsedTime;
                    RelativeAnimationSpeed = simulatedFramesPerSecond * TimeStep;

                    _elapsedTime = 0f;
                    _simulatedFrames = 0;
                }

                _previousTime = currentTime;
            }
        }

    }
}