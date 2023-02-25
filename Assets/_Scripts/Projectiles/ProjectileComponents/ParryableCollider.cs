using Project.Combat.Interfaces;
using UnityEngine;

namespace Project.Projectiles
{
    public class ParryableCollider : MonoBehaviour, IParryable
    {
        private IParryable handler;

        public void SetHandler(IParryable handler) => this.handler = handler;

        public void Parry(ParryData data) => handler.Parry(data);

        public GameObject GetParent() => handler.GetParent();
    }
}