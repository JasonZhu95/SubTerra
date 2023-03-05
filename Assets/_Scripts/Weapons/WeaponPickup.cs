using System;
using System.Collections;
using System.Collections.Generic;
using Project.Combat.Interfaces;
using Project.Utilities.GO;
using Project.Weapons;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private WeaponDataSO data;

    private Bobber bobber;
    private SpriteRenderer graphics;

    private void Awake()
    {
        graphics = GetComponentInChildren<SpriteRenderer>();

        Init();

        bobber = GetComponentInChildren<Bobber>();
    }

    private void Init()
    {
        graphics.sprite = data.ItemImage;
    }

    public object GetInteractionContext()
    {
        return data;
    }

    public void SetContext(object obj)
    {
        switch (obj)
        {
            case null:
                gameObject.SetActive(false);
                break;
            case WeaponDataSO so:
                data = so;
                Init();
                break;
        }
    }

    public void EnableInteraction()
    {
        bobber.StartBobbing();
    }

    public void DisableInteraction()
    {
        bobber.StopBobbing();
    }

    public Vector3 GetPosition() => transform.position;
}