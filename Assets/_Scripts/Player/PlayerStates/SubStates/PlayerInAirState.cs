using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    #region Local Variables

    // Input
    private int xInput;
    private bool jumpInput;
    private bool grabInput;
    private bool dashInput;

    // Checks
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isTouchingWallBack;
    private bool lastFrameIsTouchingWall;
    private bool lastFrameTouchingWallBack;
    private bool isTouchingLedge;
    private bool isJumping;
    private bool isTouchingTrampoline;

    // Other Variables
    private bool jumpInputStop;
    private bool coyoteTime;
    private bool wallJumpCoyoteTime;

    private float startWallJumpCoyoteTime;

    #endregion

    private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);
    private CollisionSenses CollisionSenses => collisionSenses ? collisionSenses : core.GetCoreComponent(ref collisionSenses);

    private Movement movement;
    private CollisionSenses collisionSenses;

    public PlayerInAirState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    #region Unity Callback Overrides
    public override void DoChecks()
    {
        base.DoChecks();

        // Set Local Booleans based off updated checks
        lastFrameIsTouchingWall = isTouchingWall;
        lastFrameTouchingWallBack = isTouchingWallBack;

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
            isTouchingWall = CollisionSenses.WallFront;
            isTouchingWallBack = CollisionSenses.WallBack;
            isTouchingLedge = CollisionSenses.LedgeHorizontal;
        }
        isTouchingTrampoline = player.DashState.DashTrampolineCheck;

        // Ledge Climb Logic
        if (isTouchingWall && !isTouchingLedge)
        {
            player.LedgeClimbState.SetDetectedPosition(player.transform.position);
        }

        // Walljump Logic
        if (!wallJumpCoyoteTime && !isTouchingWall && !isTouchingWallBack && (lastFrameIsTouchingWall || lastFrameTouchingWallBack))
        {
            StartWallJumpCoyoteTime();
        }
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        lastFrameIsTouchingWall = false;
        lastFrameTouchingWallBack = false;
        isTouchingWall = false;
        isTouchingWallBack = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Setup Coyote Times
        CheckCoyoteTime();
        CheckWallJumpCoyoteTime();

        // Local variables accessing Jump input
        xInput = player.InputHandler.NormInputX;
        jumpInput = player.InputHandler.JumpInput;
        jumpInputStop = player.InputHandler.JumpInputStop;
        grabInput = player.InputHandler.GrabInput;
        dashInput = player.InputHandler.DashInput;

        CheckJumpMultiplier();

        // State Changes
        if (player.InputHandler.AttackInputs[(int)CombatInputs.primary])
        {
            stateMachine.ChangeState(player.PrimaryAttackState);
        }
        else if (player.InputHandler.AttackInputs[(int)CombatInputs.secondary])
        {
            stateMachine.ChangeState(player.SecondaryAttackState);
        }
        else if (isGrounded && Movement?.CurrentVelocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.LandState);
        }
        else if(isTouchingLedge && !isTouchingLedge && !isGrounded)
        {
            stateMachine.ChangeState(player.LedgeClimbState);
        }
        else if (jumpInput && (isTouchingWall || isTouchingWallBack || wallJumpCoyoteTime))
        {
            StopWallJumpCoyoteTime();
            isTouchingWall = CollisionSenses.WallFront;
            player.WallJumpState.DetermineWallJumpDirection(isTouchingWall);
            stateMachine.ChangeState(player.WallJumpState);
        }
        else if (jumpInput && player.JumpState.CanJump())
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if (isTouchingWall && grabInput && isTouchingLedge)
        {
            stateMachine.ChangeState(player.WallGrabState);
        }
        else if (isTouchingWall && xInput == Movement?.FacingDirection)
        {
            stateMachine.ChangeState(player.WallSlideState);
        }
        else if (dashInput && player.DashState.CheckIfCanDash())
        {
            stateMachine.ChangeState(player.DashState);
        }
        else
        {
            Movement?.CheckIfShouldFlip(xInput);
            if (player.DashState.DashTrampolineCheck)
            {
                player.JumpState.DecreaseAmountOfJumpsLeft();
                Movement?.SetVelocityY(playerData.trampolineVelocity);
                player.DashState.ResetCanDash();
                player.DashState.DashTrampolineSetFalse();
            }
            else
            {
                Movement?.SetVelocityX(playerData.movementVelocity * xInput);
            }

            player.Anim.SetFloat("yVelocity", Movement.CurrentVelocity.y);
            player.Anim.SetFloat("xVelocity", Mathf.Abs(Movement.CurrentVelocity.x));
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    #endregion

    #region Check Functions

    // FUNCTION: Manages variable jump height
    private void CheckJumpMultiplier()
    {
        if (isJumping)
        {
            if (Movement?.CurrentVelocity.y < playerData.halfGravityThresholdMax && Movement?.CurrentVelocity.y > playerData.halfGravityThresholdMin)
            {
                player.RB.gravityScale = 2.5f;
            }
            if (isTouchingWall)
            {
                player.RB.gravityScale = 5.0f;
            }
            if (jumpInputStop)
            {
                Movement?.SetVelocityY(Movement.CurrentVelocity.y * playerData.variableJumpHeightMultiplier);
                player.RB.gravityScale = 5.0f;
                isJumping = false;
            }
            else if (Movement?.CurrentVelocity.y <= 0f)
            {
                player.RB.gravityScale = 5.0f;
                isJumping = false;
            }
        }
    }

    // FUNCTION: checks if the player can jump after falling off platform
    private void CheckCoyoteTime()
    {
        if (coyoteTime && Time.time > startTime + playerData.coyoteTime)
        {
            coyoteTime = false;
            player.JumpState.DecreaseAmountOfJumpsLeft();
        }
    }

    // FUNCITON: Checks if the player can wall jump while holding xinput
    private void CheckWallJumpCoyoteTime()
    {
        if (wallJumpCoyoteTime && Time.time > startWallJumpCoyoteTime + playerData.coyoteTime)
        {
            wallJumpCoyoteTime = false;
        }
    }
    #endregion

    #region Set Functions

    // FUNCTION: Setup Wall Jump coyote Time
    public void StartWallJumpCoyoteTime()
    {
        wallJumpCoyoteTime = true;
        startWallJumpCoyoteTime = Time.time;
    }
    public void StopWallJumpCoyoteTime() => wallJumpCoyoteTime = false;

    public void StartCoyoteTime() => coyoteTime = true;
    public void SetIsJumping() => isJumping = true;
    #endregion
}
