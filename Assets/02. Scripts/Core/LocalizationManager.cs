using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class LocalizationManager : MonoSingleton<LocalizationManager>
{
    [SerializeField] TableReference tableReference = "LobbyStringData_Table";

    Coroutine changeLocaleCoroutine;
    
    public void ChangeLanguage(int language)
    {
        changeLocaleCoroutine?.Stop(this);
        changeLocaleCoroutine = ChangeLocale(language).Start(this);
    }

    public void ChangeTableReference(string tableName)
    {
        tableReference = tableName;
    }
    
    public string GetString(string targetTable, string localeKey)
    {
        Locale currentLanguage = LocalizationSettings.SelectedLocale;
        return LocalizationSettings.StringDatabase.GetLocalizedString(targetTable, localeKey, currentLanguage);
    }

    public string GetString(string localeKey)
    {
        Locale currentLanguage = LocalizationSettings.SelectedLocale;
        return LocalizationSettings.StringDatabase.GetLocalizedString(tableReference, localeKey, currentLanguage);
    }

    public int GetLocaleIdx()
    {
        string code = LocalizationSettings.SelectedLocale.Identifier.Code;
        if (code == "ko-KR")
        {
            return 1;
        }
        
        return 0;
    }

    IEnumerator ChangeLocale(int index)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        yield break;
    }

    public int GetDefaultLanguage()
    {
        switch(Application.systemLanguage)
        {
            case SystemLanguage.Korean:
                return 1;
            default: 
                return 0; // NOTE : SystemLanguage.English
        }
    }

    public int GetLocalesCount()
    {
        return LocalizationSettings.AvailableLocales.Locales.Count;
    }
}