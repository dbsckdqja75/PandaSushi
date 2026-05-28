using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MiniGamePrepQTE : MiniGame
{
    [SerializeField] GameObject clearFx;
    
    [Space(10)]
    [SerializeField] RectTransform knife;
    [SerializeField] RectTransform focusPoint;
    [SerializeField] RectTransform focusArea;

    [Space(10)]
    [SerializeField] KeyCode[] keycodes;
    [SerializeField] TMP_Text[] keycodeTexts;
    [SerializeField] GameObject[] gamepadGuides;
    
    [Space(10)]
    [SerializeField] RectTransform[] pivots;
    [SerializeField] CanvasGroup[] groups;
    
    List<KeyCode> currentKeycodes = new();
    int currentTargetKeyIdx = 0;
    
    public override void Init(Action<bool> finishCallback)
    {
        base.Init(finishCallback);
        
        knife.gameObject.SetActive(false);
        
        currentKeycodes.Clear();

        var shuffledTargets = new int[4] { 0, 1, 2, 3 }.OrderBy(x => Guid.NewGuid()).ToList(); // W A S D
        for(int i = 0; i < 4; i++)
        {
            currentKeycodes.Add(keycodes[shuffledTargets[i]]);
            keycodeTexts[i].text = currentKeycodes[i].ToString();
            gamepadGuides[shuffledTargets[i]].transform.position = keycodeTexts[i].transform.position;
        }

        currentTargetKeyIdx = 0;

        focusPoint.position = pivots[currentTargetKeyIdx].position;
        focusArea.position = pivots[currentTargetKeyIdx].position;

        focusPoint.gameObject.SetActive(true);
        focusArea.gameObject.SetActive(true);
        clearFx.gameObject.SetActive(false);

        foreach(var group in groups)
        {
            group.alpha = 0.5f;
        }

        groups[currentTargetKeyIdx].alpha = 1;
    }
    
    void OnEnable()
    {
        playerActions = new CustomPlayerActions();
        playerActions.Player.PadNorth.started += (i) => InputKey(KeyCode.W);
        playerActions.Player.PadWest.started += (i) => InputKey(KeyCode.A);
        playerActions.Player.PadEast.started += (i) => InputKey(KeyCode.D);
        playerActions.Player.PadSouth.started += (i) => InputKey(KeyCode.S);
        
        playerActions.Player.Enable();

        RefreshGuide();
        EventManager.GetEvent(EGameEvent.OnControlChange).Subscribe(RefreshGuide);
    }
    
    void OnDisable()
    {
        playerActions.Player.Disable();
        
        EventManager.GetEvent(EGameEvent.OnControlChange).Unsubscribe(RefreshGuide);
    }
    
    protected override void FinishGame(bool isSuccess)
    {
        if (isSuccess)
        {
            StageManager.Instance.StartBuff(EBuffType.FastPlayerMove);
        }
        
        base.FinishGame(isSuccess);
    }
    
    protected override void Update()
    {
        base.Update();
        
        if(currentTargetKeyIdx < currentKeycodes.Count)
        {
            if(Input.GetKeyDown(currentKeycodes[currentTargetKeyIdx]))
            {
                InputKey(currentKeycodes[currentTargetKeyIdx]);
            }
        }
    }

    void RefreshGuide()
    {
        bool isOn = InputDetector.Instance.IsInputGamepad();
        foreach (var guide in gamepadGuides)
        {
            guide.SetActive(isOn);
        }
        
        foreach (var text in keycodeTexts)
        {
            text.gameObject.SetActive(!isOn);
        }
    }

    void InputKey(KeyCode inputKey)
    {
        if (currentTargetKeyIdx < currentKeycodes.Count)
        {
            if (currentKeycodes[currentTargetKeyIdx] == inputKey)
            {
                NextKey();
            }
        }
    }
    
    void NextKey()
    {
        knife.gameObject.SetActive(false);
        knife.position = new Vector2(pivots[currentTargetKeyIdx].position.x, knife.position.y);
        knife.gameObject.SetActive(true);
        
        groups[currentTargetKeyIdx].alpha = 0.5f;

        currentTargetKeyIdx += 1;

        if(currentTargetKeyIdx < currentKeycodes.Count)
        {
            focusPoint.position = pivots[currentTargetKeyIdx].position;
            focusArea.position = pivots[currentTargetKeyIdx].position;

            groups[currentTargetKeyIdx].alpha = 1;
        }
        else if(currentTargetKeyIdx >= currentKeycodes.Count)
        {
            focusPoint.gameObject.SetActive(false);
            focusArea.gameObject.SetActive(false);
            
            OnInputted();
            
            FinishMotion(true).Start(this);
        }
        
        SoundManager.Instance.PlaySound("SFX_Chop", 5, 9);
    }
    
    IEnumerator FinishMotion(bool isSuccess)
    {
        SoundManager.Instance.PlaySound("SFX_Notice1");
        
        clearFx.SetActive(isSuccess);
        yield return new WaitForSeconds(0.75f);
        
        FinishGame(true);
        yield break;
    }
}
