using System.Collections;
using UnityEngine;
using Cinemachine;

public class Death : CoreComponent
{
    [SerializeField] private GameObject[] deathParticles;

    private Stats Stats;

    private ParticleManager ParticleManager { get => particleManager ?? core.GetCoreComponent(ref particleManager); }
    private ParticleManager particleManager;

    public Transform RespawnPoints { get; set; }

    private SpriteRenderer playerSR;
    private Color color;

    private CinemachineVirtualCamera CVC;

    protected override void Awake()
    {
        playerSR = core.transform.parent.gameObject.GetComponent<SpriteRenderer>();
        color = playerSR.material.color;
        CVC = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
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
            color.a = 0.0f;
            playerSR.material.color = color;
            CVC.m_Follow = null;
            StartCoroutine(RespawnPlayer());
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

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(2.0f);

        core.Parent.transform.position = RespawnPoints.position;
        CVC.m_Follow = core.Parent.transform;
        color.a = 1.0f;
        playerSR.material.color = color;
    }
}
