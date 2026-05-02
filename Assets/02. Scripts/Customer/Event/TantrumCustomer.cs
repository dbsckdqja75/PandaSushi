using System.Collections;
using UnityEngine;

public class TantrumCustomer : HitableEventCustomer
{
    public override void OnServe(bool isPerfect)
    {
        orderInfo = null;
        
        if (isPerfect)
        {
            EventManager.GetEvent(EGameEvent.OnFinishedOrder).Invoke();
            EventManager.GetEvent<Vector3>(EGameEvent.OnCustomerPaid).Invoke(transform.position + (Vector3.up * 3));
        }

        eventLogic = EventMotion(isPerfect).Start(this);
        
        ReleaseIndicator();
    }

    protected override IEnumerator HitMotion()
    {
        yield return base.HitMotion();

        Leave(false);
        yield break;
    }
    
    IEnumerator EventMotion(bool onPlayEatMotion)
    {
        if (onPlayEatMotion && RandomExtensions.RandomBool()) // NOTE: 다 먹고나서 또는 먹는 도중에 이벤트 발생
        {
            motion.OnEat();
            yield return new WaitForSeconds(Random.Range(1f, GetMealTime()));
        }
        
        col.enabled = true;
        motion.OnTantrum();
        
        skinHandler.ChangeSkinColor(eventSkinColor, true);
        
        orderManager.ShowFaceFX(0, transform.position + (Vector3.up * 4));
        
        SoundManager.Instance.PlaySound("SFX_Angry", 1, 2);
        
        yield break;
    }
}
