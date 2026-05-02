using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteriorPanel : PanelUI
{
    DecoID selectedDecoID = DecoID.Deco_FloorColor;
    int selectedDesignIdx = 0;

    CameraView cameraView; 
    EnvDoor[] doors;
    DecoPoint[] decoPoints;

    [SerializeField] TMP_Text selectedIdText;
    [SerializeField] TMP_Text selectedIdxText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text priceText;
    
    [Space(10)]
    [SerializeField] GameObject consumeBox;
    [SerializeField] Button consumeBtn;
    [SerializeField] Button appliedBtn;
    [SerializeField] Button applyBtn;

    void OnEnable()
    {
        cameraView = FindAnyObjectByType<CameraView>();
        doors = StageManager.Instance.GetDoors();
        decoPoints = StageManager.Instance.GetDecoPoints();
        
        selectedDecoID = DecoID.Deco_FloorColor;
        selectedDesignIdx = decoPoints[0].GetDesignIdx();
        
        UpdateLayout();
        UpdatePointView();

        foreach (var door in doors)
        {
            door.KeepOpen();
        }
    }

    public void SwitchCategory(bool isNext)
    {
        decoPoints[(int)selectedDecoID].RestoreDesign();
        
        selectedDecoID = (DecoID)Mathf.Repeat((int)selectedDecoID + (isNext ? 1 : -1), decoPoints.Length);
        selectedDesignIdx = decoPoints[(int)selectedDecoID].GetDesignIdx();

        UpdateLayout();
        UpdatePointView();
    }

    public void SwitchIdx(bool isNext)
    {
        int decoCount = decoPoints[(int)selectedDecoID].GetDesignCount();
        selectedDesignIdx += (isNext ? 1 : -1);
        
        if (selectedDesignIdx >= decoCount)
        {
            selectedDesignIdx = -1;
        }
        else if(selectedDesignIdx < -1)
        {
            selectedDesignIdx = (decoCount - 1);
        }

        decoPoints[(int)selectedDecoID].UpdateDesign(selectedDesignIdx);

        UpdateLayout();
    }

    public void ApplySelectedDesign()
    {
        decoPoints[(int)selectedDecoID].ApplyDesign();
        
        UpdateLayout();
    }

    public void ConsumeDesign()
    {
        if (CurrencyManager.Instance.Purchase(ECurrencyType.Money, decoPoints[(int)selectedDecoID].GetDesignPrice()))
        {
            decoPoints[(int)selectedDecoID].OnConsumed(selectedDesignIdx);
            decoPoints[(int)selectedDecoID].ApplyDesign();
            
            CurrencyManager.Instance.SaveCurrencyData();
            SoundManager.Instance.PlaySound("SFX_Cash");
            
            EventManager.GetEvent(EGameEvent.OnUpdateCurrency).Invoke();
        }
        
        UpdateLayout();
    }

    void UpdateLayout()
    {
        selectedIdText.text = LocalizationManager.Instance.GetString(selectedDecoID.ToString().ToUpper());
        selectedIdxText.text = decoPoints[(int)selectedDecoID].GetName();
        descriptionText.text = decoPoints[(int)selectedDecoID].GetDescription();

        if (decoPoints[(int)selectedDecoID].HasDesign(selectedDesignIdx))
        {
            int savedIdx = decoPoints[(int)selectedDecoID].GetSavedDesignIdx(); 
            
            consumeBox.SetActive(false);
            applyBtn.gameObject.SetActive(savedIdx != selectedDesignIdx);
            appliedBtn.gameObject.SetActive(savedIdx == selectedDesignIdx);
        }
        else
        {
            int price = decoPoints[(int)selectedDecoID].GetDesignPrice();
            priceText.text = price.ToString("N0");
            consumeBtn.interactable = CurrencyManager.Instance.CanPurchase(ECurrencyType.Money, price);
            consumeBox.SetActive(true);
            
            applyBtn.gameObject.SetActive(false);
            appliedBtn.gameObject.SetActive(false);
        }
    }

    void UpdatePointView()
    {
        cameraView.UpdateTarget(decoPoints[(int)selectedDecoID].transform);
        cameraView.UpdateOffset(decoPoints[(int)selectedDecoID].GetCameraPos(), decoPoints[(int)selectedDecoID].GetCameraAngle(), 7, false);
    }

    public override void Close()
    {
        decoPoints[(int)selectedDecoID].RestoreDesign();
        
        foreach (var door in doors)
        {
            door.Close();
        }
        
        StageManager.Instance.ChangeGameState(EGameState.RoundPrepare);
    }
}
