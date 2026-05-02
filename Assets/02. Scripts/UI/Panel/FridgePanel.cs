using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FridgePanel : MonoBehaviour
{
    [SerializeField] GameObject blackground;

    [Space(10)]
    [SerializeField] FridgeSlot[] fridgeSlots;
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            OnClickCancel();
        }
    }

    public void Show()
    {
        blackground.SetActive(true);
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        blackground.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void UpdateInfo(List<Tuple<IngredientID, int>> data)
    {
        for(int i = 0; i < fridgeSlots.Length; i++)
        {
            if (i < data.Count)
            {
                fridgeSlots[i].UpdateSlot(data[i].Item1, data[i].Item2);
            }
            else
            {
                fridgeSlots[i].ResetSlot();
            }
        }
    }

    public void OnClickCancel()
    {
        EventManager.GetEvent<bool>(EGameEvent.OnSelectFridge).Invoke(false);
    }

    #if UNITY_EDITOR
    [ContextMenu("AutoFill")]
    void AutoFill()
    {
        fridgeSlots = this.GetComponentsInChildren<FridgeSlot>().ToArray();
    }
    #endif
}
