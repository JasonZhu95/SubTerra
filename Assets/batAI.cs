using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class batAI : MonoBehaviour
{
    private Path path;
    private Seeker seeker;
    private Rigidbody2D rb;

    private int currentWaypoint = 0;

    private float distance;

    //private bool reachedEndOfPath = false;

    private Vector2 direction;
    private Vector2 force;


    public Transform target;
    public Transform batGFX;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            //reachedEndOfPath = true;
            return;
        } else
        {
            //reachedEndOfPath = false;
        }

        direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (force.x >= 0.01f)
        {
            batGFX.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (force.x <= 0.01f)
        {
            batGFX.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
