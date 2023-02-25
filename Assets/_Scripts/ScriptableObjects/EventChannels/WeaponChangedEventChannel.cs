using System;
using Project.Weapons;
using UnityEngine;

namespace Project.EventChannels
{
    [CreateAssetMenu(fileName = "newWeaponChangedChannel", menuName = "Event Channels/Weapon Changes")]
    public class WeaponChangedEventChannel : EventChannelsSO<WeaponChangedEventArgs>{ }

    public class WeaponChangedEventArgs : EventArgs
    {
        public WeaponDataSO WeaponData;
        public CombatInputs WeaponInput;

        public WeaponChangedEventArgs(WeaponDataSO weaponData, CombatInputs weaponInput)
        {
            WeaponData = weaponData;
            WeaponInput = weaponInput;
        }
    }
}