using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] int slotNumber = 1;
    
    [SerializeField] TMP_Text numberText;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text dateText;
    [SerializeField] GameObject moneyIcon;
    [SerializeField] GameObject[] starIcons;

    [Space(10)]
    [SerializeField] GameObject newGameText;
    [SerializeField] GameObject deleteBtn;

    void OnEnable()
    {
        numberText.text = string.Format("#{0}", slotNumber);
        titleText.text = "";
        moneyText.text = "";
        dateText.text = "";
        
        for (int i = 0; i < starIcons.Length; i++)
        {
            starIcons[i].SetActive(false);
        }
        
        moneyIcon.SetActive(false);
        newGameText.gameObject.SetActive(false);
        deleteBtn.SetActive(false);
        
        LoadSaveInfo();
    }

    void LoadSaveInfo()
    {
        bool hasData = PlayerPrefsManager.LoadData(string.Format("Save{0}_HasSaveGame", slotNumber), false, false);
        if (hasData)
        {
            int starCount = PlayerPrefsManager.LoadData(string.Format("Save{0}_{1}", slotNumber, "UnlockLevel"), 1);
            for (int i = 0; i < starIcons.Length; i++)
            {
                starIcons[i].SetActive(i < starCount);
            }

            titleText.text = LocalizationManager.Instance.GetString(string.Format("REVIEW_TITLE_{0}", starCount));
            moneyText.text = PlayerPrefsManager.LoadData(string.Format("Save{0}_{1}", slotNumber, ECurrencyType.Money.ToString()), 0).ToString("N0");
            dateText.text = PlayerPrefsManager.LoadData(string.Format("Save{0}_LatestSavedTime", slotNumber), "", false);
            
            moneyIcon.SetActive(true);
        }
        else
        {
            newGameText.gameObject.SetActive(true);
        }
    }

    public void DeleteSave()
    {
        PlayerPrefsManager.DeleteSlotData(slotNumber, "HasSaveGame", false);
        PlayerPrefsManager.DeleteSlotData(slotNumber, "LatestSavedTime", false);
        
        List<string> dataKeys = new();
        dataKeys.Add("UnlockLevel");
        dataKeys.Add("ReviewLevel");
        dataKeys.Add(ECurrencyType.Money.ToString());
        dataKeys.Add(ECurrencyType.Star.ToString());
        dataKeys.Add("Stamina");
        dataKeys.Add("FridgeStorageData");
        for (int i = 0; i < 6; i++)
        {
            dataKeys.Add(string.Format("{0}_DecoDesignIdx", ((DecoID)i).ToString()));
            
            for (int j = 0; j < 4; j++)
            {
                dataKeys.Add(string.Format("{0}_HasDecoDesignIdx_{1}", ((DecoID)i).ToString(), j));
            }
        }

        foreach (var key in dataKeys)
        {
            PlayerPrefsManager.DeleteSlotData(slotNumber, key);
        }

        OnEnable();
    }

    public void ShowDeleteButton(bool isOn)
    {
        if (isOn == false && EventSystem.current.gameObject == deleteBtn)
        {
            return;
        }
        
        deleteBtn.SetActive(isOn && newGameText.activeSelf == false);
    }
}
