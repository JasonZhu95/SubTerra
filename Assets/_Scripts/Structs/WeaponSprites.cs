using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct WeaponSprites : INameable
{
    [HideInInspector] public string AttackName;

    public AttackPhaseSprites[] AttackPhases;

    public void SetName(string value)
    {
        AttackName = value;
    }
}

[Serializable]
public struct AttackPhaseSprites
{
    [HideInInspector] public string Name;
    public WeaponAttackPhases Phase;
    public Sprite[] Sprites;
}
