using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Weapons;
using Project.EventChannels;
using Project.StateMachine;
using Project.Interfaces;

// Class that creates state objects
public class Player : MonoBehaviour, IDataPersistence
{
    #region State Variables
    public PlayerStateMachine StateMachine { get; private set; }

    // State Objects
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallGrabState WallGrabState { get; private set; }
    public PlayerWallClimbState WallClimbState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerLedgeClimbState LedgeClimbState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerCrouchIdleState CrouchIdleState { get; private set; }
    public PlayerCrouchMoveState CrouchMoveState { get; private set; }
    public PlayerAttackState PrimaryAttackState { get; private set; }
    public PlayerAttackState SecondaryAttackState { get; private set; }
    public PlayerStunState StunState { get; private set; }

    [SerializeField]
    private PlayerData playerData;
    [SerializeField] private WeaponChangedEventChannel inventoryChannel;
    #endregion

    #region Components

    public Core Core { get; private set; }
    public Animator Anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public BoxCollider2D MovementCollider { get; private set; }
    public PlayerObstacleCollision playerObstacleCollision { get; private set; }
    public SoundManager playerSoundManager { get; private set; }

    private CollisionSenses CollisionSenses { get => collisionSenses ?? Core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    private Movement Movement => movement ? movement : Core.GetCoreComponent(ref movement);
    private Movement movement;

    private Interaction Interaction => interaction ? interaction : Core.GetCoreComponent(ref interaction);
    private Interaction interaction;


    #endregion

    #region Other Variables
    public bool WallJumpUpCheck { get; private set; }
    public bool PlayerFacingDirection { get; private set; }

    private Vector2 workspace;

    private Weapon primaryWeapon;
    private Weapon secondaryWeapon;
    #endregion

    #region Disable State Variables

    public bool DisableDash { get; set; } = true;
    public bool DisableWallJump { get; set; } = true;
    public bool DisableWallClimb { get; set; } = true;
    public bool DisableWallSlide { get; set; } = true;
    public bool DisableWallGrab { get; set; } = true;

    #endregion

    #region Unity Callback Functions

    private void Awake()
    {
        Core = GetComponentInChildren<Core>();
        playerSoundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        primaryWeapon = transform.Find("PrimaryWeapon").GetComponent<Weapon>();
        secondaryWeapon = transform.Find("SecondaryWeapon").GetComponent<Weapon>();

        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, playerData, "land");
        WallSlideState = new PlayerWallSlideState(this, StateMachine, playerData, "wallSlide");
        WallGrabState = new PlayerWallGrabState(this, StateMachine, playerData, "wallGrab");
        WallClimbState = new PlayerWallClimbState(this, StateMachine, playerData, "wallClimb");
        WallJumpState = new PlayerWallJumpState(this, StateMachine, playerData, "inAir");
        LedgeClimbState = new PlayerLedgeClimbState(this, StateMachine, playerData, "ledgeClimbState");
        DashState = new PlayerDashState(this, StateMachine, playerData, "inAir");
        CrouchIdleState = new PlayerCrouchIdleState(this, StateMachine, playerData, "crouchIdle");
        CrouchMoveState = new PlayerCrouchMoveState(this, StateMachine, playerData, "crouchMove");
        PrimaryAttackState =
            new PlayerAttackState(this, StateMachine, playerData, "attack", CombatInputs.primary, primaryWeapon,
                inventoryChannel);
        SecondaryAttackState = new PlayerAttackState(this, StateMachine, playerData, "attack", CombatInputs.secondary,
            secondaryWeapon, inventoryChannel);
        StunState = new PlayerStunState(this, StateMachine, playerData, "stun");
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();
        MovementCollider = GetComponent<BoxCollider2D>();
        playerObstacleCollision = GetComponent<PlayerObstacleCollision>();

        InputHandler.OnInteract += Interaction.TriggerInteraction;

        StateMachine.Initialize(IdleState);
    }

    private void OnDestroy()
    {
        inventoryChannel.OnEvent -= PrimaryAttackState.HandleWeaponChange;
        inventoryChannel.OnEvent -= SecondaryAttackState.HandleWeaponChange;
    }

    private void Update()
    {
        Core.LogicUpdate();
        StateMachine.CurrentState.LogicUpdate();
        if (CollisionSenses.Trampoline)
        {
            DashState.ResetCanDash();
        }
        if (Movement.FacingDirection == 1)
        {
            PlayerFacingDirection = true;
        }
        else
        {
            PlayerFacingDirection = false;
        }
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

    #region Set Functions

    public void SetWallJumpCheck() => WallJumpUpCheck = true;

    public void SetWallJumpCheckFalse() => WallJumpUpCheck = false;

    // FUNCTION: Changes player collider height during crouch
    public void SetColliderHeight(float height)
    {
        Vector2 center = MovementCollider.offset;
        workspace.Set(MovementCollider.size.x, height);

        center.y += (height - MovementCollider.size.y) / 2;

        MovementCollider.size = workspace;
        MovementCollider.offset = center;
    }
    #endregion

    #region Other Functions
    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    private void AnimationWalkTrigger()
    {
        int randomInt = Random.Range(1, 4);
        playerSoundManager.Play("PlayerWalk" + randomInt);
    }

    private void AnimationLedgeClimbUpTrigger()
    {
        playerSoundManager.Play("PlayerLedgeClimbUp");
    }

    private void AnimationCrouchMoveTrigger()
    {
        playerSoundManager.Play("PlayerCrouch");
    }

    private void AnimationWallClimbTrigger()
    {
        playerSoundManager.Play("PlayerWallClimb");
    }

    #endregion

    #region Data Persistence Save

    public void LoadData(GameData data)
    {
        this.transform.position = data.checkPointPosition;
    }

    public void SaveData(GameData data)
    {
    }

    #endregion
}
