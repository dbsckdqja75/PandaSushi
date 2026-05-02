using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBookLabel : MonoBehaviour
{
    int recipeLevel = 0;
    
    [SerializeField] GameObject iconPrefab;
    [SerializeField] GameObject assembleArrowPrefab;
    [SerializeField] GameObject finishArrowPrefab;
    
    public void Init(RecipeID targetID)
    {
        List<Sprite> rootIngredients = new();
        List<Sprite> rawIngredients = new();
        List<Sprite> prepIngredients = new();

        RecipeData recipeData = PandaResources.Instance.GetRecipeData(targetID);
        recipeLevel = recipeData.GetTargetLevel();
        
        foreach (IngredientID id in recipeData.GetIngredients())
        {
            IngredientData ingredientData = PandaResources.Instance.GetIngredientData(id);
            prepIngredients.Add(ingredientData.GetIcon());
            
            if (ingredientData.GetParentID() != id)
            {
                IngredientData rawData = PandaResources.Instance.GetIngredientData(ingredientData.GetParentID());
                rawIngredients.Add(rawData.GetIcon());

                if (rawData.GetParentID() != rawData.GetID())
                {
                    rootIngredients.Add(PandaResources.Instance.GetIngredientIcon(rawData.GetParentID()));
                }
            }
        }

        if (rootIngredients.Count > 0)
        {
            foreach (var rootIcon in rootIngredients)
            {
                Instantiate(iconPrefab, this.transform).GetComponent<Image>().sprite = rootIcon;
            }
            
            Instantiate(assembleArrowPrefab, this.transform);
        }
        
        if (rawIngredients.Count > 0)
        {
            foreach (var rawIcon in rawIngredients)
            {
                Instantiate(iconPrefab, this.transform).GetComponent<Image>().sprite = rawIcon;
            }
        
            Instantiate(assembleArrowPrefab, this.transform);
        }

        foreach (var prepIcon in prepIngredients)
        {
            Instantiate(iconPrefab, this.transform).GetComponent<Image>().sprite = prepIcon;
        }

        Instantiate(finishArrowPrefab, this.transform);
        Instantiate(iconPrefab, this.transform).GetComponent<Image>().sprite = recipeData.GetIcon();
    }

    public int GetLevel()
    {
        return recipeLevel;
    }
}
