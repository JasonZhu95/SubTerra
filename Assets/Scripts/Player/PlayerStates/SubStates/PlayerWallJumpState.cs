using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerAbilityState
{
    private int wallJumpDirection;
    private bool dashInput;
    private int xInput;

    public PlayerWallJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        xInput = player.InputHandler.NormInputX;
        player.InputHandler.UseJumpInput();
        player.JumpState.ResetAmountOfJumpsLeft();
        if (player.WallJumpUpCheck && xInput == 0)
        {
            core.Movement.SetVelocityY(playerData.wallJumpUpVelocity);
        }
        else
        {
            player.SetWallJumpCheckFalse();
            core.Movement.SetVelocity(playerData.wallJumpVelocity, playerData.wallJumpAngle, wallJumpDirection);
            core.Movement.CheckIfShouldFlip(wallJumpDirection);
        }
        player.JumpState.DecreaseAmountOfJumpsLeft();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        dashInput = player.InputHandler.DashInput;
        player.Anim.SetFloat("yVelocity", core.Movement.CurrentVelocity.y);
        player.Anim.SetFloat("xVelocity", Mathf.Abs(core.Movement.CurrentVelocity.x));

        if (dashInput && player.DashState.CanDash)
        {
            player.StateMachine.ChangeState(player.DashState);
        }
        if (player.WallJumpUpCheck)
        {
            if (Time.time > startTime + playerData.wallJumpUpTime)
            {
                isAbilityDone = true;
            }

        }
        else if (Time.time > startTime + playerData.wallJumpTime)
        {
            isAbilityDone = true;
        }
    }

    public void DetermineWallJumpDirection(bool isTouchingWall)
    {
        if (isTouchingWall)
        {
            wallJumpDirection = -core.Movement.FacingDirection;
        }
        else
        {
            wallJumpDirection = core.Movement.FacingDirection;
        }
    }

    public override void Exit()
    {
        base.Exit();

        player.SetWallJumpCheckFalse();
    }
}
