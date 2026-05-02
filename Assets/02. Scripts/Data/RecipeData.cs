using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeData", menuName = "Scriptable Object/RecipeData")]
public class RecipeData : ScriptableObject
{
    [SerializeField] RecipeID id;
    [SerializeField] int unlockLevel;

    [SerializeField] List<IngredientID> targetIngredientList;

    [SerializeField] Sprite icon;
    [SerializeField] GameObject plate;

    [SerializeField] int starReward;
    [SerializeField] int orderPrice;

    public RecipeID GetID()
    {
        return id;
    }
    
    public int GetTargetLevel()
    {
        return unlockLevel;
    }

    public int GetStarReward()
    {
        return starReward;
    }

    public int GetPrice()
    {
        return orderPrice;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public GameObject GetPlatePrefab()
    {
        return plate;
    }

    public bool HasIngredient(IngredientID targetID)
    {
        return (targetIngredientList.Where(x => x == targetID).ToList().Count > 0);
    }

    public bool IsCompletedRecipe(List<IngredientID> compareTargets)
    {
        return (compareTargets.Count == targetIngredientList.Count);
    }

    public int GetIngredientCount()
    {
        return targetIngredientList.Count;
    }

    public List<IngredientID> GetIngredients()
    {
        return targetIngredientList.ToList();
    }
}