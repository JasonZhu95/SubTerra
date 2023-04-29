using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomCameraInspectorObjects customCameraInspectorObjects;

    private Collider2D coll;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - coll.bounds.center).normalized;

            if (customCameraInspectorObjects.swapCameras && customCameraInspectorObjects.cameraOnLeft != null && customCameraInspectorObjects.cameraOnRight != null)
            {
                CameraManager.instance.SwapCamera(customCameraInspectorObjects.cameraOnLeft, customCameraInspectorObjects.cameraOnRight, exitDirection);
            }
            if (customCameraInspectorObjects.panCameraOnTrigger)
            {
                CameraManager.instance.PanCameraOnTrigger(customCameraInspectorObjects.panDistance, customCameraInspectorObjects.panTime, customCameraInspectorObjects.panDirection, false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (customCameraInspectorObjects.panCameraOnTrigger)
            {
                CameraManager.instance.PanCameraOnTrigger(customCameraInspectorObjects.panDistance, customCameraInspectorObjects.panTime, customCameraInspectorObjects.panDirection, true);
            }
        }
    }
}

[System.Serializable]
public class CustomCameraInspectorObjects
{
    public bool swapCameras = false;
    public bool panCameraOnTrigger = false;

    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    [HideInInspector] public CameraPanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;

}

[CustomEditor(typeof(CameraControlTrigger))]
public class CustomCameraEditor : Editor
{
    public CameraControlTrigger cameraControlTrigger;

    private void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (cameraControlTrigger.customCameraInspectorObjects.swapCameras)
        {
            cameraControlTrigger.customCameraInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera on Left",
                cameraControlTrigger.customCameraInspectorObjects.cameraOnLeft,
                typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

            cameraControlTrigger.customCameraInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera on Right",
                cameraControlTrigger.customCameraInspectorObjects.cameraOnRight,
                typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if (cameraControlTrigger.customCameraInspectorObjects.panCameraOnTrigger)
        {
            cameraControlTrigger.customCameraInspectorObjects.panDirection = (CameraPanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction",
                cameraControlTrigger.customCameraInspectorObjects.panDirection);

            cameraControlTrigger.customCameraInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance",
                cameraControlTrigger.customCameraInspectorObjects.panDistance);

            cameraControlTrigger.customCameraInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time",
                cameraControlTrigger.customCameraInspectorObjects.panTime);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}

public enum CameraPanDirection
{
    Up,
    Down,
    Left,
    Right
}