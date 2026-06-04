using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RecipeBookPanel : PanelUI
{
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] RectTransform listPivot;
    [SerializeField] GameObject labelPrefab;
    
    CustomPlayerActions playerActions;
    
    void OnEnable()
    {
        RefreshLayout();
        
        playerActions = new CustomPlayerActions();
        playerActions.Player.LeftStick.performed += InputScroll;
        
        playerActions.Player.MenuUp.started += (i) => UpdateScroll(10);
        playerActions.Player.MenuDown.started += (i) => UpdateScroll(-10);
        playerActions.Player.Enable();
    }

    void RefreshLayout()
    {
        int unlockLevel = PlayerPrefsManager.LoadSlotData("UnlockLevel", 1);
        
        List<RecipeBookLabel> labels = new();
        foreach (var id in PandaResources.Instance.GetRecipeKeys())
        {
            var data = PandaResources.Instance.GetRecipeData(id);
            if ((int)id < 1000 && data.GetTargetLevel() <= unlockLevel)
            {
                var label = Instantiate(labelPrefab, listPivot).GetComponent<RecipeBookLabel>();
                label.Init(id);
                labels.Add(label);
            }
        }
        
        labels.OrderBy(x => x.GetLevel());
        foreach (var label in labels)
        {
            label.transform.SetAsFirstSibling();
        }
    }
    
    void InputScroll(InputAction.CallbackContext inputContext)
    {
        Vector2 inputValue = inputContext.ReadValue<Vector2>();
        inputValue.x = 0;
        inputValue.y = Mathf.RoundToInt(inputValue.y);

        if (inputValue.y != 0)
        {
            UpdateScroll(inputValue.y);
        }
    }

    void UpdateScroll(float scrollValue)
    {
        scrollBar.value = (scrollBar.value + (scrollValue * 0.02f)); 
    }
    
    public override void Close()
    {
        if (EventManager.GameStatus != EGameState.RoundPrepare)
        {
            Time.timeScale = 1;
            
            canvasManager.ClosePanel();
        }
        else
        {
            StageManager.Instance.ChangeGameState(EGameState.RoundPrepare);
        }
    }
}
