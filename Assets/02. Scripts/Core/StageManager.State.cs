using UnityEngine;
using UnityEngine.SceneManagement;

public partial class StageManager
{
    EGameState prevState;
    
    void HandleGameStateChange(EGameState newState)
    {
        switch (newState)
        {
            case EGameState.RoundReady:
                if (prevState != EGameState.RoundReady)
                {
                    HandleRoundReady();
                }
                break;
            case EGameState.RoundStart:
                if (prevState != EGameState.RoundStart)
                {
                    HandleRoundStart();
                }
                break;
            case EGameState.RoundEnd:
                if (prevState != EGameState.RoundEnd)
                {
                    HandleRoundEnd();
                }
                break;
            case EGameState.RoundPrepare:
                HandleRoundPrepare();
                break;
            case EGameState.Pause:
                HandlePause(true);
                return;
            case EGameState.Resume:
                HandlePause(false);
                return;
            case EGameState.RoundRestart:
                HandleRestart();
                return;
            case EGameState.Leave:
                HandleLeave();
                return;
            case EGameState.Quit:
                HandleQuit();
                return;
            case EGameState.Tutorial:
                HandleTutorial();
                return;
        }

        prevState = EventManager.GameStatus;
    }

    void HandleRoundReady()
    {
        RefreshBonusState();
        RefreshUnlockLevelState();
        
        player.Unfreeze();
        player.SaveStamina();
        camera.UpdateTarget(player.transform);
        camera.UpdateOffset(new Vector3(0, 15.23f, 14), new Vector3(45, 180, 0), 0);
      
        guestSpawner.ResetSpawnTimer();
        eventSpawner.ResetSpawnTimer();
        riderSpawner.ResetOrderTimer();
        miniRecipeBox.SetActive(true);
        earlyStartBox.SetActive(true);
        earlyFinishBox.SetActive(false);

        openTimeDisplay.Hide();
        stageTimer.InitializeTimer(prepareTime, prepareTimeDisplay,
            () => { EventManager.GameStatus = EGameState.RoundStart; });

        fridgeManager.SaveStorageInfo();
        
        CurrencyManager.Instance.SaveCurrencyData();
        SoundManager.Instance.PlayMusic("Ready_WasabiMorning");
        SoundManager.Instance.PlaySound("SFX_SoldOut");
        
        canvasManager.ShowPanel(EScreenState.Ready);
        EventManager.GetEvent(EGameEvent.OnGameSaved).Invoke();
        
        Debug.Log("EGameState.RoundReady");
    }
    
    void HandleRoundStart()
    {
        player.Unfreeze();
        camera.UpdateTarget(player.transform);
        camera.UpdateOffset(new Vector3(0, 15.23f, 14), new Vector3(45, 180, 0), 0);
        
        guestSpawner.StartSpawnTimer();
        eventSpawner.StartSpawnTimer();
        riderSpawner.StartOrderTimer();
        miniRecipeBox.SetActive(false);
        earlyStartBox.SetActive(false);
        earlyFinishBox.SetActive(false);
        
        prepareTimeDisplay.Hide();
        stageTimer.InitializeTimer(gameTime, openTimeDisplay,
            () => { EventManager.GameStatus = EGameState.RoundEnd; });
        
        SoundManager.Instance.PlayMusic(RandomExtensions.RandomBool() ? "Play_LoopTheme" : "Play_PandaKitchenSprint");
        
        canvasManager.ShowPanel(EScreenState.Playing);
        
        Debug.Log("EGameState.RoundStart (START)");
    }
    
    void HandleRoundEnd()
    {
        RefreshUnlockLevelState();

        player.Freeze();
        player.SaveStamina();
        pandaBuff.ForceClear();
        miniGameLoader.ForceReset();
        
        stageTimer.ResetTimer();
        guestSpawner.ResetSpawner();
        eventSpawner.ResetSpawnTimer();
        riderSpawner.ResetSpawner();
        earlyStartBox.SetActive(false);
        earlyFinishBox.SetActive(false);
        
        foreach (var guest in FindObjectsByType<NonSeatCustomer>(FindObjectsSortMode.None))
        {
            guest.ForceStop();
        }
        
        foreach (var guest in FindObjectsByType<Customer>(FindObjectsSortMode.None))
        {
            guest.ForceStop();
        }
        
        fridgeManager.CleanOutKitchen();
        fridgeManager.SaveStorageInfo();

        CurrencyManager.Instance.SaveCurrencyData();
        SoundManager.Instance.StopMusic();
        
        canvasManager.ShowPanel(EScreenState.Result);
        EventManager.GetEvent(EGameEvent.OnGameSaved).Invoke();
        
        Debug.Log("EGameState.RoundEnd");
    }
    
    void HandleRoundPrepare()
    {
        player.Freeze();
        camera.UpdateTarget(player.transform);
        camera.UpdateOffset(new Vector3(2.38f, 4.3f, 7), new Vector3(20, 198, 0), 7, false);

        foreach (var guest in FindObjectsByType<NonSeatCustomer>(FindObjectsSortMode.None))
        {
            guest.ForceDestroy();
        }
        
        foreach (var guest in FindObjectsByType<Customer>(FindObjectsSortMode.None))
        {
            guest.ForceDestroy();
        }
        
        foreach (var rider in FindObjectsByType<Rider>(FindObjectsSortMode.None))
        {
            rider.ForceDestroy();
        }
        
        foreach (var ticket in FindObjectsByType<DeliveryTicket>(FindObjectsSortMode.None))
        {
            ticket.MissOrder();
        }
        
        foreach (var obj in FindObjectsByType<InteractionObject>(FindObjectsSortMode.None))
        {
            obj.ResetObject();
        }
        
        foreach (var door in doors)
        {
            door.Close();
            door.ResetMoiton();
        }
        
        fridgeManager.LoadStorageInfo();
        fridgeManager.ForceClose();
        roundAnalytics.ForceReset();
        
        CurrencyManager.Instance.ReloadData();
        SoundManager.Instance.PlayMusic("Prepare_TakeRest");

        canvasManager.ShowPanel(EScreenState.Prepare);

        Debug.Log("EGameState.RoundPrepare");
    }

    void HandlePause(bool onPause)
    {
        Time.timeScale = onPause ? 0 : 1;
        canvasManager.ShowPause(onPause);

        if (onPause)
        {
            if (prevState == EGameState.RoundReady || prevState == EGameState.RoundStart)
            {
                stageTimer.PauseTimer();
            }
        }
        else
        {
            EventManager.GameStatus = prevState;

            if (prevState == EGameState.RoundReady || prevState == EGameState.RoundStart)
            {
                stageTimer.ResumeTimer();
            }
        }
    }

    void HandleRestart()
    {
        Time.timeScale = 1;
        
        pandaBuff.ForceClear();
        miniGameLoader.ForceReset();
        
        player.ReleaseIngredient();
        player.ResetStamina();
        guestSpawner.ResetSpawner();
        eventSpawner.ResetSpawnTimer();
        riderSpawner.ResetSpawner();
        
        foreach (var guest in FindObjectsByType<NonSeatCustomer>(FindObjectsSortMode.None))
        {
            guest.ForceDestroy();
        }
        
        foreach (var guest in FindObjectsByType<Customer>(FindObjectsSortMode.None))
        {
            guest.ForceDestroy();
        }
        
        foreach (var rider in FindObjectsByType<Rider>(FindObjectsSortMode.None))
        {
            rider.ForceDestroy();
        }
        
        foreach (var ticket in FindObjectsByType<DeliveryTicket>(FindObjectsSortMode.None))
        {
            ticket.MissOrder();
        }
        
        foreach (var obj in FindObjectsByType<InteractionObject>(FindObjectsSortMode.None))
        {
            obj.ResetObject();
        }
        
        fridgeManager.LoadStorageInfo();
        fridgeManager.ForceClose();
        roundAnalytics.ForceReset();
        
        CurrencyManager.Instance.ReloadData();
        
        canvasManager.ShowPause(false);

        prevState = EGameState.Resume;
        EventManager.GameStatus = EGameState.RoundReady;
    }

    void HandleLeave()
    {
        Time.timeScale = 1;
        
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    void HandleQuit()
    {
        #if UNITY_EDITOR
        Debug.LogWarning("[DEBUG] 게임 종료");
        return;
        #endif
        
        Application.Quit();
    }
    
    void HandleTutorial()
    {
        // TODO: 튜토리얼 게임 클래스로 StageManager와 별도로 임시 게임 진행 처리
    }

    void RefreshBonusState()
    {
        globalState.ResetBonus();
        
        int unlockLevel = PlayerPrefsManager.LoadSlotData("UnlockLevel", 1);
        LevelConfigData[] targets = new LevelConfigData[unlockLevel];
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = levelConfigs[(levelConfigs.Length - 1) - i];
        }
        
        globalState.SetBonusValues(targets);

        // NOTE: ReviewBonus (Deco) [25]
        DecoID[] decoTargets = { DecoID.Deco_LeftTable, DecoID.Deco_RightTable, DecoID.Deco_Wall, DecoID.Deco_Door };
        foreach (DecoID id in decoTargets)
        {
            if (PlayerPrefsManager.LoadSlotData(string.Format("{0}_DecoDesignIdx", DecoID.Deco_LeftTable.ToString()), -1) > -1)
            {
                globalState.reviewBonus += 0.25f; // 명성도 보너스 +25% (리뷰)
            }
        }
    }

    void RefreshUnlockLevelState()
    {
        int unlockLevel = PlayerPrefsManager.LoadSlotData("UnlockLevel", 1);
        int currentStars = (int)CurrencyManager.Instance.GetCurrency(ECurrencyType.Star);

        for (int i = 0; i < levelConfigs.Length; i++)
        {
            if (currentStars >= levelConfigs[i].GetTargetReviews())
            {
                PlayerPrefsManager.SaveSlotData("ReviewLevel", levelConfigs[i].GetLevel());
                break;
            }
        }

        int reviewLevel = PlayerPrefsManager.LoadSlotData("ReviewLevel", 1);
        if (unlockLevel < reviewLevel)
        {
            PlayerPrefsManager.SaveSlotData("UnlockLevel", reviewLevel);
        }
    }
}