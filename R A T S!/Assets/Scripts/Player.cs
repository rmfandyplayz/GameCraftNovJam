using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Vector2 = UnityEngine.Vector2;

public class Player : MonoBehaviour
{
    // --- Internals ---
    [Header("Components")] public GrabbableBase heldObject;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer itemRenderer;
    private LightController lightController;
    private BoxCollider2D boxCollider;

    // --- Movement Variables ---
    [Header("Movement")] public float force;
    private bool dashing;
    private bool dashOnCooldown;
    public float dashMultiplier = 3;
    private Transform hand;
    private bool posessed => posessionTimer > 0;

    [Header("Sound Effects")] public AudioSource stepSound;

    // HP
    [Header("Light")] public float maxLight = 100;
    [HideInInspector] public float lightLeft;
    [SerializeField] private float lightDecayPerSecond = 1;

    // --- Held Item Variables ---
    [Header("Held Items")] public float throwSpeed;
    private bool holdingItem => heldObject != null;

    // Item we can currently pick up (for highlight & pickup)
    private GrabbableBase pickupCandidate;

    [Header("Timers")] public float dashTime;
    private float dashTimer;
    public float dashCooldownTime;
    public float pickupCooldown;
    [SerializeField] private float iFrameTime;
    private float iFrameTimer;
    public float posessionTimer;
    private float lastPossessTime;

    private Vector2 movementInput;
    [HideInInspector] public Vector2 faceDir;

    [Header("Dash Ghost")] public Sprite ghostSprite; // sprite to use for ghosts
    public Color ghostColor = new Color(1f, 1f, 0f, 0.6f); // yellow & transparent
    public float ghostLifetime = 0.25f;
    public float ghostInterval = 0.03f;

    private float ghostTimer;
    private SpriteRenderer playerSR;

    // Damage flash
    private Color originalColor;
    private float damageFlashTimer;

    [SerializeField] private float beginDamageFilter;
    private Volume dangerVolume;

    private SpriteRenderer lanternSprite;
    private GlobalPlayerUI playerUI;

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

        lanternSprite = transform.Find("Lantern").GetChild(0).GetComponent<SpriteRenderer>();

        playerUI = FindAnyObjectByType<GlobalPlayerUI>();
    }

    public void Damage(float amount)
    {
        if (iFrameTimer > 0) return;

        // --- Start red flash ---
        if (playerSR != null)
        {
            playerSR.color = Color.red;
            damageFlashTimer = 0.1f; // flash duration
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
        if (posessed)
        {
            return;
        }

        movementInput = context.ReadValue<Vector2>();
        if (movementInput.magnitude != 0)
            faceDir = movementInput;
    }

    public void Possess(float possessTime)
    {
        posessionTimer = possessTime;
        lastPossessTime = possessTime;
        if (heldObject is not null)
        {
            GrabThrow();
        }
    }

    // Update is called once per frame
    void Update()
    {
        dangerVolume.weight = Mathf.Clamp((beginDamageFilter - lightLeft) / beginDamageFilter, 0, 1);

        // --- Damage flash timer ---
        if (damageFlashTimer > 0f)
        {
            damageFlashTimer -= Time.deltaTime;
            if (damageFlashTimer <= 0f && playerSR != null)
            {
                playerSR.color = originalColor;
            }
        }


        // --- Timers ---
        pickupCooldown += Time.deltaTime;
        iFrameTimer -= Time.deltaTime;

        lightLeft -= Time.deltaTime * lightDecayPerSecond;
        lightController.lightAmount = 1 - (lightLeft / maxLight);

        // --- Dashing ---
        if (posessed)
        {
            posessionTimer -= Time.deltaTime;
            if (posessionTimer < lastPossessTime)
            {
                lastPossessTime -= 1;
                int randDir = Random.Range(0, 4);
                movementInput = randDir switch
                {
                    0 => Vector2.right,
                    1 => Vector2.down,
                    2 => Vector2.left,
                    3 => Vector2.up,
                };
                faceDir = movementInput;
            }
        }

        if (!posessed)
        {
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
        }
        // --- Movement ---

        if (!dashing)
        {
            rb.linearVelocity = movementInput * force;
        }

        // --- Movement Animation Info ---
        // --- Movement Animation Info ---
        bool isMoving = rb.linearVelocityX != 0 || rb.linearVelocityY != 0;
        animator.SetBool("Moving", isMoving);
        animator.SetBool("Possessed", posessed);


        if (!dashing)
        {
            if (faceDir.y > 0)
            {
                animator.SetFloat("Idle Index", 2);
                animator.SetFloat("Run Index", 0);
                lanternSprite.sortingOrder = playerSR.sortingOrder - 20;
                if (itemRenderer)
                    itemRenderer.sortingOrder = playerSR.sortingOrder - 20;
            }
            else if (faceDir.y < 0)
            {
                animator.SetFloat("Idle Index", 1);
                animator.SetFloat("Run Index", 1);
                lanternSprite.sortingOrder = playerSR.sortingOrder + 20;
                if (itemRenderer)
                    itemRenderer.sortingOrder = playerSR.sortingOrder + 20;
            }
            else if (faceDir.x > 0)
            {
                animator.SetFloat("Idle Index", 3);
                animator.SetFloat("Run Index", 3);
                lanternSprite.sortingOrder = playerSR.sortingOrder + 20;
                if (itemRenderer)
                    itemRenderer.sortingOrder = playerSR.sortingOrder - 20;
            }
            else if (faceDir.x < 0)
            {
                animator.SetFloat("Idle Index", 0);
                animator.SetFloat("Run Index", 2);
                lanternSprite.sortingOrder = playerSR.sortingOrder - 20;
                if (itemRenderer)
                    itemRenderer.sortingOrder = playerSR.sortingOrder + 20;
            }
        }

        // --- Holding Objects ---
        if (holdingItem)
        {
            heldObject.transform.position =
                hand.position + Quaternion.Euler(0, 0, heldObject.offsetRotation) * heldObject.offsetPos;
            heldObject.transform.rotation = Quaternion.Euler(0, 0, heldObject.offsetRotation);
        }

        lanternSprite.transform.parent.localPosition = new Vector3(
            -hand.transform.localPosition.x,
            lanternSprite.transform.parent.localPosition.y,
            0
        );

        // --- Dash Ghost ---
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
        
        playerUI.SetLampMeter((lightLeft / maxLight) * 100);

        // --- Pickup Candidate / Indicator ---
        UpdatePickupCandidate();
    }

    public void ViewRecipe(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            playerUI.OpenRecipeBook();
            return;
        }
        if (context.canceled)
        {
            playerUI.CloseRecipeBook();
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
        if (heldObject is null)
        {
            TryPickup();
        }
        else
        {
            heldObject.Use(this);
        }
    }

    public void GrabThrow()
    {
        if (pickupCooldown < .4f)
        {
            return;
        }

        if (!holdingItem) return;
        Vector2 throwForce = faceDir * throwSpeed;
        Rigidbody2D heldRB = heldObject.GetComponent<Rigidbody2D>();
        heldObject.GetComponent<Collider2D>().isTrigger = false;
        heldRB.bodyType = RigidbodyType2D.Dynamic;
        heldRB.AddForce(throwForce);
        pickupCooldown = 0;

        heldObject = null;
        itemRenderer = null;
    }

    void TryPickup()
    {
        // Only pick up the current candidate
        if (holdingItem || pickupCandidate == null)
            return;

        heldObject = pickupCandidate;
        pickupCooldown = 0;

        Rigidbody2D heldRB = heldObject.GetComponent<Rigidbody2D>();
        heldObject.GetComponent<Collider2D>().isTrigger = true;
        heldRB.bodyType = RigidbodyType2D.Kinematic;

        itemRenderer = heldObject.GetComponent<SpriteRenderer>();

        // Turn off the indicator on the picked-up item
        SetPickupIndicator(heldObject, false);
        pickupCandidate = null;
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

    public void PlayStepSound()
    {
        stepSound.Play();
    }

    // Turn indicator child on/off for a given item
    private void SetPickupIndicator(GrabbableBase target, bool state)
    {
        if (target == null) return;

        Transform indicator = null;

        // Prefer a child tagged "PickupHint"
        foreach (Transform child in target.transform)
        {
            if (child.CompareTag("PickupHint"))
            {
                indicator = child;
                break;
            }
        }

        // Fallback: use first child if no tagged hint
        if (indicator == null && target.transform.childCount > 0)
        {
            indicator = target.transform.GetChild(0);
        }

        if (indicator != null)
        {
            indicator.gameObject.SetActive(state);
        }
    }

    // Find which item we *can* pick up and toggle indicator
    private void UpdatePickupCandidate()
    {
        // Turn off old indicator first
        if (pickupCandidate != null)
        {
            SetPickupIndicator(pickupCandidate, false);
            pickupCandidate = null;
        }

        // If we're already holding something, no candidates
        if (holdingItem)
            return;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            transform.position + (Vector3)boxCollider.offset,
            boxCollider.size,
            0f
        );

        GrabbableBase best = null;
        float bestDist = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            if (!collider.gameObject.CompareTag("Item"))
                continue;

            GrabbableBase grab = collider.gameObject.GetComponent<GrabbableBase>();
            if (grab == null)
                continue;

            float dist = ((Vector2)transform.position - (Vector2)grab.transform.position).sqrMagnitude;
            if (dist < bestDist)
            {
                bestDist = dist;
                best = grab;
            }
        }

        pickupCandidate = best;

        // Turn on indicator for the new candidate (if any)
        if (pickupCandidate != null)
        {
            SetPickupIndicator(pickupCandidate, true);
        }
    }
}