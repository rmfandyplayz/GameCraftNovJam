using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Vector2 = UnityEngine.Vector2;

public class Player : MonoBehaviour
{
    // --- Internals ---
    [Header("Components")]
    public GrabbableBase heldObject;
    private Rigidbody2D rb; 
    private Animator animator;
    private SpriteRenderer itemRenderer;
    private LightController lightController;
    private BoxCollider2D boxCollider;
    
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
    private GrabbableBase pickupCandidate;   // item we can currently pick up


    [Header("Timers")]
    // --- Timers ---
    public float dashTime;
    private float dashTimer;
    public float dashCooldownTime;
    public float pickupCooldown;
    [SerializeField] private float iFrameTime;
    private float iFrameTimer;

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

    // NEW: damage flash
    private Color originalColor;
    private float damageFlashTimer;


    [SerializeField] private float beginDamageFilter;
    private Volume dangerVolume;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerSR = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        hand = transform.GetChild(0);

        originalColor = playerSR.color;

        lightLeft = maxLight;
        lightController = transform.Find("DisapperingLight").GetComponent<LightController>();

        dangerVolume = transform.Find("DangerVolume").GetComponent<Volume>();
    }

    public void Damage(float amount)
    {
        if (iFrameTimer > 0) return;

        // --- Start red flash ---
        if (playerSR != null)
        {
            playerSR.color = Color.red;
            damageFlashTimer = 0.1f;   // flash duration
        }

        if (lightLeft <= 0)
        {
            FadeoutTransition.SceneTransition("DieScreen");
            iFrameTimer = 999;
        }
        else
        {
            iFrameTimer = iFrameTime;
        }

        lightLeft -= amount;
    }

    
public void Dash()
{
    if (dashOnCooldown)
        return;

    dashing = true;
    dashOnCooldown = true;

    Vector2 dashDir;

    if (movementInput.sqrMagnitude > 0.01f)
        dashDir = movementInput;
    else if (faceDir.sqrMagnitude > 0.01f)
        dashDir = faceDir;
    else
        dashDir = Vector2.right; 

    dashDir = dashDir.normalized;

    // Actually set dash velocity
    rb.linearVelocity = dashDir * force * dashMultiplier;
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
        dangerVolume.weight = Mathf.Clamp((beginDamageFilter - lightLeft) / beginDamageFilter,0,1);

        // --- Timers ---
        
            // --- Damage flash timer ---
        if (damageFlashTimer > 0f)
        {
            damageFlashTimer -= Time.deltaTime;
            if (damageFlashTimer <= 0f && playerSR != null)
            {
                playerSR.color = originalColor;
            }
        }

        pickupCooldown += Time.deltaTime;
        iFrameTimer -= Time.deltaTime;

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
                if(itemRenderer)
                    itemRenderer.sortingOrder = playerSR.sortingOrder - 20;
            }
            else if (faceDir.y < 0)
            {
                animator.SetFloat("Idle Index", 1);
                animator.SetFloat("Run Index", 1);
                if(itemRenderer)
                    itemRenderer.sortingOrder = playerSR.sortingOrder + 20;
            }
            else if (faceDir.x > 0)
            {
                animator.SetFloat("Idle Index", 3);
                animator.SetFloat("Run Index", 3);
                if(itemRenderer)
                    itemRenderer.sortingOrder = playerSR.sortingOrder - 20;
            }
            else if (faceDir.x < 0)
            {
                animator.SetFloat("Idle Index", 0);
                animator.SetFloat("Run Index", 2);
                if(itemRenderer)
                    itemRenderer.sortingOrder = playerSR.sortingOrder + 20;
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

        UpdatePickupCandidate();
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
            itemRenderer = null;
        }
        else
        {
            TryPickup();
        }
    }
    
    
    void TryPickup()
    {
        // Check for Pickups
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + (Vector3)boxCollider.offset, boxCollider.size, 0);
        heldObject = null;
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag("Item"))
            {
                heldObject = collider.gameObject.GetComponent<GrabbableBase>();
                break;
            }
        }

        if (heldObject is null)
            return;
        
        SetPickupIndicator(heldObject, false);
        pickupCandidate = null;

        pickupCooldown = 0;
        
        Rigidbody2D heldRB = heldObject.GetComponent<Rigidbody2D>();
        heldObject.GetComponent<Collider2D>().isTrigger = true;
        heldRB.bodyType = RigidbodyType2D.Kinematic;

        itemRenderer = heldObject.GetComponent<SpriteRenderer>();
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

    private void SetPickupIndicator(GrabbableBase target, bool state)
    {
        if (target == null) return;

        // Assuming the indicator is the first child of the item
        if (target.transform.childCount > 0)
        {
            Transform child = target.transform.GetChild(0);
            if (child != null)
                child.gameObject.SetActive(state);
        }
    }

    private void UpdatePickupCandidate()
{
    // Turn off old indicator first
    if (pickupCandidate != null)
    {
        SetPickupIndicator(pickupCandidate, false);
        pickupCandidate = null;
    }

    // If we’re already holding something, no candidates
    if (holdingItem)
        return;

    // Same OverlapBox you use in TryPickup
    Collider2D[] colliders = Physics2D.OverlapBoxAll(
        transform.position + (Vector3)boxCollider.offset,
        boxCollider.size,
        0f
    );

    foreach (Collider2D collider in colliders)
    {
        if (collider.gameObject.CompareTag("Item"))
        {
            pickupCandidate = collider.gameObject.GetComponent<GrabbableBase>();
            break;
        }
    }

    // Turn on indicator for the new candidate (if any)
    if (pickupCandidate != null)
    {
        SetPickupIndicator(pickupCandidate, true);
    }
}

}

