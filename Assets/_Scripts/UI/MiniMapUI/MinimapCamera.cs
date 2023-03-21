using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private Transform player;
    private Vector3 cameraPosition;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        cameraPosition = player.position;
        cameraPosition.z = -10f;
        transform.position = cameraPosition;
    }
}
