using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IHitbox
{
    event Action<RaycastHit2D[]> OnDetected;
}
