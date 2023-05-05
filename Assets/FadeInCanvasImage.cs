using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInCanvasImage : MonoBehaviour
{
    [SerializeField] float fadeTime = 2f;

    [SerializeField] Transform cameraTarget;
    
    private CanvasGroup canvasGroup;
    private Camera mainCamera;


    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        mainCamera = Camera.main;

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        // FADE IN
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f; // Ensure alpha is set to 1 when coroutine is done

        // MOVE CAMERA
        mainCamera.transform.position = cameraTarget.position;

        // Wait 1 second
        yield return new WaitForSeconds(1f);

        // FADE OUT
        elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0.5f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0.5f; // Ensure alpha is set to 0.5 when coroutine is done
    }
}