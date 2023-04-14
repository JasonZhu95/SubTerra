using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Entity
{
    private Stats Stats => stats ? stats : Core.GetCoreComponent(ref stats);
    private Stats stats;

    public Ranger_MoveState moveState { get; private set; }
    public Ranger_IdleState idleState { get; private set; }
    public Ranger_PlayerDetectedState playerDetectedState { get; private set; }
    public Ranger_MeleeAttackState meleeAttackState { get; private set; }
    public Ranger_LookForPlayerState lookForPlayerState { get; private set; }
    public Ranger_StunState stunState { get; private set; }
    public Ranger_DeadState deadState { get; private set; }
    public Ranger_DodgeState dodgeState { get; private set; }
    public Ranger_RangedAttackState rangedAttackState { get; private set; }
    public Ranger_BeamAttackState beamAttackState { get; private set; }

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
    public D_DodgeState dodgeStateData;
    [SerializeField]
    public D_RangedAttackState rangedAttackStateData;
    [SerializeField]
    public D_BeamAttackState beamAttackStateData;

    [SerializeField]
    private Transform meleeAttackPosition;
    [SerializeField]
    private Transform rangedAttackPosition;
    [SerializeField]
    private Transform beamAttackPosition;

    public override void Awake()
    {
        base.Awake();

        moveState = new Ranger_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new Ranger_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new Ranger_PlayerDetectedState(this, stateMachine, "playerDetected", playerDetectedStateData, this);
        meleeAttackState = new Ranger_MeleeAttackState(this, stateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData, this);
        lookForPlayerState = new Ranger_LookForPlayerState(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        stunState = new Ranger_StunState(this, stateMachine, "stunned", stunStateData, this);
        deadState = new Ranger_DeadState(this, stateMachine, "dead", deadStateData, this);
        dodgeState = new Ranger_DodgeState(this, stateMachine, "dodge", dodgeStateData, this);
        rangedAttackState = new Ranger_RangedAttackState(this, stateMachine, "rangedAttack", rangedAttackPosition, rangedAttackStateData, this);
        beamAttackState = new Ranger_BeamAttackState(this, stateMachine, "beamAttack", beamAttackPosition, beamAttackStateData, this);
    }

    private void Start()
    {
        stateMachine.Initialize(moveState);
    }

    //public override void Damage(AttackDetails attackDetails)
    //{
    //    base.Damage(attackDetails);

    //    if (isDead)
    //    {
    //        stateMachine.ChangeState(deadState);
    //    }
    //    else if (isStunned && stateMachine.currentState != stunState)
    //    {
    //        stateMachine.ChangeState(stunState);
    //    }
    //    else if (CheckPlayerInMinAggroRange())
    //    {
    //        stateMachine.ChangeState(rangedAttackState);
    //    }
    //    else if (!CheckPlayerInMinAggroRange())
    //    {
    //        lookForPlayerState.SetTurnImmediately(true);
    //        stateMachine.ChangeState(lookForPlayerState);
    //    }
    //}

    private void Die()
    {
        stateMachine.ChangeState(deadState);
    }

    private void OnEnable()
    {
        //Stats.OnHealthZero += Die;
    }

    private void OnDisable()
    {
        //Stats.OnHealthZero -= Die;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);
        Gizmos.DrawWireCube(beamAttackPosition.position, beamAttackStateData.beamDimensions);
    }
}