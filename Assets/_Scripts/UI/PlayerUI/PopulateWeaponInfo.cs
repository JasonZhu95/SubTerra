using System;
using Project.Weapons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class PopulateWeaponInfo: MonoBehaviour
    {
        [SerializeField] private Image weaponIcon;
        [SerializeField] private TMP_Text weaponName;
        [SerializeField] private TMP_Text weaponDescription;

        public void SetWeaponInfo(WeaponDataSO data)
        {
            weaponIcon.sprite = data.ItemImage;
            weaponName.text = data.Name;
            weaponDescription.text = data.Description;
        }
    }
}
