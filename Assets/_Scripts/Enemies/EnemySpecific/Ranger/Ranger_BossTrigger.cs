using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.UI;

public class Ranger_BossTrigger : MonoBehaviour
{
    [SerializeField] private GameObject rangerFightGO;
    [SerializeField] private GameObject rangerPreFightGO;
    [SerializeField] private BossManager bossManager;
    [SerializeField] private DoorAnimated bossDoors;
    [SerializeField] private DialogueTriggerOnRange dialogueTrigger;
    private bool startFight;

    private void Start()
    {
        if (bossManager.RangerBossDefeated)
        {
            rangerFightGO.SetActive(false);
            rangerPreFightGO.SetActive(false);
            dialogueTrigger.gameObject.SetActive(false);
        }
        else
        {
            rangerPreFightGO.SetActive(true);
        }
    }

    private void Update()
    {
        if (!bossManager.RangerBossDefeated)
        {
            if (startFight && dialogueTrigger.DialogueHasBeenPlayed)
            {
                rangerFightGO.SetActive(true);
                rangerPreFightGO.SetActive(false);
            }
            else
            {
                rangerFightGO.SetActive(false);
                rangerPreFightGO.SetActive(true);
            }
        }
        else
        {
            bossDoors.OpenDoor();
            rangerFightGO.SetActive(false);
            rangerPreFightGO.SetActive(false);
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