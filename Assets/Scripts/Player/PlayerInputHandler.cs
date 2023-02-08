using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 RawMovementInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpInputStop { get; private set; }

    [SerializeField]
    private float InputHoldTime = 0.2f;

    private float jumpInputStartTime;

    private void Update()
    {
        CheckJumpInputHoldTime();
    }

    // FUNCTION: Normalize player input as a vector 2
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();

        NormInputX = (int)(RawMovementInput * Vector2.right).normalized.x;
        NormInputY = (int)(RawMovementInput * Vector2.up).normalized.y;
    }

    // FUNCTION: Check when a player presses jump button and releases it
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            JumpInput = true;
            JumpInputStop = false;
            jumpInputStartTime = Time.time;
        }
        
        if (context.canceled)
        {
            JumpInputStop = true;
        }
    }

    // FUNCTION: Set a player jumpinput to false when called
    public void UseJumpInput() => JumpInput = false;

    // FUNCTION: Manages Variable Jump Height
    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + InputHoldTime)
        {
            JumpInput = false;
        }
    }
}
