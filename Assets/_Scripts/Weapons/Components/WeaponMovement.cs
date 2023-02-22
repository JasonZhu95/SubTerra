using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : WeaponComponent<MovementData, AttackMovement>
{
    private Movement coreMovement;
    private Movement CoreMovement => coreMovement ? coreMovement : Core.GetCoreComponent(ref coreMovement);
    private CollisionSenses collisionSenses;
    private CollisionSenses CollisionSenses => collisionSenses ? collisionSenses : Core.GetCoreComponent(ref collisionSenses);

    private void HandleStartMovement()
    {
        if (CollisionSenses.Ground)
        {
            Debug.Log("Grounded");
            CoreMovement.SetVelocity(currentAttackData.Velocity, currentAttackData.Direction, CoreMovement.FacingDirection);
        }
        else
        {
            CoreMovement.SetVelocityX(0f);
        }
    }

    private void HandleStopMovement()
    {
        CoreMovement.SetVelocityZero();
    }

    protected override void Start()
    {
        base.Start();

        eventHandler.OnStartMovement += HandleStartMovement;
        eventHandler.OnStopMovement += HandleStartMovement;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        eventHandler.OnStartMovement -= HandleStartMovement;
        eventHandler.OnStopMovement -= HandleStartMovement;
    }
}
