using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3 : Entity
{
    public E3_IdleState idleState { get; private set; }
    public E3_MoveState moveState { get; private set; }

    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;

    public override void Awake()
    {
        base.Awake();

        idleState = new E3_IdleState(this, stateMachine, "idle", idleStateData, this);
        moveState = new E3_MoveState(this, stateMachine, "move", moveStateData, this);
    }

    private void Start()
    {
        stateMachine.Initialize(moveState);
    }
}
