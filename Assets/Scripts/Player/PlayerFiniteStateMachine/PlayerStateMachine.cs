using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds reference to what the current state of the player is in
public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }  // Shorthand for a getter and setter

    // Initialize player state on game start
    public void Initialize(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    // Function that changes the state of the player
    public void ChangeState(PlayerState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
