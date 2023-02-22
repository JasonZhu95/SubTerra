using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : ProjectileComponent<ProjectileMovementData>
{
    private Rigidbody2D rb;

    protected override void Init()
    {
        Data = Projectile.Data.GetComponentData<ProjectileMovementData>();
        rb.velocity = Data.Velocity * transform.right;
    }

    private void FixedUpdate()
    {
        if (Data.ApplyContinuously)
        {
            rb.velocity = Data.Velocity * transform.right;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
    }
}

public class ProjectileMovementData : ProjectileComponentData
{
    public ProjectileMovementData() : base()
    {
        ComponentDependencies.Add(typeof(ProjectileMovement));
    }

    public bool ApplyContinuously;
    public float Velocity;
}
