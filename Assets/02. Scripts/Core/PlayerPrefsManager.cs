using System;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    [SerializeField] bool onStartWithReset = false;
    static int saveSlotNumber = 1;
    
    void Awake()
    {
        Init();
    }

    void Init()
    {
        if(onStartWithReset)
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public static void ChangeTargetSlot(int targetNumber)
    {
        saveSlotNumber = targetNumber;
        
        SaveData(string.Format("Save{0}_HasSaveGame", targetNumber), true, false);
    }

    public static void SaveData<T>(string key, T value, bool encryption = true)
    {
        string convertValue = value.ToString();
        string saveKey = encryption ? EncryptAES.Encrypt256(key) : key;
        string saveValue = encryption ? EncryptAES.Encrypt256(convertValue) : convertValue;

        PlayerPrefs.SetString(saveKey, saveValue);
    }

    public static T LoadData<T>(string key, T defaultValue = default(T), bool encryption = true)
    {
        string saveKey = encryption ? EncryptAES.Encrypt256(key) : key;
        if(!PlayerPrefs.HasKey(saveKey))
        {
            return defaultValue;
        }

        string saveValue = PlayerPrefs.GetString(saveKey);
        string dataValue = encryption ? EncryptAES.Decrypt256(saveValue) : saveValue;

        if(dataValue.Length <= 0)
        {
            return defaultValue;
        }
        
        try
        {
            var convertedValue = (T)Convert.ChangeType(dataValue, typeof(T));
            return convertedValue;
        }
        catch (Exception e)
        {
            return defaultValue;
        }
    }
    
    public static void SaveSlotData<T>(string key, T value, bool encryption = true)
    {
        string convertValue = value.ToString();
        string saveKey = encryption ? EncryptAES.Encrypt256(string.Format("Save{0}_{1}", saveSlotNumber, key)) : string.Format("Save{0}_{1}", saveSlotNumber, key);
        string saveValue = encryption ? EncryptAES.Encrypt256(convertValue) : convertValue;

        PlayerPrefs.SetString(saveKey, saveValue);
        PlayerPrefs.SetString(string.Format("Save{0}_LatestSavedTime", saveSlotNumber), DateTime.Now.ToString("yyyy-MM-dd\nHH:mm:dd"));
    }
    
    public static T LoadSlotData<T>(string key, T defaultValue = default(T), bool encryption = true)
    {
        string saveKey = encryption ? EncryptAES.Encrypt256(string.Format("Save{0}_{1}", saveSlotNumber, key)) : string.Format("Save{0}_{1}", saveSlotNumber, key);
        if(!PlayerPrefs.HasKey(saveKey))
        {
            return defaultValue;
        }

        string saveValue = PlayerPrefs.GetString(saveKey);
        string dataValue = encryption ? EncryptAES.Decrypt256(saveValue) : saveValue;

        if(dataValue.Length <= 0)
        {
            return defaultValue;
        }
        
        try
        {
            var convertedValue = (T)Convert.ChangeType(dataValue, typeof(T));
            return convertedValue;
        }
        catch (Exception e)
        {
            return defaultValue;
        }
    }

    public static bool HasData(string dataKey, bool encryption = true)
    {
        string saveKey = encryption ? EncryptAES.Encrypt256(dataKey) : dataKey;
        return PlayerPrefs.HasKey(saveKey);
    }

    public static void DeleteData(string dataKey, bool encryption = true)
    {
        string saveKey = encryption ? EncryptAES.Encrypt256(dataKey) : dataKey;
        PlayerPrefs.DeleteKey(saveKey);
    }
    
    public static void DeleteSlotData(int slotNumber, string dataKey, bool encryption = true)
    {
        string saveKey = encryption ? EncryptAES.Encrypt256(string.Format("Save{0}_{1}", slotNumber, dataKey)) : string.Format("Save{0}_{1}", slotNumber, dataKey);
        PlayerPrefs.DeleteKey(saveKey);
    }
}
