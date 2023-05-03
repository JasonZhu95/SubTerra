﻿using System.Collections;
using UnityEngine;
using Cinemachine;
using Project.Managers;
using Project.LevelSetup;
using System;

public class Death : CoreComponent
{
    [SerializeField] private GameObject[] deathParticles;

    [SerializeField] private GameObject coinObject;
    private BossManager bossManager;

    private Stats Stats;

    private ParticleManager ParticleManager { get => particleManager ?? core.GetCoreComponent(ref particleManager); }
    private ParticleManager particleManager;

    private RespawnManager respawnManager;
    private LevelLoaderManager levelLoaderManager;

    private int coinWorkspace;
    private int coinWorkspaceAmount;

    private bool subscribedToEvent = false;

    public event Action OnDeath;

    protected override void Awake()
    {
        respawnManager = GameObject.Find("Managers").transform.Find("RespawnManager").GetComponent<RespawnManager>();
        levelLoaderManager = GameObject.Find("LevelLoader").GetComponent<LevelLoaderManager>();
        bossManager = GameObject.Find("Managers").transform.Find("GameManager").GetComponent<BossManager>();
    }

    public override void Init(Core core)
    {
        base.Init(core);
        Stats = core.GetCoreComponent(ref Stats);

        if (!subscribedToEvent)
        {
            Stats.Health.OnCurrentValueZero += Die;
            subscribedToEvent = true;
        }
    }

    public void Die()
    {
        foreach (var particle in deathParticles)
        {
            ParticleManager.StartParticles(particle);
        }

        if (core.Parent.name == "Player")
        {
            OnDeath?.Invoke();
            respawnManager.PlayerDeathSwitchActive(true);
            levelLoaderManager.PlayTransition();
            Stats.Health.Increase(Stats.Health.MaxValue);
        }
        else if (core.Parent.name == "Demon King")
        {
            StartCoroutine(DemonDefeated(2.0f));
        }
        else if (core.Parent.name == "Ranger")
        {
            StartCoroutine(RangerDefeated(1.8f));
        }
        else if (core.Parent.name == "Temple Guardian")
        {
            StartCoroutine(TempleDefeated(3.3f));
        }
        else
        {
            InstantiateCoins();
            core.Parent.SetActive(false);
        }
    }

    public void DeathZoneDamage()
    {
        foreach (var particle in deathParticles)
        {
            ParticleManager.StartParticles(particle);
        }

        if (core.Parent.name == "Player")
        {
            levelLoaderManager.PlayTransition();
            respawnManager.PlayerDeathSwitchActive(false);
            if (Stats.Health.CurrentValue == 1f)
            {
                Die();
            }
            else
            {
                Stats.Health.Decrease(1f);
            }
        }
        else
        {
            InstantiateCoins();
            core.Parent.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (Stats && !subscribedToEvent)
        {
            Stats.Health.OnCurrentValueZero += Die;
            subscribedToEvent = true;
        }
    }

    private void OnDisable()
    {
        Stats.Health.OnCurrentValueZero -= Die;
        subscribedToEvent = false;
    }

    private void InstantiateCoins()
    {
        coinWorkspace = Stats.CoinDeathValue;
        coinWorkspaceAmount = coinWorkspace / 10;
        coinWorkspace = coinWorkspace % 10;
        LoopCoinAmount(coinWorkspaceAmount, 10);

        coinWorkspaceAmount = coinWorkspace / 5;
        coinWorkspace = coinWorkspace % 5;
        LoopCoinAmount(coinWorkspaceAmount, 5);

        LoopCoinAmount(coinWorkspace, 1);

    }

    private void LoopCoinAmount(int amount, int coinValue)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject instance = Instantiate(coinObject, gameObject.transform.position, Quaternion.identity);
            instance.GetComponent<CoinValueSet>().SetCoinValue(coinValue);
        }
    }

    // COROUTINES
    private IEnumerator DemonDefeated(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        bossManager.DemonBossDefeated = true;
        InstantiateCoins();
    }
    private IEnumerator RangerDefeated(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        bossManager.RangerBossDefeated = true;
        InstantiateCoins();
    }
    private IEnumerator TempleDefeated(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        bossManager.TempleBossDefeated = true;
        InstantiateCoins();
    }
}
