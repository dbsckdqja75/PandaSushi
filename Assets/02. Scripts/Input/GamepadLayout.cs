using UnityEngine;
using UnityEngine.UI;

public class GamepadLayout : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] Button closeBtn;
    GamepadNavigator navigator;
    
    void OnEnable()
    {
        UpdateLabel();
        UpdateCloseEvent();

        EventManager.GetEvent(EGameEvent.OnControlChange).Subscribe(UpdateLabel);
        EventManager.GetEvent(EGameEvent.OnControlChange).Subscribe(UpdateCloseEvent);
    }

    void OnDisable()
    {
        EventManager.GetEvent(EGameEvent.OnControlChange).Unsubscribe(UpdateLabel);
        EventManager.GetEvent(EGameEvent.OnControlChange).Unsubscribe(UpdateCloseEvent);
    }

    void UpdateLabel()
    {
        content.SetActive(InputDetector.currentInputType > 0);
    }

    void UpdateCloseEvent()
    {
        if (closeBtn != null)
        {
            navigator = GameObject.FindAnyObjectByType<GamepadNavigator>();
            if (navigator != null)
            {
                navigator.GetLobbyActions().Player.Back.started += (i) => ClosePanel();
            }
        }
    }

    void ClosePanel()
    {
        if (closeBtn.gameObject.activeSelf)
        {
            closeBtn.onClick.Invoke();
        }
    }
}
