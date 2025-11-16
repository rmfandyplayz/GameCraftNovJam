using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Oven : MonoBehaviour
{
    private List<FoodItem> foodRequired;
    private List<FoodItem> acquiredItems = new();


    private void Start()
    {
        foodRequired = FindObjectsByType<FoodItem>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).ToList();
    }

    public void StoreItem(FoodItem item)
    {
        foodRequired.Remove(item);
        Destroy(item.gameObject);

        if (foodRequired.Count == 0)
        {
            Debug.Log("Time Tah Go!");
        }
    }
}
