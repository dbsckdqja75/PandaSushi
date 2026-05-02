using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPlateUI : MonoBehaviour
{
    [SerializeField] GameObject mainLayout;
    [SerializeField] GameObject iconPrefab;
    
    [Space(10)]
    [SerializeField] RectTransform listPivot;
    [SerializeField] GridLayoutGroup listGroup;
    
    void OnEnable()
    {
        EventManager.GetEvent<List<IngredientID>>(EGameEvent.OnChangedPlayerPlate).Subscribe(UpdatePlayerPlateInfo);
        EventManager.GetEvent<RecipeID>(EGameEvent.OnHoldPlayerDish).Subscribe(UpdatePlayerPlateInfo);
        
        mainLayout.SetActive(false);
    }

    void OnDisable()
    {
        EventManager.GetEvent<List<IngredientID>>(EGameEvent.OnChangedPlayerPlate).Unsubscribe(UpdatePlayerPlateInfo);
        EventManager.GetEvent<RecipeID>(EGameEvent.OnHoldPlayerDish).Unsubscribe(UpdatePlayerPlateInfo);
    }

    void UpdatePlayerPlateInfo(List<IngredientID> ingredientList)
    {
        foreach (Transform child in listPivot)
        {
            Destroy(child.gameObject);
        }
        
        if (ingredientList != null && ingredientList.Count > 0)
        {
            foreach (var ingredient in ingredientList)
            {
                Image icon = Instantiate(iconPrefab, listPivot).GetComponent<Image>();
                icon.sprite = PandaResources.Instance.GetIngredientIcon(ingredient);
                icon.gameObject.SetActive(true);
            }
            
            RefreshListGroup();
            
            mainLayout.SetActive(true);
        }
        else
        {
            mainLayout.SetActive(false);
        }
    }
    
    void UpdatePlayerPlateInfo(RecipeID dish)
    {
        foreach (Transform child in listPivot)
        {
            Destroy(child.gameObject);
        }
        
        if (dish != RecipeID.NULL)
        {
            Image icon = Instantiate(iconPrefab, listPivot).GetComponent<Image>();
            icon.sprite = PandaResources.Instance.GetRecipeData(dish).GetIcon();
            icon.gameObject.SetActive(true);

            RefreshListGroup();
            
            mainLayout.SetActive(true);
        }
        else
        {
            mainLayout.SetActive(false);
        }
    }

    void RefreshListGroup()
    {
        if (listPivot.childCount <= 3)
        {
            listGroup.cellSize = new Vector2(160, 160);
        }
        else
        {
            listGroup.cellSize = new Vector2(100, 100);
        }
    }
}
