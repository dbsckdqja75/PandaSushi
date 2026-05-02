using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[RequireComponent(typeof(Image))]
public class LocalizeImage : MonoBehaviour
{
    Image targetImage;
    [SerializeField] Sprite[] sprites;

    void Awake()
    {
        targetImage = this.GetComponent<Image>();
    }
    
    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += UpdateLocale;

        UpdateLocale(LocalizationSettings.SelectedLocale);
    }

    void OnDisable() 
    {
        LocalizationSettings.SelectedLocaleChanged -= UpdateLocale;
    }
    
    void UpdateLocale(Locale locale)
    {
        int targetIdx = LocalizationManager.Instance.GetLocaleIdx();
        if (targetIdx < sprites.Length)
        {
            targetImage.sprite = sprites[targetIdx];
        }
    }
}
