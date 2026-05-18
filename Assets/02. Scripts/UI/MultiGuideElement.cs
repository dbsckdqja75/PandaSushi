using UnityEngine;

public class MultiGuideElement : GuideElement
{
    [SerializeField] GameObject desktopGuide;
    [SerializeField] GameObject gamepadGuide;

    void OnEnable()
    {
        RefreshPlatformGuide();
        
        EventManager.GetEvent(EGameEvent.OnControlChange).Subscribe(RefreshPlatformGuide);
    }

    void OnDisable()
    {
        EventManager.GetEvent(EGameEvent.OnControlChange).Unsubscribe(RefreshPlatformGuide);
    }

    void RefreshPlatformGuide()
    {
        bool isGamePad = InputDetector.Instance.IsInputGamepad();
        desktopGuide.SetActive(isGamePad == false);
        gamepadGuide.SetActive(isGamePad);
    }
}
