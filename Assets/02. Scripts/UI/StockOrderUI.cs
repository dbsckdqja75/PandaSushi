using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StockOrderUI : PanelUI
{
    IngredientID selectedID;
    int orderCount = 1;
    int selectedTotalPrice = 0;
    
    [SerializeField] Image icon;
    [SerializeField] TMP_Text countText;
    [SerializeField] TMP_Text orderText;
    
    [SerializeField] TMP_Text priceText;
    [SerializeField] TMP_Text totalPriceText;

    [SerializeField] GameObject selectedInfoBox;
    [SerializeField] GameObject selectGuideBox;
    [SerializeField] Button purchaseButton;
    
    [SerializeField] List<StockSlot> stockSlotList = new();

    FridgeManager fridgeManager;
    FridgeStorage fridgeStorage;

    protected override void Awake()
    {
        base.Awake();

        fridgeManager = FindAnyObjectByType<FridgeManager>();
        fridgeStorage = fridgeManager.GetStorage();
    }

    void OnEnable()
    {
        selectedInfoBox.SetActive(false);
        selectGuideBox.SetActive(true);
        
        orderCount = 1;
        orderText.text = string.Format("+{0}", orderCount);
        
        int unlockLevel = PlayerPrefsManager.LoadSlotData("UnlockLevel", 1);
        List<IngredientID> ingredientKeys = PandaResources.Instance.GetRawIngredientKeys();
        for (int i = 0; i < ingredientKeys.Count; i++)
        {
            stockSlotList[i].Init(ingredientKeys[i], unlockLevel, OnSelectSlot, fridgeStorage);
        }

        for (int i = ingredientKeys.Count; i < stockSlotList.Count; i++)
        {
            if (i < stockSlotList.Count)
            {
                stockSlotList[i].gameObject.SetActive(false);
            }
        }
    }

    public void SwitchOrderCount(bool isAdd)
    {
        int addValue = (isAdd ? 1 : -1);
        orderCount += Input.GetKey(KeyCode.LeftShift) ? (addValue * 10) : addValue;
        
        if (orderCount > 100)
        {
            orderCount = Mathf.Clamp(orderCount - 100, 1, 100 );
        }
        else if (orderCount <= 0)
        {
            orderCount = Mathf.Clamp(100 + orderCount, 1, 100);
        }

        RefreshLayout();
    }

    public void PurchaseStockOrder()
    {
        if (CurrencyManager.Instance.Purchase(ECurrencyType.Money, selectedTotalPrice))
        {
            fridgeStorage.Add(selectedID, orderCount);
            OnSelectSlot(selectedID);
            
            for (int i = 0; i < stockSlotList.Count; i++)
            {
                stockSlotList[i].RefreshLabel();
            }
            
            fridgeManager.SaveStorageInfo();
            CurrencyManager.Instance.SaveCurrencyData();
            
            SoundManager.Instance.PlaySound("SFX_Cash");
            
            EventManager.GetEvent(EGameEvent.OnUpdateCurrency).Invoke();
            EventManager.GetEvent(EGameEvent.OnGameSaved).Invoke();
        }
        
        RefreshLayout();
    }

    void OnSelectSlot(IngredientID targetID)
    {
        selectedInfoBox.SetActive(true);
        selectGuideBox.SetActive(false);
        
        selectedID = targetID;
        orderCount = 1;
        
        RefreshLayout();
        
        SoundManager.Instance.PlaySound("SFX_Switch1");
    }

    void RefreshLayout()
    {
        IngredientData data = PandaResources.Instance.GetIngredientData(selectedID);
        
        orderText.text = string.Format("+{0}", orderCount);
        
        icon.sprite = data.GetIcon();
        countText.text = fridgeStorage.GetStoredCount(selectedID).ToString();

        int price = data.GetPrice();
        selectedTotalPrice = data.GetPrice() * orderCount;

        priceText.text = string.Format("{0} : {1}", LocalizationManager.Instance.GetString("INGREDIENT_PRICE_TEXT"), price);
        totalPriceText.text = string.Format("{0} : {1}", LocalizationManager.Instance.GetString("TOTAL_PRICE_TEXT"), selectedTotalPrice);

        int money = (int)CurrencyManager.Instance.GetCurrency(ECurrencyType.Money);
        purchaseButton.interactable = (selectedTotalPrice <= money);
    }
}
