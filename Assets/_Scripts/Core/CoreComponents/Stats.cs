using System;
using UnityEngine;

public class Stats : CoreComponent, IDataPersistence
{
    [field: SerializeField] public Stat Health { get; private set; }
    [field: SerializeField] public Stat Poise { get; private set; }
    [field: SerializeField] public int CoinDeathValue { get; private set; }
    [SerializeField] private float PoiseRecoveryPerSecond;

    protected override void Awake()
    {
        base.Awake();

        Health.Init();
        Poise.Init();
    }

    private void Update()
    {
        Poise.Increase(PoiseRecoveryPerSecond * Time.deltaTime);
    }

    public void LoadData(GameData data)
    {
        if (core.Parent.name == "Player")
        {
            Health.MaxValue = data.maxHealth;
            Health.currentValue = data.currentHealth;
        }
    }

    public void SaveData(ref GameData data)
    {
        if (core.Parent.name == "Player")
        {
            data.maxHealth = Health.MaxValue;
            data.currentHealth = Health.currentValue;
        }
    }
}

[System.Serializable]
public class Stat
{
    [SerializeField]
    public float currentValue;
    [field: SerializeField] public float MaxValue { get; set; }

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

    public event Action<float> OnCurrentValueChange;

    public event Action OnCurrentValueZero;

    public void Init() => CurrentValue = MaxValue;

    public void Increase(float amount) => CurrentValue += amount;

    public void Decrease(float amount) => CurrentValue -= amount;
}