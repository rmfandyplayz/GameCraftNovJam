using System.Data.Common;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // --- Internals ---
    private Rigidbody2D rb;
    private BoxCollider2D collider;
    public Transform heldObject;
    private Transform player;
    public Transform collidedObject;
    private Animator animator;
    
    // --- Movement Variables ---
    public float force;
    private bool dashing = false;
    private bool dashOnCooldown = false;

    // --- Held Item Variables ---
    private bool holdingItem = true;
    private bool throwing = false;
    public float throwSpeed;
    private bool locked = false;
    private bool targetUp = false;
    private bool targetRight = false;
    private bool targetLeft = false;
    private bool targetDown = false;
    // --- Sprite Variables ---
    private bool facingUp = true;
    private bool facingRight = false;
    private bool facingLeft = false;
    private bool facingDown = false;

    // --- Timers ---
    private float dashTimer = 0;
    public float dashTime;
    public float dashCooldownTime;
    private float throwTimer;
    public float throwTime;
    public float pickupCooldown;

    private bool isCollidingWithObject = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        player = GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        // --- Timers ---

        dashTimer += Time.deltaTime;
        throwTimer += Time.deltaTime;
        pickupCooldown += Time.deltaTime;

        // --- Dashing ---

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !dashOnCooldown)
        {
            dashing = true;
        }
        if (dashOnCooldown)
        {
            if (dashTimer > dashCooldownTime)
            {
                dashOnCooldown = false;
            }
        }
        if (dashing)
        {
            if (!dashOnCooldown)
            {
                dashTimer = 0;
            }
            if ((Input.GetKey("w") || Input.GetKey("s")) && !dashOnCooldown)
            {
                rb.linearVelocityY *= 3.0f;
                dashOnCooldown = true;
            }
            if ((Input.GetKey("a") || Input.GetKey("d")) && !dashOnCooldown)
            {
                rb.linearVelocityX *= 3.0f;
                dashOnCooldown = true;
            }
            if (dashTimer > dashTime)
            {
                rb.linearVelocityX = 0;
                rb.linearVelocityY = 0;
                dashing = false;
            }
        }

        // --- Movement ---

        else
        {
            if (Input.GetKey("w"))
            {
                rb.linearVelocityY = force;
                facingUp = true;
                facingRight = false;
                facingLeft = false;
                facingDown = false;
            }
            if (Input.GetKey("s"))
            {
                rb.linearVelocityY = -force;
                if (!Input.GetKey("w"))
                {
                    facingDown = true;
                    facingUp = false;
                    facingRight = false;
                    facingLeft = false;
                }
            }
            if (Input.GetKey("d"))
            {
                rb.linearVelocityX = force;
                if (!(Input.GetKey("w") || Input.GetKey("s")))
                {
                    facingRight = true;
                    facingUp = false;
                    facingDown = false;
                    facingLeft = false;
                }
            }
            if (Input.GetKey("a"))
            {
                rb.linearVelocityX = -force;
                if (!(Input.GetKey("w") || Input.GetKey("s") || Input.GetKey("d")))
                {
                    facingRight = false;
                    facingUp = false;
                    facingDown = false;
                    facingLeft = true;
                }
            }
            if (!(Input.GetKey("w") || Input.GetKey("s")))
            {
                rb.linearVelocityY = 0;
            }
            if (!(Input.GetKey("a") || Input.GetKey("d")))
            {
                rb.linearVelocityX = 0;
            }
        }

        // --- Movement Animation Info ---

        if (rb.linearVelocityX == 0 && rb.linearVelocityY == 0)
        {
            animator.SetBool("Moving", false);
        }
        else
        {
            animator.SetBool("Moving", true);
        }
        if (facingUp)
        {
            animator.SetFloat("Idle Index", 2);
            animator.SetFloat("Run Index", 0);
        }
        else if (facingDown)
        {
            animator.SetFloat("Idle Index", 1);
            animator.SetFloat("Run Index", 1);
        }
        else if (facingRight)
        {
            animator.SetFloat("Idle Index", 3);
            animator.SetFloat("Run Index", 3);
        }
        else if (facingLeft)
        {
            animator.SetFloat("Idle Index", 0);
            animator.SetFloat("Run Index", 2);
        }

        // --- Holding Objects ---
        if (holdingItem)
        {
            heldObject.position = new UnityEngine.Vector3(player.position.x + 1, player.position.y + .9f, player.position.z);
        }

        // --- Throwing Objects ---
        
        if (holdingItem && Input.GetKey("q") && pickupCooldown > .4)
        {
            holdingItem = false;
            throwTimer = 0;
            throwing = true;
        }
        if (throwing == true)
        {
            if (facingUp && !locked)
            {
                targetUp = true;
                locked = true;
                
            }
            if (facingDown && !locked)
            {
                targetDown = true;
                locked = true;
            }
            if (facingLeft && !locked)
            {
                targetLeft = true;
                locked = true;
            }
            if (facingRight && !locked)
            {
                targetRight = true;
                locked = true;
            }
            if (targetRight)
            {
                heldObject.position = new UnityEngine.Vector3(heldObject.position.x + throwSpeed, heldObject.position.y, heldObject.position.z);
            }
            if (targetLeft)
            {
                heldObject.position = new UnityEngine.Vector3(heldObject.position.x - throwSpeed, heldObject.position.y, heldObject.position.z);
            }
            if (targetUp)
            {
                heldObject.position = new UnityEngine.Vector3(heldObject.position.x, heldObject.position.y + throwSpeed, heldObject.position.z);
            }
            if (targetDown)
            {
                heldObject.position = new UnityEngine.Vector3(heldObject.position.x, heldObject.position.y - throwSpeed, heldObject.position.z);
            }
            if (throwTimer > throwTime)
            {
                targetUp = false;
                targetDown = false;
                targetLeft = false;
                targetRight = false;
                locked = false;
                throwing = false;
            }
        }
        // Picking Up Objects
        if (Input.GetKey("q") && !holdingItem && !throwing && isCollidingWithObject)
        {
            Pickup();
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
        {
            // --- Picking up Object
        if (collision.CompareTag("Item") && !holdingItem && !throwing)
        {
            isCollidingWithObject = true;
            collidedObject = collision.transform;
            }
        }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        // --- Picking up Object
        if (collision.CompareTag("Item") && !holdingItem)
        {
            isCollidingWithObject = false;
        }
    }
    
    void Pickup()
    {
        holdingItem = true;
        heldObject = collidedObject;
        pickupCooldown = 0;
    }
}

