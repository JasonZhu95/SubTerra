using System;
using UnityEngine;

namespace Project.EventChannels
{
    public abstract class EventChannelsSO<T> : ScriptableObject
    {
        public event EventHandler<T> OnEvent;

        public void RaiseEvent(object sender, T context)
        {
            OnEvent?.Invoke(sender, context);
        }
    }
}
