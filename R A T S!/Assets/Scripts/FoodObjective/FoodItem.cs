using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [HideInInspector] public SpriteRenderer spriteRenderer;

    public bool finalMeal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
