namespace Game2DWaterKit.Demo
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(CameraFollow))]
    public class CameraFollowInspector : Editor
    {
        private CameraFollow targetObject;

        private void OnEnable()
        {
            targetObject = target as CameraFollow;
        }

        private void OnSceneGUI()
        {
            Vector3 cameraPosition = targetObject.transform.position;
            Vector2 smoothFollowWindowExtents = targetObject.smoothFollowWindow;

            Vector3[] smoothFollowWindow = new Vector3[] {
                new Vector3(cameraPosition.x - smoothFollowWindowExtents.x,cameraPosition.y - smoothFollowWindowExtents.y),
                new Vector3(cameraPosition.x - smoothFollowWindowExtents.x,cameraPosition.y + smoothFollowWindowExtents.y),
                new Vector3(cameraPosition.x + smoothFollowWindowExtents.x,cameraPosition.y + smoothFollowWindowExtents.y),
                new Vector3(cameraPosition.x + smoothFollowWindowExtents.x,cameraPosition.y - smoothFollowWindowExtents.y),
                new Vector3(cameraPosition.x - smoothFollowWindowExtents.x,cameraPosition.y - smoothFollowWindowExtents.y)
            };

            using (new Handles.DrawingScope(Color.green))
            {
                Handles.DrawPolyLine(smoothFollowWindow);
            }
        }

    }
}
