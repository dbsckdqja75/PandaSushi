using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipePlate : MonoBehaviour
{
    bool isCooked = false;

    [SerializeField] Vector3 plateOrigin = Vector3.zero;
    [SerializeField] GlobalState globalState;

    // IngredientData ingredientData;
    GameObject plateModel;

    RecipeID dish = RecipeID.NULL;
    List<IngredientID> ingredientList = new();

    Mixer mixer;
    ProcessHandler processHandler;

    void Awake()
    {
        mixer = FindAnyObjectByType<Mixer>();
        processHandler = this.GetComponent<ProcessHandler>();
    }

    public void Plate(List<IngredientID> ingredientID, bool forceReset = false)
    {
        if (forceReset)
        {
            ResetPlate();
        }

        if(isCooked == false)
        {
            IngredientData ingredientData = PandaResources.Instance.GetIngredientData(ingredientID[0]);
            
            foreach (var id in ingredientID)
            {
                ingredientList.Add(id);
            }

            if (ingredientList.Count > 1) // NOTE : 1개 이상일때 조합 처리
            {
                mixer.MixIngredient(this);
            }
            else
            {
                UpdatePlateModel(ingredientData.GetPlatePrefab());
            }
        }
    }

    public void Plate(RecipeID dishID)
    {
        isCooked = true;
        
        dish = dishID;
        
        RecipeData recipeData = PandaResources.Instance.GetRecipeData(dishID);
        UpdatePlateModel(recipeData.GetPlatePrefab());
    }

    public void DirtyPlate()
    {
        isCooked = true;
        
        dish = RecipeID.NULL;
        ingredientList.Clear();
        
        UpdatePlateModel(PandaResources.Instance.GetDirtyPlatePrefab());
    }

    public void StartPrep(Action<bool> finishCallback)
    {
        IngredientData ingredientData = PandaResources.Instance.GetIngredientData(ingredientList[0]);
        List<IngredientID> newIngredients = new List<IngredientID>();
        newIngredients.Add(ingredientData.GetPrepareTarget());

        float processTime = ingredientData.GetPrepareTime() * (2f - globalState.prepTimeBonus);
        processHandler.StartProcess(processTime, (isSuccess) =>
        {
            if (isSuccess)
            {
                UpdateIngredient(newIngredients);
            }
            
            finishCallback?.Invoke(isSuccess);
        }, true);
    }

    public void ForcePrep()
    {
        IngredientData ingredientData = PandaResources.Instance.GetIngredientData(ingredientList[0]);
        List<IngredientID> newIngredients = new List<IngredientID>();
        newIngredients.Add(ingredientData.GetPrepareTarget());
        
        UpdateIngredient(newIngredients);
    }

    public void UpdateIngredient(List<IngredientID> newIngredients)
    {
        ingredientList = newIngredients;
        
        GameObject prefab = PandaResources.Instance.GetMixedModel(ingredientList);
        UpdatePlateModel(prefab);
    }

    public void UpdateRecipe(List<IngredientID> newIngredients, RecipeData recipeData)
    {
        isCooked = true;

        ingredientList = newIngredients;

        dish = recipeData.GetID();

        UpdatePlateModel(recipeData.GetPlatePrefab());

        Debug.LogFormat("UpdateRecipe - {0}", dish.ToString());
    }

    void UpdatePlateModel(GameObject prefab)
    {
        if (plateModel != null)
        {
            Destroy(plateModel);
        }

        plateModel = Instantiate(prefab, transform.position, prefab.transform.rotation, transform);
    }

    void ResetIngredient()
    {
        isCooked = false;

        dish = RecipeID.NULL;
        ingredientList.Clear();
    }

    public void ResetPlate()
    {
        if (processHandler != null)
        {
            processHandler.StopProcess();
        }

        ResetIngredient();

        if(plateModel)
        {
            Destroy(plateModel);

            plateModel = null;
        }
    }

    public RecipeID GetDishID()
    {
        return dish;
    }

    public List<IngredientID> GetPlatedIngredientID()
    {
        return ingredientList;
    }

    public GameObject GetPlateModel()
    {
        return plateModel;
    }

    public bool IsCooking()
    {
        return processHandler.IsProcessing();
    }

    public bool IsAnyPlated()
    {
        return (plateModel != null);
    }

    public bool IsDirty()
    {
        return (isCooked && ingredientList.Count <= 0 && dish == RecipeID.NULL);
    }
    
    public bool IsEmpty()
    {
        return (plateModel == null);
    }
    
    public bool CanPrep(ECookType cookType)
    {
        if(ingredientList.Count == 1)
        {
            IngredientData ingredientData = PandaResources.Instance.GetIngredientData(ingredientList[0]);
            if((int)ingredientData.GetPrepareTarget() >= 100 && ingredientData.GetCookType() == cookType)
            {
                if (ingredientData.GetPrepareTime() > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool CanFinishDish(out RecipeData recipe) // NOTE: 겹치는 레시피 중에서 지금 완성 가능한 레시피 조합
    {
        if (ingredientList.Count > 1 && isCooked == false)
        {
            var finishableRecipe = PandaResources.Instance.GetFinishableRecipe(ingredientList);
            if (finishableRecipe != null)
            {
                recipe = finishableRecipe;
                return true;
            }
        }

        recipe = null;
        return false;
    }
    
    public bool CanPlate(List<IngredientID> ingredientID)
    {
        if (isCooked || dish != RecipeID.NULL)
        {
            return false;
        }
        
        if (ingredientList.Count <= 0)
        {
            return true;
        }
        
        foreach (var id in ingredientID)
        {
            if (ingredientList.Contains(id))
            {
                return false;
            }
        }

        List<IngredientID> targetIngredients = ingredientList.ToList();
        foreach (var id in ingredientID)
        {
            targetIngredients.Add(id);
        }
        
        return mixer.CanMix(targetIngredients);
    }
}
