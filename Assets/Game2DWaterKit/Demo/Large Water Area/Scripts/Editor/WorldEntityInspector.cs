namespace Game2DWaterKit.Demo
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(WorldEntity))]
    public class WorldEntityInspector : Editor
    {
        private WorldEntity targetObject;
        private bool _hasRendererAttached;

        public SerializedProperty _useRendererBoundsWidth;
        public SerializedProperty _width;
        public SerializedProperty _onRespawn;

        private void OnEnable()
        {
            targetObject = target as WorldEntity;

            _hasRendererAttached = targetObject.GetComponent<Renderer>() != null;

            _useRendererBoundsWidth = serializedObject.FindProperty("useRendererBoundsWidth");
            _width = serializedObject.FindProperty("width");
            _onRespawn = serializedObject.FindProperty("onRespawn");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(!_hasRendererAttached);
            EditorGUILayout.PropertyField(_useRendererBoundsWidth);
            EditorGUI.EndDisabledGroup();

            if (!_hasRendererAttached)
                EditorGUILayout.HelpBox("No renderer is attached. Using the specified width as the entity width!",MessageType.Info);

            EditorGUI.BeginDisabledGroup(_hasRendererAttached && _useRendererBoundsWidth.boolValue);
            EditorGUILayout.PropertyField(_width);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(_onRespawn);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
