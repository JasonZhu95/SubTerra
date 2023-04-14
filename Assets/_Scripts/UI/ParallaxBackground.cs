using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Camera mainCamera;
    public Transform subject;

    private Vector2 startPosition;
    private float startZ;

    private Vector2 travelDistance => (Vector2)mainCamera.transform.position - startPosition;
    private float distanceFromSubject => transform.position.z - subject.position.z;
    private float clippingPlane => (mainCamera.transform.position.z + (distanceFromSubject > 0 ? mainCamera.farClipPlane : mainCamera.nearClipPlane));
    private float parallaxFactor => Mathf.Abs(distanceFromSubject) / clippingPlane;


    private void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    private void Update()
    {
        Vector2 newPos = startPosition + travelDistance * parallaxFactor;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);
    }

}
