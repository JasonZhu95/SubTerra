using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisableAbility : MonoBehaviour
{
    [SerializeField]
    private Player player;
    private float duration = 0.3f;

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
            //Play an animation on pickup maybe
            DestroyItem();

        }
    }

    private void DestroyItem()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(AnimateItemPickup());
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
        Destroy(gameObject);
    }
}
