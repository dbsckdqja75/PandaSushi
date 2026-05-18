using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GamepadCanvasLayout : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] Button closeBtn;
    [SerializeField] InputActionReference closeInput;

    void OnEnable()
    {
        UpdateLabel();
        
        closeInput.action.started += ClosePanel;
        closeInput.action.Enable();

        EventManager.GetEvent(EGameEvent.OnControlChange).Subscribe(UpdateLabel);
    }

    void OnDisable()
    {
        closeInput.action.started -= ClosePanel;
        closeInput.action.Disable();
        
        EventManager.GetEvent(EGameEvent.OnControlChange).Unsubscribe(UpdateLabel);
    }

    void UpdateLabel()
    {
        content.SetActive(InputDetector.Instance.IsInputGamepad());
    }

    void ClosePanel(InputAction.CallbackContext inputContext)
    {
        if (closeBtn != null && closeBtn.gameObject.activeSelf)
        {
            closeBtn.onClick.Invoke();
        }
    }
}
