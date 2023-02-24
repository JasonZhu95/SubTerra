using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    private Stats stats;
    private CollisionSenses collisionSenses;
    private Movement movement;
    private ParticleManager particleManager;

    public void Damage(DamageData data)
    {
        OnDamage?.Invoke(data.Source);

        var modifiedData = DamageModifiers.ApplyModifiers(data);
        Debug.Log($"{core.Parent.name} Damage by {modifiedData.DamageAmount}");

    }
}
