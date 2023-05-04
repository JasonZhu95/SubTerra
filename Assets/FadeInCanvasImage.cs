using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInCanvasImage : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [SerializeField] float fadeTime = 2f;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f; // Ensure alpha is set to 1 when coroutine is done
    }
}