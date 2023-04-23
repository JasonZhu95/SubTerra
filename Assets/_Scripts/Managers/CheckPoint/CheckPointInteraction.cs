using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Interfaces;
using Project.Managers;

public class CheckPointInteraction : MonoBehaviour, IInteractable
{
    private bool playerInRange;

    [Header("Obelisk")]
    [SerializeField] private GameObject obelisk;

    private Animator obeliskAnim;
    private GameObject player;
    private CollisionSenses playerCollision;
    private PlayerInputHandler inputHandler;
    private CheckPointManager checkPointManager;

    private void Awake()
    {
        obeliskAnim = obelisk.GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        inputHandler = player.GetComponent<PlayerInputHandler>();
        checkPointManager = GameObject.Find("Managers").transform.Find("CheckPointManager").GetComponent<CheckPointManager>();
    }

    private void Start()
    {
        playerCollision = player.GetComponent<Player>().Core.transform.GetChild(1).GetComponent<CollisionSenses>();
    }

    private void Update()
    {
        if (playerInRange && playerCollision.Ground)
        {
            obeliskAnim.SetBool("active", true);
            if (inputHandler.InteractPressed)
            {
                obeliskAnim.SetBool("triggered", true);
                checkPointManager.SetCheckPoint(gameObject);
                checkPointManager.HealOnCheckpointSet();
                inputHandler.InteractPressed = false;
            }
        }
        else
        {
            obeliskAnim.SetBool("active", false);
        }
    }

    public object GetInteractionContext()
    {
        return null;
    }

    public void SetContext(object obj)
    {

    }

    public void EnableInteraction()
    {
        playerInRange = true;
    }

    public void DisableInteraction()
    {
        playerInRange = false;
    }

    public Vector3 GetPosition() => transform.position;
}
