using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MixData", menuName = "Scriptable Object/MixData")]
public class MixData : ScriptableObject
{
    [SerializeField] List<IngredientID> targetIngredients = new List<IngredientID>();
    
    [Space(10)]
    [SerializeField] Sprite icon;
    [SerializeField] GameObject plateModel;

    public GameObject GetPlatePrefab()
    {
        return plateModel;
    }
    
    public bool IsMatch(List<IngredientID> compareIngredients)
    {
        if (targetIngredients.Count == compareIngredients.Count)
        {
            var sortedIngredients = compareIngredients.OrderBy(x => (int)x).ToList();
            for(int i = 0; i < targetIngredients.Count; i++)
            {
                if (targetIngredients[i] != sortedIngredients[i])
                {
                    return false;
                }
            }

            return true;
        }
        
        return false;
    }
}
