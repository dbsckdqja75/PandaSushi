using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadNavigator : MonoBehaviour
{
    GamepadGuideElement currentFocusElement = null;
    RectTransform currentGuide = null;
    RectTransform currentTarget = null;

    CustomPlayerActions playerActions;
    
    void Awake()
    {
        playerActions = new CustomPlayerActions();
        playerActions.Player.MenuMove.performed += SwitchElement;
        playerActions.Player.MenuPrevious.started += (i) => SwitchElement(0);
        playerActions.Player.MenuNext.started += (i) => SwitchElement(1);
        playerActions.Player.MenuUp.started += (i) => SwitchElement(2);
        playerActions.Player.MenuDown.started += (i) => SwitchElement(3);
        playerActions.Player.Select.started += (i) => PressElement();

        EventManager.GetEvent<GamepadGuideElement>(EGameEvent.OnChangeGamepadFoucs).Subscribe(ChangeFocusElement);
    }

    void OnEnable()
    {
        playerActions.Player.Enable();
    }

    void OnDestroy()
    {
        playerActions.Player.Disable();
        ResetFocus();
        
        EventManager.GetEvent<GamepadGuideElement>(EGameEvent.OnChangeGamepadFoucs).Unsubscribe(ChangeFocusElement);
    }

    void Update()
    {
        if (currentGuide != null && currentTarget != null)
        {
            currentGuide.position = currentTarget.position;
            currentGuide.sizeDelta = currentTarget.sizeDelta;
        }
    }

    void SwitchElement(InputAction.CallbackContext inputContext)
    {
        Vector2 inputValue = inputContext.ReadValue<Vector2>();
        inputValue.x = Mathf.RoundToInt(inputValue.x);
        inputValue.y = Mathf.RoundToInt(inputValue.y);

        int direction = 0;
        if (inputValue.x != 0)
        {
            direction = (inputValue.x < 0) ? 0 : 1;
        }
        
        if (inputValue.y != 0)
        {
            direction = (inputValue.y < 0) ? 3 : 2;
        }
        
        SwitchElement(direction);
    }

    void SwitchElement(int direction)
    {
        if (currentFocusElement != null)
        {
            currentFocusElement.OnPressDPad(direction);
        }
    }

    void PressElement()
    {
        if (currentFocusElement != null)
        {
            currentFocusElement.OnPress();
        }
    }

    public void ChangeFocusElement(GamepadGuideElement element)
    {
        if (element == null)
        {
            ResetFocus();
            return;
        }
        
        if (element == currentFocusElement || element.gameObject.activeSelf == false)
        {
            return;
        }

        if (currentFocusElement != null)
        {
            currentFocusElement.OnDeselect();
        }

        currentFocusElement = element;
        UpdateGuide(element.GetGuidePreset().GetPrefab(), currentFocusElement.transform);
        currentFocusElement.OnSelect();
    }

    void UpdateGuide(GameObject prefab, Transform target)
    {
        if (prefab != null)
        {
            if (currentGuide != null)
            {
                Destroy(currentGuide.gameObject);
            }

            currentGuide = Instantiate(prefab, target.root).GetComponent<RectTransform>();
        }

        currentTarget = target.gameObject.GetComponent<RectTransform>();
        currentGuide.position = currentTarget.position;
        currentGuide.sizeDelta = currentTarget.sizeDelta;
        currentGuide.SetParent(currentTarget.root);
    }

    public void ResetFocus()
    {
        if (currentFocusElement != null)
        {
            currentFocusElement.OnDeselect();
            currentFocusElement = null;
        }

        if (currentGuide != null)
        {
            Destroy(currentGuide.gameObject);
            currentGuide = null;
        }

        currentTarget = null;
    }
}
