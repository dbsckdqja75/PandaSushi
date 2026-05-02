using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReviewNotice : MonoBehaviour
{
    [SerializeField] GameObject content; 
    [SerializeField] Image iconImage; 
    [SerializeField] TMP_Text valueText;
    [SerializeField] Sprite[] faceSprites;

    void Awake()
    {
        content.SetActive(false);
    }

    void OnEnable()
    {
        if (EventManager.GameStatus != EGameState.RoundStart)
        {
            content.SetActive(false);
            return;
        }
        
        EventManager.GetEvent<int>(EGameEvent.OnUpdateReview).Subscribe(ShowNotice);
    }

    void OnDisable()
    {
        EventManager.GetEvent<int>(EGameEvent.OnUpdateReview).Unsubscribe(ShowNotice);
    }

    void ShowNotice(int displayValue)
    {
        content.SetActive(false);

        if (displayValue > 0)
        {
            iconImage.sprite = faceSprites[0];
            valueText.text = string.Format("+ {0}", displayValue);
        }
        else
        {
            iconImage.sprite = faceSprites[1];
            valueText.text = string.Format("{0}", displayValue);
        }
        
        content.SetActive(true);
    }
}
