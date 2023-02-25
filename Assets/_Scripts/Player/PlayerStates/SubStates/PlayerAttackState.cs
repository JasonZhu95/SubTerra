using Project.Weapons;
using Project.EventChannels;

public class PlayerAttackState : PlayerAbilityState
{
    private readonly Weapon weapon;

    private readonly CombatInputs inputIndex;

    private int xInput;

    private bool primaryInput;
    private bool secondaryInput;

    private bool shouldCheckFlip;
    private bool canInterrupt;

    private WeaponChangedEventChannel weaponChangedEventChannel;

    public PlayerAttackState(
        Player player,
        PlayerStateMachine stateMachine,
        PlayerData playerData,
        string animBoolName,
        CombatInputs inputIndex,
        Weapon weapon,
        WeaponChangedEventChannel inventoryChannel
    ) : base(player, stateMachine, playerData, animBoolName)
    {
        this.inputIndex = inputIndex;
        this.weapon = weapon;

        weaponChangedEventChannel = inventoryChannel;
        weaponChangedEventChannel.OnEvent += HandleWeaponChange;

        weapon.OnExit += AnimationFinishTrigger;
        weapon.AnimEventHandler.OnEnableFlip += EnableFlip;
        weapon.AnimEventHandler.OnDisableFlip += DisableFlip;
        weapon.AnimEventHandler.OnEnableInterrupt += SetCanInterrupt;
    }

    public override void Enter()
    {
        base.Enter();

        shouldCheckFlip = true;
        canInterrupt = false;

        player.InputHandler.UseAttackInput(inputIndex);
        weapon.SetInput(player.InputHandler.AttackInputsHold[(int)inputIndex]);
        weapon.Enter();
        Movement.SetVelocityX(0f);
    }

    public override void Exit()
    {
        base.Exit();

        weapon.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        primaryInput = player.InputHandler.AttackInputs[(int)CombatInputs.primary];
        secondaryInput = player.InputHandler.AttackInputs[(int)CombatInputs.secondary];

        weapon.SetInput(player.InputHandler.AttackInputsHold[(int)inputIndex]);

        if (shouldCheckFlip)
        {
            Movement?.CheckIfShouldFlip(xInput);
        }

        weapon.Tick();

        if (!canInterrupt) return;

        if (primaryInput || secondaryInput || xInput != 0)
        {
            isAbilityDone = true;
        }
    }

    public bool CheckIfCanAttack() => weapon.WeaponData != null;

    private void SetCanInterrupt() => canInterrupt = true;

    public void HandleWeaponChange(object sender, WeaponChangedEventArgs context)
    {
        if (context.WeaponInput == inputIndex)
        {
            if (stateMachine.CurrentState == this)
                stateMachine.ChangeState(player.IdleState);

            weapon.Init(core);
            weapon.WeaponData = context.WeaponData;
        }
    }

    private void EnableFlip() => shouldCheckFlip = true;
    private void DisableFlip() => shouldCheckFlip = false;

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        isAbilityDone = true;
    }

}
