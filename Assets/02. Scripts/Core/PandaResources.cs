using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PandaResources : MonoSingleton<PandaResources>
{
    // NOTE : 음식과 관련된 데이터는 모두 이곳에서 불러오고 처리
    [SerializeField] GameObject emptyPlatePrefab;
    [SerializeField] GameObject dirtyPlatePrefab;
    [SerializeField] Sprite emptySprite;
    
    [SerializeField] List<IngredientData> allIngredientData = new();
    [SerializeField] List<MixData> allMixData = new();
    [SerializeField] List<RecipeData> allRecipeData = new();
    [SerializeField] List<DecoData> allDecoData = new();

    Dictionary<IngredientID, IngredientData> ingredientData;
    Dictionary<RecipeID, RecipeData> recipeData;
    Dictionary<DecoID, List<DecoData>> decoData;

    protected override void Init()
    {
        ingredientData = new();
        recipeData = new();
        decoData = new();
        
        foreach(IngredientData data in allIngredientData.OrderBy(x => x.GetTargetLevel()))
        {
            IngredientID id = data.GetID();
            if(ingredientData.ContainsKey(id) == false)
            {
                ingredientData.Add(id, data);
            }
        }

        foreach(RecipeData data in allRecipeData)
        {
            RecipeID id = data.GetID();
            if(recipeData.ContainsKey(id) == false)
            {
                recipeData.Add(id, data);
            }
        }
        
        foreach(DecoData data in allDecoData)
        {
            DecoID id = data.GetID();
            if(decoData.ContainsKey(id) == false)
            {
                decoData.Add(id, new());
            }
            
            decoData[id].Add(data);
        }
        
        foreach(DecoID decoID in decoData.Keys.ToList())
        {
            decoData[decoID] = decoData[decoID].OrderBy(x => x.GetOrder()).ToList();
        }
    }

    public GameObject GetIngredientModel(IngredientID targetID)
    {
        if(ingredientData.ContainsKey(targetID))
        {
            return ingredientData[targetID].GetPlatePrefab();
        }

        return emptyPlatePrefab;
    }

    public Sprite GetIngredientIcon(IngredientID targetID)
    {
        if(ingredientData.ContainsKey(targetID))
        {
            return ingredientData[targetID].GetIcon();
        }

        return emptySprite;
    }

    public IngredientData GetIngredientData(IngredientID targetID)
    {
        if(ingredientData.ContainsKey(targetID))
        {
            return ingredientData[targetID];
        }

        return null;
    }

    public Sprite GetRecipeIcon(RecipeID targetID)
    {
        if(recipeData.ContainsKey(targetID))
        {
            return recipeData[targetID].GetIcon();
        }

        return emptySprite;
    }

    public RecipeData GetRecipeData(RecipeID targetID)
    {
        if(recipeData.ContainsKey(targetID))
        {
            return recipeData[targetID];
        }

        return null;
    }

    public DecoData GetDecoData(DecoID targetID, int targetIdx)
    {
        if (decoData.ContainsKey(targetID))
        {
            if (targetID >= 0 &&  targetIdx < decoData[targetID].Count)
            {
                return decoData[targetID][targetIdx];
            }
        }

        return null;
    }

    public int GetDecoCount(DecoID targetID)
    {
        if (decoData.ContainsKey(targetID))
        {
            return decoData[targetID].Count;
        }
        
        return 0;
    }

    public int GetDecoPrice(DecoID targetID, int targetIdx)
    {
        if (decoData.ContainsKey(targetID))
        {
            return decoData[targetID][targetIdx].GetPrice();
        }

        return 0;
    }

    public List<IngredientID> GetRawIngredientKeys()
    {
        var targetKeys = ingredientData.Keys.Where(x => (int)x < 100 || (int)x >= 1000).ToList();
        targetKeys.Add(IngredientID.Prepared_Crabstick);
        targetKeys.Add(IngredientID.Prepared_Gyoza);
        targetKeys.Add(IngredientID.Prepared_Dumpling);
        
        return targetKeys.OrderBy(id => (int)id).ToList();
    }

    public List<RecipeID> GetRecipeKeys()
    {
        return recipeData.Keys.ToList();
    }

    public GameObject GetDirtyPlatePrefab()
    {
        return dirtyPlatePrefab;
    }

    public GameObject GetMixedModel(List<IngredientID> targetID)
    {
        var matchData = allMixData.Where(x => x.IsMatch(targetID)).FirstOrDefault();
        if (matchData != null)
        {
            return matchData.GetPlatePrefab();
        }
        
        return GetIngredientModel(targetID[0]);
    }

    public RecipeData GetFinishableRecipe(List<IngredientID> targets) // NOTE : 현재 재료 구성만으로 조합 가능한 단 하나의 레시피
    {
        foreach (var recipeTarget in recipeData.Values.Where(x => x.GetIngredientCount() == targets.Count))
        {
            bool canFinish = true;
            foreach (var currentTarget in targets)
            {
                if (recipeTarget.HasIngredient(currentTarget) == false)
                {
                    canFinish = false;
                    break;
                }
            }

            if (canFinish)
            {
                return recipeTarget;
            }
        }

        return null;
    }

    public List<RecipeData> GetSatisfyRecipe(List<IngredientID> targets)
    {
        List<RecipeData> satisfiedRecipe = new List<RecipeData>();

        foreach(var recipe in recipeData.Values)
        {
            bool hasIngredient = true;
            foreach(var target in targets) // NOTE : 재료들로 조합 가능 여부 확인
            {
                if(recipe.HasIngredient(target) == false)
                {
                    hasIngredient = false;
                    break;
                }
            }

            // NOTE : 조합 가능한 경우에만 레시피 추가
            if(hasIngredient)
            {
                satisfiedRecipe.Add(recipe);
            }
        }

        return satisfiedRecipe;
    }
}
