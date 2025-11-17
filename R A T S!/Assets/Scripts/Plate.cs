using System;
using System.Collections.Generic;
using UnityEngine;

public class Plate : GrabbableBase
{
    [SerializeField] private Vector2 checkForThings;
    [SerializeField] private LayerMask layerMask;

    private Stack<FoodItem> foodItems = new();

    [SerializeField] private Vector3 itemOffset;

    private SpriteRenderer spriteRenderer;

    // Track last oven that had its hint turned on
    private Oven lastOvenWithHint;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Use(Player player)
    {
        foreach (Collider2D collider in Physics2D.OverlapBoxAll(transform.position, checkForThings, 0, layerMask))
        {
            if (!collider.gameObject.CompareTag("FoodObjective")) 
                continue;
            
            // 1) Pick up food onto the plate
            if (collider.GetComponent<FoodItem>() is { } item)
            {
                foodItems.Push(item);
                collider.enabled = false; // mark as "on plate"
                // turn off that food's hint (since it's no longer pickable)
                SetHintOnChildren(item.transform, false);
                return;
            }

            // 2) Put food from plate into oven
            if (collider.GetComponent<Oven>() is { } oven)
            {
                if (foodItems.Count == 0) return;
                if (foodItems.Peek().finalMeal) return;

                oven.StoreItem(foodItems.Pop());
                // we just removed from plate, so hints will update automatically next frame
                return;
            }

            // 3) Serve table with final meal (no special hint logic here right now)
            if (collider.GetComponent<ServeTable>() is { } table && foodItems.Peek().finalMeal)
            {
                table.PlaceItem(foodItems.Pop());
                return;
            }
        }
    }

    private void Update()
    {
        // Keep food stacked on the plate visually
        for (int i = 0; i < foodItems.Count; i++)
        {
            FoodItem item = foodItems.ToArray()[i];
            if (!item) continue;

            item.transform.position = transform.position + itemOffset * i;
            item.spriteRenderer.sortingOrder = spriteRenderer.sortingOrder + (i + 1);
        }

        UpdateInteractionHints();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, checkForThings);
    }

    // ---------------- HINT LOGIC ----------------

    // Turn hints ON/OFF for a target that already has children with hint sprites
    private void SetHintOnChildren(Transform targetRoot, bool state)
    {
        if (targetRoot == null) return;

        // Look through ALL descendants, including inactive ones
        foreach (Transform child in targetRoot.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag("PickupHint"))
            {
                child.gameObject.SetActive(state);
            }
        }
    }

    private void UpdateInteractionHints()
    {
        // --- 0) Always turn OFF the last oven hint at the start of the frame ---
        if (lastOvenWithHint != null)
        {
            SetHintOnChildren(lastOvenWithHint.transform, false);
            lastOvenWithHint = null;
        }

        // 1) Find everything in the plate's interaction box
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            transform.position,
            checkForThings,
            0f,
            layerMask
        );

        // 2) First, turn OFF hints for all food/ovens inside the box.
        //    (We’ll turn the correct ones back ON in the second pass.)
        foreach (var col in colliders)
        {
            if (!col.gameObject.CompareTag("FoodObjective")) 
                continue;

            if (col.TryGetComponent<FoodItem>(out var food))
            {
                SetHintOnChildren(food.transform, false);
            }

            if (col.TryGetComponent<Oven>(out var oven))
            {
                SetHintOnChildren(oven.transform, false);
            }

            if (col.TryGetComponent<ServeTable>(out var table))
            {
                SetHintOnChildren(table.transform, false);
            }
        }

        // 3) Second pass: enable hints only when the plate can actually interact.

        foreach (var col in colliders)
        {
            if (!col.gameObject.CompareTag("FoodObjective"))
                continue;

            // --- Food hints: show when the plate can pick up this food ---
            if (col.TryGetComponent<FoodItem>(out var food))
            {
                // We consider "can pick up" = its collider is still enabled
                if (col.enabled)
                {
                    SetHintOnChildren(food.transform, true);
                }
            }

            // --- Oven hints: show when the plate can give food to an oven ---
            if (col.TryGetComponent<Oven>(out var oven))
            {
                if (foodItems.Count > 0 && !foodItems.Peek().finalMeal)
                {
                    SetHintOnChildren(oven.transform, true);
                    lastOvenWithHint = oven;   // remember which oven we lit up
                }
            }

            if (col.TryGetComponent<ServeTable>(out var table))
            {
                if (foodItems.Count > 0 && foodItems.Peek().finalMeal)
                {
                    SetHintOnChildren(table.transform, true);
                }
            }
        }
    }
}
