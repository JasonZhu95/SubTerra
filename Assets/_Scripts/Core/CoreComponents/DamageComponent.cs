using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Interfaces;
using Project.Modifiers;
using Project.Weapons;
using UnityEngine;

public class DamageComponent : CoreComponent, IDamageable
{
    public event Action<GameObject> OnDamage;

    [SerializeField] private GameObject damageParticles;
    [SerializeField] private string damageSfxToPlay;

    public ModifierContainer<DamageModifier, DamageData> DamageModifiers { get; private set; } =
        new ModifierContainer<DamageModifier, DamageData>();

    private Stats Stats => stats ? stats : core.GetCoreComponent(ref stats);
    private ParticleManager ParticleManager => particleManager ?? core.GetCoreComponent(ref particleManager);

    private Stats stats;
    private ParticleManager particleManager;

    private GameObject player;
    private GameObject enemyCollision;
    [SerializeField]
    private SpriteRenderer sr = null;
    [SerializeField]
    private SpriteRenderer primarySR = null;
    [SerializeField]
    private SpriteRenderer secondarySR = null;
    private Color color;

    [SerializeField] private float invincibilityDuration = 1.5f;

    private GameObject playerCombatComponent;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.Find("Player");
        sr = player.GetComponent<SpriteRenderer>();
        color = sr.material.color;
        playerCombatComponent = player.transform.GetChild(0).GetChild(2).gameObject;
        enemyCollision = player.transform.GetChild(0).GetChild(7).gameObject;
    }

    private void OnEnable()
    {
        player.layer = LayerMask.NameToLayer("Player");
        enemyCollision.layer = LayerMask.NameToLayer("Default");
    }

    public void Damage(DamageData data)
    {
        if (core.Parent.name == "Player")
        {
            StartCoroutine(BecomeInvincible());
        }

        OnDamage?.Invoke(data.Source);

        var modifiedData = DamageModifiers.ApplyModifiers(data);

        if (modifiedData.DamageAmount <= 0.0f) return;

        Stats?.Health.Decrease(modifiedData.DamageAmount);
        ParticleManager?.StartParticlesWithRandomRotation(damageParticles);
        if (damageSfxToPlay != null)
        {
            FindObjectOfType<SoundManager>().Play(damageSfxToPlay);
        }

    }

    private IEnumerator BecomeInvincible()
    {
        player.layer = LayerMask.NameToLayer("PlayerInvincible");
        enemyCollision.layer = LayerMask.NameToLayer("PlayerInvincible");
        playerCombatComponent.layer = LayerMask.NameToLayer("PlayerInvincible");

        for (int i = 0; i < 5; i++)
        {
            color.a = 0.5f;
            sr.material.color = color;
            primarySR.material.color = color;
            secondarySR.material.color = color;

            yield return new WaitForSeconds(invincibilityDuration / 10);

            color.a = .75f;
            sr.material.color = color;
            primarySR.material.color = color;
            secondarySR.material.color = color;
            yield return new WaitForSeconds(invincibilityDuration / 10);
        }
        player.layer = LayerMask.NameToLayer("Player");
        enemyCollision.layer = LayerMask.NameToLayer("Default");
        playerCombatComponent.layer = LayerMask.NameToLayer("PlayerDamageable");
        color.a = 1f;
        sr.material.color = color;
        primarySR.material.color = color;
        secondarySR.material.color = color;
    }
}