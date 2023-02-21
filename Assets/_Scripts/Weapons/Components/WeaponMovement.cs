using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : WeaponComponent<MovementData, AttackMovement>
{
    private Movement coreMovement;
    private Movement CoreMovement => coreMovement ? coreMovement : Core.GetCoreComponent(ref coreMovement);

    private void HandleStartMovement()
    {
        CoreMovement.SetVelocity(currentAttackData.Velocity, currentAttackData.Direction, CoreMovement.FacingDirection);
    }

    private void HandleStopMovement()
    {
        CoreMovement.SetVelocityZero();
    }

    //This is a comment
    protected override void OnEnable()
    {
        base.OnEnable();

        eventHandler.OnStartMovement += HandleStartMovement;
        eventHandler.OnStopMovement += HandleStartMovement;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        eventHandler.OnStartMovement -= HandleStartMovement;
        eventHandler.OnStopMovement -= HandleStartMovement;
    }
}
