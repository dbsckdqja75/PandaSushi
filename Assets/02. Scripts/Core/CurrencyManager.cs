using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CurrencyManager : MonoSingleton<CurrencyManager>
{
    Dictionary<ECurrencyType, float> currency = new();
    const float maxValue = 100000000f;
    
    protected override void Init()
    {
        currency = new Dictionary<ECurrencyType, float>();
        currency.Add(ECurrencyType.Money, 0f);
        currency.Add(ECurrencyType.Star, 0f);

        LoadCurrencyData();
    }

    void LoadCurrencyData()
    {
        foreach(ECurrencyType type in currency.Keys.ToList())
        {
            currency[type] = Mathf.Clamp(PlayerPrefsManager.LoadSlotData(type.ToString(), 0f), 0f, maxValue);
            
            PlayerPrefsManager.SaveSlotData(string.Format("Current_{0}", type.ToString()), currency[type]);
        }

        EventManager.GetEvent(EGameEvent.OnUpdateCurrency).Invoke();
    }

    public void ReloadData()
    {
        LoadCurrencyData();
    }

    public void SaveCurrencyData()
    {
        foreach(ECurrencyType type in currency.Keys.ToList())
        {
            PlayerPrefsManager.SaveSlotData(type.ToString(), GetCurrency(type));
        }
    }

    public float GetCurrency(ECurrencyType type)
    {
        if(currency.ContainsKey(type))
        {
            currency[type] = Mathf.Clamp(PlayerPrefsManager.LoadSlotData(string.Format("Current_{0}", type.ToString()), 0f), 0f, maxValue);
            return currency[type];
        }

        return 0;
    }

    public bool CanPurchase(ECurrencyType currencyType, int price)
    {
        return (GetCurrency(currencyType) >= price);
    }

    public bool Purchase(ECurrencyType currencyType, int price)
    {
        if(CanPurchase(currencyType, price))
        {
            currency[currencyType] -= price;
            currency[currencyType] = Mathf.Clamp(currency[currencyType], 0f, maxValue);
            
            PlayerPrefsManager.SaveSlotData(string.Format("Current_{0}", currencyType.ToString()), currency[currencyType]);

            EventManager.GetEvent(EGameEvent.OnUpdateCurrency).Invoke();
            return true;
        }

        return false;
    }

    public void RewardCurrency(ECurrencyType currencyType, float amount)
    {
        if(currency.ContainsKey(currencyType) && amount > 0)
        {
            float currentValue = Mathf.Clamp(GetCurrency(currencyType) + amount, 0f, maxValue);
            currency[currencyType] = currentValue;
            
            PlayerPrefsManager.SaveSlotData(string.Format("Current_{0}", currencyType.ToString()), currency[currencyType]);

            EventManager.GetEvent(EGameEvent.OnUpdateCurrency).Invoke();
        }
    }
}
