using UnityEngine;
using TMPro;

public class DisplayTimer : MonoBehaviour
{
    [SerializeField] string timerForamt = "{0}";
    [SerializeField] TMP_Text timerText;

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
    
    public virtual void UpdateTimerText(float time)
    {
        float timeToDisplay = Mathf.Max(0, time);
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        
        string formattedTime = $"{minutes:00}:{seconds:00}";
        timerText.text = string.Format(timerForamt, formattedTime);
    }
}
