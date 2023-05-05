using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// Singleton class that handles swapping between multiple cameras
public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineVirtualCamera[] virtualCameras;
    [Header("Default Respawn Camera")]
    [SerializeField] private CinemachineVirtualCamera respawnCamera;

    [Header("Lerping the Y Dampining of Player During jumps/falls")]
    [SerializeField] private float fallPanAmount = 0.25f;
    [SerializeField] private float fallYPanTime = 0.35f;
    public float fallSpeedYDampingChangeThreshold = -15f;
    private float normYPanAmount;

    public bool isLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine lerpYPanCoroutine;
    private Coroutine panCameraCoroutine;

    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer framingTranposer;

    private Vector2 startingTrackedObjectOffset;

    private void Awake()
    {
        Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < virtualCameras.Length; i++)
        {
            if (virtualCameras[i].enabled)
            {
                currentCamera = virtualCameras[i];
                framingTranposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        // Set Y dampning amount based off of inspector value
        normYPanAmount = framingTranposer.m_YDamping;

        // Set starting position of tracked objects offset
        startingTrackedObjectOffset = framingTranposer.m_TrackedObjectOffset;
    }

    #region Lerp Functions

    public void LerpYDamping(bool isPlayerFalling)
    {
        lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        isLerpingYDamping = true;

        //Get starting damping values;
        float startDampAmount = framingTranposer.m_YDamping;
        float endDampAmount = 0f;

        if (isPlayerFalling)
        {
            endDampAmount = fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = normYPanAmount;
        }

        // Lerp for the pan values given
        float elapsedTime = 0f;
        while (elapsedTime < fallYPanTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / fallYPanTime));
            framingTranposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        isLerpingYDamping = false;
    }

    #endregion

    #region Pan Camera Functions

    public void PanCameraOnTrigger(float panDistance, float panTime, CameraPanDirection panDirection, bool panToStartingPos)
    {
        panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, CameraPanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        // handle pan on trigger
        if (!panToStartingPos)
        {
            // Set The direction and distance of the pan
            switch (panDirection)
            {
                case CameraPanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case CameraPanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case CameraPanDirection.Left:
                    endPos = Vector2.right;
                    break;
                case CameraPanDirection.Right:
                    endPos = Vector2.left;
                    break;
                case CameraPanDirection.DownRight:
                    endPos = new Vector2(1f, -1f);
                    break;
                case CameraPanDirection.DownLeft:
                    endPos = new Vector2(-1f, -1f);
                    break;
                default:
                    break;
            }

            endPos *= panDistance;
            startingPos = startingTrackedObjectOffset;
            endPos += startingPos;
        }
        // Handle direction settings when moving back to starting position
        else
        {
            startingPos = framingTranposer.m_TrackedObjectOffset;
            endPos = startingTrackedObjectOffset;
        }

        // Pan the camera over time
        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
            framingTranposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }
    }

    #endregion

    #region Swap Camera
    public void SwapCameraOnRespawn()
    {
        currentCamera.enabled = false;
        respawnCamera.enabled = true;
        currentCamera = respawnCamera;
        framingTranposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    public void SwapCameraX(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        if (currentCamera == cameraFromLeft && triggerExitDirection.x < 0f)
        {
            // Activate new camera
            cameraFromRight.enabled = true;

            // Deactivate old camera
            cameraFromLeft.enabled = false;

            // Set New camera as current camera
            currentCamera = cameraFromRight;

            // Update composer
            framingTranposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        else if (currentCamera == cameraFromRight && triggerExitDirection.x > 0f)
        {
            // Activate new camera
            cameraFromLeft.enabled = true;

            // Deactivate old camera
            cameraFromRight.enabled = false;

            // Set New camera as current camera
            currentCamera = cameraFromLeft;

            // Update composer
            framingTranposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }

    public void SwapCameraY(CinemachineVirtualCamera cameraFromTop, CinemachineVirtualCamera cameraFromBottom, Vector2 triggerExitDirection)
    {
        if (currentCamera == cameraFromTop && triggerExitDirection.y > 0f)
        {
            // Activate new camera
            cameraFromBottom.enabled = true;

            // Deactivate old camera
            cameraFromTop.enabled = false;

            // Set New camera as current camera
            currentCamera = cameraFromBottom;

            // Update composer
            framingTranposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        else if (currentCamera == cameraFromBottom && triggerExitDirection.y < 0f)
        {
            // Activate new camera
            cameraFromTop.enabled = true;

            // Deactivate old camera
            cameraFromBottom.enabled = false;

            // Set New camera as current camera
            currentCamera = cameraFromTop;

            // Update composer
            framingTranposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }
    #endregion
}
