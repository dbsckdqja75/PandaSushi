using UnityEngine;

public class PanelUI : MonoBehaviour
{
    protected CanvasManager canvasManager;

    protected virtual void Awake()
    {
        canvasManager = FindAnyObjectByType<CanvasManager>();
    }

    public virtual void Close()
    {
        canvasManager.ShowPanel(EScreenState.Prepare);
    }
}