using UnityEngine;

namespace Project.Weapons
{
    [System.Serializable]
    public struct DrawStruct : INameable
    {
        [HideInInspector]
        public string Name;
        public AnimationCurve curve;
        public float drawTime;

        public void SetName(string value)
        {
            Name = value;
        }
    }
}
