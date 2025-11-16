using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Utilities for a single ingredient.
/// </summary>
public class Ingredient : MonoBehaviour
{
    [SerializeField] Image ingredientSprite;
    [SerializeField] Image statusSprite;

    [SerializeField] Sprite incompleteSprite;
    [SerializeField] Sprite completeSprite;

    void Start()
    {
        statusSprite.sprite = incompleteSprite;
    }

    public void SetIngredient(Sprite ingredientSprite)
    {
        this.ingredientSprite.color = Color.white;
        transform.GetChild(0).GetComponent<Image>().color = Color.white;
        this.ingredientSprite.sprite = ingredientSprite;
    }

    public void SetComplete()
    {
        statusSprite.sprite = completeSprite;
    }

}
