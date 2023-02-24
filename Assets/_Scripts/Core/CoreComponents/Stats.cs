using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : CoreComponent
{
    [field: SerializeField] public Stat Health { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Health.Init();
    }

    private void Update()
    {

    }
}

[System.Serializable]
public class Stat
{
    private float currentValue;
    [field: SerializeField] public float MaxValue { get; private set; }

    public float CurrentValue
    {
        get => currentValue;
        private set
        {
            currentValue = Mathf.Clamp(value, 0f, MaxValue);
            OnCurrentValueChange?.Invoke(currentValue);

            if (currentValue <= 0f)
            {
                OnCurrentValueZero?.Invoke();
            }
        }
    }

    public event Action<float> OnMaxValueChange;
    public event Action<float> OnCurrentValueChange;

    public event Action OnCurrentValueZero;

    public void Init() => CurrentValue = MaxValue;

    public void Increase(float amount) => CurrentValue += amount;
    public void Decrease(float amount) => CurrentValue -= amount;
}