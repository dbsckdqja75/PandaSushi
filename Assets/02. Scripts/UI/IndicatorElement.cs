using UnityEngine;
using UnityEngine.UI;

public class IndicatorElement : MonoBehaviour 
{
    [SerializeField] protected Image iconImg;
    [SerializeField] protected Image progressImg;

    protected Transform targetTrf;
    protected Vector3 targetOffset = Vector3.zero;

    void Update()
    {
        if(targetTrf != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(targetTrf.position + targetOffset);
        }
    }

    public void UpdateIcon(Sprite newSprite)
    {
        if(iconImg != null)
        {
            iconImg.sprite = newSprite;
        }
    }

    public virtual void UpdateFill(float amount)
    {
        amount = Mathf.Clamp(amount, 0f, 1f);

        progressImg.fillAmount = amount;
    }

    public void UpdateTarget(Transform newTarget)
    {
        targetTrf = newTarget;
        Update();
    }

    public void UpdateOffset(Vector3 newOffset)
    {
        targetOffset = newOffset;
        Update();
    }
    
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}