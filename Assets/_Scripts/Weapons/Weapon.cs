using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    [SerializeField] private int numberOfAttacks;
    [SerializeField] private float attackCounterResetCooldown;

    public int CurrentAttackCounter
    {
        get => currentAttackCounter;
        private set => currentAttackCounter = value >= numberOfAttacks ? 0 : value; 
    }

    public event Action OnExit;
    public event Action OnEnter;

    private Animator anim;
    private AnimationEventHandler eventHandler;

    public GameObject BaseGameObject { get; private set; }
    public GameObject WeaponSpriteGameObject { get; private set; }

    private int currentAttackCounter;

    private Timer attackCounterResetTimer;

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

    private void Awake()
    {
        BaseGameObject = transform.Find("Base").gameObject;
        WeaponSpriteGameObject = transform.Find("WeaponSprite").gameObject;

        anim = BaseGameObject.GetComponent<Animator>();
        eventHandler = BaseGameObject.GetComponent<AnimationEventHandler>();

        attackCounterResetTimer = new Timer(attackCounterResetCooldown);
    }

    private void Update()
    {
        attackCounterResetTimer.Tick();
    }

    private void ResetAttackCounter() => CurrentAttackCounter = 0;

    private void OnEnable()
    {
        eventHandler.OnFinish += Exit;
        attackCounterResetTimer.OnTimerDone += ResetAttackCounter;
    }

    private void OnDisable()
    {
        eventHandler.OnFinish -= Exit;
        attackCounterResetTimer.OnTimerDone -= ResetAttackCounter;
    }
}
