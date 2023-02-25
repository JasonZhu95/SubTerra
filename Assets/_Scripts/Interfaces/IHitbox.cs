using UnityEngine;
using System;

namespace Project.Combat.Interfaces
{
    public interface IHitbox
    {
        event Action<RaycastHit2D[]> OnDetected;
    }
}
