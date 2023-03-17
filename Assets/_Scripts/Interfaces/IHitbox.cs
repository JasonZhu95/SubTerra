using UnityEngine;
using System;

namespace Project.Interfaces
{
    public interface IHitbox
    {
        event Action<RaycastHit2D[]> OnDetected;
    }
}
