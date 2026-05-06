using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class InputDetector : MonoBehaviour
{
    public static int currentInputType = 0;
    
    [SerializeField] GameObject gamepadNavigatorPrefab;
    GamepadNavigator navigator;

    public void OnControlChanged(PlayerInput input)
    {
        if (input.user.controlScheme.Value.name == "Gamepad")
        {
            if (Gamepad.current is DualShockGamepad || Gamepad.current is DualSenseGamepadHID)
            {
                UpdateInputType(1);
            }
            else
            {
                UpdateInputType(2);
            }

            if (navigator == null)
            {
                navigator = Instantiate(gamepadNavigatorPrefab).GetComponent<GamepadNavigator>();
            }
            
            Cursor.visible = false;
            
            return;
        }
        
        if (navigator != null)
        {
            Cursor.visible = true;
            
            navigator.ResetFocus();
            Destroy(navigator.gameObject);
        }

        UpdateInputType(0);
    }

    void UpdateInputType(int targetID)
    {
        if (currentInputType != targetID)
        {
            currentInputType = targetID;
        }
    }
}
