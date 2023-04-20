using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Managers;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursor;
    private Texture2D cursorClicked;

    [SerializeField]
    private PlayerInput playerInput;

    private void Awake()
    {
        ChangeCursor(cursor);
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void ChangeCursor(Texture2D cursorType)
    {
        Cursor.SetCursor(cursorType, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        if (playerInput.currentActionMap.name == "UI" || playerInput.currentActionMap == null)
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }
    }
}
