using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFootsteps : MonoBehaviour
{
    private bool playerInRange;
    private float distanceToCenter;
    private GameObject player;
    private SoundManager soundManager;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    private void Update()
    {
        if (playerInRange)
        {
            distanceToCenter = Vector3.Distance(transform.position, player.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void AnimationEnemyWalkSFX()
    {
        if (playerInRange)
        {
            float volume = 1f - Mathf.Clamp01(distanceToCenter / 5f);
            volume *= volume;
            float finalVolume = volume * 0.7f;
            soundManager.ChangeVolume("Enemy1Walk", finalVolume);
            soundManager.Play("Enemy1Walk");
        }
    }

}
