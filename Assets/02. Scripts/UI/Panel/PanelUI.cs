using UnityEngine;

public class PanelUI : MonoBehaviour
{
    protected CanvasManager canvasManager;

    protected virtual void Awake()
    {
        canvasManager = FindAnyObjectByType<CanvasManager>();
    }
    
    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }
    
    public virtual void Close()
    {
        canvasManager.ShowPanel(EScreenState.Prepare);
    }
}