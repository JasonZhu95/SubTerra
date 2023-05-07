using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAudioOnTrigger : MonoBehaviour
{
    [Header("Music On Left")]
    [SerializeField] private string audioFileLeft;
    [Header("Music On Right")]
    [SerializeField] private string audioFileRight;

    private SoundManager soundManager;

    private Collider2D coll;

    private void Start()
    {
        coll = gameObject.GetComponent<Collider2D>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - coll.bounds.center).normalized;

            if (exitDirection.x > 0f)
            {
                soundManager.Play(audioFileLeft);
                soundManager.StopPlay(audioFileRight);
            }
            else if (exitDirection.x < 0f)
            {
                soundManager.Play(audioFileRight);
                soundManager.StopPlay(audioFileLeft);
            }
        }
    }
}
