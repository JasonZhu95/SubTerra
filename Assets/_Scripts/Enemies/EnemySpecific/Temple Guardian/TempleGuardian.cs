using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleGuardian : Entity
{
    public TempleGuardian_MoveState moveState { get; private set; }
    public TempleGuardian_IdleState idleState { get; private set; }
    public TempleGuardian_PlayerDetectedState playerDetectedState { get; private set; }
    public TempleGuardian_MeleeAttackState meleeAttackState { get; private set; }
    public TempleGuardian_LookForPlayerState lookForPlayerState { get; private set; }
    // public TempleGuardian_StunState stunState { get; private set; }
    // public TempleGuardian_DeadState deadState { get; private set; }
    // public TempleGuardian_DodgeState dodgeState { get; private set; }


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
    //[SerializeField]
    //private D_StunState stunStateData;
    //[SerializeField]
    //private D_DeadState deadStateData;
    //[SerializeField]
    //public D_DodgeState dodgeStateData;

    [SerializeField]
    private Transform meleeAttackPosition;
    //[SerializeField]
    //private Transform playerPosition;

    private Stats Stats => stats ? stats : Core.GetCoreComponent(ref stats);
    private Stats stats;

    public override void Awake()
    {
        base.Awake();

        moveState = new TempleGuardian_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new TempleGuardian_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new TempleGuardian_PlayerDetectedState(this, stateMachine, "playerDetected", playerDetectedStateData, this);
        meleeAttackState = new TempleGuardian_MeleeAttackState(this, stateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData, this);
        lookForPlayerState = new TempleGuardian_LookForPlayerState(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        // stunState = new TempleGuardian_StunState(this, stateMachine, "stunned", stunStateData, this);
        // deadState = new TempleGuardian_DeadState(this, stateMachine, "dead", deadStateData, this);
        // dodgeState = new TempleGuardian_DodgeState(this, stateMachine, "dodge", dodgeStateData, this);
    }

    private void Start()
    {
        stateMachine.Initialize(moveState);

        // Stats.Health.OnCurrentValueZero += () => stateMachine.ChangeState(deadState);
        // Stats.Health.OnCurrentValueBelowHalf += () => stateMachine.ChangeState(arrowRainState);
        // Stats.Health.OnCurrentValueBelowQuarter += () => stateMachine.ChangeState(arrowRainState);
    }

    private void OnDestroy()
    {
        // Stats.Health.OnCurrentValueZero -= () => stateMachine.ChangeState(deadState);
        // Stats.Health.OnCurrentValueBelowHalf -= () => stateMachine.ChangeState(arrowRainState);
        // Stats.Health.OnCurrentValueBelowQuarter -= () => stateMachine.ChangeState(arrowRainState);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);
    }
}
