using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallClimbState : PlayerTouchingWallState
{
    public PlayerWallClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (yInput == 1)
            {
                Movement?.SetVelocityY(playerData.wallClimbVelocity);
            }
            else if (yInput == -1)
            {
                Movement?.SetVelocityY(-playerData.wallClimbVelocityDown);
            }
            else if (yInput == 0 && !player.DisableWallGrab)
            {
                player.StateMachine.ChangeState(player.WallGrabState);
            }

            if (!grabInput)
            {
                stateMachine.ChangeState(player.InAirState);
            }
        }
    }
}
