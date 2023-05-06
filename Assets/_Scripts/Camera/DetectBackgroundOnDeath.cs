using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Managers;

public class DetectBackgroundOnDeath : MonoBehaviour
{
    private RespawnManager respawnManager;
    [SerializeField] private Animator forestBackground;
    [SerializeField] private Animator ruinsBackground;
    [SerializeField] private Animator templeBackground;

    [SerializeField] private Animator backgroundToPlay;

    private void Awake()
    {
        respawnManager = GameObject.Find("Managers").transform.Find("RespawnManager").GetComponent<RespawnManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        respawnManager.OnFullDeath += SetBackground;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        respawnManager.OnFullDeath -= SetBackground;
    }

    private void SetBackground()
    {
        forestBackground.SetBool("start", false);
        ruinsBackground.SetBool("start", false);
        templeBackground.SetBool("start", false);
        backgroundToPlay.SetBool("start", true);
    }
}
