using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mixer : MonoBehaviour
{
    public bool CanMix(List<IngredientID> targets)
    {
        var recipeList = PandaResources.Instance.GetSatisfyRecipe(targets);
        if (recipeList != null && recipeList.Count > 0)
        {
            return true;
        }

        return false;
    }
    
    public bool MixIngredient(RecipePlate targetPlate)
    {
        List<IngredientID> allTargets = targetPlate.GetPlatedIngredientID().ToList();
        return (TryMix(allTargets, targetPlate) != null);
    }
    
    public RecipeID TrySingleMix(IngredientID target)
    {
        var recipeData = PandaResources.Instance.GetFinishableRecipe(new List<IngredientID>() { target });
        if (recipeData != null)
        {
            return recipeData.GetID();
        }

        return RecipeID.NULL;
    }

    RecipeData TryMix(List<IngredientID> targets, RecipePlate targetPlate)
    {
        var recipeList = PandaResources.Instance.GetSatisfyRecipe(targets);
        if(recipeList != null && recipeList.Count > 0)
        {
            // NOTE: 유일 레시피는 완성 처리
            if(recipeList.Count == 1)
            {
                if(recipeList[0].IsCompletedRecipe(targets))
                {
                    targetPlate.UpdateRecipe(targets, recipeList[0]);
                    return recipeList[0];
                }
            }

            targetPlate.UpdateIngredient(targets);

            return recipeList[0];
        }
        else
        {
            Debug.Log("조합 불가");
        }

        return null;
    }
}