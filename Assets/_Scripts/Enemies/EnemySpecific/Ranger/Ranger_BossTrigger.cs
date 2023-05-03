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
    private bool playerResetNeeded = false;

    public Death playerDeathScript;

    private void Start()
    {
        playerDeathScript = FindObjectOfType<Player>().GetComponentInChildren<Death>();
        playerDeathScript.OnDeath += () => playerDeath();

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

                if (playerResetNeeded)
                {
                    rangerFightGO.GetComponentInChildren<Ranger>().PlayerDeath();
                    playerResetNeeded = false;
                }
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

    private void OnDestroy()
    {
        playerDeathScript.OnDeath -= () => playerDeath();
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

    private void playerDeath()
    {
        playerResetNeeded = true;
    }
}
