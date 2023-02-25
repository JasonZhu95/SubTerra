using UnityEngine;
using Cinemachine;

namespace Project.Managers
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private GameObject player;
        [SerializeField] private float respawnTime;

        private float respawnTimeStart;

        private bool respawn;

        private CinemachineVirtualCamera CVC;

        private void Start()
        {
            CVC = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
        }

        private void Update()
        {
            CheckRespawn();
        }

        public void Respawn()
        {
            respawnTimeStart = Time.time;
            respawn = true;
        }

        private void CheckRespawn()
        {
            if (Time.time >= respawnTimeStart + respawnTime && respawn)
            {
                var playerTemp = Instantiate(player, respawnPoint);
                CVC.m_Follow = playerTemp.transform;
                respawn = false;
            }
        }
    }
}