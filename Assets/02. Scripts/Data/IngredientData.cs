using UnityEngine;

[CreateAssetMenu(fileName = "IngredientData", menuName = "Scriptable Object/IngredientData")]
public class IngredientData : ScriptableObject
{
    [SerializeField] int unlockLevel = 0;

    [SerializeField] IngredientID id;
    [SerializeField] IngredientID parentId;
    
    [Space(10)]
    [SerializeField] ECookType cookType;
    
    [Space(10)]
    [SerializeField] IngredientID ovenTarget; // NOTE : 오븐 조리 이후에 상위 재료
    [SerializeField] float ovenTime = 0;
    
    [Space(10)]
    [SerializeField] IngredientID prepareTarget; // NOTE : 손질한 이후에 상위 재료
    [SerializeField] float prepareTime = 0;

    [Space(20)]
    [SerializeField] Sprite icon;
    [SerializeField] GameObject plateModel;

    [SerializeField] int stockOrderPrice = 0;

    public IngredientID GetID()
    {
        return id;
    }

    public int GetTargetLevel()
    {
        return unlockLevel;
    }

    public int GetPrice()
    {
        return stockOrderPrice;
    }

    public ECookType GetCookType()
    {
        return cookType;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public GameObject GetPlatePrefab()
    {
        return plateModel;
    }

    public IngredientID GetOvenTarget()
    {
        return ovenTarget;
    }

    public float GetOvenTime()
    {
        return ovenTime;
    }

    public IngredientID GetPrepareTarget()
    {
        return prepareTarget;
    }

    public float GetPrepareTime()
    {
        return prepareTime;
    }

    public IngredientID GetParentID()
    {
        return parentId;
    }
}