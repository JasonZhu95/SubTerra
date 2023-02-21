using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "newWeaponData", menuName = "Data/Weapon Data/Basic Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [field: SerializeField] public int NumberOfAttacks { get; private set; }
    
    [field: SerializeReference] public List<ComponentData> ComponentDataList { get; private set; }

    public T GetData<T>()
    {
        return ComponentDataList.OfType<T>().FirstOrDefault();
    }

    [ContextMenu("Add Sprite Data")]
    private void AddSpriteData() => ComponentDataList.Add(new WeaponSpriteData());
    [ContextMenu("Add Movement Data")]
    private void AddMovementData() => ComponentDataList.Add(new MovementData());
}
