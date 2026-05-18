using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StockSlot : MonoBehaviour, ICanvasSlot
{
    int targetID = -1;
    
    [SerializeField] Button btn;
    [SerializeField] Image icon;
    [SerializeField] TMP_Text labelText;

    FridgeStorage fridgeStorage;
    
    public void Init(IngredientID targetID, int unlockLevel, Action<IngredientID> onClick, FridgeStorage fridgeStorage)
    {
        this.fridgeStorage = fridgeStorage;
        
        this.targetID = (int)targetID;
        if (this.targetID >= 0)
        {
            IngredientData data = PandaResources.Instance.GetIngredientData(targetID);
            icon.sprite = data.GetIcon();
            
            if (data.GetTargetLevel() <= unlockLevel)
            {
                icon.color = Color.white;
                labelText.text = fridgeStorage.GetStoredCount(targetID).ToString();
                btn.interactable = true;
                btn.onClick.AddListener(() => onClick?.Invoke(targetID));
            }
            else
            {
                icon.color = Color.black;
                labelText.text = "";
                btn.interactable = false;
            }
        }
    }
    
    void OnDisable()
    {
        ResetSlot();
    }
    
    void ResetSlot()
    {
        targetID = -1;
        icon.color = Color.black;
        labelText.text = "";
    }
    
    public void OnClick()
    {
        if (btn != null && btn.interactable)
        {
            btn.onClick.Invoke();
        }
    }

    public bool CanSelect()
    {
        if (targetID != -1)
        {
            return btn != null && btn.interactable;
        }
        
        return false;
    }

    public Transform GetTransform()
    {
        return this.transform;
    }

    public void RefreshLabel()
    {
        if (targetID >= 0 && btn.interactable)
        {
            labelText.text = fridgeStorage.GetStoredCount((IngredientID)targetID).ToString();
        }
    }
}
