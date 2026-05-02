using Action = System.Action;
using UnityEngine;

public partial class StageManager : MonoSingleton<StageManager>
{
    [SerializeField] float prepareTime = 30;
    [SerializeField] float gameTime = 150;

    [Space(10)]
    [SerializeField] MiniGameLoader miniGameLoader;
    [SerializeField] PandaBuff pandaBuff;

    [Space(10)]
    [SerializeField] DisplayTimer prepareTimeDisplay;
    [SerializeField] DisplayTimer openTimeDisplay;

    [Space(10)]
    [SerializeField] GameObject miniRecipeBox;
    [SerializeField] GameObject earlyStartBox;
    [SerializeField] GameObject earlyFinishBox;
    [SerializeField] GameObject fridgePanel;

    [Space(20)]
    [SerializeField] EnvDoor[] doors;
    [SerializeField] DecoPoint[] decoPoints;

    [Space(10)]
    [SerializeField] LevelConfigData[] levelConfigs;
    [SerializeField] GlobalState globalState;

    Player player;
    StageTimer stageTimer;
    GuestSpawner guestSpawner;
    RiderSpawner riderSpawner;
    EventSpawner eventSpawner;
    
    CameraView camera;
    FridgeManager fridgeManager;
    CanvasManager canvasManager;
    NotificationUI notificationUI;
    RoundAnalytics roundAnalytics;
    
    void Awake()
    {
        player = FindAnyObjectByType<Player>();
        stageTimer = this.GetComponent<StageTimer>();
        guestSpawner = this.GetComponent<GuestSpawner>();
        riderSpawner = this.GetComponent<RiderSpawner>();
        eventSpawner = this.GetComponent<EventSpawner>();
        
        camera = FindAnyObjectByType<CameraView>();
        fridgeManager = FindAnyObjectByType<FridgeManager>();
        canvasManager = FindAnyObjectByType<CanvasManager>();
        notificationUI = FindAnyObjectByType<NotificationUI>();
        roundAnalytics = FindAnyObjectByType<RoundAnalytics>();
        
        LocalizationManager.Instance.ChangeTableReference("StageStringData_Table");
        
        SoundManager.Instance.PlaySound("SFX_NewOrder");
        
        EventManager.GetEvent(EGameEvent.OnGameSaved).Subscribe(() => notificationUI.ShowNotification(ENotificationType.SavedGame));
        EventManager.GetEvent(EGameEvent.OnSoldOutOrder).Subscribe(ActivateEarlyCloseBox);
        EventManager.OnGameStateChanged += HandleGameStateChange;
    }

    void OnDisable()
    {
        EventManager.GetEvent(EGameEvent.OnGameSaved).Clear();
        EventManager.GetEvent(EGameEvent.OnSoldOutOrder).Clear();
        EventManager.OnGameStateChanged -= HandleGameStateChange;
    }

    void Start()
    {
        EventManager.GameStatus = EGameState.RoundPrepare;
    }

    void Update()
    {
        // NOTE: DEBUG
        // if(Input.GetKey(KeyCode.Alpha4) && Time.timeScale != 0)
        // {
        //     Time.timeScale = 3;
        // }
        // else if(Time.timeScale != 0)
        // {
        //     Time.timeScale = 1;
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha0))
        // {
        //     CurrencyManager.Instance.RewardCurrency(ECurrencyType.Money, 900000);
        //     CurrencyManager.Instance.RewardCurrency(ECurrencyType.Star, 500);
        //     
        //     RefreshBonusState();
        //     RefreshUnlockLevelState();
        //     
        //     EventManager.GetEvent(EGameEvent.OnUpdateCurrency).Invoke();
        // }
        //

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!canvasManager.IsShowingPanel() && !miniGameLoader.IsPlaying() && !fridgePanel.activeSelf)
            {
                if (Time.timeScale != 0)
                {
                    EventManager.GameStatus = EGameState.Pause;
                }
                else
                {
                    EventManager.GameStatus = EGameState.Resume;
                }
            }
            else if(EventManager.GameStatus == EGameState.Pause)
            {
                EventManager.GameStatus = EGameState.Resume;
            }
        }
    }

    public void ChangeGameState(EGameState newState)
    {
        EventManager.GameStatus = newState;
    }
    
    public void OnClickEarlyStart()
    {
        ChangeGameState(EGameState.RoundStart);
    }

    public void OnClickEarlyFinish()
    {
        ChangeGameState(EGameState.RoundEnd);
    }

    public void OnClickRecipeBook()
    {
        Time.timeScale = 0;
        
        canvasManager.ShowPanel(EScreenState.RecipeBook, false);
    }

    public void CallDeliveryRider()
    {
        riderSpawner.CallRider();
    }
    
    public void StartBuff(EBuffType buffType)
    {
        pandaBuff.Grant(buffType);
    }

    public void StartMiniGame(EMiniGameType gameType, Action onSuccess, Action onFail)
    {
        player.OnSearching();
        miniGameLoader.Load(gameType, (isSuccess) =>
        {
            player.FinishSearching();
            
            if (isSuccess)
            {
                onSuccess?.Invoke();
            }
            else
            {
                onFail?.Invoke();
            }
        });
    }

    void ActivateEarlyCloseBox()
    {
        earlyFinishBox.SetActive(true);
        
        SoundManager.Instance.PlaySound("SFX_SoldOut");
    }

    public Transform GetPlayerTransform()
    {
        return player.transform;
    }

    public EnvDoor[] GetDoors()
    {
        return doors;
    }

    public DecoPoint[] GetDecoPoints()
    {
        return decoPoints;
    }
}
