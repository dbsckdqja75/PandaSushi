using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeBookPanel : PanelUI
{
    [SerializeField] RectTransform listPivot;
    [SerializeField] GameObject labelPrefab;
    
    void OnEnable()
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
