using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverPlateInfo
{
    public RecipeID recipeID;
    public List<IngredientID> ingredientList;
    public Transform target;

    public HoverPlateInfo(List<IngredientID> ingredientList, Transform target, RecipeID recipeID = RecipeID.NULL)
    {
        this.recipeID = recipeID;
        this.ingredientList = ingredientList;
        this.target = target;
    }
}

public class HoverPlateUI : MonoBehaviour
{
    [SerializeField] GameObject layoutPrefab;
    [SerializeField] RectTransform spawnPivot;

    HoverPlateElement infoLayout = null;

    void OnEnable()
    {
        EventManager.GetEvent<HoverPlateInfo>(EGameEvent.OnHoverPlate).Subscribe(UpdateHoverPlateInfo);
    }

    void OnDisable()
    {
        EventManager.GetEvent<HoverPlateInfo>(EGameEvent.OnHoverPlate).Unsubscribe(UpdateHoverPlateInfo);
    }

    void UpdateHoverPlateInfo(HoverPlateInfo info)
    {
        if (infoLayout == null && ObjectPool.Instance)
        {
            infoLayout = ObjectPool.Instance.Spawn<HoverPlateElement>(layoutPrefab, spawnPivot, false);
        }
        
        infoLayout.gameObject.SetActive(false);

        if(info != null && info.target != null)
        {
            this.StopAllCoroutines();
            RefreshHoverPlateRender(info).Start(this);
        }
    }

    IEnumerator RefreshHoverPlateRender(HoverPlateInfo info)
    {
        yield return new WaitForEndOfFrame();
        infoLayout.UpdateTarget(info.target);
        infoLayout.UpdateOffset(new Vector3(0, 1.25f, 0));
        
        if (info.recipeID != RecipeID.NULL)
        {
            infoLayout.UpdateIcon(info.recipeID);
            infoLayout.gameObject.SetActive(true);
        }
        else if (info.ingredientList != null && info.ingredientList.Count > 0)
        {
            infoLayout.UpdateIcon(info.ingredientList);
            infoLayout.gameObject.SetActive(true);
        }
        
        yield break;
    }
}