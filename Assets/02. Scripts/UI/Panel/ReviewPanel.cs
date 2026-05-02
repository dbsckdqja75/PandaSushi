using UnityEngine;
using TMPro;

public class ReviewPanel : PanelUI
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] GameObject[] displayStars;
    [SerializeField] ReviewLevel[] unlockedLevels;
    [SerializeField] ReviewLabel[] reviewLabels;
    
    void OnEnable()
    {
        int unlockLevel = PlayerPrefsManager.LoadSlotData("UnlockLevel", 1);
        int currentLevel = PlayerPrefsManager.LoadSlotData("ReviewLevel", 1);
        
        RefreshDisplayStars(currentLevel);
        RefreshUnlockLevels(unlockLevel);
        RefreshReviews(currentLevel);
    }
    
    void RefreshDisplayStars(int targetLevel)
    {
        string mainContext = LocalizationManager.Instance.GetString("REVIEW_RATING_TEXT");
        string subContext = LocalizationManager.Instance.GetString("REVIEW_CURRENCY_TEXT");
        string value = CurrencyManager.Instance.GetCurrency(ECurrencyType.Star).ToString("N0");
        titleText.text = string.Format("{0} ({1} : {2})", mainContext, subContext, value);
        
        for (int i = 0; i < displayStars.Length; i++)
        {
            Debug.LogFormat("{0} < {1} = {2}", i, targetLevel, i < targetLevel);
            displayStars[i].SetActive(i < targetLevel);
        }
    }

    void RefreshUnlockLevels(int targetLevel)
    {
        for (int i = 0; i < unlockedLevels.Length; i++)
        {
            if (i < (targetLevel - 1))
            {
                unlockedLevels[i].Unlock(i == (targetLevel - 2));
            }
            else
            {
                unlockedLevels[i].Lock();
            }
        }
    }

    void RefreshReviews(int maxLevel)
    {
        for (int i = 0; i < reviewLabels.Length; i++)
        {
            int star = Random.Range(1, maxLevel + 1);
            string contextKey = string.Format("REVIEW_STAR{0}_CONTEXT{1}", star, Random.Range(1, 6 + 1));
            
            reviewLabels[i].UpdateLabel(LocalizationManager.Instance.GetString("ReviewStringData", contextKey), star);
        }
    }
}
