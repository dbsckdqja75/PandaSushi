using System.Collections;
using UnityEngine;

public class HijackCustomer : EventCustomer
{
    [SerializeField] FakeShadow fakeShadow;
    [SerializeField] GameObject ufoPrefab;
    
    GameObject ufo;

    protected override void OnDisable()
    {
        if (ufo != null)
        {
            ufo.SetActive(false);
        }
        
        base.OnDisable();
    }

    public override void OnServe(bool isPerfect)
    {
        orderInfo = null;
        
        if (isPerfect)
        {
            orderManager.ShowFaceFX(1, transform.position + (Vector3.up * 4)); // 행복
            
            EventManager.GetEvent(EGameEvent.OnFinishedOrder).Invoke();
            EventManager.GetEvent<Vector3>(EGameEvent.OnCustomerPaid).Invoke(transform.position + (Vector3.up * 3));
            
            eventLogic = EventMotion().Start(this);
        }
        else
        {
            orderManager.ShowFaceFX(2, transform.position + (Vector3.up * 4)); // 실망
            
            eventLogic = EventMotion(false).Start(this);
        }

        ReleaseIndicator();
    }

    IEnumerator EventMotion(bool onPlayEatMotion = true)
    {
        if (onPlayEatMotion)
        {
            motion.OnEat();

            if (RandomExtensions.RandomBool()) // NOTE: 다 먹고나서 또는 먹는 도중에 모션 발생
            {
                yield return new WaitForSeconds(Random.Range(0.1f, GetMealTime()));
            }
        }
        
        if (targetTable != null)
        {
            targetTable.UnseatCustomer();
        }
        
        SoundManager.Instance.PlaySound("SFX_UFO");
        
        ufo = ObjectPool.Instance.Spawn(ufoPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        
        motion.OnHijack();
        fakeShadow.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(8f);
        ForceDestroy();
        yield break;
    }
}
