using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Entity
{
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
    public Ranger_ArrowRainState arrowRainState { get; private set; }

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
    public D_ArrowRainState arrowRainStateData;

    [SerializeField]
    private Transform meleeAttackPosition;
    [SerializeField]
    private Transform rangedAttackPosition;
    [SerializeField]
    private Transform beamAttackPosition;
    [SerializeField]
    private Transform playerPosition;

    private Stats Stats => stats ? stats : Core.GetCoreComponent(ref stats);
    private Stats stats;

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
        arrowRainState = new Ranger_ArrowRainState(this, stateMachine, "arrowRain", playerPosition, arrowRainStateData, this);
    }

    private void Start()
    {
        stateMachine.Initialize(moveState);

        Stats.Health.OnCurrentValueZero += () => stateMachine.ChangeState(deadState);
        Stats.Health.OnCurrentValueBelowHalf += () => stateMachine.ChangeState(arrowRainState);
    }

    private void OnDestroy()
    {
        Stats.Health.OnCurrentValueZero -= () => stateMachine.ChangeState(deadState);
        Stats.Health.OnCurrentValueBelowHalf -= () => stateMachine.ChangeState(arrowRainState);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);
        Gizmos.DrawWireCube(beamAttackPosition.position, beamAttackStateData.beamDimensions);
    }
}
