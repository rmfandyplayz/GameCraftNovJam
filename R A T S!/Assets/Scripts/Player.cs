using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

public class Player : MonoBehaviour
{
    // --- Internals ---
    [Header("Components")]
    public GrabbableBase heldObject;
    private Rigidbody2D rb;
    public Transform collidedObject;
    private Animator animator;
    private SpriteRenderer broomRenderer;
    private LightController lightController;
    
    // --- Movement Variables ---
    [Header("Movement")]
    public float force;
    private bool dashing;
    private bool dashOnCooldown;
    public float dashMultiplier = 3;
    private Transform hand;
    
    // HP
    [Header("Light")]
    public float maxLight = 100;
    [HideInInspector] public float lightLeft;
    [SerializeField] private float lightDecayPerSecond = 1;
    

    // --- Held Item Variables ---
    [Header("Held Items")]
    public float throwSpeed;
    private bool holdingItem => heldObject != null;

    [Header("Timers")]
    // --- Timers ---
    public float dashTime;
    private float dashTimer;
    public float dashCooldownTime;
    public float pickupCooldown;

    private bool isCollidingWithObject;

    private Vector2 movementInput;
    [HideInInspector] public Vector2 faceDir;

    [Header("Dash Ghost")]
    public Sprite ghostSprite;           // sprite to use for ghosts
    public Color ghostColor = new Color(1f, 1f, 0f, 0.6f); // yellow & transparent
    public float ghostLifetime = 0.25f;  
    public float ghostInterval = 0.03f;  

    private float ghostTimer = 0f;
    private SpriteRenderer playerSR;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerSR = GetComponent<SpriteRenderer>();
        hand = transform.GetChild(0);
        broomRenderer = GameObject.FindGameObjectWithTag("Item").transform.parent.GetComponent<SpriteRenderer>();

        lightLeft = maxLight;
        lightController = transform.Find("DisapperingLight").GetComponent<LightController>();
    }

    public void Damage(float amount)
    {
        lightLeft -= amount;
    }
    
    public void Dash()
    {
        if (dashOnCooldown)
            return;
        dashing = true;
        dashOnCooldown = true;
        rb.linearVelocity *= dashMultiplier;
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        if(movementInput.magnitude != 0)
            faceDir = movementInput;
    }

    // Update is called once per frame
    void Update()
    {

        // --- Timers ---
        
        pickupCooldown += Time.deltaTime;

        lightLeft -= Time.deltaTime * lightDecayPerSecond;
        lightController.lightAmount = 1 - (lightLeft / maxLight);

        // --- Dashing ---
        if (dashOnCooldown)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer > dashTime)
            {
                dashing = false;
                if (dashTimer > dashTime + dashCooldownTime)
                {
                    dashOnCooldown = false;
                    dashTimer = 0;
                }
            }
        }

        // --- Movement ---
        if (!dashing)
        {
            rb.linearVelocity = movementInput * force;
        }
        
        // --- Movement Animation Info ---


        animator.SetBool("Moving", rb.linearVelocityX == 0 && rb.linearVelocityY == 0);

        if (!dashing)
        {
            if (faceDir.y > 0)
            {
                animator.SetFloat("Idle Index", 2);
                animator.SetFloat("Run Index", 0);
                broomRenderer.sortingOrder = 4;
            }
            else if (faceDir.y < 0)
            {
                animator.SetFloat("Idle Index", 1);
                animator.SetFloat("Run Index", 1);
                broomRenderer.sortingOrder = 4;
            }
            else if (faceDir.x > 0)
            {
                animator.SetFloat("Idle Index", 3);
                animator.SetFloat("Run Index", 3);
                broomRenderer.sortingOrder = 4;
            }
            else if (faceDir.x < 0)
            {
                animator.SetFloat("Idle Index", 0);
                animator.SetFloat("Run Index", 2);
                broomRenderer.sortingOrder = 6;
            }
        }

        // --- Holding Objects ---
        if (holdingItem)
        {
            heldObject.transform.position = hand.position + Quaternion.Euler(0,0, heldObject.offsetRotation) * heldObject.offsetPos;
            heldObject.transform.rotation = Quaternion.Euler(0,0, heldObject.offsetRotation);
        }

        // --- Throwing Objects ---



        if (dashing)
        {
            ghostTimer += Time.deltaTime;
        if (ghostTimer >= ghostInterval)
        {
            ghostTimer = 0f;
            SpawnGhost();
        }
    }
    else
    {
        ghostTimer = 0f; // reset when dash ends
    }
    }

    public int GetFacingIndex()
    {
        if (faceDir.y > 0)
        {
            return 2;
        }
        else if (faceDir.y < 0)
        {
            return 1;
        }
        else if (faceDir.x > 0)
        {
            return 3;
        }
        return 0;
    }

    public void UseItem(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;
        heldObject?.Use(this);
    }

    public void GrabThrow()
    {
        if (pickupCooldown < .4f)
        {
            return;
        }

        if (holdingItem)
        {
            Vector2 throwForce = faceDir * throwSpeed;
            Rigidbody2D heldRB = heldObject.GetComponent<Rigidbody2D>();
            heldObject.GetComponent<Collider2D>().isTrigger = false;
            heldRB.bodyType = RigidbodyType2D.Dynamic;
            heldRB.AddForce(throwForce);
            pickupCooldown = 0;

            heldObject = null;
        }
        else
        {
            if (isCollidingWithObject)
            {
                Pickup();
            }
        }
    }
    
        void OnTriggerEnter2D(Collider2D collision)
        {
            // --- Picking up Object
            if (collision.CompareTag("Item") && !holdingItem)
            {
                isCollidingWithObject = true;
                collidedObject = collision.transform.parent;
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
        heldObject = collidedObject.GetComponent<GrabbableBase>();
        pickupCooldown = 0;
        
        Rigidbody2D heldRB = heldObject.GetComponent<Rigidbody2D>();
        heldObject.GetComponent<Collider2D>().isTrigger = true;
        heldRB.bodyType = RigidbodyType2D.Kinematic;
    }

    private void SpawnGhost()
    {
        GameObject g = new GameObject("DashGhost");
        g.transform.position = transform.position;
        g.transform.rotation = transform.rotation;

        SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
        sr.sprite = ghostSprite != null ? ghostSprite : playerSR.sprite;
        sr.flipX = playerSR.flipX;
        sr.flipY = playerSR.flipY;
        sr.sortingLayerID = playerSR.sortingLayerID;
        sr.sortingOrder = playerSR.sortingOrder - 1;

        sr.color = ghostColor;

        Destroy(g, ghostLifetime);
    }

}

