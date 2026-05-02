using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FridgeManager : MonoBehaviour
{
    [SerializeField] Fridge fridgeObject;
    [SerializeField] FridgePanel fridgeUI;
    [SerializeField] FridgeStorage fridgeStorage;

    Player player;

    void Awake()
    {
        player = GameObject.FindAnyObjectByType<Player>();
    }

    void OnEnable()
    {
        EventManager.GetEvent<bool>(EGameEvent.OnSelectFridge).Subscribe(OnSelectFridge);
        EventManager.GetEvent<IngredientID>(EGameEvent.OnClickFridgeSlot).Subscribe(OnTakeIngredient);
    }

    void OnDisable()
    {
        EventManager.GetEvent<bool>(EGameEvent.OnSelectFridge).Unsubscribe(OnSelectFridge);
        EventManager.GetEvent<IngredientID>(EGameEvent.OnClickFridgeSlot).Unsubscribe(OnTakeIngredient);
    }

    void OnTakeIngredient(IngredientID ingredientID)
    {
        if (fridgeStorage.Take(ingredientID))
        {
            player.HoldIngredient(ingredientID, (int)ingredientID >= 1000 ? (RecipeID)((int)ingredientID) : RecipeID.NULL);
        }
        
        EventManager.GetEvent<bool>(EGameEvent.OnSelectFridge).Invoke(false);
    }

    void OnSelectFridge(bool isOpen)
    {
        if (isOpen)
        {
            Open();
            
            SoundManager.Instance.PlaySound("SFX_Drop1");
        }
        else
        {
            Close();
            
            SoundManager.Instance.PlaySound("SFX_Drop2");
        }
    }

    void Open()
    {
        fridgeUI.UpdateInfo(fridgeStorage.GetPairData());
        fridgeUI.Show();
        fridgeObject.OnOpen();
    }
    
    void Close()
    {
        player.FinishSearching();
        
        fridgeUI.Hide();
        fridgeObject.OnClose();
    }

    public void CleanOutKitchen()
    {
        List<IngredientID> rawIngredientKeys = PandaResources.Instance.GetRawIngredientKeys();
        List<IngredientID> expireIngredientKeys = new();
        foreach (var pair in fridgeStorage.GetPairData())
        {
            if (rawIngredientKeys.Contains(pair.Item1) == false)
            {
                expireIngredientKeys.Add(pair.Item1);
            }
        }
        
        fridgeStorage.ClearTargets(expireIngredientKeys);
    }
    
    public void LoadStorageInfo()
    {
        string loadJsonData = PlayerPrefsManager.LoadSlotData("FridgeStorageData", "");
        Dictionary<IngredientID, int> loadData = new();
        if (loadJsonData.Length > 0)
        {
            FridgeStorageData loadStorageData = JsonUtility.FromJson<FridgeStorageData>(loadJsonData);
            for (int i = 0; i < loadStorageData.keys.Length; i++)
            {
                loadData.Add(loadStorageData.keys[i], loadStorageData.values[i]);
            }
        }
        else
        {
            loadData.Add(IngredientID.Rice, 3);
            loadData.Add(IngredientID.Nori, 3);

            loadData.Add(IngredientID.Engery_Drink, 12);
        }
        
        fridgeStorage.Load(loadData);
        Debug.LogWarningFormat("[LoadFridgeStorage] Count {0}", loadData.Count);
    }

    public void SaveStorageInfo()
    {
        var currentData = fridgeStorage.GetPairData();
        
        FridgeStorageData saveData = new FridgeStorageData();
        saveData.keys = new IngredientID[currentData.Count];
        saveData.values = new int[currentData.Count];

        for (int i = 0; i < currentData.Count; i++)
        {
            saveData.keys[i] = currentData[i].Item1;
            saveData.values[i] = currentData[i].Item2;
        }
        
        PlayerPrefsManager.SaveSlotData("FridgeStorageData", JsonUtility.ToJson(saveData));
        Debug.LogWarningFormat("[SaveFridgeStorage] {0}", JsonUtility.ToJson(saveData));
    }

    public void RandomTake()
    {
        var targets = fridgeStorage.GetTakeableIngredientID();
        if (targets.Count > 0)
        {
            fridgeStorage.Take(targets[Random.Range(0, targets.Count)]);
            
            EventManager.GetEvent<bool>(EGameEvent.OnSelectFridge).Invoke(false);
        }
    }
    
    public void ForceClose()
    {
        fridgeUI.Hide();
        fridgeObject.OnClose();
    }

    public FridgeStorage GetStorage()
    {
        return fridgeStorage;
    }
}
