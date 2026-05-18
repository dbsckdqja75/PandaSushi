using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SceneManagement;

public class InputDetector : MonoSingleton<InputDetector>
{
    public int currentInputType { get; private set; }
    string prevControlScheme = "";
    
    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameObject gamepadNavigatorPrefab;
    GamepadNavigator navigator;

    protected override void Init()
    {
        base.Init();

        currentInputType = 0;
    }

    void OnEnable()
    {
        prevControlScheme = playerInput.user.controlScheme.Value.name;
        playerInput.controlsChangedEvent.AddListener(OnControlChanged);
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        playerInput.controlsChangedEvent.RemoveAllListeners();
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnControlChanged(PlayerInput input)
    {
        if (prevControlScheme == input.user.controlScheme.Value.name)
        {
            return;
        }

        prevControlScheme = input.user.controlScheme.Value.name;
        if (prevControlScheme == "Gamepad")
        {
            if (Gamepad.current is DualShockGamepad || Gamepad.current is DualSenseGamepadHID)
            {
                UpdateInputType(1);
            }
            else
            {
                UpdateInputType(2);
            }

            SpawnGamepadNavigator();
            
            return;
        }
        
        Cursor.visible = true;
        
        UpdateInputType(0);
        
        if (navigator != null)
        {
            Destroy(navigator.gameObject);
            
            EventManager.GetEvent(EGameEvent.OnControlChange).Invoke();
        }
    }

    void SpawnGamepadNavigator()
    {
        if (navigator == null)
        {
            Cursor.visible = false;
            
            navigator = Instantiate(gamepadNavigatorPrefab, this.transform).GetComponent<GamepadNavigator>();
            
            EventManager.GetEvent(EGameEvent.OnControlChange).Invoke();
        }
    }

    void UpdateInputType(int targetID)
    {
        if (currentInputType != targetID)
        {
            currentInputType = targetID;
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (playerInput.camera == null)
        {
            playerInput.camera = Camera.main;

            if (navigator != null)
            {
                navigator.ResetFocus();
                Destroy(navigator.gameObject);
                navigator = null;
                
                SpawnGamepadNavigator();
            }
        }
    }

    public bool IsInputGamepad()
    {
        return (currentInputType > 0);
    }
}
