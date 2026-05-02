using System;
using UnityEngine;

public class StageTimer : MonoBehaviour
{
    bool isStarted;
    bool isPaused;
    
    float initalDuration;
    float remainingTime;

    DisplayTimer display;
    
    Action onTimerExpired;

    public int RemainingTime => Mathf.FloorToInt(remainingTime);
 
    void Update()
    {
        if (isStarted && !isPaused)
        {
            remainingTime -= Time.deltaTime;
            
            if (display != null)
            {
                display.UpdateTimerText(remainingTime);
            }
            
            if (RemainingTime <= 0)
            {
                ResetTimer();
                
                onTimerExpired?.Invoke();
            }
        }
    }

    public void InitializeTimer(float duration, DisplayTimer display, Action expiredCallback)
    {
        ResetTimer();
        
        initalDuration = duration;
        remainingTime = duration;

        onTimerExpired = expiredCallback;
        
        isStarted = true;
        isPaused = false;
        
        if (display != null)
        {
            this.display = display;
            display.UpdateTimerText(remainingTime);
            
            this.display.gameObject.SetActive(true);
        }
    }

    public void ResumeTimer()
    {
        isPaused = false;
    }

    public void PauseTimer()
    {
        isPaused = true;
    }

    public void ResetTimer()
    {
        isStarted = false;
        isPaused = false;

        if (display != null)
        {
            display.gameObject.SetActive(false);
        }
    }
}
