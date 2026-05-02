using UnityEngine;

public class DecoPoint : MonoBehaviour
{
    [SerializeField] protected DecoID targetID;
    protected int currentDesignIdx = -1;

    [SerializeField] Vector3 cameraPos;
    [SerializeField] Vector3 cameraAngle;
    
    protected GameObject currentModel;

    void OnEnable()
    {
        RestoreDesign();
    }

    public virtual void UpdateDesign(int idx)
    {
        currentDesignIdx = idx;
    }
    
    protected virtual void ResetDesign()
    {
        currentDesignIdx = -1;
    }

    public void RestoreDesign()
    {
        string dataKey = string.Format("{0}_DecoDesignIdx", targetID.ToString());
        int savedDesignIdx = PlayerPrefsManager.LoadSlotData(dataKey, -1);

        if (currentDesignIdx != savedDesignIdx)
        {
            currentDesignIdx = savedDesignIdx;
            UpdateDesign(currentDesignIdx);
        }
    }

    public void ApplyDesign()
    {
        string dataKey = string.Format("{0}_DecoDesignIdx", targetID.ToString());
        PlayerPrefsManager.SaveSlotData(dataKey, currentDesignIdx);
        
        EventManager.GetEvent(EGameEvent.OnGameSaved).Invoke();
    }
    
    public void OnConsumed(int idx)
    {
        string dataKey = string.Format("{0}_HasDecoDesignIdx_{1}", targetID.ToString(), idx.ToString());
        PlayerPrefsManager.SaveSlotData(dataKey, true);
    }

    public DecoID GetID()
    {
        return targetID;
    }

    public string GetName()
    {
        if (currentDesignIdx < 0)
        {
            return LocalizationManager.Instance.GetString("DECO_TITLE_NONE");
        }
        
        return LocalizationManager.Instance.GetString(string.Format("{0}_TITLE_{1}", targetID.ToString().ToUpper(), currentDesignIdx));
    }

    public string GetDescription()
    {
        if (currentDesignIdx < 0)
        {
            return "";
        }
        
        return LocalizationManager.Instance.GetString(string.Format("{0}_DES_{1}", targetID.ToString().ToUpper(), currentDesignIdx));
    }

    public int GetDesignPrice()
    {
        return PandaResources.Instance.GetDecoPrice(targetID, currentDesignIdx);
    }

    public virtual int GetDesignCount()
    {
        return PandaResources.Instance.GetDecoCount(targetID);
    }

    public int GetSavedDesignIdx()
    {
        string dataKey = string.Format("{0}_DecoDesignIdx", targetID.ToString());
        return PlayerPrefsManager.LoadSlotData(dataKey, -1);
    }

    public bool HasDesign(int idx)
    {
        if (idx >= 0)
        {
            string dataKey = string.Format("{0}_HasDecoDesignIdx_{1}", targetID.ToString(), idx.ToString());
            return PlayerPrefsManager.LoadSlotData(dataKey, false);
        }

        return true;
    }

    public int GetDesignIdx()
    {
        return currentDesignIdx;
    }

    public Vector3 GetCameraPos()
    {
        return cameraPos;
    }
    
    public Vector3 GetCameraAngle()
    {
        return cameraAngle;
    }
}
