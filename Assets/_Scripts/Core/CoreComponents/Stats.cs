using System;
using UnityEngine;
using Project.Interfaces;

public class Stats : CoreComponent, IDataPersistence
{
    [field: SerializeField] public Stat Health { get; private set; }
    [field: SerializeField] public Stat Poise { get; private set; }
    [field: SerializeField] public int CoinDeathValue { get; private set; }
    [SerializeField] private float PoiseRecoveryPerSecond;

    private float startPlayTime;
    private float savedPlaytime;
    private float finalPlayTime;

    protected override void Awake()
    {
        base.Awake();

        Health.Init();
        Poise.Init();
        startPlayTime = Time.time;
    }

    private void Update()
    {
        Poise.Increase(PoiseRecoveryPerSecond * Time.deltaTime);
    }

    public void LoadData(GameData data)
    {
        if (core.Parent.name == "Player")
        {
            savedPlaytime = data.playTime;
            Health.MaxValue = data.maxHealth;
            Health.currentValue = data.currentHealth;
        }
    }

    public void SaveData(GameData data)
    {
        if (core.Parent.name == "Player")
        {
            finalPlayTime = (Time.time - startPlayTime) + savedPlaytime;
            data.playTime = finalPlayTime;
            data.playTime = finalPlayTime;
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

    public bool triggeredBelow100Life = false;
    public bool trigggered75Life = false;
    public bool triggeredHalfLifeAttack = false;
    public bool triggeredQuarterLifeAttack = false;

    public float CurrentValue
    {
        get => currentValue;
        private set
        {
            currentValue = Mathf.Clamp(value, 0f, MaxValue);
            OnCurrentValueChange?.Invoke(currentValue);

            if (currentValue / MaxValue < 1 && !triggeredBelow100Life)
            {
                OnCurrentHealthBelow100?.Invoke();
                triggeredBelow100Life = true;
            }

            if (currentValue / MaxValue <= 0.75 && !trigggered75Life)
            {
                OnCurrentHealthBelow75?.Invoke();
                trigggered75Life = true;
            }

            if (currentValue / MaxValue <= 0.5 && !triggeredHalfLifeAttack)
            {
                OnCurrentValueBelowHalf?.Invoke();
                triggeredHalfLifeAttack = true;
            }

            if (currentValue / MaxValue <= 0.25 && !triggeredQuarterLifeAttack)
            {
                OnCurrentValueBelowQuarter?.Invoke();
                triggeredQuarterLifeAttack = true;
            }

            if (currentValue <= 0f)
            {
                OnCurrentValueZero?.Invoke();
            }
        }
    }

    public event Action<float> OnCurrentValueChange;

    public event Action OnCurrentValueZero;

    public event Action OnCurrentHealthBelow100;

    public event Action OnCurrentHealthBelow75;

    public event Action OnCurrentValueBelowHalf;

    public event Action OnCurrentValueBelowQuarter;

    public event Action OnMaxHPIncrease;

    public void Init() => CurrentValue = MaxValue;

    public void Increase(float amount) => CurrentValue += amount;

    public void Decrease(float amount) => CurrentValue -= amount;

    public void SetHealth(float amount) => CurrentValue = amount;

    public void IncreaseMaxHealth(float amount)
    {
        MaxValue += amount;
        currentValue += amount;
        OnMaxHPIncrease?.Invoke();
    }
}