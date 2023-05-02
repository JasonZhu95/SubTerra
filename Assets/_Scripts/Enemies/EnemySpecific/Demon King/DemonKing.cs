using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKing : Entity
{
    // Slime States
    public DemonKing_SlimeMoveState slimeMoveState { get; private set; }
    public DemonKing_SlimeIdleState slimeIdleState { get; private set; }
    public DemonKing_TransformToDemonKingState transformToDemonKingState { get; private set; }

    // Demon King States
    public DemonKing_MoveState moveState { get; private set; }
    public DemonKing_IdleState idleState { get; private set; }
    public DemonKing_PlayerDetectedState playerDetectedState { get; private set; }
    public DemonKing_MeleeAttackState meleeAttackState { get; private set; }
    public DemonKing_LookForPlayerState lookForPlayerState { get; private set; }
    public DemonKing_StunState stunState { get; private set; }
    public DemonKing_DeadState deadState { get; private set; }
    public DemonKing_ChargeState chargeState { get; private set; }

    // Slime Data
    [SerializeField]
    private D_MoveState slimeMoveStateData;
    [SerializeField]
    private D_IdleState slimeIdleStateData;
    [SerializeField]
    private D_TransformToDemonKingState transformToDemonKingStateData;

    // Demon King Data
    [SerializeField]
    private D_MoveState moveStateData;
    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_PlayerDetected playerDetectedStateData;
    [SerializeField]
    private D_MeleeAttack meleeAttackStateData;
    [SerializeField]
    private D_LookForPlayer lookForPlayerStateData;
    [SerializeField]
    private D_StunState stunStateData;
    [SerializeField]
    private D_DeadState deadStateData;
    [SerializeField]
    private D_ChargeState chargeStateData;

    [SerializeField]
    private Transform meleeAttackPosition;

    private Stats Stats => stats ? stats : Core.GetCoreComponent(ref stats);
    private Stats stats;

    public override void Awake()
    {
        base.Awake();

        // Slime States
        slimeMoveState = new DemonKing_SlimeMoveState(this, stateMachine, "slimeMove", slimeMoveStateData, this);
        slimeIdleState = new DemonKing_SlimeIdleState(this, stateMachine, "slimeIdle", slimeIdleStateData, this);
        transformToDemonKingState = new DemonKing_TransformToDemonKingState(this, stateMachine, "transform", transformToDemonKingStateData, this);

        // Demon King States
        moveState = new DemonKing_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new DemonKing_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new DemonKing_PlayerDetectedState(this, stateMachine, "playerDetected", playerDetectedStateData, this);
        meleeAttackState = new DemonKing_MeleeAttackState(this, stateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData, this);
        lookForPlayerState = new DemonKing_LookForPlayerState(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        stunState = new DemonKing_StunState(this, stateMachine, "stunned", stunStateData, this);
        deadState = new DemonKing_DeadState(this, stateMachine, "dead", deadStateData, this);
        chargeState = new DemonKing_ChargeState(this, stateMachine, "charge", chargeStateData, this);
    }

    private void Start()
    {
        stateMachine.Initialize(slimeIdleState);

        Stats.Health.OnCurrentHealthBelow100 += () => stateMachine.ChangeState(transformToDemonKingState);
        Stats.Health.OnCurrentValueZero += () => stateMachine.ChangeState(deadState);
    }

    private void OnDestroy()
    {
        Stats.Health.OnCurrentHealthBelow100 -= () => stateMachine.ChangeState(transformToDemonKingState);
        Stats.Health.OnCurrentValueZero -= () => stateMachine.ChangeState(deadState);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);
    }
}
