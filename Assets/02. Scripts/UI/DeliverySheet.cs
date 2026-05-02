using UnityEngine;
using TMPro;

public class DeliverySheet : IndicatorElement
{
    [SerializeField] TMP_Text pendingCountText;
    
    [Space(10)]
    [SerializeField] float warningThreshold = 0.25f;
    
    [Space(10)]
    [SerializeField] Color noramlColor = Color.white;
    [SerializeField] Color warningColor = Color.red;

    public void UpdateInfo(RecipeID targetID, int pendingCount)
    {
        iconImg.sprite = PandaResources.Instance.GetRecipeIcon(targetID);
        pendingCountText.text = (pendingCount > 0) ? string.Format("+{0}", pendingCount) : "";
    }

    public override void UpdateFill(float amount)
    {
        base.UpdateFill(amount);

        progressImg.color = (progressImg.fillAmount <= warningThreshold) ? warningColor : noramlColor;
    }
}
