using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] GameObject infoPanel; // 라운드 플레이
    [SerializeField] RectTransform panelPivot;
    
    [Space(10)]
    [SerializeField] GameObject pausePrefab; // 일시정지
    [SerializeField] GameObject settingsPrefab; // 설정
    [SerializeField] GameObject resultPrefab; // 라운드 결과
    [SerializeField] GameObject preparePrefab; // 재정비
    [SerializeField] GameObject stockOrderPrefab; // 재고 관리
    [SerializeField] GameObject reviewPrefab; // 리뷰 관리
    [SerializeField] GameObject interiorPrefab; // 인테리어 관리
    [SerializeField] GameObject recipeBookPrefab; // 레시피북

    GameObject foregroundPanel;
    GameObject currentMenuPanel;

    Dictionary<EScreenState, GameObject> panelList = new();
    
    void Awake()
    {
        panelList.Add(EScreenState.Result, resultPrefab);
        panelList.Add(EScreenState.Prepare, preparePrefab);
        panelList.Add(EScreenState.PrepareStockOrder, stockOrderPrefab);
        panelList.Add(EScreenState.PrepareReview, reviewPrefab);
        panelList.Add(EScreenState.PrepareInterior, interiorPrefab);
        panelList.Add(EScreenState.RecipeBook, recipeBookPrefab);

        if (panelPivot.childCount > 0)
        {
            foreach (Transform child in panelPivot)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void ShowPanel(EScreenState screenState, bool hideInfoPanel = true)
    {
        infoPanel.SetActive(!hideInfoPanel);

        if (currentMenuPanel != null)
        {
            Destroy(currentMenuPanel);
        }
        
        if (panelList.ContainsKey(screenState))
        {
            currentMenuPanel = Instantiate(panelList[screenState], panelPivot);
        }
        else
        {
            infoPanel.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        if (currentMenuPanel != null)
        {
            Destroy(currentMenuPanel);
        }
    }

    public void ShowPause(bool isOn)
    {
        ShowForeground(isOn ? pausePrefab : null);
    }

    public void ShowSetting()
    {
        ShowForeground(settingsPrefab);
        foregroundPanel.GetComponent<Settings>().Init(() => ShowPause(true));
    }

    void ShowForeground(GameObject targetrefab)
    {
        if (foregroundPanel)
        {
            Destroy(foregroundPanel);
        }

        if (targetrefab)
        {
            foregroundPanel = Instantiate(targetrefab, panelPivot);
            foregroundPanel.transform.SetAsLastSibling();
        }
    }

    public bool IsShowingPanel()
    {
        return currentMenuPanel != null;
    }
}
