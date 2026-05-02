using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffIcon : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image progressImg;
    [SerializeField] TMP_Text timerText;

    void OnEnable()
    {
        OnExit();
    }

    public void UpdateIcon(Sprite newSprite)
    {
        icon.sprite = newSprite;
    }

    public void UpdateTimer(int remainTime, float amount)
    {
        amount = Mathf.Clamp(amount, 0f, 1f);
        progressImg.fillAmount = amount;
        
        timerText.text = remainTime.ToString();
    }

    public void OnHover()
    {
        timerText.gameObject.SetActive(true);
    }

    public void OnExit()
    {
        timerText.gameObject.SetActive(false);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}