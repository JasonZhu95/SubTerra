using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerAbilityState
{
    public bool CanDash { get; private set; }

    private bool isHolding;
    private bool dashInputStop;
    private bool isTouchingGround;
    private bool cornerDashCorrection;
    private int yInput;

    private Vector2 dashDirection;
    private Vector2 dashDirectionInput;
    private Vector2 lastAfterImagePosition;
    private Vector2 workspace;

    private float lastDashTime;

    public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        CanDash = false;
        player.InputHandler.UseDashInput();

        isHolding = true;
        dashDirection = Vector2.right * player.FacingDirection;

        Time.timeScale = playerData.holdTimeScale;
        startTime = Time.unscaledTime;
        workspace = Vector2.right * player.FacingDirection;
    }

    public override void Exit()
    {
        base.Exit();

        if (player.CurrentVelocity.y > 0)
        {
            player.SetVelocityY(player.CurrentVelocity.y * playerData.dashEndYMultiplier);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        isTouchingGround = player.CheckIfGrounded();
        yInput = player.InputHandler.NormInputY;
        cornerDashCorrection = player.CheckForCornerDashCorrection();

        if (!isExitingState)
        {
            if (cornerDashCorrection)
            {
                player.transform.position = player.transform.position + new Vector3(0.0f, playerData.dashCorrectionHeight, 0.0f);
                cornerDashCorrection = false;
            }
            player.Anim.SetFloat("yVelocity", player.CurrentVelocity.y);
            player.Anim.SetFloat("xVelocity", Mathf.Abs(player.CurrentVelocity.x));

            if(isHolding)
            {
                dashDirectionInput = player.InputHandler.DashDirectionInput;
                dashInputStop = player.InputHandler.DashInputStop;

                if (dashDirectionInput != Vector2.zero)
                {
                    dashDirection = dashDirectionInput;
                    dashDirection.Normalize();
                }

                float angle = Vector2.SignedAngle(Vector2.right, dashDirection);

                if (dashInputStop || Time.unscaledTime >= startTime + playerData.maxHoldTime)
                {
                    isHolding = false;
                    Time.timeScale = 1f;
                    startTime = Time.time;
                    player.CheckIfShouldFlip(Mathf.RoundToInt(dashDirection.x));
                    player.RB.drag = playerData.drag;
                    if (isTouchingGround && yInput == -1)
                    {
                        dashDirection = workspace;
                    }
                    if (!trampolineDetected)
                    {
                        player.SetVelocity(playerData.dashVelocity, dashDirection);
                    }
                    else
                    {
                        player.RB.drag = 0f;
                        DashTrampolineSet();
                        isAbilityDone = true;
                    }
                    PlaceAfterImage();
                }
            }
            else
            {
                if (!trampolineDetected)
                {
                    player.SetVelocity(playerData.dashVelocity, dashDirection);
                }
                else
                {
                    player.RB.drag = 0f;
                    DashTrampolineSet();
                    isAbilityDone = true;
                }
                CheckIfShouldPlaceAfterImage();
                if (Time.time >= startTime + playerData.dashTime)
                {
                    player.RB.drag = 0f;
                    isAbilityDone = true;
                    lastDashTime = Time.time;
                }
            }
        }
    }

    private void CheckIfShouldPlaceAfterImage()
    {
        if (Vector2.Distance(player.transform.position, lastAfterImagePosition) >= playerData.distBetweenAfterImages)
        {
            PlaceAfterImage();
        }
    }

    private void PlaceAfterImage()
    {
        PlayerAfterImagePool.Instance.GetFromPool();
        lastAfterImagePosition = player.transform.position;
    }

    public bool CheckIfCanDash()
    {
        return CanDash && Time.time > lastDashTime + playerData.dashCooldown;
    }

    public void ResetCanDash() => CanDash = true;
}
