using System;
using UnityEngine;

namespace Project.EventChannels
{
    [CreateAssetMenu(fileName = "newTriggerChannel", menuName = "Event Channels/Trigger")]
    public class TriggerEventChannel : EventChannelsSO<EventArgs>
    {
    }
}