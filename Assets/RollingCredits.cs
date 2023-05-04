using System.Collections;
using UnityEngine;
using TMPro;

public class RollingCredits : MonoBehaviour
{
    public float scrollSpeed = 10.0f;
    private TextMeshProUGUI textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        StartCoroutine(ScrollText());
    }

    IEnumerator ScrollText()
    {
        yield return new WaitForSeconds(2.0f); // Wait for 2 seconds before scrolling

        float scrollPosition = 0.0f;
        // pos Y when the text is below the bottom
        float startingY = textMesh.rectTransform.anchoredPosition.y;
        // pos Y when text is above the top
        float endingY = 1650f;

        while (scrollPosition < 1.0f)
        {
            scrollPosition += Time.deltaTime / scrollSpeed;
            textMesh.rectTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(startingY, endingY, scrollPosition));
            yield return null;
        }
    }
}
