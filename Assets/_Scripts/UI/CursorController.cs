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
    private PlayerInput playerInput = null;

    private void Awake()
    {
        ChangeCursor(cursor);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void ChangeCursor(Texture2D cursorType)
    {
        Cursor.SetCursor(cursorType, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        if (playerInput != null)
        {
            // If the player is in a UI State, turn the cursor on.
            if (playerInput.currentActionMap.name == "UI")
            {
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
            }
        }

        // Allows cursor changes when player input is not set.  Essentially for the main menu
        if (playerInput == null)
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }
    }
}
