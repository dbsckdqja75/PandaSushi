using System.Collections;
using UnityEngine;

public class HitableEventCustomer : EventCustomer
{
    [SerializeField] protected float leaveSpeed = 3f;
    [SerializeField] protected Color eventSkinColor = Color.white;
    [SerializeField] protected GameObject hitFxPrefab;
    
    protected BoxCollider col;
    protected PandaSkinHandler skinHandler;

    protected override void Awake()
    {
        base.Awake();
        
        col = this.GetComponent<BoxCollider>();
        skinHandler = this.GetComponent<PandaSkinHandler>();
    }

    protected override void OnDisable()
    {
        skinHandler.RestoreSkin();
        
        base.OnDisable();
    }
    
    protected void OnReward(int starValue)
    {
        CurrencyManager.Instance.RewardCurrency(ECurrencyType.Star, starValue);
        EventManager.GetEvent<int>(EGameEvent.OnUpdateReview).Invoke(starValue);
    }

    protected void OnPenalty(int penaltyValue)
    {
        CurrencyManager.Instance.Purchase(ECurrencyType.Star, penaltyValue);
        EventManager.GetEvent<int>(EGameEvent.OnUpdateReview).Invoke(-penaltyValue);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            this.col.enabled = false;
            
            OnHit();
            OnReward(1);
        }
    }
    
    protected virtual void OnHit()
    {
        skinHandler.RestoreSkin();
        eventLogic = HitMotion().Start(this);
    }

    protected virtual IEnumerator HitMotion()
    {
        extraSpeed = leaveSpeed;
        
        motion.OnHit(true);
        
        skinHandler.ChangeSkinColor(Color.white, false, 0.8f);
        skinHandler.RestoreSkin(true);
        
        ObjectPool.Instance.SpawnFX(hitFxPrefab, transform.position).Hide(1f);
        
        SoundManager.Instance.PlaySound("SFX_Hit", 1, 2);
        
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => motion.IsPlaying("Sit Hit") == false);
        yield return new WaitForSeconds(0.5f);
        yield break;
    }
}