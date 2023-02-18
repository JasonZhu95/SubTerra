using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveWeapon : Weapon
{
    private List<IDamageable> detectedDamageable = new List<IDamageable>();

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

    }


}
