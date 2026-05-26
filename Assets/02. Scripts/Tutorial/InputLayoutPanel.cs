using UnityEngine;

public class InputLayoutPanel : PanelUI
{
    [SerializeField] GameObject[] inputGuides; // Keyboard | PS | Xbox
    
    void OnEnable()
    {
        SoundManager.Instance.PlaySound("SFX_Open1");
        
        RefreshInputGuide();
        EventManager.GetEvent(EGameEvent.OnControlChange).Subscribe(RefreshInputGuide);
    }

    void OnDisable()
    {
        EventManager.GetEvent(EGameEvent.OnControlChange).Unsubscribe(RefreshInputGuide);
    }

    void RefreshInputGuide()
    {
        foreach (var guide in inputGuides)
        {
            guide.SetActive(false);
        }
        
        if (InputDetector.Instance.IsInputGamepad())
        {
            inputGuides[InputDetector.Instance.currentInputType].SetActive(true);
        }
        else
        {
            inputGuides[0].SetActive(true);
        }
    }

    public override void Close()
    {
        canvasManager.ShowPause(true);
        canvasManager.ClosePanel();
    }
}
