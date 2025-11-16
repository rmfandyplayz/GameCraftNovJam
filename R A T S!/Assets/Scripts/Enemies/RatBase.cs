using System;
using System.Collections.Generic;
using UnityEngine;

public class RatBase : MonoBehaviour
{
    [Header("Stats")]
    public int Health = 1;

    [Header("VFX")]
    public GameObject deathEffect;
    public float iFrameDuration = 0.2f;
    private bool isInvulnerable = false;
    private Color defaultColor;

    [Header("Conditions")]
    public bool movingEnemy = true;

    [Header("Sight")]
    [SerializeField] private LayerMask seeCollisionMask;
    [SerializeField] private float seeDistance;

    
    private PathfindingManager pathManager;

    public Stack<PathfindingNode> currentRoute = new();
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
    private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pathManager = FindAnyObjectByType<PathfindingManager>();
        player = FindAnyObjectByType<Player>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        defaultColor = spriteRenderer.color;
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
        if (movingEnemy == false)
        {
            rb.linearVelocity = new Vector2(0,0);
            return;
        }

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
        
        //Debug.Log(CanSeePlayer());

    }

    public bool CanSeePlayer()
    {
        if ((transform.position - player.transform.position).magnitude > seeDistance)
        {
            return false;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position,
            seeDistance, seeCollisionMask);

        return hit.collider is null;
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

    public void TakeDamage()
    {
        if (isInvulnerable) return;  // ignore hit

        Health -= 1;
        StartCoroutine(IFrames());   // start invulnerability

        if (Health < 1)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    private System.Collections.IEnumerator IFrames()
    {
        isInvulnerable = true;
    
        // flash red
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(iFrameDuration);

        // restore color
        spriteRenderer.color = defaultColor;

        isInvulnerable = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, seeDistance);
    }
}
