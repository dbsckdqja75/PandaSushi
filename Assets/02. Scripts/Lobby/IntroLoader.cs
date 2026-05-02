using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroLoader : MonoBehaviour
{
    [SerializeField] int targetSceneIdx = 1;
    [SerializeField] GameObject loadingScreen;
    
    void Start()
    {
        this.StopAllCoroutines();
        LoadScene().Start(this);
    }

    IEnumerator LoadScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetSceneIdx);
        asyncOperation.allowSceneActivation = false;

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(2.125f);
        
        loadingScreen.SetActive(true);

        float progress = 0;
        while(!asyncOperation.isDone || asyncOperation.allowSceneActivation == false)
        {
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
