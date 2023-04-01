using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Project.EventChannels;
using Project.Managers;

public class PlayerInputHandler : MonoBehaviour
{
    #region Variables

    private PlayerInput playerInput;

    // Gameplay Actions
    public Vector2 RawMovementInput { get; private set; }
    public Vector2 RawDashDirectionInput { get; private set; }
    public Vector2Int DashDirectionInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpInputStop { get; private set; }
    public bool GrabInput { get; private set; }
    public bool DashInput { get; private set; }
    public bool DashInputStop { get; private set; }
    public bool InteractPressed { get; set; }
    public bool PausePressed { get; set; }
    public bool InventoryPressed { get; set; }
    public bool InteractShopPressed { get; private set; }

    public bool[] AttackInputs { get; private set; }
    public bool[] AttackInputsHold { get; private set; }

    // UI Actions
    public bool BlockActionInput { get; set; } = false;
    public bool MainActionUIInput { get; set; }
    public bool BackActionUIInput { get; set; }
    public Vector2 RawMenuNavigationInput { get; private set; }
    public int NormMenuInputX { get; private set; }
    public int NormMenuInputY { get; private set; }

    [SerializeField]
    private float inputHoldTime = 0.2f;

    private float jumpInputStartTime;
    private float dashInputStartTime;

    public event Action<bool> OnInteract;

    [Header("Event Channels")] [SerializeField]
    private GameStateEventChannel GameStateEventChannel;

    public InputActionAsset inputActionAsset;

    #endregion

    #region Unity Callback Functions

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        GameStateEventChannel.OnAfterStateChange += HandleGameStateChange;
    }

    private void OnDestroy()
    {
        GameStateEventChannel.OnAfterStateChange -= HandleGameStateChange;
    }

    private void Start()
    {
        int count = Enum.GetValues(typeof(CombatInputs)).Length;
        AttackInputs = new bool[count];
        AttackInputsHold = new bool[count];
    }

    private void Update()
    {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();
    }

    #endregion

    #region Game State Functions

    public void SwitchToActionMap(String actionMapString)
    {
        playerInput.SwitchCurrentActionMap(actionMapString);
        if (actionMapString == "UI")
        {
            Time.timeScale = 0f;
        }
        if (actionMapString == "Gameplay")
        {
            Time.timeScale = 1f;
        }
    }

    private void HandleGameStateChange(object sender, GameStateEventArgs context)
    {
        switch (context.State)
        {
            case GameState.UI:
                playerInput.SwitchCurrentActionMap("UI");
                break;
            case GameState.Gameplay:
                playerInput.SwitchCurrentActionMap("Gameplay");
                break;
        }
    }

    #endregion

    #region Input Functions

    // FUNCTION: Normalize player input as a vector 2
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();

        NormInputX = Mathf.RoundToInt(RawMovementInput.x);
        NormInputY = Mathf.RoundToInt(RawMovementInput.y);
    }

    // FUNCTION: Check when a player presses jump input
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

    // FUNCTION: Check when a player presses grab input
    public void OnGrabInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GrabInput = true;
        }
        if (context.canceled)
        {
            GrabInput = false;
        }
    }

    // FUNCTION: Check when a player presses dash input
    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            DashInput = true;
            DashInputStop = false;
            dashInputStartTime = Time.time;
        }

        if (context.canceled)
        {
            DashInputStop = true;
        }
    }

    // FUNCTION: Determines the direction for player dash
    public void OnDashDirectionInput(InputAction.CallbackContext context)
    {
        RawDashDirectionInput = context.ReadValue<Vector2>();

        DashDirectionInput = Vector2Int.RoundToInt(RawDashDirectionInput.normalized);
    }

    // FUNCTION: Checks primary attack button
    public void OnPrimaryAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AttackInputs[(int)CombatInputs.primary] = true;
            AttackInputsHold[(int)CombatInputs.primary] = true;
        }

        if (context.canceled)
        {
            AttackInputsHold[(int)CombatInputs.primary] = false;
            AttackInputs[(int)CombatInputs.primary] = false;
        }
    }

    // FUNCTION: Checks secondary attack button
    public void OnSecondaryAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AttackInputs[(int)CombatInputs.secondary] = true;
            AttackInputsHold[(int)CombatInputs.secondary] = true;
        }

        if (context.canceled)
        {
            AttackInputs[(int)CombatInputs.secondary] = false;
            AttackInputsHold[(int)CombatInputs.secondary] = false;
        }
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.started && !BlockActionInput)
        {
            OnInteract?.Invoke(true);
            InteractShopPressed = true;
            InteractPressed = true;
        }

        if (context.canceled)
        {
            OnInteract?.Invoke(false);
            InteractPressed = false;
            InteractShopPressed = false;
        }
    }

    public void OnPauseInput(InputAction.CallbackContext context)
    {
        if (context.started && !BlockActionInput)
        {
            BlockActionInput = true;
            PausePressed = !PausePressed;
        }
    }

    public void OnInventoryInput(InputAction.CallbackContext context)
    {
        if (context.started && !BlockActionInput)
        {
            BlockActionInput = true;
            InventoryPressed = !InventoryPressed;
        }
    }

    public void OnMainActionUIInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            MainActionUIInput = !MainActionUIInput;
        }
    }

    public void OnBackActionUIInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            BackActionUIInput = !BackActionUIInput;
        }
    }

    public void OnMenuNavigationInput(InputAction.CallbackContext context)
    {
        RawMenuNavigationInput = context.ReadValue<Vector2>();

        NormMenuInputX = Mathf.RoundToInt(RawMenuNavigationInput.x);
        NormMenuInputY = Mathf.RoundToInt(RawMenuNavigationInput.y);
    }

    #endregion

    #region Other Functions

    public void UseJumpInput() => JumpInput = false;

    public void UseDashInput() => DashInput = false;

    public void UseAttackInput(CombatInputs input) => AttackInputs[(int)input] = false;

    // FUNCTION: Manages Variable Jump Heig t
    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }

    // FUNCTION: Stop Dash Input after a certain amount of time
    private void CheckDashInputHoldTime()
    {
        if (Time.time >= dashInputStartTime + inputHoldTime)
        {
            DashInput = false;
        }
    }

    #endregion
}

public enum CombatInputs
{
    primary,
    secondary
}