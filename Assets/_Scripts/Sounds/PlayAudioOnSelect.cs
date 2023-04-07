using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayAudioOnSelect : MonoBehaviour
{
    private GameObject lastSelected;
    private HashSet<string> namesOfButtonsNoAudio;

    private void Awake()
    {
        // Defensive check for game start
        lastSelected = null;
        namesOfButtonsNoAudio = new HashSet<string>();
        namesOfButtonsNoAudio.Add("Choice0");
        namesOfButtonsNoAudio.Add("ItemActionButton(Clone)");
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != lastSelected && EventSystem.current.currentSelectedGameObject != EventSystem.current.firstSelectedGameObject )
        {
            // The selected object has changed
            EventSystem.current.firstSelectedGameObject = null;
            lastSelected = EventSystem.current.currentSelectedGameObject;
            
            // Potential Improvement:  Store all strings that we don't want to check into a hashset or dictionary
            //                         if the name exists do not play the sound;
            if (!namesOfButtonsNoAudio.Contains(EventSystem.current.currentSelectedGameObject.name))
            {
                FindObjectOfType<SoundManager>().Play("UIHover");
            }

        }
    }
}
