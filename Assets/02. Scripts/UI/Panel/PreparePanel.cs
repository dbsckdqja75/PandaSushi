using UnityEngine;

public class PreparePanel : PanelUI
{
    public void OnClickStockOrder()
    {
        canvasManager.ShowPanel(EScreenState.PrepareStockOrder);
    }
    
    public void OnClickReview()
    {
        canvasManager.ShowPanel(EScreenState.PrepareReview);
    }
    
    public void OnClickInterior()
    {
        canvasManager.ShowPanel(EScreenState.PrepareInterior);
    }

    public void OnClickRecipeBook()
    {
        canvasManager.ShowPanel(EScreenState.RecipeBook);
    }

    public void OnClickPause()
    {
        StageManager.Instance.ChangeGameState(EGameState.Pause);
    }

    public override void Close()
    {
        StageManager.Instance.ChangeGameState(EGameState.RoundReady);
    }
}
