using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    [field: SerializeField] public ProjectileDataSO Data { get; private set; }

    public Rigidbody2D RB { get; private set; }
    public GameObject SpawningEntity { get; private set; }
    public Vector3 SpawningEntityPos { get; private set; }

    public event Action OnInit;
    public event Action OnBeforeDisable;

    public bool CanDamage { get; private set; }

    public void CreateProjectile(ProjectileDataSO data)
    {
        Data = data;
        var comps = gameObject.AddDependenciesToGO<ProjectileComponent>(Data.GetAllDependencies());
        comps.ForEach(item => item.SetReferences());
    }

    public void Init(GameObject spawningEntity)
    {
        SpawningEntity = spawningEntity;
        SpawningEntityPos = spawningEntity.transform.position;
        SetCanDamage(true);
        OnInit?.Invoke();
    }

    public void Disable()
    {
        OnBeforeDisable?.Invoke();
        Destroy(gameObject);
    }

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    public void SetCanDamage(bool value) => CanDamage = value;
}
