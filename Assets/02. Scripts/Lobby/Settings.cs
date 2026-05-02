using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Settings : MonoBehaviour
{
    [SerializeField] TMP_Text graphicsQualityValueText;
    [SerializeField] LocalizeText screenModeValueText;
    [SerializeField] TMP_Text screenResolutionValueText;
    [SerializeField] TMP_Text languageValueText;
    [SerializeField] TMP_Text musicVolumeValueText;
    [SerializeField] TMP_Text sfxVolumeValueText;

    #if UNITY_EDITOR || UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS
    [SerializeField] GameObject screenModeVBox;
    [SerializeField] GameObject screenResolutionBox;
    #endif
    
    const int defaultMusicVolume = 50;
    const int defaultSfxVolume = 80;

    Action closeEvent = null;

    void OnEnable()
    {
        UpdateSettings();
    }

    public void Init(Action closeCallback)
    {
        closeEvent = closeCallback;
    }

    public void UpdateSettings()
    {
        int graphicsQuality = PlayerPrefsManager.LoadData("GraphicsQualitySetting", 2);
        UpdateGraphicsQuality(graphicsQuality);
        
        #if (!UNITY_WEBGL && !UNITY_ANDROID && !UNITY_IOS) 
        bool screenMode = PlayerPrefsManager.LoadData("ScreenModeSetting", Screen.fullScreen);
        UpdateScreenMode(screenMode);
        
        int screenWidth = PlayerPrefsManager.LoadData("ScreenResolutionWidth", Screen.width);
        int screenHeight = PlayerPrefsManager.LoadData("ScreenResolutionHeight", Screen.height);
        UpdateScreenResolution(screenWidth, screenHeight);
        #endif
        
        int languageIdx = PlayerPrefsManager.LoadData("LanguageSetting", LocalizationManager.Instance.GetDefaultLanguage());
        UpdateLanguge(languageIdx);

        int musicVolume = PlayerPrefsManager.LoadData("MusicVolumeSetting", defaultMusicVolume);
        UpdateMusicVolume(musicVolume);
        
        int sfxVolume = PlayerPrefsManager.LoadData("SfxVolumeSetting", defaultSfxVolume);
        UpdateSfxVolume(sfxVolume);
        
        #if UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS
        screenModeVBox.SetActive(false);
        screenResolutionBox.SetActive(false);
        #endif
    }
    
    public void SwitchGraphicsQuality(bool isNext)
    {
        if (isNext)
        {
            QualitySettings.IncreaseLevel();
        }
        else
        {
            QualitySettings.DecreaseLevel();
        }
        
        UpdateGraphicsQuality(QualitySettings.GetQualityLevel());
        PlayerPrefsManager.SaveData("GraphicsQualitySetting", QualitySettings.GetQualityLevel());
    }

    public void UpdateGraphicsQuality(int graphicsQuality)
    {
        QualitySettings.SetQualityLevel(graphicsQuality);
        
        graphicsQualityValueText.text = "";
        for (int i = 0; i < graphicsQuality+1; i++)
        {
            graphicsQualityValueText.text += "★";
        }
    }

    public void ToggleScreenMode()
    {
        bool toggleMode = !Screen.fullScreen;
        
        UpdateScreenMode(toggleMode);
        PlayerPrefsManager.SaveData("GraphicsQualitySetting", toggleMode);
    }

    public void UpdateScreenMode(bool isFull)
    {
        if (isFull)
        {
            Screen.fullScreen = true;
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            screenModeValueText.SetLocaleString("LobbyStringData_Table", "SCREEN_MODE_FULLSCREEN");
        }
        else
        {
            Screen.fullScreen = false;
            Screen.fullScreenMode = FullScreenMode.Windowed;
            screenModeValueText.SetLocaleString("LobbyStringData_Table", "SCREEN_MODE_WINDOWED");
        }
    }
    
    public void UpdateScreenResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
        screenResolutionValueText.text = string.Format("{0}x{1}", width, height);
    }

    public void SwitchResolution(bool isNext)
    {
        List<Resolution> resolutions = new();
        foreach (Resolution resolution in Screen.resolutions.Where(x => x.refreshRateRatio.numerator == Screen.resolutions[Screen.resolutions.Length-1].refreshRateRatio.numerator))
        {
            if (resolution.width >= 1280 && resolution.height >= 720) // NOTE: 최소 해상도
            {
                resolutions.Add(resolution);
            }
        }

        if (resolutions.Count > 0)
        {
            int idx = PlayerPrefsManager.LoadData("ScreenResolutionIDX", -1);
            int screenWidth = PlayerPrefsManager.LoadData("ScreenResolutionWidth", Screen.width);
            int screenHeight = PlayerPrefsManager.LoadData("ScreenResolutionHeight", Screen.height);

            if (idx < 0)
            {
                idx = (resolutions.Count - 1);
                
                for (int i = 0; i < resolutions.Count; i++)
                {
                    if (resolutions[i].width == screenWidth && resolutions[i].height == screenHeight)
                    {
                        idx = i;
                        break;
                    }
                }
            }

            idx = (int)Mathf.Repeat(idx + (isNext ? 1 : -1), resolutions.Count);
            UpdateScreenResolution(resolutions[idx].width, resolutions[idx].height);

            PlayerPrefsManager.SaveData("ScreenResolutionIDX", idx);
            PlayerPrefsManager.SaveData("ScreenResolutionWidth", resolutions[idx].width);
            PlayerPrefsManager.SaveData("ScreenResolutionHeight", resolutions[idx].height);
        }
    }

    public void SwitchLanguage(bool isNext)
    {
        int languageIdx = PlayerPrefsManager.LoadData("LanguageSetting", LocalizationManager.Instance.GetDefaultLanguage());
        languageIdx = (int)Mathf.Repeat(languageIdx + (isNext ? 1 : -1), LocalizationManager.Instance.GetLocalesCount());
        
        UpdateLanguge(languageIdx);
        
        PlayerPrefsManager.SaveData("LanguageSetting", languageIdx);
    }
    
    public void UpdateLanguge(int languageIdx)
    {
        switch (languageIdx)
        {
            case 1:
                languageValueText.text = LocalizationManager.Instance.GetString("LobbyStringData_Table", "LANGUAGE_KOREAN");
                break;
            default:
                languageValueText.text = LocalizationManager.Instance.GetString("LobbyStringData_Table", "LANGUAGE_ENGLISH");;
                break;
        }
        
        LocalizationManager.Instance.ChangeLanguage(languageIdx);
    }

    public void SwitchMusicVolume(bool isIncrease)
    {
        int volume = PlayerPrefsManager.LoadData("MusicVolumeSetting", defaultMusicVolume);
        volume = Mathf.Clamp(volume + (isIncrease ? 5 : -5), 0, 100);
        
        UpdateMusicVolume(volume);
        PlayerPrefsManager.SaveData("MusicVolumeSetting", volume);
    }

    public void UpdateMusicVolume(int volume)
    {
        SoundManager.Instance.ChangeMusicVolume(volume * 0.01f);
        musicVolumeValueText.text = volume.ToString();
    }
    
    public void SwitchSfxVolume(bool isIncrease)
    {
        int volume = PlayerPrefsManager.LoadData("SfxVolumeSetting", defaultSfxVolume);
        volume = Mathf.Clamp(volume + (isIncrease ? 5 : -5), 0, 100);
        
        UpdateSfxVolume(volume);
        PlayerPrefsManager.SaveData("SfxVolumeSetting", volume);
        
        SoundManager.Instance.PlaySound("SFX_Ring");
    }

    public void UpdateSfxVolume(int volume)
    {
        SoundManager.Instance.ChangeSfxVolume(volume * 0.01f);
        sfxVolumeValueText.text = volume.ToString();
    }

    public void OnClickClose()
    {
        closeEvent?.Invoke();
    }
}
