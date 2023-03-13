using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisableAbility : MonoBehaviour, IDataPersistence
{
    [SerializeField]
    private Player player;
    private float duration = 0.3f;
    private bool collected = false;

    [SerializeField] private string id;

    [ContextMenu("Generate Guid for Item ID")]

    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (gameObject.name)
            {
                case "Dash":
                    player.DisableDash = false;
                    break;
                case "WallAbilities":
                    player.DisableWallJump = false;
                    player.DisableWallClimb = false;
                    player.DisableWallGrab = false;
                    player.DisableWallSlide = false;
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
                    Debug.Log("Invalid Ability State");
                    break;
            }
            // TODO: Add animation
            DestroyItem();

        }
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
        gameObject.SetActive(false);
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

    public void SaveData(ref GameData data)
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
