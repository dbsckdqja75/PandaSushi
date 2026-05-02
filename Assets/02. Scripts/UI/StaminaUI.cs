using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] protected Image progressImg;
    [SerializeField] protected Animator animator;
    
    [Space(10)]
    [SerializeField] float warningThreshold = 0.3f;
    
    [Space(10)]
    [SerializeField] Color noramlColor = Color.white;
    [SerializeField] Color warningColor = Color.red;
    
    void OnEnable()
    {
        EventManager.GetEvent<float>(EGameEvent.OnUpdateStamina).Subscribe(UpdateFill);
    }

    void OnDisable()
    {
        EventManager.GetEvent<float>(EGameEvent.OnUpdateStamina).Unsubscribe(UpdateFill);
    }

    public void UpdateFill(float amount)
    {
        progressImg.fillAmount = amount;

        bool isWarning = (progressImg.fillAmount <= warningThreshold);
        progressImg.color = isWarning ? warningColor : noramlColor;
        animator.SetBool("isWarning", isWarning);
    }
}
