using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtilities
{
    public static float AngleFromVector2(Vector2 velocity)
    {
        velocity.Normalize();
        return Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
    }

    public static Vector2 Vector2FromAngle(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    public static Vector2 RotateVector2(Vector2 vector)
    {
        float angle = AngleFromVector2(vector);

        float x2 = Mathf.Cos(angle) * vector.x - Mathf.Sin(angle) * vector.y;
        float y2 = Mathf.Sin(angle) * vector.x + Mathf.Cos(angle) * vector.y;

        return new Vector2(x2, y2);
    }
}

public static class LayerMaskUtilities
{
    public static bool IsLayerInLayerMask(RaycastHit2D hit, LayerMask mask) =>
        IsLayerInLayerMask(hit.collider.gameObject.layer, mask);

    public static bool IsLayerInLayerMask(int layer, LayerMask mask) => ((1 << layer) & mask) > 0;
}