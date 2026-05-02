using System.Collections;
using UnityEngine;

public class PukeCustomer : HitableEventCustomer
{
    [SerializeField] float pukeTime = 6f;
    [SerializeField] GameObject pukeDirtyPrefab;

    ProcessHandler processHandler;

    protected override void Awake()
    {
        base.Awake();
        
        processHandler = this.GetComponent<ProcessHandler>();
    }

    public override void ForceStop()
    {
        base.ForceStop();
        
        processHandler.StopProcess();
    }

    protected override void OnExpireMealTime()
    {
        orderInfo = null;

        col.enabled = true;
        motion.OnPuke();
        
        processHandler.StartProcess(pukeTime, OnExpirePukeTime);
        skinHandler.ChangeSkinColor(eventSkinColor, true);
        
        orderManager.ShowFaceFX(3, transform.position + (Vector3.up * 4)); // 구토
        
        SoundManager.Instance.PlaySound("SFX_Puke");
    }

    void OnExpirePukeTime(bool isTimeOver)
    {
        this.StopAllCoroutines();
        
        processHandler.StopProcess();
        skinHandler.RestoreSkin();

        if (isTimeOver)
        {
            extraSpeed = 3;
            
            targetTable.SetExtraDirty(pukeDirtyPrefab);

            OnPenalty(Random.Range(3, 5 + 1));
            
            SoundManager.Instance.PlaySound("SFX_Mosquito");
            
            Leave(false);
        }
        else
        {
            CurrencyManager.Instance.RewardCurrency(ECurrencyType.Star, 1);
            EventManager.GetEvent<int>(EGameEvent.OnUpdateReview).Invoke(1);
            
            eventLogic = HitMotion().Start(this);
        }
    }

    protected override void OnHit()
    {
        OnExpirePukeTime(false);
    }

    IEnumerator HitMotion()
    {
        yield return base.HitMotion();

        var table = targetTable;
        Leave(false);
        table.WipePlate();
        yield break;
    }
}
