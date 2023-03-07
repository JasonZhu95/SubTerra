using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnCollision : MonoBehaviour
{

    [SerializeField] private Death deathComponent;

    private void Awake()
    {
        deathComponent = GameObject.Find("Player").transform.GetChild(0).GetChild(6).GetComponent<Death>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            deathComponent.RespawnPoints = gameObject.transform;
        }
    }
}
