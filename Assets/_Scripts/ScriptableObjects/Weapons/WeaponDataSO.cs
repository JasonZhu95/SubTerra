using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "newWeaponData", menuName = "Data/Weapon Data/Basic Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [field: SerializeField] public int NumberOfAttacks { get; private set; }
    [field: SerializeField] public string WeaponName { get; private set; }
    [field: SerializeField, TextArea(3,10)] public string WeaponDescription { get; private set; }
    [field: SerializeField] public Sprite PickupSprite { get; private set; }
    [field: SerializeField] public RuntimeAnimatorController AnimatorController { get; private set; }

    [SerializeField] private List<WeaponComponentData> componentDatas = new List<WeaponComponentData>();
    public List<WeaponComponentData> ComponentDatas => componentDatas;

    private void Awake()
    {
        #if UNITY_EDITOR
        AddDataComponent(new WeaponSpriteData());
        #endif
    }

    private void OnValidate()
    {
        ComponentDatas.ForEach(item => item.OnValidate());
    }

    public List<Type> GetAllDependencies()
    {
        List<Type> dependencies = new List<Type>();

        foreach (var item in componentDatas)
        {
            foreach (var dependency in item.ComponentDependencies)
            {
                if (dependencies.FirstOrDefault(e => e.GetType() == dependency) == null)
                {
                    dependencies.Add(dependency);
                }
            }
        }
        return dependencies;
    }

    public T GetComponentData<T>() where T : WeaponComponentData
    {
        return ComponentDatas.OfType<T>().FirstOrDefault();
    }

#if UNITY_EDITOR
    public void AddDataComponent<T>(T data) where T : WeaponComponentData
    {
        if (ComponentDatas.Where(item => item.GetType() == data.GetType()).FirstOrDefault() != null) return;
        ComponentDatas.Add(data);
        ComponentDatas.Sort((a, b) => string.Compare(a.ComponentName, b.ComponentName));
    }
#endif
}
