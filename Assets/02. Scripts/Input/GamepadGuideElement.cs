using System;
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

    RectTransform rectTrf;

    void Awake()
    {
        rectTrf = this.GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        if (onStartFoucs)
        {
            EventManager.GetEvent(EGameEvent.OnControlChange).Subscribe(OnEnable);
            EventManager.GetEvent<GamepadGuideElement>(EGameEvent.OnChangeGamepadFoucs).Invoke(this);
        }
    }

    void OnDisable()
    {
        if (onStartFoucs)
        {
            EventManager.GetEvent(EGameEvent.OnControlChange).Unsubscribe(OnDisable);
        }
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
        targetButton.onClick.Invoke();
    }

    public void Switch(GamepadGuideElement element)
    {
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

    public RectTransform GetRectTrf()
    {
        return rectTrf;
    }
}
