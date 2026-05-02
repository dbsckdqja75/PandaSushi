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
    [SerializeField] RectTransform[] pivots;
    [SerializeField] CanvasGroup[] groups;
    
    List<KeyCode> currentKeycodes = new();
    int currentTargetKeyIdx = 0;
    
    public override void Init(Action<bool> finishCallback)
    {
        base.Init(finishCallback);
        
        knife.gameObject.SetActive(false);
        
        currentKeycodes.Clear();
        var shuffledKeyCodeList = keycodes.OrderBy(x => Guid.NewGuid()).ToList();
        for(int i = 0; i < 4; i++)
        {
            currentKeycodes.Add(shuffledKeyCodeList[i]);
            keycodeTexts[i].text = currentKeycodes[i].ToString();
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
