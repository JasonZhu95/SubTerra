using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.UI;

public class Demon_BossTrigger : MonoBehaviour
{
    [SerializeField] private GameObject demonFightGO;
    [SerializeField] private GameObject demonPreFightGO;
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

        if (bossManager.DemonBossDefeated)
        {
            demonFightGO.SetActive(false);
            demonPreFightGO.SetActive(false);
            dialogueTrigger.gameObject.SetActive(false);
        }
        else
        {
            demonPreFightGO.SetActive(true);
        }
    }

    private void Update()
    {
        if (!bossManager.DemonBossDefeated)
        {
            if (startFight && dialogueTrigger.DialogueHasBeenPlayed)
            {
                bossDoors.CloseDoor();
                demonFightGO.SetActive(true);
                demonPreFightGO.SetActive(false);

                if (playerResetNeeded)
                {
                    demonFightGO.GetComponentInChildren<DemonKing>().PlayerDeath();
                    playerResetNeeded = false;
                }
            }
            else
            {
                demonFightGO.SetActive(false);
                demonPreFightGO.SetActive(true);
            }
        }
        else
        {
            bossDoors.OpenDoor();
            demonFightGO.SetActive(false);
            demonPreFightGO.SetActive(false);
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
            bossDoors.OpenDoor();
            startFight = false;
        }
    }

    private void playerDeath()
    {
        playerResetNeeded = true;
    }
}
