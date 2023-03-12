using System;
using System.Runtime.CompilerServices;
using Project.Combat.Interfaces;
using Project.EventChannels;
using Project.Weapons;
using UnityEngine;

public class PlayerInventory : CoreComponent, IDataPersistence
{
    public WeaponDataSO[] weapons;

    private Interaction interaction;

    private Interaction Interaction => interaction ? interaction : core.GetCoreComponent(ref interaction);

    [SerializeField] private WeaponChangedEventChannel InventoryChangeChannel;
    [SerializeField] private WeaponPickupEventChannel WeaponPickupChannel;

    private WeaponPickup weaponPickup;

    public int CoinsHeld { get; private set; }

    public void SetWeapon(WeaponDataSO data, CombatInputs input)
    {
        if (weaponPickup != null)
        {
            weaponPickup.SetContext(weapons[(int) input]);
            weaponPickup = null;
        }
        
        weapons[(int)input] = data;
        InventoryChangeChannel.RaiseEvent(this, new WeaponChangedEventArgs(data, input));
    }

    private void Start()
    {
        Interaction.OnInteract += HandleInteraction;
        
        for (var i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
                InventoryChangeChannel.RaiseEvent(this, new WeaponChangedEventArgs(weapons[i], (CombatInputs) i));
        }
    }


    private void HandleInteraction(IInteractable context)
    {
        if (context is not WeaponPickup) return;

        weaponPickup = context as WeaponPickup;

        var data = weaponPickup.GetInteractionContext() as WeaponDataSO;
        
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
            {
                SetWeapon(data, (CombatInputs) i);
                return;
            }
        }

        WeaponPickupChannel.RaiseEvent(this, new WeaponPickupEventArgs(
            data,
            weapons[0],
            weapons[1]
        ));
    }

    private void OnDisable()
    {
        interaction.OnInteract += HandleInteraction;
    }

    public void IncreaseCoins(int amount) => CoinsHeld += amount;
    public void DecreaseCoins(int amount) => CoinsHeld -= amount;

    public void LoadData(GameData data)
    {
        CoinsHeld = data.coinCount;
    }

    public void SaveData(ref GameData data)
    {
        data.coinCount = CoinsHeld;
    }
}