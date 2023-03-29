using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenuSelect : MonoBehaviour
{
    [SerializeField]
    private GameObject firstSelected;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}
