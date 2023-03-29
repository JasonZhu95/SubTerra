using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonArrowHandler : MonoBehaviour, IPointerEnterHandler
{
    private GameObject arrowContainer;
    private EventSystem eventSystem;

    private void Awake()
    {
        arrowContainer = gameObject.transform.GetChild(0).gameObject;
    }

    private void OnEnable()
    {
        eventSystem = EventSystem.current;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(gameObject);
    }
}
