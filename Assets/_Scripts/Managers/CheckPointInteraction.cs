using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Interfaces;
using Project.Managers;

public class CheckPointInteraction : MonoBehaviour, IInteractable
{
    private bool playerInRange;

    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    private GameObject player;
    private CollisionSenses playerCollision;

    private PlayerInputHandler inputHandler;

    private CheckPointManager checkPointManager;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        inputHandler = player.GetComponent<PlayerInputHandler>();
        checkPointManager = GameObject.Find("Managers").transform.Find("CheckPointManager").GetComponent<CheckPointManager>();
        visualCue.SetActive(false);
    }

    private void Start()
    {
        playerCollision = player.GetComponent<Player>().Core.transform.GetChild(1).GetComponent<CollisionSenses>();
    }

    private void Update()
    {
        if (playerInRange && playerCollision.Ground)
        {
            visualCue.SetActive(true);
            if (inputHandler.InteractPressed)
            {
                checkPointManager.SetCheckPoint(gameObject);
                inputHandler.InteractPressed = false;
            }
        }
        else
        {
            visualCue.SetActive(false);
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
