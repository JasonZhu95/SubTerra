using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerState
{
    private Vector2 detectedPosition;
    private Vector2 cornerPosition;
    private Vector2 startPosition;
    private Vector2 stopPosition;

    private bool isHanging;
    private bool isClimbing;
    private bool jumpInput;
    private bool isTouchingCeiling;

    private int xInput;
    private int yInput;

    public PlayerLedgeClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        player.Anim.SetBool("climbLedge", false);
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        isHanging = true;
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocityZero();
        player.transform.position = detectedPosition;
        cornerPosition = player.DetermineCornerPosition();

        startPosition.Set(cornerPosition.x - (player.FacingDirection * playerData.startOffset.x), cornerPosition.y - playerData.startOffset.y);
        stopPosition.Set(cornerPosition.x + (player.FacingDirection * playerData.stopOffset.x), cornerPosition.y + playerData.stopOffset.y);

        player.transform.position = startPosition;
    }

    public override void Exit()
    {
        base.Exit();

        isHanging = false;
        if (isClimbing)
        {
            player.transform.position = stopPosition;
            isClimbing = false;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            if (isTouchingCeiling)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
            else
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
        else
        {
            xInput = player.InputHandler.NormInputX;
            yInput = player.InputHandler.NormInputY;
            jumpInput = player.InputHandler.JumpInput;

            player.SetVelocityZero();
            player.transform.position = startPosition;

            // State Changes
            if (xInput == player.FacingDirection && isHanging && !isClimbing)
            {
                CheckForSpace();
                isClimbing = true;
                player.Anim.SetBool("climbLedge", true);
            }
            else if (yInput == -1 && isHanging && !isClimbing)
            {
                stateMachine.ChangeState(player.InAirState);
            }
            else if (jumpInput && !isClimbing)
            {
                player.WallJumpState.DetermineWallJumpDirection(true);
                stateMachine.ChangeState(player.WallJumpState);
            }
        }
    }

    public void SetDetectedPosition(Vector2 position) => detectedPosition = position;

    private void CheckForSpace()
    {
        isTouchingCeiling = Physics2D.Raycast(cornerPosition + (Vector2.up * 0.015f) + (Vector2.right * player.FacingDirection * 0.015f)
            , Vector2.up, playerData.standColliderHeight, playerData.whatIsGround);
        player.Anim.SetBool("isTouchingCeiling", isTouchingCeiling);
    }
}
