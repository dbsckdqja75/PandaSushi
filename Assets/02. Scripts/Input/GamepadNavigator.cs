using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadNavigator : MonoBehaviour
{
    Transform spawnPivot;

    GamepadGuideElement currentFocusElement = null;
    RectTransform currentGuide = null;

    LobbyActions lobbyActions;
    
    void Awake()
    {
        spawnPivot = FindAnyObjectByType<Canvas>().transform;
        
        lobbyActions = new LobbyActions();
        lobbyActions.Player.Move.performed += SwitchElement;
        lobbyActions.Player.MenuPrevious.started += (i) => SwitchElement(0);
        lobbyActions.Player.MenuNext.started += (i) => SwitchElement(1);
        lobbyActions.Player.MenuUp.started += (i) => SwitchElement(2);
        lobbyActions.Player.MenuDown.started += (i) => SwitchElement(3);
        lobbyActions.Player.Select.started += (i) => PressElement();

        EventManager.GetEvent<GamepadGuideElement>(EGameEvent.OnChangeGamepadFoucs).Subscribe(ChangeFocusElement);
    }

    void Start()
    {
        EventManager.GetEvent(EGameEvent.OnControlChange).Invoke();
    }

    void OnEnable()
    {
        lobbyActions.Player.Enable();
    }

    void OnDisable()
    {
        EventManager.GetEvent<GamepadGuideElement>(EGameEvent.OnChangeGamepadFoucs).Unsubscribe(ChangeFocusElement);
        ResetFocus();
        
        lobbyActions.Player.Disable();
    }

    void SwitchElement(InputAction.CallbackContext inputContext)
    {
        Vector2 inputValue = inputContext.ReadValue<Vector2>();

        int direction = 0;
        if (Mathf.Abs(inputValue.x) > 0.5f)
        {
            direction = (inputValue.x < 0) ? 0 : 1;
        }
        
        if (Mathf.Abs(inputValue.y) > 0.5f)
        {
            direction = (inputValue.y < 0) ? 3 : 2;
        }
        
        SwitchElement(direction);
        
        Debug.LogFormat("inputContext {0}", inputValue);
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

        if (element.gameObject.activeSelf == false)
        {
            return;
        }

        GameObject guidePrefab = null;
        if (currentFocusElement != null)
        {
            if (element.GetGuidePreset() != currentFocusElement.GetGuidePreset())
            {
                guidePrefab = element.GetGuidePreset().GetPrefab();
            }
            
            currentFocusElement.OnDeselect();
        }
        else
        {
            guidePrefab = element.GetGuidePreset().GetPrefab();
        }

        currentFocusElement = element;
        UpdateGuide(guidePrefab, currentFocusElement.GetRectTrf());
        currentFocusElement.OnSelect();
    }

    void UpdateGuide(GameObject prefab, RectTransform target)
    {
        if (prefab != null)
        {
            if (currentGuide != null)
            {
                Destroy(currentGuide.gameObject);
            }

            currentGuide = Instantiate(prefab, spawnPivot).GetComponent<RectTransform>();
        }

        currentGuide.position = target.position;
        currentGuide.sizeDelta = target.sizeDelta;
    }

    public void ResetFocus()
    {
        if (currentFocusElement != null)
        {
            currentFocusElement.OnDeselect();
        }

        if (currentGuide != null)
        {
            Destroy(currentGuide.gameObject);
        }
    }

    public LobbyActions GetLobbyActions()
    {
        return lobbyActions;
    }
}
