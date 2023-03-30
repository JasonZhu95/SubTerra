using UnityEngine;
using System;
using Project.Utilities.GO;

namespace Project.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private WeaponDataSO weaponData;

        public WeaponDataSO WeaponData
        {
            get => weaponData;
            set
            {
                weaponData = value;
                GenerateWeapon();
            }
        }

        private bool currentInput;

        public bool CurrentInput
        {
            get => currentInput;
            private set
            {
                if (value != currentInput)
                    OnInputChange?.Invoke(value);
                currentInput = value;
            }
        }

        public Animator Anim { get; private set; }

        private WeaponAnimationEventHandler animEventHandler;

        public WeaponAnimationEventHandler AnimEventHandler
        {
            get => animEventHandler
                ? animEventHandler
                : (animEventHandler = GetComponentInChildren<WeaponAnimationEventHandler>());
            private set => animEventHandler = value;
        }

        public event Action OnEnter;
        public event Action OnExit;

        public event Action<int> OnCounterChange;

        public event Action<bool> OnInputChange;

        private int currentAttackCounter;

        public int CurrentAttackCounter
        {
            get => currentAttackCounter;
            private set
            {
                if (value >= WeaponData.NumberOfAttacks)
                {
                    currentAttackCounter = 0;
                }
                else
                {
                    currentAttackCounter = value;
                }

                OnCounterChange?.Invoke(currentAttackCounter);
            }
        }

        public Core Core { get; private set; }

        public GameObject BaseGO { get; private set; }

        private void Awake()
        {
            BaseGO = transform.Find("Base").gameObject;

            Anim = GetComponentInChildren<Animator>();

            AnimEventHandler = GetComponentInChildren<WeaponAnimationEventHandler>();

            gameObject.SetActive(false);
        }

        private void Start()
        {
            AnimEventHandler.OnFinish += OnExit;
        }

        private void OnDisable()
        {
        }

        public void Init(Core core)
        {
            Core = core;
        }

        public static Component GetComp(Type compType, GameObject GO)
        {
            return GO.GetComponent(compType);
        }

        public void Enter()
        {
            gameObject.SetActive(true);
            Anim.SetBool(WeaponBoolAnimParameters.active.ToString(), true);

            Anim.SetInteger(WeaponIntAnimParameters.counter.ToString(), CurrentAttackCounter);

            OnEnter?.Invoke();
            OnInputChange?.Invoke(CurrentInput);
        }

        public void Exit()
        {
            OnExit?.Invoke();

            CurrentAttackCounter++;
            Anim.SetBool(WeaponBoolAnimParameters.active.ToString(), false);
            gameObject.SetActive(false);
        }

        public void Tick()
        {
        }

        public void SetInput(bool input) => CurrentInput = input;

        public void GenerateWeapon()
        {
            CurrentAttackCounter = 0;

            if (WeaponData == null)
            {
                Debug.LogError($"{this} has no associated data");
                return;
            }

            var addedComps = gameObject.AddDependenciesToGO<WeaponComponent>(weaponData.GetAllDependencies());

            Anim.runtimeAnimatorController = WeaponData.AnimatorController;

            foreach (var comp in addedComps)
            {
                comp.SetReferences();
            }
        }
    }

    public enum WeaponBoolAnimParameters
    {
        active,
        hold,
        cancel,
    }

    public enum WeaponTriggerAnimParameters
    {
        parry,
    }

    public enum WeaponIntAnimParameters
    {
        counter,
    }

    [System.Serializable]
    public enum WeaponAttackPhases
    {
        Anticipation,
        Idle,
        Action,
        Cancel,
        Break,
        Parry
    }
}