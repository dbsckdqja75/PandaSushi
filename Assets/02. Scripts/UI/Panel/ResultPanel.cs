using UnityEngine;
using TMPro;

public class ResultPanel : PanelUI
{
    [SerializeField] TMP_Text rewardedText;
    [SerializeField] TMP_Text seatCountText;
    [SerializeField] TMP_Text deliveryCountText;
    [SerializeField] TMP_Text orderCountText;
    [SerializeField] TMP_Text totalRewardedText;
    [SerializeField] TMP_Text positiveReviewsText;
    [SerializeField] TMP_Text negativeReviewsText;
    
    void OnEnable()
    {
        RoundAnalytics roundAnalytics = FindAnyObjectByType<RoundAnalytics>();
        RoundAnalytics.RoundData data = roundAnalytics.GetData();
        
        rewardedText.text = data.rewarded.ToString("N0");
        seatCountText.text = data.seatCount.ToString();
        deliveryCountText.text = data.deliveryCount.ToString();
        orderCountText.text = data.orderCount.ToString();
        totalRewardedText.text = string.Format("+{0}", data.rewarded.ToString("N0"));
        
        positiveReviewsText.text = string.Format("+{0}", Mathf.Abs(data.positiveCount).ToString("N0"));
        negativeReviewsText.text = string.Format("-{0}", Mathf.Abs(data.negativeCount).ToString("N0"));
    }

    protected override void Update()
    {
        return;
    }

    public override void Close()
    {
        EventManager.GameStatus = EGameState.RoundPrepare;
    }
}
