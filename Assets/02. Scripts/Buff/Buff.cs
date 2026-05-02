using System.Collections;
using UnityEngine;

public class Buff : MonoBehaviour
{
    [SerializeField] protected float buffTime;
    [SerializeField] protected GameObject fxPrefab;
    [SerializeField] protected GlobalState globalState;
    
    protected float currentTime;
    protected BuffIcon buffIcon;

    public virtual void Init(BuffIcon newIcon)
    {
        buffIcon = newIcon;
        currentTime = buffTime;

        BuffLogic().Start(this);
        
        Transform fxPoint = StageManager.Instance.GetPlayerTransform();
        ObjectPool.Instance.SpawnFX(fxPrefab, fxPoint.position, fxPoint).Hide(1f, true);
    }
        
    public virtual void Finish()
    {
        this.StopAllCoroutines();
        buffIcon?.Hide();
            
        Destroy(this.gameObject);
    }

    public void Extend()
    {
        this.StopAllCoroutines();
        
        currentTime = buffTime;
        BuffLogic().Start(this);
    }
        
    protected IEnumerator BuffLogic()
    {
        while (currentTime > 0)
        {
            buffIcon.UpdateTimer(Mathf.Clamp((int)currentTime, 0, (int)Mathf.Ceil(currentTime)), currentTime/buffTime);
            
            currentTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
            
        Debug.LogWarning("버프 종료");
        
        buffIcon.UpdateTimer(0, 0);
        Finish();
        yield break;
    }
}