using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private void Awake()
        {
            player = GameObject.Find("Player");
            playerSR = player.GetComponent<SpriteRenderer>();
            checkPointManager = GameObject.Find("Managers").transform.Find("CheckPointManager").GetComponent<CheckPointManager>();
            color = playerSR.material.color;
            enemyCollision = player.transform.Find("Core").Find("EnemyCollision").GetComponent<EnemyCollision>();
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
            player.SetActive(false);

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
            player.SetActive(true);
        }

    }
}
