using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class Plate : GrabbableBase
{
    [SerializeField] private Vector2 checkForThings;
    [SerializeField] private LayerMask layerMask;

    private Stack<FoodItem> foodItems = new();

    [SerializeField] private Vector3 itemOffset;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Use(Player player)
    {
        foreach (Collider2D collider in  Physics2D.OverlapBoxAll(transform.position, checkForThings, 0, layerMask))
        {
            if (!collider.gameObject.CompareTag("FoodObjective")) continue;
            
            if (collider.GetComponent<FoodItem>() is { } item)
            {
                foodItems.Push(item);
                collider.enabled = false;
                return;
            }

            if (collider.GetComponent<Oven>() is { } oven)
            {
                if (foodItems.Count == 0) return;
                if (foodItems.Peek().finalMeal) return;
                oven.StoreItem(foodItems.Pop());
                return;
            }

            if (collider.GetComponent<ServeTable>() is { } table && foodItems.Peek().finalMeal)
            {
                table.PlaceItem(foodItems.Pop());
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < foodItems.Count; i++)
        {
            FoodItem item = foodItems.ToArray()[i];
            if (!item)
            {
                continue;
            }
            item.transform.position = transform.position + itemOffset * i;
            item.spriteRenderer.sortingOrder = spriteRenderer.sortingOrder + (i+1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, checkForThings);
    }
}