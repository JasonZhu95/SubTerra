using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Script activates arrows when the button is selected
// Added On the disabling of multiple clicks from this script as well
public class ButtonArrowHandler : MonoBehaviour, IPointerEnterHandler
{
    private GameObject arrowContainer;
    private EventSystem eventSystem;
    private Button thisButton;

    public bool DisableClickOnAwake = false;
    private bool disableButtonClick = false;

    private void Awake()
    {
        arrowContainer = gameObject.transform.GetChild(0).gameObject;
        thisButton = gameObject.GetComponent<Button>();
        if (disableButtonClick)
        {
            thisButton.onClick.AddListener(DisableOnClick);
        }
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

    private void DisableOnClick()
    {
        if (!disableButtonClick)
        {
            disableButtonClick = true;
            thisButton.interactable = false;
            StartCoroutine(ReEnableButton());
        }
    }

    private IEnumerator ReEnableButton()
    {
        yield return new WaitForSeconds(0.4f);
        thisButton.interactable = true;
        disableButtonClick = false;
    }
}
