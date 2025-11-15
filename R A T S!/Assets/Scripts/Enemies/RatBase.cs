using System;
using System.Collections.Generic;
using UnityEngine;

public class RatBase : MonoBehaviour
{
    private PathfindingManager pathManager;

    private Stack<PathfindingNode> currentRoute = null;
    private PathfindingNode currentTargetedNode;

    private Vector3 goalPos;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float accel;
    [SerializeField] private float reachedDist = .01f;

    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pathManager = FindAnyObjectByType<PathfindingManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Reroute()
    {
        PathfindingNode newWanderNode = pathManager.GetRandomNode();
        
        currentRoute = pathManager.PathfindTo(
            transform.position, 
            newWanderNode.transform.position);
        goalPos = newWanderNode.transform.position;
        currentTargetedNode = currentRoute.Pop();
    }

    void ReachedNode()
    {
        if (currentRoute.Count == 0)
        {
            Reroute();
            return;
        }
        currentTargetedNode = currentRoute.Pop();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentRoute is null)
        {
            Reroute();
        }

        Vector3 distance = currentTargetedNode.transform.position - transform.position;
        if (distance.magnitude <= reachedDist)
        {
            ReachedNode();
            // is this framerate dependent? yes. do i care? now
            return;
        }
        
        
        rb.AddForce(distance.normalized * (accel * Time.deltaTime * rb.mass), ForceMode2D.Impulse);
        rb.linearVelocity = rb.linearVelocity.normalized * Mathf.Min(rb.linearVelocity.magnitude, maxSpeed);
    }

    private void OnDrawGizmos()
    {
        if (currentTargetedNode is null)
        {
            return;
        }
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, goalPos);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, currentTargetedNode.transform.position);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (currentTargetedNode.transform.position - transform.position).normalized);
    }
}
