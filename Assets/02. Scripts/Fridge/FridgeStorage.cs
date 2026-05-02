using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class FridgeStorageData
{
    public IngredientID[] keys;
    public int[] values;
}

public class FridgeStorage : MonoBehaviour
{
    Dictionary<IngredientID, int> storage = new();

    void OnEnable()
    {
        EventManager.GetEvent<IngredientID>(EGameEvent.OnStoreFridge).Subscribe(Store);
    }

    void OnDisable()
    {
        EventManager.GetEvent<IngredientID>(EGameEvent.OnStoreFridge).Unsubscribe(Store);
    }

    public void Load(Dictionary<IngredientID, int> data)
    {
        storage.Clear();

        foreach(var item in data)
        {
            storage.Add(item.Key, item.Value);
        }
    }

    void Store(IngredientID targetID)
    {
        Add(targetID);
    }

    public void Add(IngredientID targetID, int addValue = 1)
    {
        if(storage.ContainsKey(targetID) == false)
        {
            storage.Add(targetID, 0);
        }

        storage[targetID] += addValue;
    }
    
    public bool Take(IngredientID targetID)
    {
        if(storage.ContainsKey(targetID))
        {
            if(storage[targetID] > 0)
            {
                storage[targetID] -= 1;

                return true;
            }
        }

        return false;
    }

    public int GetStoredCount(IngredientID targetID)
    {
        if (storage.ContainsKey(targetID))
        {
            return storage[targetID];
        }

        return 0;
    }

    public bool CanReservable(IngredientID targetID)
    {
        return storage.ContainsKey(targetID) ? (storage[targetID] > 0) : false;
    }

    public void ClearTargets(List<IngredientID> targets)
    {
        foreach (var id in targets)
        {
            if (storage.ContainsKey(id))
            {
                storage.Remove(id);
            }
        }
    }

    public List<IngredientID> GetTakeableIngredientID()
    {
        List<IngredientID> data = new();
        foreach (var info in storage)
        {
            if (info.Value > 0)
            {
                data.Add(info.Key);
            }
        }

        return data;
    }

    public List<Tuple<IngredientID, int>> GetPairData()
    {
        List<Tuple<IngredientID, int>> data = new();
        foreach (var info in storage)
        {
            data.Add(new Tuple<IngredientID, int>(info.Key, info.Value));
        }

        return data;
    }
}
