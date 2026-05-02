using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    [SerializeField] Camera lobbyCamera;
    [SerializeField] Animator lobbyCameraAnimator;
    
    [Space(10)]
    [SerializeField] Settings settingPanel;
    
    [SerializeField] Animator fridgeAnimator;
    [SerializeField] GameObject saveSelectPanel;

    [SerializeField] Vector3 startCameraPos;
    [SerializeField] Vector3 startCameraRot;
    [SerializeField] float startCameraFOV = 41;
    
    [Space(10)]
    [SerializeField] int targetSceneIdx = 2;
    [SerializeField] GameObject loadingScreen;
    
    void Awake()
    {
        LocalizationManager.Instance.ChangeTableReference("LobbyStringData_Table");
        
        settingPanel.Init(() => settingPanel.gameObject.SetActive(false));
        settingPanel.UpdateSettings();
    }

    void OnEnable()
    {
        lobbyCameraAnimator.SetBool("isOpenFridge", false);
        fridgeAnimator.SetBool("isOpen", false);
        
        SoundManager.Instance.PlayMusic("Lobby_ServiceAtThePass");
        InvokeRepeating("PlayLobbyMusicLoop", 130,  110);
        
        if (StageManager.Instance != null)
        {
            Destroy(StageManager.Instance.gameObject);
        }
        
        if (CurrencyManager.Instance != null)
        {
            Destroy(CurrencyManager.Instance.gameObject);
        }
        
        if (PandaResources.Instance != null)
        {
            Destroy(PandaResources.Instance.gameObject);
        }
        
        if (ObjectPool.Instance != null)
        {
            Destroy(ObjectPool.Instance.gameObject);
        }
        
        if (GuideUI.Instance != null)
        {
            Destroy(GuideUI.Instance.gameObject);
        }
        
        EventManager.ClearEvents();
    }

    void PlayLobbyMusicLoop()
    {
        SoundManager.Instance.PlayMusic("Play_PandaKitchenSprint");
    }

    public void OnClickStart()
    {
        lobbyCameraAnimator.SetBool("isOpenFridge", true);
        fridgeAnimator.SetBool("isOpen", true);
        
        saveSelectPanel.SetActive(true);
    }

    public void OnClickSetting()
    {
        settingPanel.gameObject.SetActive(true);
    }

    public void OnClickReturnSlot()
    {
        lobbyCameraAnimator.SetBool("isOpenFridge", false);
        fridgeAnimator.SetBool("isOpen", false);
        
        saveSelectPanel.SetActive(false);
    }

    public void OnClickSaveSlot(int targetSlot = 1)
    {
        PlayerPrefsManager.ChangeTargetSlot(targetSlot);

        CancelInvoke();
        SoundManager.Instance.StopMusic();
        
        loadingScreen.SetActive(true);
        LoadScene().Start(this);
    }

    public void OnClickQuit()
    {
        CancelInvoke();
        Application.Quit();
    }

    IEnumerator LoadScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetSceneIdx);
        asyncOperation.allowSceneActivation = false;

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.25f);
        // yield return new WaitForSeconds(1.5f);

        float progress = 0;
        while(!asyncOperation.isDone || asyncOperation.allowSceneActivation == false)
        {
            // loadingProgressImage.fillAmount = Mathf.Lerp(loadingProgressImage.fillAmount, progress, Time.deltaTime);

            if(asyncOperation.progress < 0.9f)
            {
                progress = asyncOperation.progress;
            }
            else
            {
                progress = 1;
                asyncOperation.allowSceneActivation = true;
            }

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}
