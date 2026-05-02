using System.Collections.Generic;
using UnityEngine;

public class PlayerHandSlot : MonoBehaviour
{
    [SerializeField] Transform ingredientPoint;

    [Space(10)]
    [SerializeField] GameObject handPlate;
    [SerializeField] GameObject[] handTools;

    GameObject curIngredientModel;

    void Start()
    {
        ResetHand();
    }

    void Update()
    {
        
    }
    
    public void OnHold(RecipeID targetID)
    {
        ResetHand();

        var recipeData = PandaResources.Instance.GetRecipeData(targetID);
        
        GameObject prefab = recipeData.GetPlatePrefab();
        curIngredientModel = Instantiate(prefab, ingredientPoint.transform.position, prefab.transform.rotation, ingredientPoint);
        curIngredientModel.transform.localEulerAngles = new Vector3(-90, 0, 0);
    }

    public void OnHold(List<IngredientID> targetID)
    {
        ResetHand();

        GameObject prefab = PandaResources.Instance.GetMixedModel(targetID);
        curIngredientModel = Instantiate(prefab, ingredientPoint.transform.position, prefab.transform.rotation, ingredientPoint);
        curIngredientModel.transform.localEulerAngles = new Vector3(-90, 0, 0);
    }

    public void OnRelease()
    {
        ResetHand();
    }

    void ResetHand()
    {
        foreach(GameObject tool in handTools)
        {
            tool.SetActive(false);
        }

        if(curIngredientModel)
        {
            Destroy(curIngredientModel);
        }
    }
}
