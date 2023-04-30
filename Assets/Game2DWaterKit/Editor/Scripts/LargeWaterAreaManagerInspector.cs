namespace Game2DWaterKit
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(LargeWaterAreaManager)),CanEditMultipleObjects]
    public class LargeWaterAreaManagerInspector : Editor
    {
        private SerializedProperty _mainCamera;
        private SerializedProperty _waterObject;
        private SerializedProperty _waterObjectCount;
        private SerializedProperty _isConstrained;
        private SerializedProperty _constrainedRegionXMin;
        private SerializedProperty _constrainedRegionXMax;

        private static readonly GUIContent _mainCameraLabel = new GUIContent("Main Camera" , "Sets the main camera that will be used to determine the visibility of the water object. So when a water object goes invisible to this camera, it gets respawned.");
        private static readonly GUIContent _waterObjectLabel = new GUIContent("Water Object","Set the water object. Please make sure that the water object width is at least half of the Main Camera viewing frustum width.");
        private static readonly GUIContent _waterObjectCountLabel = new GUIContent("Water Object Count","Sets the number of water objects to spawn when the game starts.");
        private static readonly GUIContent _isConstrainedLabel = new GUIContent("Limit To A Specific Region", "Controls whether to limit the water to a certain region instead of having it act like endless water?");
        private static readonly GUIContent _constrainedRegionXMinLabel = new GUIContent("X-Min","The left coordinate of the region to limit the water to. (world space x-coordinate)");
        private static readonly GUIContent _constrainedRegionXMaxLabel = new GUIContent("X-Max","The right coordinate of the region to limit the water to. (world space x-coordinate)");

        private static readonly Color _constrainedRegionOutlineColor = Color.green;
        private static readonly Color _constrainedRegionRectFaceColor = GetSemiTransparentColor(Color.green, 0.05f);

        private void OnEnable()
        {
            _mainCamera = serializedObject.FindProperty("_mainCamera");
            _waterObject = serializedObject.FindProperty("_waterObject");
            _waterObjectCount = serializedObject.FindProperty("_waterObjectCount");
            _isConstrained = serializedObject.FindProperty("_isConstrained");
            _constrainedRegionXMin = serializedObject.FindProperty("_constrainedRegionXMin");
            _constrainedRegionXMax = serializedObject.FindProperty("_constrainedRegionXMax");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_mainCamera,_mainCameraLabel);
            EditorGUILayout.PropertyField(_waterObject,_waterObjectLabel);
            EditorGUILayout.PropertyField(_waterObjectCount,_waterObjectCountLabel);

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            bool isConstrained = EditorGUILayout.ToggleLeft(_isConstrainedLabel, _isConstrained.boolValue);
            if (EditorGUI.EndChangeCheck())
                _isConstrained.boolValue = isConstrained;

            if (isConstrained)
            {
                EditorGUILayout.PropertyField(_constrainedRegionXMin, _constrainedRegionXMinLabel);
                EditorGUILayout.PropertyField(_constrainedRegionXMax, _constrainedRegionXMaxLabel);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            var largeWaterArea = target as LargeWaterAreaManager;
            var waterObject = largeWaterArea.WaterObject;

            if (waterObject != null && largeWaterArea.IsConstrained)
            {
                float xMin = largeWaterArea.ConstrainedRegionXMin;
                float xMax = largeWaterArea.ConstrainedRegionXMax;
                float height = waterObject.MainModule.Height;
                float yMin = waterObject.MainModule.Position.y - height * 0.5f;

                Handles.DrawSolidRectangleWithOutline(new Rect(position: new Vector2(xMin, yMin), size: new Vector2(xMax - xMin, height)), _constrainedRegionRectFaceColor, _constrainedRegionOutlineColor);
            }
        }

        private static Color GetSemiTransparentColor(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}
