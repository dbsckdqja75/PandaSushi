using UnityEngine;
using TMPro;

public class CurrencyLabel : MonoBehaviour
{
    int prevValue = 0;
    
    [SerializeField] TMP_Text currencyText;
    [SerializeField] protected Animator animator;

    protected virtual void OnEnable()
    {
        EventManager.GetEvent(EGameEvent.OnUpdateCurrency).Subscribe(RefreshUI);
        
        prevValue = (int)CurrencyManager.Instance.GetCurrency(ECurrencyType.Money);
        RefreshUI();
    }

    protected virtual void OnDisable()
    {
        EventManager.GetEvent(EGameEvent.OnUpdateCurrency).Unsubscribe(RefreshUI);
    }

    protected void RefreshUI()
    {
        int newValue = (int)CurrencyManager.Instance.GetCurrency(ECurrencyType.Money);
        if (prevValue != newValue)
        {
            animator.SetTrigger("OnProfit");
        }

        prevValue = newValue;
        currencyText.text = newValue.ToString("N0");
    }
}
