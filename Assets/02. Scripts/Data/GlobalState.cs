using UnityEngine;

[CreateAssetMenu(fileName = "GlobalStateData", menuName = "Scriptable Object/GlobalStateData")]
public class GlobalState : ScriptableObject
{
    public bool isSinkDishFull = false;

    public float riderSpeedMultiple = 1f;
    public float playerSpeedMultiple = 1f;

    public float tipBonus = 1f;
    public float reviewBonus = 1f;
    public float mealTimeBonus = 1f;
    public float prepTimeBonus = 1f;
    public float cookTimeBonus = 1f;
    public float spawnTimeBonus = 1f;
    public float eventTimeBonus = 1f;
    public float deliveryTimeBonus = 1f;

    public void SetBonusValues(LevelConfigData[] targets)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            tipBonus += targets[i].GetTipBonusMultiple();
            mealTimeBonus += targets[i].GetMealTimeMultiple();
            prepTimeBonus += targets[i].GetPrepTimeMultiple();
            cookTimeBonus += targets[i].GetCookTimeMultiple();
            spawnTimeBonus += targets[i].GetSpawnTimeMultiple();
            eventTimeBonus += targets[i].GetEventTimeMultiple();
            deliveryTimeBonus += targets[i].GetDeliveryTimeMultiple();
        }
    }
    
    public void ResetBonus()
    {
        tipBonus = reviewBonus = 1f;
        mealTimeBonus = prepTimeBonus = cookTimeBonus = 1f;
        spawnTimeBonus = eventTimeBonus = deliveryTimeBonus = 1f;
    }
}
