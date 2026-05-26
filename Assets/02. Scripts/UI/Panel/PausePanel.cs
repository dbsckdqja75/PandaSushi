using UnityEngine;

public class PausePanel : PanelUI
{
    [SerializeField] GameObject restartBtn;
    
    void Start()
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

    public void OnClickInputLayout()
    {
        canvasManager.ShowPanel(EScreenState.InputLayout);
        canvasManager.ShowPause(false);
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
