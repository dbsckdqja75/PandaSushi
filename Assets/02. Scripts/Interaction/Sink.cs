using UnityEngine;

public class Sink : InteractionObject
{
    int maxDishCount = -1;
    int curDirtyDishes = 0;

    [SerializeField] float sinkDuration = 1.5f;

    [Space(10)]
    [SerializeField] Vector3 assemblePosOffset;
    [SerializeField] Vector3 assembleRotOffset;

    [Space(10)]
    [SerializeField] GameObject sinkWater;
    [SerializeField] GameObject waterFX;
    [SerializeField] GameObject clearFX;
    
    [Space(10)]
    [SerializeField] GameObject[] dirtyDishes;
    
    [Space(10)]
    [SerializeField] GlobalState globalState;

    Player player;
    ProcessHandler processHandler;

    void Awake()
    {
        player = GameObject.FindAnyObjectByType<Player>();
        processHandler = this.GetComponent<ProcessHandler>();
    }

    void OnEnable()
    {
        ResetObject();
        
        EventManager.GetEvent(EGameEvent.OnCleanedSeat).Subscribe(AddDirtyDish);
        EventManager.GetEvent(EGameEvent.OnHighlightSink).Subscribe(OnHighlight);
    }

    void OnDisable()
    {
        EventManager.GetEvent(EGameEvent.OnCleanedSeat).Unsubscribe(AddDirtyDish);
        EventManager.GetEvent(EGameEvent.OnHighlightSink).Unsubscribe(OnHighlight);
    }

    void OnHighlight()
    {
        GuideUI.Instance.ShowGuide(EGuideType.Warning_Yellow, this.transform, Vector3.up * 3);
    }

    void StartWash()
    {
        OnMouseOut();
        
        Vector3 startPos = (transform.position + assemblePosOffset);
        Vector3 startRot = assembleRotOffset;

        player.StartHandling(startPos, startRot);
        
        waterFX.SetActive(true);
        clearFX.SetActive(false);
        processHandler.StartProcess(sinkDuration, (isSuccess) =>
        {
            curDirtyDishes = 0;
            
            ResetObject();
            clearFX.SetActive(true);
            
            player.FinishHandling();
            
            CurrencyManager.Instance.RewardCurrency(ECurrencyType.Star, 1);
            EventManager.GetEvent<int>(EGameEvent.OnUpdateReview).Invoke(1);
            
            SoundManager.Instance.PlaySound("SFX_Stab", 1, 2);
        });
        
        SoundManager.Instance.PlaySound("SFX_WashDishes", 1, 2);
        
        GuideUI.Instance.HideGuide(EGuideType.Warning_Yellow);
        EventManager.GetEvent<Transform>(EGameEvent.OnHoverFocus).Invoke(null);
    }

    void AddDirtyDish()
    {
        if (curDirtyDishes < maxDishCount)
        {
            curDirtyDishes += 1;
            
            UpdateDishes();
        }
    }

    void UpdateDishes()
    {
        globalState.isSinkDishFull = (curDirtyDishes >= maxDishCount);
        
        sinkWater.SetActive(curDirtyDishes > 0);

        for (int i = 0; i < maxDishCount; i++)
        {
            dirtyDishes[i].SetActive(i < curDirtyDishes);
        }
    }
    
    public override void OnMouseHover()
    {
        foreach(var interior in interiorList)
        {
            interior.SetActive(true);
        }
    }
    
    public override void OnMouseOut()
    {
        foreach(var interior in interiorList)
        {
            interior.SetActive(false);
        }
    }

    public override void OnSelect(Player player)
    {
        if (VectorExtensions.IsNearDistance(transform.position, player.transform.position, targetDistance))
        {
            if (player.IsHolding() == false && processHandler.IsProcessing() == false && curDirtyDishes > 0)
            {
                StartWash();
            }
        }
    }

    public override void ResetObject()
    {
        maxDishCount = dirtyDishes.Length;
        curDirtyDishes = 0;
        
        UpdateDishes();
        
        waterFX.SetActive(false);
        
        GuideUI.Instance.HideGuide(EGuideType.Warning_Yellow);
    }
}
