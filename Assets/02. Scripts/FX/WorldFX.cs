using System.Collections;
using UnityEngine;

public class WorldFX : MonoBehaviour
{
    public void Hide(float delay, bool isDestroy = false)
    {
        this.StopAllCoroutines();
        DelayedHide(delay, isDestroy).Start(this);
    }
    
    public void Hide(bool isDestroy = false)
    {
        if (isDestroy)
        {
            Destroy(this.gameObject);
            return;
        }
        
        this.gameObject.SetActive(false);
    }
    
    public void UpdatePositionToUGUI(Vector3 pos)
    {
        transform.position = WorldToCanvsPoint(pos);
    }

    IEnumerator DelayedHide(float delay, bool isDestroy)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(delay);
        Hide(isDestroy);
        yield break;
    }

    public Vector3 WorldToCanvsPoint(Vector3 pos, float depth = 5)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = depth;

        return Camera.main.ScreenToWorldPoint(worldPos);
    }
}
