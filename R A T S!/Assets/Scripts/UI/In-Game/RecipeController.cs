using UnityEngine;
using UnityEngine.UI;

public class RecipeController : MonoBehaviour
{
    Ingredients ingredients;

    Image test;

    [SerializeField] Ingredient[] ingredientList;
    [SerializeField] Sprite[] ingredientImages;

    [SerializeField] GameObject recipeIcon;
    [SerializeField] GameObject recipeBook;
    [SerializeField] GameObject keyBind;

    void Start()
    {
        recipeIcon.SetActive(true);
        keyBind.SetActive(true);
        recipeBook.SetActive(false);
    }

    public void OpenIngredients()
    {
        recipeIcon.SetActive(false);
        keyBind.SetActive(false);
        recipeBook.SetActive(true);
    }

    public void CloseIngredients()
    {
        recipeIcon.SetActive(true);
        keyBind.SetActive(true);
        recipeBook.SetActive(false);
    }

    public void CompleteIngredient(int ingredientSpriteIndex)
    {
        ingredientList[ingredientSpriteIndex].SetComplete();
    }

    public void SetIngredient(int ingredientSpriteIndex, Ingredients ingredientID)
    {
        ingredientList[ingredientSpriteIndex].SetIngredient(GetIngredientImage(ingredientID));
    }

    Sprite GetIngredientImage(Ingredients ingredientID)
    {
        return ingredientImages[(int) ingredientID];
    }
}


public enum Ingredients
{
    BLOOD,
    BONES,
    BRAIN,
    EYEBALLS,
    FLIES,
    FROG,
    GOO,
    HAND,
    LETTUCE,
    MUD,
    NOODLES,
    PIG_SKULL,
    RAT_POISON,
    TOMATO
}
