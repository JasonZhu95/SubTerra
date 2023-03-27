using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseAnimationEvent : MonoBehaviour
{
    public void DisableMenuOnUnpause()
    {
        gameObject.SetActive(false);
    }
}
