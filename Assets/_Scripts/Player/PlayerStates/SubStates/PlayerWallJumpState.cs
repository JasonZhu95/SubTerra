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
        playerSoundManager.Play("PlayerWallJump");
        xInput = player.InputHandler.NormInputX;
        player.InputHandler.UseJumpInput();
        player.JumpState.ResetAmountOfJumpsLeft();
        if (player.WallJumpUpCheck && xInput == 0)
        {
            Movement?.SetVelocityY(playerData.wallJumpUpVelocity);
        }
        else
        {
            player.SetWallJumpCheckFalse();
            Movement?.SetVelocity(playerData.wallJumpVelocity, playerData.wallJumpAngle, wallJumpDirection);
            Movement?.CheckIfShouldFlip(wallJumpDirection);
        }
        player.JumpState.DecreaseAmountOfJumpsLeft();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        dashInput = player.InputHandler.DashInput;
        player.Anim.SetFloat("yVelocity", Movement.CurrentVelocity.y);
        player.Anim.SetFloat("xVelocity", Mathf.Abs(Movement.CurrentVelocity.x));

        if (dashInput && player.DashState.CanDash && !player.DisableDash)
        {
            player.StateMachine.ChangeState(player.DashState);
        }
        else if (player.InputHandler.AttackInputs[(int)CombatInputs.primary] &&
            player.PrimaryAttackState.CheckIfCanAttack())
        {
            stateMachine.ChangeState(player.PrimaryAttackState);
        }
        else if (player.InputHandler.AttackInputs[(int)CombatInputs.secondary] &&
                 player.SecondaryAttackState.CheckIfCanAttack())
        {
            stateMachine.ChangeState(player.SecondaryAttackState);
        }
        else if (player.WallJumpUpCheck)
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
            wallJumpDirection = -Movement.FacingDirection;
        }
        else
        {
            wallJumpDirection = Movement.FacingDirection;
        }
    }

    public override void Exit()
    {
        base.Exit();

        player.SetWallJumpCheckFalse();
    }
}
