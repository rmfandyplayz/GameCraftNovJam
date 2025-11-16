using UnityEngine;

/// <summary>
/// Functions for player UI control.
/// </summary>
public class GlobalPlayerUI : MonoBehaviour
{
    [SerializeField] RecipeController recipeController;
    [SerializeField] LampMeterController lampController;

    public void SetLampMeter(float newAmount)
    {
        lampController.SetLampMeter(newAmount);
    }

    public void OpenRecipeBook()
    {
        recipeController.OpenIngredients();
    }

    public void CloseRecipeBook()
    {
        recipeController.CloseIngredients();
    }

    /// <summary>
    /// Marks a target ingredient as complete
    /// </summary>
    /// <param name="ingredientIndex"></param>
    public void CompleteIngredient(int ingredientIndex)
    {
        recipeController.CompleteIngredient(ingredientIndex);
    }

    /// <summary>
    /// Sets an ingredient on the recipe book to a desired ingredient
    /// via the ingredientID.
    /// </summary>
    /// <param name="ingredientSpriteIndex"></param>
    /// <param name="ingredientID"></param>
    public void SetIngredient(int ingredientSpriteIndex, Sprite ingredientID)
    {
        recipeController.SetIngredient(ingredientSpriteIndex, ingredientID);
    }
}
