using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public FiniteStateMachine stateMachine;

    public D_Entity entityData;

    public Animator anim { get; private set; }
    public AnimationToStatemachine atsm { get; private set; }
    public int lastDamageDirection { get; private set; }
    public Core Core { get; private set; }

    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Transform ledgeCheck;
    [SerializeField]
    private Transform playerCheck;
    [SerializeField]
    private Transform groundCheck;

    private float currentHealth;
    private float currentStunResistance;
    private float lastDamageTime;

    private Vector2 velocityWorkspace;

    protected bool isStunned;
    protected bool isDead;

    protected Movement Movement { get => movement ?? Core.GetCoreComponent(ref movement); }
    protected Movement movement;

    private Stats Stats => stats ? stats : Core.GetCoreComponent(ref stats);
    private Stats stats;

    public virtual void Awake()
    {
        Core = GetComponentInChildren<Core>();

        currentHealth = Stats.Health.MaxValue;
        currentStunResistance = entityData.stunResistance;

        anim = GetComponent<Animator>();
        atsm = GetComponent<AnimationToStatemachine>();

        stateMachine = new FiniteStateMachine();
    }

    public virtual void Update()
    {
        Core.LogicUpdate();
        stateMachine.currentState.LogicUpdate();

        anim.SetFloat("yVelocity", Movement.RB.velocity.y);

        if (Time.time >= lastDamageTime + entityData.stunRecoveryTime)
        {
            ResetStunResistance();
        }
    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    public virtual bool CheckPlayerInMinAggroRange()
    {
        return Physics2D.Raycast(playerCheck.position, transform.right, entityData.minAgroDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInMaxAggroRange()
    {
        return Physics2D.Raycast(playerCheck.position, transform.right, entityData.maxAgroDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.Raycast(playerCheck.position, transform.right, entityData.closeRangeActionDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInMidRangeAction()
    {
        return Physics2D.Raycast(playerCheck.position, transform.right, entityData.midRangeActionDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInCircleRange()
    {
        return Physics2D.OverlapCircle(playerCheck.position, entityData.circleAgroDistance);
    }

    public virtual void DamageHop(float velocity)
    {
        velocityWorkspace.Set(Movement.RB.velocity.x, velocity);
        Movement.RB.velocity = velocityWorkspace;
    }

    public virtual void ResetStunResistance()
    {
        isStunned = false;
        currentStunResistance = entityData.stunResistance;
    }

    public virtual void ResetHealth()
    {
        Stats.Health.SetHealth(Stats.Health.MaxValue);
    }

    public virtual void ResetPosition()
    {
        transform.position = entityData.spawnPosition;
    }

    public virtual void DisableBoxColliders2D(GameObject parentObject)
    {
        BoxCollider2D parentCollider = parentObject.GetComponent<BoxCollider2D>();
        if (parentCollider != null)
        {
            parentCollider.enabled = false;
        }

        // Disable BoxCollider2D components in all child objects recursively
        foreach (Transform child in parentObject.transform)
        {
            DisableBoxColliders2D(child.gameObject);
        }
    }

    public virtual void OnDrawGizmos()
    {
        if (Core != null)
        {
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * Movement.FacingDirection * entityData.wallCheckDistance));   // check wall
            Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * entityData.ledgeCheckDistance));                            // check ledge

            Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * Movement.FacingDirection * entityData.closeRangeActionDistance), 0.2f);  // check close range action
            Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * Movement.FacingDirection * entityData.minAgroDistance), 0.2f);           // check min agro
            Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * Movement.FacingDirection * entityData.midRangeActionDistance), 0.2f);    // check mid agro
            Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * Movement.FacingDirection * entityData.maxAgroDistance), 0.2f);           // check max agro
        }
    }
}
