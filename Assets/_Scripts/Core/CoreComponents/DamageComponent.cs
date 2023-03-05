using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Combat.Interfaces;
using Project.Modifiers;
using Project.Weapons;
using UnityEngine;

public class DamageComponent : CoreComponent, IDamageable
{
    public event Action<GameObject> OnDamage;

    [SerializeField] private GameObject damageParticles;

    public ModifierContainer<DamageModifier, DamageData> DamageModifiers { get; private set; } =
        new ModifierContainer<DamageModifier, DamageData>();

    private Stats Stats => stats ? stats : core.GetCoreComponent(ref stats);

    private CollisionSenses CollisionSenses => collisionSenses ?? core.GetCoreComponent(ref collisionSenses);

    private Movement Movement => movement ?? core.GetCoreComponent(ref movement);

    private ParticleManager ParticleManager => particleManager ?? core.GetCoreComponent(ref particleManager);

    private Movement movement;
    private Stats stats;
    private CollisionSenses collisionSenses;
    private ParticleManager particleManager;
    private GameObject player;
    private GameObject enemyCollision;
    private SpriteRenderer sr;
    private Color color;

    [SerializeField] private float invincibilityDuration = 1.5f;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.Find("Player");
        sr = player.GetComponent<SpriteRenderer>();
        color = sr.material.color;
        enemyCollision = player.transform.GetChild(0).GetChild(7).gameObject;

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

    }

    private IEnumerator BecomeInvincible()
    {
        player.layer = LayerMask.NameToLayer("PlayerInvincible");
        enemyCollision.layer = LayerMask.NameToLayer("PlayerInvincible");

        for (int i = 0; i < 5; i++)
        {
            color.a = 0.5f;
            sr.material.color = color;

            yield return new WaitForSeconds(invincibilityDuration / 10);

            color.a = .75f;
            sr.material.color = color;
            yield return new WaitForSeconds(invincibilityDuration / 10);
        }

        player.layer = LayerMask.NameToLayer("Player");
        enemyCollision.layer = LayerMask.NameToLayer("Default");
        color.a = 1f;
        sr.material.color = color;
    }
}