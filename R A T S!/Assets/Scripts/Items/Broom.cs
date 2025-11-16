using System;
using UnityEngine;

public class Broom : GrabbableBase
{
    private Animator animator;
    private Rigidbody2D rb;

    [SerializeField] private float lethalVelocity;

    private float attackTimer;
    private const float attackTime = .6f;

    private bool attacking => attackTimer > 0 || rb.linearVelocity.magnitude > lethalVelocity;

    [Header("Sound Effects")]
    public AudioSource attack;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!attacking)
            return;

        if (other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<RatBase>().TakeDamage();
        }
    }

    public override void Use(Player player)
    {
        if (attacking)
            return;
        animator.SetInteger("FaceDIr", player.GetFacingIndex());
        attack.Play();
        animator.SetTrigger("Swing");
        attackTimer = attackTime;
    }
}
