using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FridgeSlot : MonoBehaviour
{
    int targetID = -1;
    
    [SerializeField] Button btn;
    [SerializeField] Image icon;
    [SerializeField] TMP_Text labelText;
    
    void Awake()
    {
        btn.onClick.AddListener(OnClick);
    }

    public void ResetSlot()
    {
        targetID = -1;
        icon.enabled = false;
        labelText.text = "";
    }

    public void UpdateSlot(IngredientID ingredientID, int count)
    {
        targetID = (int)ingredientID;
        
        UpdateIcon(PandaResources.Instance.GetIngredientIcon(ingredientID));
        UpdateCount(count);
    }

    void UpdateIcon(Sprite newSprite)
    {
        if(newSprite != null)
        {
            icon.sprite = newSprite;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }
    }

    void UpdateCount(int count)
    {
        if(count > 0)
        {
            btn.interactable = true;
            labelText.text = count.ToString();
        }
        else
        {
            btn.interactable = false;
            labelText.text = "";
        }
    }

    void OnClick()
    {
        if (targetID != -1)
        {
            EventManager.GetEvent<IngredientID>(EGameEvent.OnClickFridgeSlot).Invoke((IngredientID)targetID);
        }
        // else
        // {
        //     ResetSlot();
        //     EventManager.GetEvent<bool>(EGameEvent.OnSelectFridge).Invoke(false);
        // }
    }

    #if UNITY_EDITOR
    [ContextMenu("AutoFill")]
    void AutoFill()
    {
        btn = this.GetComponentInChildren<Button>();
        icon = btn.GetComponent<Image>();
        labelText = this.GetComponentInChildren<TMP_Text>();
    }
    #endif
}
