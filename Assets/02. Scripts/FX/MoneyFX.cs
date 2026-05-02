using System;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine;

public class MoneyFX : WorldFX
{
    public void MoveTo(Transform target, Action finishCallback)
    {
        this.StopAllCoroutines();
        MoveMotion(target, finishCallback).Start(this);
    }
    
    IEnumerator MoveMotion(Transform endTarget, Action finishCallback)
    {
        Vector3 startPos = transform.position;
        Vector3 startRot = transform.eulerAngles;
        Vector3 endRot = new Vector3(90 + Random.Range(-25f, 25f), 0, Random.Range(-25f, 25f));

        float duration = 0.25f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.smoothDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easeT = Mathf.Pow(t, 2);

            Vector3 endPos = WorldToCanvsPoint(endTarget.position);
            transform.position = Vector3.Lerp(startPos, endPos, easeT);
            transform.eulerAngles = Vector3.Lerp(startRot, endRot, easeT);
            transform.localScale = Vector3.Lerp(Vector3.one, (Vector3.one * 0.5f), easeT);
            
            yield return new WaitForEndOfFrame();
        }
        
        finishCallback?.Invoke();
        Hide(true);
        
        yield break;
    }
}
