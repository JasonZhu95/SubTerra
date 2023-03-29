using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonArrowHandler : MonoBehaviour
{
    private GameObject arrowContainer;

    private void Awake()
    {
        arrowContainer = gameObject.transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            arrowContainer.SetActive(true);
        }
        else
        {
            arrowContainer.SetActive(false);
        }
    }
}
