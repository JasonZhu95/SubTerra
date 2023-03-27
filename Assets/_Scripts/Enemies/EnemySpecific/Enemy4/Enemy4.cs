using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy4 : Entity
{
    public E4_IdleState idleState { get; private set; }
    public E4_MoveState moveState { get; private set; }

    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;

    public override void Awake()
    {
        base.Awake();

        idleState = new E4_IdleState(this, stateMachine, "idle", idleStateData, this);
        moveState = new E4_MoveState(this, stateMachine, "move", moveStateData, this);
    }

    private void Start()
    {
        stateMachine.Initialize(moveState);
    }
}
