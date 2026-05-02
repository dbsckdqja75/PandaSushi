using UnityEngine;

[CreateAssetMenu(fileName = "DecoData", menuName = "Scriptable Object/DecoData")]
public class DecoData : ScriptableObject
{
    [Space(10)]
    [SerializeField] int price = 3;
    [SerializeField] DecoID targetCategory;
    [SerializeField] int targetIdx;
    [SerializeField] GameObject decoPrefab;
    
    [Space(10)]
    [SerializeField] Vector3 decoOffsetPos;
    [SerializeField] Vector3 decoOffsetAngle;

    public DecoID GetID()
    {
        return targetCategory;
    }

    public int GetPrice()
    {
        return price;
    }

    public int GetOrder()
    {
        return targetIdx;
    }

    public GameObject GetModelPrefab()
    {
        return decoPrefab;
    }
    
    public Vector3 GetOffsetPos()
    {
        return decoOffsetPos;
    }
    
    public Vector3 GetOffsetAngle()
    {
        return decoOffsetAngle;
    }
}
