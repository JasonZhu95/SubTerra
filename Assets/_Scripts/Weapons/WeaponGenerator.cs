using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WeaponGenerator : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private WeaponDataSO data;

    private List<WeaponComponent> componentAlreadyOnWeapon = new List<WeaponComponent>();

    private List<WeaponComponent> componentsAddedToWeapon = new List<WeaponComponent>();

    private List<Type> componentDependencies = new List<Type>();

    private void Start()
    {
        GenerateWeapon(data);
    }

    public void GenerateWeapon(WeaponDataSO data)
    {
        weapon.SetData(data);

        componentAlreadyOnWeapon.Clear();
        componentsAddedToWeapon.Clear();
        componentDependencies.Clear();

        componentAlreadyOnWeapon = GetComponents<WeaponComponent>().ToList();

        componentDependencies = data.GetAllDependencies();

        foreach (var dependency in componentDependencies)
        {
            if (componentsAddedToWeapon.FirstOrDefault(component => component.GetType() == dependency))
                continue;

            var weaponComponent = componentAlreadyOnWeapon.FirstOrDefault(component => component.GetType() == dependency);

            if (weaponComponent == null)
            {
                weaponComponent = gameObject.AddComponent(dependency) as WeaponComponent;
            }

            weaponComponent.Init();
            componentsAddedToWeapon.Add(weaponComponent);
        }

        var componentsToRemove = componentAlreadyOnWeapon.Except(componentsAddedToWeapon);

        foreach (var weaponComponent in componentsToRemove)
        {
            Destroy(weaponComponent);
        }
    }
}
