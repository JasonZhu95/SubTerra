using System.Collections;
using UnityEngine;
using Cinemachine;
using Project.Managers;
using Project.LevelSetup;

public class Death : CoreComponent
{
    [SerializeField] private GameObject[] deathParticles;

    private Stats Stats;

    private ParticleManager ParticleManager { get => particleManager ?? core.GetCoreComponent(ref particleManager); }
    private ParticleManager particleManager;

    private RespawnManager respawnManager;
    private LevelLoaderManager levelLoaderManager;

    protected override void Awake()
    {
        respawnManager = GameObject.Find("Managers").transform.Find("RespawnManager").GetComponent<RespawnManager>();
        levelLoaderManager = GameObject.Find("LevelLoader").GetComponent<LevelLoaderManager>();
    }

    public override void Init(Core core)
    {
        base.Init(core);
        Stats = core.GetCoreComponent(ref Stats);

        Stats.Health.OnCurrentValueZero += Die;
    }

    public void Die()
    {
        foreach (var particle in deathParticles)
        {
            ParticleManager.StartParticles(particle);
        }

        if (core.Parent.name == "Player")
        {
            levelLoaderManager.PlayTransition();
            respawnManager.PlayerDeathSwitchActive(true);
            Stats.Health.Increase(Stats.Health.MaxValue);
        }
        else
        {
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
            if (Stats.Health.CurrentValue == 10f)
            {
                Die();
            }
            else
            {
                Stats.Health.Decrease(10f);
            }
            //TODO: Hardcoded Health decrease.  Change to heart system
        }
        else
        {
            core.Parent.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (Stats)
        {
            Stats.Health.OnCurrentValueZero += Die;
        }
    }

    private void OnDisable()
    {
        Stats.Health.OnCurrentValueZero -= Die;
    }
}
