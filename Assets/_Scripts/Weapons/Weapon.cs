using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    #region Variables
    [field: SerializeField] public WeaponDataSO Data {get; private set;}
    [SerializeField] private float attackCounterResetCooldown;

    public event Action OnExit;
    public event Action OnEnter;

    private Animator anim;
    public AnimationEventHandler EventHandler { get; private set; }

    public GameObject BaseGameObject { get; private set; }
    public GameObject WeaponSpriteGameObject { get; private set; }

    public Core Core { get; private set; }

    private int currentAttackCounter;

    private Timer attackCounterResetTimer;

    #endregion

    #region State Functions
    public void Enter()
    {
        Debug.Log($"{transform.name}: Entered");

        anim.SetBool("active", true);
        anim.SetInteger("counter", CurrentAttackCounter);
        attackCounterResetTimer.StopTimer();

        OnEnter?.Invoke();
    }

    private void Exit()
    {
        anim.SetBool("active", false);
        CurrentAttackCounter++;
        attackCounterResetTimer.StartTimer();

        OnExit?.Invoke();
    }
    #endregion

    #region Unity Callback Functions
    private void Awake()
    {
        BaseGameObject = transform.Find("Base").gameObject;
        WeaponSpriteGameObject = transform.Find("WeaponSprite").gameObject;

        anim = BaseGameObject.GetComponent<Animator>();
        EventHandler = BaseGameObject.GetComponent<AnimationEventHandler>();

        attackCounterResetTimer = new Timer(attackCounterResetCooldown);
    }

    private void Update()
    {
        attackCounterResetTimer.Tick();
    }

    private void OnEnable()
    {
        EventHandler.OnFinish += Exit;
        attackCounterResetTimer.OnTimerDone += ResetAttackCounter;
    }

    private void OnDisable()
    {
        EventHandler.OnFinish -= Exit;
        attackCounterResetTimer.OnTimerDone -= ResetAttackCounter;
    }
    #endregion

    #region Other Functions
    public void SetCore(Core core)
    {
        Core = core;
    }

    public int CurrentAttackCounter
    {
        get => currentAttackCounter;
        private set => currentAttackCounter = value >= Data.NumberOfAttacks ? 0 : value;
    }

    private void ResetAttackCounter() => CurrentAttackCounter = 0;
    #endregion
}
