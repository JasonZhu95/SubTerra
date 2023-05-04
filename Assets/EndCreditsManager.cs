using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndCreditsManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    [SerializeField]
    private Image blackImage;

    public void RollCredits()
    {
        textMeshPro.gameObject.SetActive(true);
        blackImage.gameObject.SetActive(true);
    }
}
