namespace Game2DWaterKit
{
    using UnityEngine;
    using UnityEditor;

    public abstract partial class Game2DWaterKitInspector : Editor
    {

        private static class Game2DWaterKitObjectResizeTool
        {
            public static System.Action RepaintInspector;

            private static bool _hasPendingChanges;
            private static bool _isCurrentActiveTransformValid;
            private static Transform _activeTransform;
            private static SerializedObject _currentActiveSerializedObject;

            public static bool IsResizing { get; private set; }

            public static void WatchForObjectSizeChanges(SceneView sceneView)
            {
                var currentActiveTransform = Selection.activeTransform;

                if (currentActiveTransform == null)
                    return;

                if (currentActiveTransform != _activeTransform)
                {
                    _activeTransform = currentActiveTransform;
                    _isCurrentActiveTransformValid = currentActiveTransform.GetComponent<Game2DWaterKitObject>() != null;

                    if (_isCurrentActiveTransformValid)
                        _currentActiveSerializedObject = new SerializedObject(currentActiveTransform.GetComponent<Game2DWaterKitObject>());
                }

                if (_isCurrentActiveTransformValid)
                {
                    var currentEvent = Event.current;

                    if (currentEvent == null || !currentEvent.isMouse || currentEvent.type == EventType.MouseMove)
                        return;

                    bool isScalingTool = Tools.current != Tool.Move && Tools.current != Tool.Rotate && Tools.current != Tool.View;

                    if (isScalingTool && currentActiveTransform.localScale != Vector3.one)
                    {
                        bool isMouseDragEvent = currentEvent.type == EventType.MouseDrag;

                        _hasPendingChanges |= isMouseDragEvent;

                        if (_hasPendingChanges && !isMouseDragEvent)
                        {
                            _currentActiveSerializedObject.Update();
                            var sizeProperty = _currentActiveSerializedObject.FindProperty("_size");
                            sizeProperty.vector2Value = Vector2.Scale(sizeProperty.vector2Value, currentActiveTransform.localScale);
                            _currentActiveSerializedObject.ApplyModifiedProperties();
                            Undo.RecordObject(_currentActiveSerializedObject.targetObject, "setting new size");
                            currentActiveTransform.localScale = Vector3.one;

                            _hasPendingChanges = false;
                        }

                        if (isMouseDragEvent && RepaintInspector != null)
                            RepaintInspector.Invoke();

                        IsResizing = isMouseDragEvent;
                    }
                }
            }
        }

    }

}