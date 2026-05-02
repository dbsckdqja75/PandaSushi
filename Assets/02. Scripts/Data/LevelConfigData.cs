using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfigData", menuName = "Scriptable Object/LevelConfigData")]
public class LevelConfigData : ScriptableObject
{
    [SerializeField] int targetLevel = 1;
    [SerializeField] int requireReviews = 0;

    [Space(10)]
    [SerializeField] float tipBonusMultiple = 0f;
    // [SerializeField] float reviewBonusMultiple = 0f; // NOTE: 레벨당 25% 균등
    [SerializeField] float mealTimeMultiple = 0f;
    [SerializeField] float prepTimeMultiple = 0f;
    [SerializeField] float cookTimeMultiple = 0f;
    [SerializeField] float spawnTimeMultiple = 0f; // NOTE: 손님 스폰 간격
    [SerializeField] float eventTimeMultiple = 0f; // NOTE: 이벤트 스폰 간격
    [SerializeField] float deliveryTimeMultiple = 0f; // NOTE: 배달 주문 간격

    public int GetLevel()
    {
        return targetLevel;
    }
    
    public int GetTargetReviews()
    {
        return requireReviews;
    }
    
    public float GetTipBonusMultiple()
    {
        return tipBonusMultiple;
    }
    
    // public float GetReviewBonusMultiple()
    // {
    //     return reviewBonusMultiple;
    // }
    
    public float GetMealTimeMultiple()
    {
        return mealTimeMultiple;
    }
    
    public float GetPrepTimeMultiple()
    {
        return prepTimeMultiple;
    }
    
    public float GetCookTimeMultiple()
    {
        return cookTimeMultiple;
    }
    
    public float GetSpawnTimeMultiple()
    {
        return spawnTimeMultiple;
    }
    
    public float GetEventTimeMultiple()
    {
        return eventTimeMultiple;
    }
    
    public float GetDeliveryTimeMultiple()
    {
        return deliveryTimeMultiple;
    }
}
