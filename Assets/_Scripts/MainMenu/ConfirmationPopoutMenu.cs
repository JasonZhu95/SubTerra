using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ConfirmationPopoutMenu : Menu
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public void ActivateMenu(string displayText, UnityAction confirmAction, UnityAction cancelAction)
    {
        gameObject.SetActive(true);
        
        // Set Display Text
        this.displayText.text = displayText;

        // Remove any existing listeners.  Defensive check.
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        // Assign OnClick Functions for button click
        confirmButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            confirmAction();
        });
        cancelButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            cancelAction();
        });
    }

    private void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }
}
