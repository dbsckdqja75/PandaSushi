using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GamepadGuideElement : MonoBehaviour
{
    [SerializeField] bool onStartFoucs;
    [SerializeField] GamepadGuideData targetPreset;
    
    [Space(10)]
    [SerializeField] UnityEvent onDeselectEvent;
    [SerializeField] UnityEvent onSelectEvent;
    [SerializeField] UnityEvent onPressEvent;
    
    [Space(10)]
    [SerializeField] UnityEvent onWestEvent; // D-Pad Left
    [SerializeField] UnityEvent onEastEvent; // D-Pad Right
    [SerializeField] UnityEvent onNorthEvent; // D-Pad Up
    [SerializeField] UnityEvent onSouthEvent; // D-Pad Down

    void OnEnable()
    {
        if (onStartFoucs)
        {
            StartFocus();
            
            EventManager.GetEvent(EGameEvent.OnControlChange).Subscribe(StartFocus);
        }
    }

    void OnDisable()
    {
        if (onStartFoucs)
        {
            EventManager.GetEvent(EGameEvent.OnControlChange).Unsubscribe(StartFocus);
        }
    }

    void StartFocus()
    {
        EventManager.GetEvent<GamepadGuideElement>(EGameEvent.OnChangeGamepadFoucs).Invoke(this);
    }

    public void OnDeselect()
    {
        onDeselectEvent.Invoke();
    }

    public void OnSelect()
    {
        onSelectEvent.Invoke();
    }

    public void OnPress()
    {
        onPressEvent.Invoke();
    }

    public void ForceClick(Button targetButton)
    {
        if (targetButton.gameObject.activeSelf && targetButton.interactable)
        {
            targetButton.onClick.Invoke();
        }
    }
    
    public void ForceClickWithHide(Button targetButton)
    {
        EventManager.GetEvent<GamepadGuideElement>(EGameEvent.OnChangeGamepadFoucs).Invoke(null);
        
        ForceClick(targetButton);
    }

    public void Switch(GamepadGuideElement element)
    {
        if (element != null && element.gameObject.activeSelf == false)
        {
            return;
        }
        
        EventManager.GetEvent<GamepadGuideElement>(EGameEvent.OnChangeGamepadFoucs).Invoke(element);
    }

    public void OnPressDPad(int direction)
    {
        switch (direction)
        {
            case 0:
            onWestEvent.Invoke();
            break;
            case 1:
            onEastEvent.Invoke();
            break;
            case 2:
            onNorthEvent.Invoke();
            break;
            case 3:
            onSouthEvent.Invoke();
            break;
            default:
            break;
        }
    }

    public GamepadGuideData GetGuidePreset()
    {
        return targetPreset;
    }
}
