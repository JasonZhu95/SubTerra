using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DirectTowardsAccessibleTarget : ComponentModifier<DirectTowardsAccessibleTargetData>
{
    private Rigidbody2D rb;

    private AccessibleTargetModifier targets;
    private Transform target;

    private float rotationSpeed;
    private float startTime;

    protected override void Init()
    {
        base.Init();

        Data = Projectile.Data.GetComponentData<DirectTowardsAccessibleTargetData>();

        rotationSpeed = Data.StartRotSpeed;
        startTime = Time.time;

        DetermineTarget();
    }

    private void DetermineTarget()
    {
        if (!isActive || !modifiers.TryGetModifier(out targets)) return;

        float minDist = Mathf.Infinity;

        foreach (Collider2D item in targets.ModifierValue)
        {

            float distance = Vector2.Distance(transform.position, item.transform.position);
            if (distance < minDist)
            {
                minDist = distance;
                target = item.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!target)
        {
            rb.angularVelocity = 0f;
            return;
        }

        SetRotationSpeed();

        Vector3 desiredDir = target.position - transform.position;
        desiredDir.Normalize();

        float rotateAmount = Vector3.Cross(desiredDir, transform.right).z;

        rb.angularVelocity = -rotateAmount * rotationSpeed;
    }

    private void SetRotationSpeed()
    {
        rotationSpeed = Data.StartRotSpeed +
                        Data.evaluationCurve.Evaluate((Time.time - startTime) / Data.TimeToMaxRotSpeed) *
                        (Data.EndRotSpeed - Data.StartRotSpeed);
    }

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
    }
}

public class DirectTowardsAccessibleTargetData : ProjectileComponentData
{
    public float TimeToMaxRotSpeed = 1f;
    public float StartRotSpeed = 0f;
    public float EndRotSpeed = 10f;
    public AnimationCurve evaluationCurve;

    public DirectTowardsAccessibleTargetData()
    {
        ComponentDependencies.Add(typeof(DirectTowardsAccessibleTarget));
        ComponentDependencies.Add(typeof(ProjectileModifiers));
    }
}
