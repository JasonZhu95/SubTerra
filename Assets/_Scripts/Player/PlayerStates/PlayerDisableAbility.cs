using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Interfaces;

public class PlayerDisableAbility : MonoBehaviour, IDataPersistence
{
    [SerializeField]
    private Player player;
    private float duration = 0.3f;
    private bool collected = false;

    [SerializeField] Animator tutorialAnim;

    [SerializeField] private string id;
    private PlayerInputHandler inputHandler;
    private SpriteRenderer sr;

    private bool canvasIsActive = false;

    [ContextMenu("Generate Guid for Item ID")]

    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Awake()
    {
        inputHandler = GameObject.FindWithTag("Player").GetComponent<PlayerInputHandler>();
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (canvasIsActive)
        {
            if (inputHandler.MainActionUIInput)
            {
                tutorialAnim.SetBool("start", false);
                inputHandler.MainActionUIInput = false;
                StartCoroutine(ReEnableGameplay());
                canvasIsActive = false;
            }
        }
    }

    private IEnumerator ReEnableGameplay()
    {
        yield return new WaitForSeconds(.5f);
        inputHandler.SwitchToActionMap("Gameplay");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (gameObject.name)
            {
                case "Dash":
                    player.DisableDash = false;
                    startTutorialAnim();
                    break;
                case "WallAbilities":
                    player.DisableWallJump = false;
                    player.DisableWallClimb = false;
                    player.DisableWallGrab = false;
                    player.DisableWallSlide = false;
                    startTutorialAnim();
                    break;
                //case "WallClimb":
                //    player.DisableWallClimb = false;
                //    break;
                //case "WallSlide":
                //    player.DisableWallSlide = false;
                //    break;
                //case "WallGrab":
                //    player.DisableWallGrab = false;
                //    break;
                default:
                    break;
            }
            // TODO: Add animation
            DestroyItem();

        }
    }

    private void startTutorialAnim()
    {
        tutorialAnim.SetBool("start", true);
        StartCoroutine(SetCanvasBoolAfterAnimation());
        inputHandler.SwitchToActionMap("UINoPause");
    }

    private IEnumerator SetCanvasBoolAfterAnimation()
    {
        yield return new WaitForSeconds(.5f);
        canvasIsActive = true;
    }

    internal void DestroyItem()
    {
        if (!collected)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            collected = true;
            StartCoroutine(AnimateItemPickup());
        }
    }

    private IEnumerator AnimateItemPickup()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
            yield return null;
        }
        sr.enabled = false;
    }

    public void LoadData(GameData data)
    {
        data.abilityCollected.TryGetValue(id, out collected);
        if (collected)
        {
            gameObject.SetActive(false);
        }
        player.DisableDash = data.disableDash;
        player.DisableWallJump = data.disableWallJump;
        player.DisableWallClimb = data.disableWallClimb;
        player.DisableWallGrab = data.disableWallGrab;
        player.DisableWallSlide = data.disableWallSlide;
    }

    public void SaveData(GameData data)
    {
        if (data.abilityCollected.ContainsKey(id))
        {
            data.abilityCollected.Remove(id);
        }
        data.abilityCollected.Add(id, collected);
        data.disableDash = player.DisableDash;
        data.disableWallJump = player.DisableWallJump;
        data.disableWallClimb = player.DisableWallClimb;
        data.disableWallGrab = player.DisableWallGrab;
        data.disableWallSlide = player.DisableWallSlide;
    }

}
