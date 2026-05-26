using UnityEngine;
using TMPro;

public class TutorialPanel : InputLayoutPanel
{
    int currentPage = 0;

    [SerializeField] TMP_Text pageCountText;
    [SerializeField] GameObject[] pages;

    public void ChangePage(bool isNext)
    {
        foreach (var page in pages)
        {
            page.SetActive(false);
        }

        currentPage = (int)Mathf.Repeat(currentPage + (isNext ? 1 : -1), pages.Length);
        pages[currentPage].SetActive(true);

        pageCountText.text = string.Format("{0} / {1}", currentPage + 1, pages.Length);
    }

    public override void Close()
    {
        #if !UNITY_EDITOR
        PlayerPrefsManager.SaveData("FinishedTutorial", true, false);
        #endif
        
        EventManager.GameStatus = EGameState.RoundPrepare;
        
        Destroy(this.gameObject);
    }
}
