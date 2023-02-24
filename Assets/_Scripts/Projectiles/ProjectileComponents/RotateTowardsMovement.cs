using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsMovement : ProjectileComponent<RotateTowardsMovementData>
{
    private Rigidbody2D rb;

    private void FixedUpdate()
    {
        if (rb.velocity == Vector2.zero)
            return;

        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}

public class RotateTowardsMovementData : ProjectileComponentData
{
    public RotateTowardsMovementData()
    {
        ComponentDependencies.Add(typeof(RotateTowardsMovement));
    }
}