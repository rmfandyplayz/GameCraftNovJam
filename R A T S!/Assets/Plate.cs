using System;
using JetBrains.Annotations;
using UnityEngine;

public class Plate : GrabbableBase
{
    [SerializeField] private Vector2 checkForThings;
    [SerializeField] private LayerMask layerMask;

    [CanBeNull] private FoodItem currentItem;


    public override void Use(Player player)
    {
        foreach (Collider2D collider in  Physics2D.OverlapBoxAll(transform.position, checkForThings, 0, layerMask))
        {
            Debug.Log(collider.gameObject.name);
            if (!collider.gameObject.CompareTag("FoodObjective")) continue;
            Debug.Log("IS AN OBJECTIVE THINGY!");
            
            if (collider.GetComponent<FoodItem>() is { } item)
            {
                currentItem ??= item;
            } else if (collider.GetComponent<Oven>() is { } oven)
            {
                if (currentItem is null) return;
                oven.StoreItem(currentItem);
                currentItem = null;
            }
        }
    }

    private void Update()
    {
        if (!currentItem) return;

        currentItem.transform.position = transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, checkForThings);
    }
}
