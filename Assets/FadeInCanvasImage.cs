using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeInCanvasImage : MonoBehaviour
{
    [SerializeField] float fadeTime = 2f;

    [SerializeField] CinemachineVirtualCamera currentCamera;
    [SerializeField] CinemachineVirtualCamera cameraToSwap;

    [SerializeField] Transform rangerSpawnPoint;
    [SerializeField] Transform templeGuardianSpawnPoint;
    [SerializeField] Transform playerSpawnPoint;

    [SerializeField] GameObject player;
    [SerializeField] GameObject ranger;
    [SerializeField] GameObject templeGuardian;

    private CanvasGroup canvasGroup;

    private CameraManager cameraManager;

    private PlayerInputHandler inputHandler;

    private SoundManager soundManager;


    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        cameraManager = FindObjectOfType<CameraManager>();
        inputHandler = GameObject.FindWithTag("Player").GetComponent<PlayerInputHandler>();
        soundManager = FindObjectOfType<SoundManager>();

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        // lock controls
        inputHandler.SwitchToActionMap("UINoPause");
        inputHandler.BlockActionInput = true;

        // FADE IN
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f; // Ensure alpha is set to 1 when coroutine is done

        // MOVE PLAYER
        player.transform.position = playerSpawnPoint.transform.position;
        // INSTANTIATE BOSSES
        Instantiate(ranger, rangerSpawnPoint.position, rangerSpawnPoint.rotation);
        Instantiate(templeGuardian, templeGuardianSpawnPoint.position, templeGuardianSpawnPoint.rotation);

        // MOVE CAMERA
        cameraManager.SwapCameraOnBossDeath(currentCamera, cameraToSwap);

        // Wait
        yield return new WaitForSeconds(2f);

        // FADE OUT
        elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0.5f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0.5f; // Ensure alpha is set to 0.5 when coroutine is done

        yield return new WaitForSeconds(26f);

        soundManager.StopPlay("BossDemonDefeated");
        SceneManager.LoadScene("MainMenu");
    }
}