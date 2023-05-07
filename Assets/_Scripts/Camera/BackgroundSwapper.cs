using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSwapper : MonoBehaviour
{
    [SerializeField] private Animator forestBackground;
    [SerializeField] private Animator ruinsBackground;
    [SerializeField] private Animator templeBackground;
    [SerializeField] private Animator backgroundToChange;

    private Collider2D coll;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            forestBackground.SetBool("start", false);
            ruinsBackground.SetBool("start", false);
            templeBackground.SetBool("start", false);
            backgroundToChange.SetBool("start", true);
        }
    }
}
