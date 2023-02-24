using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponModifiers : WeaponComponent<WeaponModifierData>
{
    [SerializeReference] private List<AttackModifier> modifiers = new List<AttackModifier>();
    public List<AttackModifier> Modifiers { get => modifiers; private set => modifiers = value; }

    private AttackRanged attackRanged;

    public bool TryGetModifier<T>(out T comp) where T : AttackModifier
    {
        comp = Modifiers.OfType<T>().FirstOrDefault();
        return comp != null;
    }

    public void AddModifier<T>(T value) where T : AttackModifier
    {
        if (Modifiers.OfType<T>().FirstOrDefault() == null)
        {
            Modifiers.Add(value);
        }
    }

    private void ResetModifiers() => Modifiers.Clear();
    private void SendModifiersToProjectiles(GameObject proj)
    {
        if (proj.TryGetComponent(out ProjectileModifiers comp))
        {
            comp.SetModifiers(Modifiers);
        }
    }

    public override void SetReferences()
    {
        base.SetReferences();
        attackRanged = GetComponent<AttackRanged>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        weapon.OnExit += ResetModifiers;

        if (attackRanged) attackRanged.OnProjectileSpawned += SendModifiersToProjectiles;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        weapon.OnExit += ResetModifiers;

        if (attackRanged) attackRanged.OnProjectileSpawned -= SendModifiersToProjectiles;
    }
}

public class WeaponModifierData : WeaponComponentData
{
    public WeaponModifierData()
    {
        ComponentDependencies.Add(typeof(WeaponModifiers));
    }
}