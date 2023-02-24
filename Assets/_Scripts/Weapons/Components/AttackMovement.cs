using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMovement : WeaponComponent<AttackMovementData>
{
    private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);

    private Movement movement;

    private float velocity;

    public void StartMovement()
    {
        velocity = data.GetAttackData(counter).Velocity;

        Movement.SetVelocityX(velocity * Movement.FacingDirection);
    }

    private void FixedUpdate()
    {
        Movement.SetVelocityX(velocity * Movement.FacingDirection);
    }

    public void StopMovement()
    {
        velocity = 0f;
        Movement.SetVelocityX(0f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        eventHandler.OnStartMovement += StartMovement;
        eventHandler.OnStopMovement += StopMovement;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        eventHandler.OnStartMovement -= StartMovement;
        eventHandler.OnStopMovement -= StopMovement;
    }
}

[System.Serializable]
public class AttackMovementData : WeaponComponentData<MovementData>
{
    public AttackMovementData()
    {
        ComponentDependencies.Add(typeof(AttackMovement));
    }
}

public struct MovementData : INameable
{
    [HideInInspector]
    public string AttackName;
    public float Velocity;

    public void SetName(string value) => AttackName = value;
}