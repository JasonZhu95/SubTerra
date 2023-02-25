using UnityEngine;

namespace Project.Weapons
{
    [System.Serializable]
    public struct TargeterShape : INameable
    {
        [HideInInspector] public string Name;

        public bool debug;

        public Vector2 Offset;
        public Vector2 Size;

        public LayerMask damageableLayer;
        public LayerMask groundLayer;

        public void SetName(string value) => Name = value;
    }
}