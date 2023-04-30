using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temple_BossTrigger : MonoBehaviour
{
    [SerializeField] private GameObject templeFightGO;
    [SerializeField] private GameObject templePreFightGO;
    [SerializeField] private BossManager bossManager;
    [SerializeField] private DoorAnimated bossDoors;
    private bool startFight;

    private void Start()
    {
        if (bossManager.TempleBossDefeated)
        {
            templeFightGO.SetActive(false);
            templePreFightGO.SetActive(false);
        }
        else
        {
            templePreFightGO.SetActive(true);
        }
    }

    private void Update()
    {
        if (!bossManager.TempleBossDefeated)
        {
            if (startFight)
            {
                templeFightGO.SetActive(true);
                templePreFightGO.SetActive(false);
            }
            else
            {
                templeFightGO.SetActive(false);
                templePreFightGO.SetActive(true);
            }
        }
        else
        {
            bossDoors.OpenDoor();
            templeFightGO.SetActive(false);
            templePreFightGO.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            startFight = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            startFight = false;
        }
    }
}
