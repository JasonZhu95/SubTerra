using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Managers;

// Script that is responsible for detecting with respawn point
// the player should be moved to according to collision.
public class RespawnCollision : MonoBehaviour
{
    private RespawnManager respawnManager;

    private void Awake()
    {
        respawnManager = GameObject.Find("Managers").transform.Find("RespawnManager").GetComponent<RespawnManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            respawnManager.RespawnPoints = gameObject.transform;
        }
    }
}
