using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

public class MiniGamePrepSpace : MiniGame
{
    [SerializeField] GameObject clearFx;
    [SerializeField] GameObject failFx;
    
    [Space(10)]
    [SerializeField] RectTransform knife;
    [SerializeField] Image fillBar;
    [SerializeField] RectTransform fillPoint;
    [SerializeField] RectTransform hitPivotPoint;

    [SerializeField] TMP_Text guideText;

    [Space(10), Range(0, 1f)]
    [SerializeField] float progress = 0;

    [Range(0, 1f)]
    [SerializeField] float startRange = 0.5f;
    
    [Range(0.5f, 1f)]
    [SerializeField] float endRange = 1f;

    [Space(10)]
    [Range(0.5f, 0.95f)]
    [SerializeField] float hitPivot = 0.75f;

    [Range(0, 1f)]
    [SerializeField] float hitPivotScale = 1f;

    [Space(10)]
    [SerializeField] float offsetX = 0;

    bool isPingPong = false;

    float pingPongTimer = 0;

    float minHitAreaWidth, maxHitAreaWidth;
    float mainHitAreaPoint;
    
    public override void Init(Action<bool> finishCallback)
    {
        base.Init(finishCallback);
        
        knife.gameObject.SetActive(false);

        hitPivot = Random.Range(startRange, endRange);
        hitPivotScale = Random.Range(startRange, endRange);

        clearFx.SetActive(false);
        failFx.SetActive(false);

        guideText.text = "[SPACE]";
        
        pingPongTimer = 0;
        isPingPong = true;
    }
    
    protected override void FinishGame(bool isSuccess)
    {
        if (isSuccess)
        {
            Debug.Log("Space 미니게임 성공");
            
            StageManager.Instance.StartBuff(EBuffType.FastPlayerMove);
        }
        
        SoundManager.Instance.PlaySound("SFX_Chop", 5, 9);
        
        base.FinishGame(isSuccess);
    }
    
    protected override void Update()
    {
        base.Update();
        
        if(isPingPong)
        {
            UpdatePingPong();
            UpdateInput();
        }
    }

    void UpdateInput()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            isPingPong = false;

            OnInputted();

            if(progress > (hitPivot - mainHitAreaPoint) && progress < (hitPivot + mainHitAreaPoint))
            {
                guideText.text = "PERFECT!";
                Debug.LogFormat("Perfect! {0}", progress);

                FinishMotion(true).Start(this);
                return;
            }

            guideText.text = "MISS!";
            Debug.LogFormat("MISS! {0}", progress);

            FinishMotion(false).Start(this);
        }
    }

    void UpdatePingPong()
    {
        pingPongTimer += Time.deltaTime;
        progress = PingPongEaseIn(pingPongTimer, 1f, 3f);
        
        Vector2 startPos = new Vector2(fillBar.rectTransform.anchoredPosition.x - Mathf.Abs(fillBar.rectTransform.rect.x) + Mathf.Abs(offsetX), fillBar.rectTransform.anchoredPosition.y);
        Vector2 endPos = new Vector2(fillBar.rectTransform.anchoredPosition.x + Mathf.Abs(fillBar.rectTransform.rect.x) - Mathf.Abs(offsetX), fillBar.rectTransform.anchoredPosition.y);

        fillPoint.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);
        fillBar.fillAmount = progress;
            
        minHitAreaWidth = Mathf.Abs(fillBar.rectTransform.rect.x) * 0.2f;
        maxHitAreaWidth = Mathf.Abs(fillBar.rectTransform.rect.x) * 0.5f;

        Vector2 hitPos = Vector2.Lerp(startPos, endPos, hitPivot);

        hitPivotPoint.anchoredPosition = hitPos;
        hitPivotPoint.sizeDelta = new Vector2(Mathf.Lerp(minHitAreaWidth, maxHitAreaWidth, hitPivotScale) , hitPivotPoint.sizeDelta.y);
        mainHitAreaPoint = (Mathf.Abs(hitPivotPoint.rect.x) / Mathf.Abs(fillBar.rectTransform.rect.x)) * 0.5f;
    }
    
    float PingPongEaseIn(float time, float timeScale, float easePower)
    {
        float t = Mathf.PingPong(time * timeScale, 1f);
        return Mathf.Clamp(Mathf.Pow(t, easePower), 0, 1);
    }

    IEnumerator FinishMotion(bool isSuccess)
    {
        knife.gameObject.SetActive(false);
        knife.position = new Vector2(fillPoint.transform.position.x, knife.position.y);
        knife.gameObject.SetActive(true);
        
        clearFx.SetActive(isSuccess);
        failFx.SetActive(!isSuccess);
        
        SoundManager.Instance.PlaySound("SFX_Chop", 5, 9);
        yield return new WaitForSeconds(0.75f);
        FinishGame(isSuccess);
        yield break;
    }
}
