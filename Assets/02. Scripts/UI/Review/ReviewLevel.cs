using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReviewLevel : MonoBehaviour
{
    [SerializeField] Color unlockedColor;
    [SerializeField] Color lockedColor;
    
    [Space(10)]
    [SerializeField] Image mainImg;
    [SerializeField] GameObject lockImg;
    [SerializeField] GameObject bestRecordLabel;
    [SerializeField] TMP_Text targetReviewsText;
    [SerializeField] TMP_Text bonusText;
    
    [Space(10)]
    [SerializeField] LevelConfigData targetConfig;

    void OnEnable()
    {
        targetReviewsText.text = string.Format("[{0}]", targetConfig.GetTargetReviews());
    }

    public void Lock()
    {
        mainImg.color = lockedColor;
        lockImg.SetActive(true);
        bestRecordLabel.SetActive(false);

        UpdateBonusInfo("???");
    }

    public void Unlock(bool onBestLevel)
    {
        mainImg.color = unlockedColor;
        lockImg.SetActive(false);
        bestRecordLabel.SetActive(onBestLevel);

        string bonusInfo = "";
        bonusInfo += GetBonusContext("BONUS_TIP_TEXT", targetConfig.GetTipBonusMultiple());
        bonusInfo += GetBonusContext("BONUS_MEALTIME_TEXT", targetConfig.GetMealTimeMultiple());
        bonusInfo += GetBonusContext("BONUS_PREPTIME_TEXT", targetConfig.GetPrepTimeMultiple());
        bonusInfo += GetBonusContext("BONUS_COOKTIME_TEXT", targetConfig.GetCookTimeMultiple());
        bonusText.text = bonusInfo.Trim();
    }

    public void UpdateBonusInfo(string context)
    {
        bonusText.text = context;
    }

    string GetBonusContext(string stringKey, float configValue)
    {
        if (configValue <= 0)
        {
            return "";
        }

        return string.Format("{0} {1}%\n", LocalizationManager.Instance.GetString(stringKey), (int)(configValue * 100));
    }
}
