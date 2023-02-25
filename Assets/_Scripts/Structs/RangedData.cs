using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Weapons
{
    [System.Serializable]
    public struct RangedData : INameable
    {
        [HideInInspector]
        public string AttackName;
        public bool debug;
        public WeaponProjectileSpawnPoint[] AttackData;

        public void SetName(string value) => AttackName = value;
    }
}
