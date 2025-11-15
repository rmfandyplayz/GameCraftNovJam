using System;
using UnityEngine;

public class Broom : GrabbableBase
{
    private Animator animator;

    private float attackTimer;
    private const float attackTime = .6f;

    private bool attacking => attackTimer > 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
        animator.SetInteger("FaceDIr", player.GetFacingIndex());
        animator.SetTrigger("Swing");
        attackTimer = attackTime;
    }
}
