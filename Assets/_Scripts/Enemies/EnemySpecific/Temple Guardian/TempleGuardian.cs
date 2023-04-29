using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleGuardian : Entity
{
    // Human States
    public TempleGuardian_MoveState moveState { get; private set; }
    public TempleGuardian_IdleState idleState { get; private set; }
    public TempleGuardian_PlayerDetectedState playerDetectedState { get; private set; }
    public TempleGuardian_MeleeAttackState meleeAttackState { get; private set; }
    public TempleGuardian_LookForPlayerState lookForPlayerState { get; private set; }
    public TempleGuardian_ChargeState chargeState { get; private set; }
    public TempleGuardian_DeadState deadState { get; private set; }
    public TempleGuardian_TransformToMummyState transformToMummyState { get; private set; }

    // Mummy States
    public TempleGuardian_MummyMoveState mummyMoveState { get; private set; }
    public TempleGuardian_MummyIdleState mummyIdleState { get; private set; }
    public TempleGuardian_MummyPlayerDetectedState mummyPlayerDetectedState { get; private set; }
    public TempleGuardian_MummyLookForPlayerState mummyLookForPlayerState { get; private set; }
    public TempleGuardian_MummyChargeState mummyChargeState { get; private set; }
    public TempleGuardian_MummyMeleeAttackState mummyMeleeAttackState { get; private set; }


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
    private D_ChargeState chargeStateData;
    [SerializeField]
    private D_DeadState deadStateData;
    [SerializeField]
    private D_TransformToMummyState transformToMummyStateData;

    [SerializeField]
    private Transform meleeAttackPosition;
    //[SerializeField]
    //private Transform playerPosition; 

    private Stats Stats => stats ? stats : Core.GetCoreComponent(ref stats);
    private Stats stats;

    public override void Awake()
    {
        base.Awake();

        // Human States
        moveState = new TempleGuardian_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new TempleGuardian_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new TempleGuardian_PlayerDetectedState(this, stateMachine, "playerDetected", playerDetectedStateData, this);
        meleeAttackState = new TempleGuardian_MeleeAttackState(this, stateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData, this);
        lookForPlayerState = new TempleGuardian_LookForPlayerState(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        chargeState = new TempleGuardian_ChargeState(this, stateMachine, "charge", chargeStateData, this);
        deadState = new TempleGuardian_DeadState(this, stateMachine, "dead", deadStateData, this);
        transformToMummyState = new TempleGuardian_TransformToMummyState(this, stateMachine, "transformToMummy", transformToMummyStateData, this);
        
        // Mummy States
        mummyMoveState = new TempleGuardian_MummyMoveState(this, stateMachine, "mummyMove", moveStateData, this);
        mummyIdleState = new TempleGuardian_MummyIdleState(this, stateMachine, "mummyIdle", idleStateData, this);
        mummyPlayerDetectedState = new TempleGuardian_MummyPlayerDetectedState(this, stateMachine, "mummyPlayerDetected", playerDetectedStateData, this);
        mummyLookForPlayerState = new TempleGuardian_MummyLookForPlayerState(this, stateMachine, "mummyLookForPlayer", lookForPlayerStateData, this);
        mummyChargeState = new TempleGuardian_MummyChargeState(this, stateMachine, "mummyCharge", chargeStateData, this);
        mummyMeleeAttackState = new TempleGuardian_MummyMeleeAttackState(this, stateMachine, "mummyMeleeAttack", meleeAttackPosition, meleeAttackStateData, this);
    }

    private void Start()
    {
        stateMachine.Initialize(moveState);

        Stats.Health.OnCurrentHealthBelow75 += () => stateMachine.ChangeState(transformToMummyState);
        Stats.Health.OnCurrentValueZero += () => stateMachine.ChangeState(deadState);
    }

    private void OnDestroy()
    {
        Stats.Health.OnCurrentHealthBelow75 += () => stateMachine.ChangeState(transformToMummyState);
        Stats.Health.OnCurrentValueZero -= () => stateMachine.ChangeState(deadState);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);
    }
}
