using UnityEngine;
using UnityEngine.UI;

public class InputIcon : MonoBehaviour
{
    [SerializeField] Image targetImage; 
    [SerializeField] Sprite[] sprites; // NOTE: 0 - PS | 1 -Xbox 

    void OnEnable()
    {
        UpdateIcon();
        
        EventManager.GetEvent(EGameEvent.OnControlChange).Subscribe(UpdateIcon);
    }

    void OnDisable()
    {
        EventManager.GetEvent(EGameEvent.OnControlChange).Unsubscribe(UpdateIcon);
    }

    void UpdateIcon()
    {
        if (InputDetector.Instance.currentInputType > 0)
        {
            targetImage.sprite = sprites[InputDetector.Instance.currentInputType - 1];
            targetImage.enabled = true;
        }
        else
        {
            targetImage.enabled = false;
        }
    }
}
