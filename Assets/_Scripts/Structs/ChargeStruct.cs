using UnityEngine;

namespace Project.Weapons
{
    [System.Serializable]
    public struct ChargeStruct : INameable
    {
        [HideInInspector]
        public string Name;

        public bool StartWithOne;

        public float ChargeTime;
        public int NumOfCharges;

        public GameObject ChargeParticlesPrefab;
        public GameObject MaxChargeParticlesPrefab;

        public Vector2 Offset;

        public void SetName(string value) => Name = value;
    }
}
