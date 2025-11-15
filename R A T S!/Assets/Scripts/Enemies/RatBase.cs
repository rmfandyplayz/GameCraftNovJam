using System;
using System.Collections.Generic;
using UnityEngine;

public class RatBase : MonoBehaviour
{
    private PathfindingManager pathManager;

    private Stack<PathfindingNode> currentRoute = new();
    private PathfindingNode currentTargetedNode;
    private bool hasLoaded = false;

    private Vector3 goalPos;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float accel;
    [SerializeField] private float reachedDist = .01f;

    private float stuckTimer = 0;
    [SerializeField] private float maxStuckTime;
    [SerializeField] private float maxStuckSpeed;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pathManager = FindAnyObjectByType<PathfindingManager>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (!hasLoaded)
        {
            hasLoaded = true;
            Reroute();
        }
        Vector3 distance = currentTargetedNode.transform.position - transform.position;
        if (distance.magnitude <= reachedDist)
        {
            ReachedNode();
            // is this framerate dependent? yes. do i care? now
            return;
        }
        if (rb.linearVelocity.magnitude <= maxStuckSpeed)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > maxStuckTime)
            {
                currentRoute.Push(currentTargetedNode);
                currentTargetedNode = pathManager.GetNearestNode(transform.position);
            }
        }
        else
        {
            stuckTimer = 0;
        }
        
        
        rb.AddForce(distance.normalized * (accel * Time.deltaTime * rb.mass), ForceMode2D.Impulse);
        rb.linearVelocity = rb.linearVelocity.normalized * Mathf.Min(rb.linearVelocity.magnitude, maxSpeed);
        
        animator.SetBool("IsHoriz", Mathf.Abs(rb.linearVelocityX) > Mathf.Abs(rb.linearVelocityY));
        animator.SetBool("IsMoving", rb.linearVelocity.magnitude > .01f);
        spriteRenderer.flipX = rb.linearVelocityX > 0;

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
