using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The state in which the player is touching the ground
public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }
}
