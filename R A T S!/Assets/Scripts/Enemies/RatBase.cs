using System;
using System.Collections.Generic;
using System.Linq;
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

    [Header("sound effects")]
    public AudioSource attack;
    public AudioSource hurt;

    [Header("Conditions")]
    public bool movingEnemy = true;

    [Header("Sight")]
    [SerializeField] private LayerMask seeCollisionMask;
    [SerializeField] private float seeDistance;

    
    private PathfindingManager pathManager;

    public Stack<PathfindingNode> currentRoute = new();
    private PathfindingNode currentTargetedNode;
    private bool hasLoaded = false;
    private Vector3? overridePos = null;

    private Vector3 goalPos;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float accel;
    [SerializeField] private float reachedDist = .01f;
    [SerializeField] private float biteDistance;
    [SerializeField] private float damage;

    private float stuckTimer = 0;
    [SerializeField] private float maxStuckTime;
    [SerializeField] private float maxStuckSpeed;

    private Rigidbody2D rb;
    private Animator animator;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    protected Player player;
    private BoxCollider2D playerCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pathManager = FindAnyObjectByType<PathfindingManager>();
        player = FindAnyObjectByType<Player>();
        playerCollider = player.GetComponents<BoxCollider2D>().First(n => !n.isTrigger);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        defaultColor = spriteRenderer.color;
    }

    public void CallTo(Vector3 position)
    {
        currentRoute = pathManager.PathfindTo(transform.position, position);
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

    public virtual void Attack()
    {
        player.Damage(damage);
        if (movingEnemy)
        {
            attack.Play();
        }   
        //slight shake when dealing damage
        FindAnyObjectByType<PlayerCamera>().ShakeCamera(0.03f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSeePlayer())
        {
            overridePos = player.transform.position;
            if ((player.transform.position - transform.position).magnitude <= biteDistance)
            {
                //player hurt
                Attack();
            }
        }
        else
        {
            if (overridePos is not null)
            {
                currentRoute = pathManager.PathfindTo(transform.position, (Vector3)overridePos);
            }
            overridePos = null;
        }
        
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

        Vector3 distance;
        if(overridePos is null) 
            distance = currentTargetedNode.transform.position - transform.position;
        else
        {
            distance = (Vector3)overridePos - transform.position;
        }
            
        
        if (distance.magnitude <= reachedDist && overridePos is null)
        {
            ReachedNode();
            // is this framerate dependent? yes. do i care? now
            return;
        }
        if (rb.linearVelocity.magnitude <= maxStuckSpeed && overridePos is null)
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
        Vector3 tryPoint = player.transform.position + (Vector3)playerCollider.offset;
        if ((transform.position - tryPoint).magnitude > seeDistance)
        {
            //Debug.Log("Not in range");
            return false;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, tryPoint - transform.position,
            (tryPoint - transform.position).magnitude, seeCollisionMask);
        

        return hit.collider is null;
    }

    private void OnDrawGizmos()
    {
    }

    public void TakeDamage()
    {
        if (isInvulnerable) return;  // ignore hit
        
        Health -= 1;
        if (movingEnemy == true){
            hurt.Play();
        }
        

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
        
        Gizmos.DrawLine(transform.position, player.transform.position + (Vector3)playerCollider.offset);
    }
}
