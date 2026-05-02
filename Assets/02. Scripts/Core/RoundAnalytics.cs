using UnityEngine;

public class RoundAnalytics : MonoBehaviour
{
    public class RoundData
    {
        public float rewarded = 0;
        public int seatCount = 0;
        public int deliveryCount = 0;
        public int orderCount = 0;
        public int positiveCount = 0;
        public int negativeCount = 0;
    }

    RoundData roundData = new();

    void OnEnable()
    {
        EventManager.GetEvent<float>(EGameEvent.OnRewardedOrder).Subscribe(OnRewarded);
        EventManager.GetEvent(EGameEvent.OnSeatedGuest).Subscribe(OnSeated);
        EventManager.GetEvent(EGameEvent.OnFinishedDelivery).Subscribe(OnFinishedDelivery);
        EventManager.GetEvent(EGameEvent.OnFinishedOrder).Subscribe(OnFinishedOrder);
        EventManager.GetEvent<int>(EGameEvent.OnUpdateReview).Subscribe(OnReviewed);
    }

    void OnDisable()
    {
        EventManager.GetEvent<float>(EGameEvent.OnRewardedOrder).Unsubscribe(OnRewarded);
        EventManager.GetEvent(EGameEvent.OnSeatedGuest).Unsubscribe(OnSeated);
        EventManager.GetEvent(EGameEvent.OnFinishedDelivery).Unsubscribe(OnFinishedDelivery);
        EventManager.GetEvent(EGameEvent.OnFinishedOrder).Unsubscribe(OnFinishedOrder);
        EventManager.GetEvent<int>(EGameEvent.OnUpdateReview).Unsubscribe(OnReviewed);
    }

    void OnRewarded(float addValue)
    {
        roundData.rewarded += addValue;
    }

    void OnSeated()
    {
        roundData.seatCount += 1;
    }

    void OnFinishedDelivery()
    {
        roundData.deliveryCount += 1;
    }

    void OnFinishedOrder()
    {
        roundData.orderCount += 1;
    }
    
    void OnReviewed(int reviewValue)
    {
        if (reviewValue > 0)
        {
            roundData.positiveCount += reviewValue;
        }
        else
        {
            roundData.negativeCount += reviewValue;
        }
    }

    public void ForceReset()
    {
        roundData.rewarded = roundData.seatCount = roundData.deliveryCount = roundData.orderCount = 0;
        roundData.positiveCount = roundData.negativeCount = 0;
    }

    public RoundData GetData()
    {
        return roundData;
    }
}
