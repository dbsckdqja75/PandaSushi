using UnityEngine;
using UnityEngine.UI;

public class PauseMenuButton : MonoBehaviour, ICanvasSlot
{
    [SerializeField] Button btn;

    void OnEnable()
    {
        btn.enabled = true;
        btn.interactable = true;
    }

    void OnDisable()
    {
        btn.enabled = false;
        btn.interactable = false;
    }

    public void OnClick()
    {
        if (btn.enabled && btn.interactable)
        {
            btn.onClick.Invoke();
        }
    }

    public bool CanSelect()
    {
        return (btn.enabled && btn.interactable);
    }
    
    public Transform GetTransform()
    {
        return this.transform;
    }
}
