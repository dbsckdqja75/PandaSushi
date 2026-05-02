using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverPlateElement : WorldLayoutElement
{
    [SerializeField] Image[] icons;

    public void UpdateIcon(RecipeID targetID)
    {
        OnDisable();
        
        icons[0].sprite = icons[0].sprite = PandaResources.Instance.GetRecipeIcon(targetID);
        icons[0].enabled = true;
        icons[0].gameObject.SetActive(true);
    }

    public void UpdateIcon(List<IngredientID> targetID)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            if (i < targetID.Count)
            {
                icons[i].sprite = PandaResources.Instance.GetIngredientIcon(targetID[i]);
                icons[i].enabled = true;
                icons[i].gameObject.SetActive(true);
            }
            else
            {
                icons[i].enabled = false;
                icons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnDisable()
    {
        foreach (var icon in icons)
        {
            icon.enabled = false;
            icon.gameObject.SetActive(false);
        }
    }
}
