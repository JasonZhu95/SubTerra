using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckJumpCollide : MonoBehaviour
{
    [SerializeField]
    GameObject dustJump;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            Instantiate(dustJump, transform.position, dustJump.transform.rotation);
        }
    }
}
