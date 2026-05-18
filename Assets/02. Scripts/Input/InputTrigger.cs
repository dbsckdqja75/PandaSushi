using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputTrigger : MonoBehaviour
{
    [SerializeField] InputActionReference targetInput;
    [SerializeField] UnityEvent targetEvent;
    
    void OnEnable()
    {
        targetInput.action.canceled += OnInput;
        targetInput.action.Enable();
    }

    void OnDisable()
    {
        targetInput.action.canceled += OnInput;
        targetInput.action.Disable();
    }

    void OnInput(InputAction.CallbackContext inputContext)
    {
        targetEvent.Invoke();
    }
}
