using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectileHitline : ProjectileComponent<ProjectileHitlineData>, IHitbox
{
    public event Action<RaycastHit2D[]> OnDetected;

    private RaycastHit2D[] hits;

    protected override void Init()
    {
        base.Init();

        Data = Projectile.Data.GetComponentData<ProjectileHitlineData>();

        if (!Data.DoInitialCheck)
            return;

        hits = Physics2D.LinecastAll(transform.position, Projectile.SpawningEntityPos, Data.LayerMask);

        if (hits.Length > 0)
            CheckHits();
    }

    private void FixedUpdate()
    {
        hits = Physics2D.RaycastAll(
            transform.position,
            transform.right,
            Projectile.RB.velocity.magnitude * Time.deltaTime,
            Data.LayerMask
            );

        if (hits.Length > 0)
            CheckHits();
    }

    private void CheckHits()
    {
        OnDetected?.Invoke(hits);
    }
}

public class ProjectileHitlineData : ProjectileComponentData
{
    public LayerMask LayerMask;
    public bool DoInitialCheck = true;

    public ProjectileHitlineData()
    {
        ComponentDependencies.Add(typeof(ProjectileHitline));
    }
}
