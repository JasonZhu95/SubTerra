using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("First Selected Button")]

    [SerializeField] protected Button firstSelected;

    protected virtual void OnEnable()
    {
        SetFirstSelected(firstSelected);
    }

    public void SetFirstSelected(Button firstSelectedButton)
    {
        firstSelectedButton.Select();
    }
}
