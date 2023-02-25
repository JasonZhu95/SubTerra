using System;
using Project.EventChannels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.UI
{
    public class DeselectUIEventSystem : MonoBehaviour
    {
        private EventSystem eventSystem;
        [SerializeField] private TriggerEventChannel triggerDisableSelectedChannel;

        private void Awake()
        {
            eventSystem = GetComponent<EventSystem>();

            triggerDisableSelectedChannel.OnEvent += DisableSelectedUI;
        }

        private void DisableSelectedUI(object sender, EventArgs arg)
        {
            eventSystem.SetSelectedGameObject(null);
        }
        
        
    }
}