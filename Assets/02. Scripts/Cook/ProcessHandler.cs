using System;
using System.Collections;
using UnityEngine;

public class ProcessHandler : MonoBehaviour
{
    bool isProcessing = false;
    float resumeTime = 0;
    
    [SerializeField] IndicatorType indicatorType = IndicatorType.CookProgress;
    [SerializeField] float indicatorHeight = 0;
    IndicatorElement indicator;

    public void StartProcess(float processTime, Action<bool> finishCallback, bool canPause = false)
    {
        ResetIndicator();
        indicator = IndicatorUI.Instance.ShowIndicator(indicatorType, this.transform, new Vector3(0, indicatorHeight, 0));

        Process(processTime, finishCallback, canPause).Start(this);
        
        Debug.Log("조리/요리/이벤트 프로세스 시작");
    }

    public void StopProcess()
    {
        this.StopAllCoroutines();
        ResetIndicator();
        
        isProcessing = false;
        
        Debug.Log("조리/요리/이벤트 프로세스 중단");
    }

    void ResetIndicator()
    {
        if (indicator != null)
        {
            indicator.Hide();
            indicator = null;
        }
    }

    public bool IsProcessing()
    {
        return isProcessing;
    }

    IEnumerator Process(float startTime, Action<bool> finishCallback, bool canPause)
    {
        isProcessing = true;

        float currentTimer = (resumeTime > 0) ? resumeTime : startTime;
        while (currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
            if (indicator != null)
            {
                indicator.UpdateFill(currentTimer / startTime);
            }

            if (canPause)
            {
                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                {
                    resumeTime = currentTimer;
                    finishCallback?.Invoke(false);
                    yield break;
                }
            }

            yield return new WaitForEndOfFrame();
        }

        isProcessing = false;
        resumeTime = 0;
        ResetIndicator();
                
        finishCallback?.Invoke(true);
        yield break;
    }
}
