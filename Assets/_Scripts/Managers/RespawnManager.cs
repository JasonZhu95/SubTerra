using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Script Responsible for handling Respawn on Player Death.
// Starts a coroutine that reactivates the player and moves it to the
// Appropriate respawn point
namespace Project.Managers
{
    public class RespawnManager : MonoBehaviour
    {
        public Transform RespawnPoints { get; set; }
        public Transform[] CheckPoints { get; set; }

        private CheckPointManager checkPointManager;
        private GameObject player;
        private SpriteRenderer playerSR;
        private Color color;
        private EnemyCollision enemyCollision;

        private SpriteRenderer weapon1baseSR;
        private SpriteRenderer weapon1weaponSR;
        private SpriteRenderer weapon2baseSR;
        private SpriteRenderer weapon2weaponSR;

        private void Awake()
        {
            player = GameObject.Find("Player");
            playerSR = player.GetComponent<SpriteRenderer>();
            checkPointManager = GameObject.Find("Managers").transform.Find("CheckPointManager").GetComponent<CheckPointManager>();
            color = playerSR.material.color;
            enemyCollision = player.transform.Find("Core").Find("EnemyCollision").GetComponent<EnemyCollision>();
            weapon1baseSR = player.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
            weapon2baseSR = player.transform.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>();
            weapon1weaponSR = player.transform.GetChild(1).GetChild(1).GetComponent<SpriteRenderer>();
            weapon2weaponSR = player.transform.GetChild(2).GetChild(1).GetComponent<SpriteRenderer>();
        }

        public void PlayerDeathSwitchActive(bool fullDeath)
        {
            StartCoroutine(ResetPlayerOnRespawn(fullDeath));
        }

        // full death bool determines if respawn or checkpoint position
        private IEnumerator ResetPlayerOnRespawn(bool fullDeath)
        {
            color.a = 1f;
            playerSR.material.color = color;
            player.GetComponent<PlayerInput>().enabled = false;
            playerSR.enabled = false;
            weapon1baseSR.enabled = false;
            weapon2baseSR.enabled = false;
            weapon1weaponSR.enabled = false;
            weapon2weaponSR.enabled = false;
            

            yield return new WaitForSeconds(2.0f);

            if (fullDeath)
            {
                player.transform.position = checkPointManager.GoToLastCheckPoint();
            }
            else
            {
                player.transform.position = RespawnPoints.position;
            }
            enemyCollision.CanSetDeathZoneCollision = true;
            player.GetComponent<PlayerInput>().enabled = true;
            player.GetComponent<SpriteRenderer>().enabled = true;
            weapon1baseSR.enabled = true;
            weapon2baseSR.enabled = true;
            weapon1weaponSR.enabled = true;
            weapon2weaponSR.enabled = true;
        }

    }
}
