using UnityEngine;
using TMPro;

public class OrderSheet : IndicatorElement
{
    [SerializeField] TMP_Text numberText;
    
    [Space(10)]
    [SerializeField] float warningThreshold = 0.25f;
    
    [Space(10)]
    [SerializeField] Color noramlColor = Color.white;
    [SerializeField] Color warningColor = Color.red;

    public void UpdateInfo(int number)
    {
        numberText.text = $"{number:000}";
    }
    
    public override void UpdateFill(float amount)
    {
        base.UpdateFill(amount);

        progressImg.color = (progressImg.fillAmount <= warningThreshold) ? warningColor : noramlColor;
    }
}
