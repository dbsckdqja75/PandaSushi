using System;
using UnityEngine;
using TMPro;

public class MiniGame : MonoBehaviour
{
    protected bool isPlaying = false;
    
    [SerializeField] protected float gameTime = 10f;
    protected float timer = 0;

    [Space(10)]
    [SerializeField] TMP_Text timerText;
    
    Action<bool> onFinish;

    public virtual void Init(Action<bool> finishCallback)
    {
        this.onFinish = finishCallback;

        timer = gameTime;
        isPlaying = true;
    }

    protected virtual void Update()
    {
        if (isPlaying)
        {
            if (timer > 0)
            {
                UpdateTimerDisplay(string.Format("{0}", Mathf.Clamp((int)timer, 0, gameTime)));

                timer -= Time.deltaTime;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ForceCancel();
                }
            }
            else
            {
                ForceCancel();
            }
        }
    }

    public void ForceCancel()
    {
        FinishGame(false);
    }

    protected void UpdateTimerDisplay(string context)
    {
        timerText.text = context;
    }

    protected void OnInputted()
    {
        isPlaying = false;
        UpdateTimerDisplay("FINISHED");
    }

    protected virtual void FinishGame(bool isSuccess)
    {
        onFinish?.Invoke(isSuccess);

        Destroy(this.gameObject);
    }
}