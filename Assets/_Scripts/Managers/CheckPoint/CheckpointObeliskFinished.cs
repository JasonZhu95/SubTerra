using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointObeliskFinished : MonoBehaviour
{
    private Animator obeliskAnim;

    private void Awake()
    {
        obeliskAnim = gameObject.GetComponent<Animator>();
    }

    public void ObeliskAnimationFinished()
    {
        obeliskAnim.SetBool("triggered", false);
    }

    public void PlayObeliskActiveSFX()
    {
        FindObjectOfType<SoundManager>().Play("ObeliskActive");
    }

    public void PlayObeliskTriggeredSFX()
    {
        FindObjectOfType<SoundManager>().StopPlay("ObeliskActive");
        FindObjectOfType<SoundManager>().Play("ObeliskTriggered");
    }

    public void PlayObeliskRespawnSFX()
    {
        FindObjectOfType<SoundManager>().Play("ObeliskRespawn");
    }
}
