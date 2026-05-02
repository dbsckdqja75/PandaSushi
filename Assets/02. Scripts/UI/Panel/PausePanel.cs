using UnityEngine;

public class PausePanel : PanelUI
{
    [SerializeField] GameObject restartBtn;
    
    void OnEnable()
    {
        restartBtn.SetActive(canvasManager.IsShowingPanel() == false);
    }
    
    public void OnClickResume()
    {
        StageManager.Instance.ChangeGameState(EGameState.Resume);
    }
    
    public void OnClickRestart()
    {
        StageManager.Instance.ChangeGameState(EGameState.RoundRestart);
    }

    public void OnClickSetting()
    {
        canvasManager.ShowSetting();
    }
    
    public void OnClickLeave()
    {
        StageManager.Instance.ChangeGameState(EGameState.Leave);
    }

    public override void Close()
    {
        OnClickResume();
    }
}
