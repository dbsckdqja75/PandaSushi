using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Button))]
public class GamepadButton : MonoBehaviour
{
    [SerializeField] InputActionReference targetInput;
    Button targetButton;
    
    void Awake()
    {
        targetButton = this.GetComponent<Button>();
    }

    void OnEnable()
    {
        targetInput.action.started += OnClick;
        
        targetInput.action.Enable();
    }

    void OnDisable()
    {
        targetInput.action.started -= OnClick;
        
        targetInput.action.Disable();
    }

    void OnClick(InputAction.CallbackContext inputContext)
    {
        if (targetButton != null && targetButton.gameObject.activeSelf && targetButton.interactable)
        {
            targetButton.onClick.Invoke();
        }
    }
}
