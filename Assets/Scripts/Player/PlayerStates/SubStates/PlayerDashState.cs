using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerAbilityState
{
    public bool CanDash { get; private set; }

    private float lastDashTime;

    public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public bool CheckIfCanDash()
    {
        return CanDash && Time.time > lastDashTime + playerData.dashCooldown;
    }

    public void ResetCanDash() => CanDash = true;

}
