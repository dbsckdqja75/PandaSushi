using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    enum ButtonSoundType 
    {
        CLICK,
        CANCEL,
        BOOK_OPEN,
        PANEL_OPEN,
        TICK,
        CLOSE,
        APPLY,
    }

    [SerializeField] ButtonSoundType soundType = ButtonSoundType.CLICK;

    void Awake()
    {
        string soundName = "";
        Button button = this.GetComponent<Button>();

        switch(soundType)
        {
            case ButtonSoundType.CANCEL:
            soundName = "SFX_Cancel";
            break;
            case ButtonSoundType.BOOK_OPEN:
            soundName = "SFX_Open1";
            break;
            case ButtonSoundType.PANEL_OPEN:
            soundName = "SFX_Switch1";
            break;
            case ButtonSoundType.TICK:
            soundName = "SFX_Tick1";
            break;
            case ButtonSoundType.CLOSE:
            soundName = "SFX_Tap1";
            break;
            case ButtonSoundType.APPLY:
            soundName = RandomExtensions.RandomBool() ? "SFX_Drop1" : "SFX_Drop2";
            break;
            default:
            soundName = "SFX_Click1";
            break;
        }

        button.onClick.AddListener(() => SoundManager.Instance.PlaySound(soundName) );
    }
}
