using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSwapper : MonoBehaviour
{
    [SerializeField] private Animator backgroundOnLeft;
    [SerializeField] private Animator backgroundOnRight;

    private Collider2D coll;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - coll.bounds.center).normalized;

            if (exitDirection.x < 0f)
            {
                backgroundOnLeft.SetBool("start", false);
                backgroundOnRight.SetBool("start", true);
            }
            else if (exitDirection.x > 0f)
            {
                backgroundOnLeft.SetBool("start", true);
                backgroundOnRight.SetBool("start", false);
            }
        }
    }
}
