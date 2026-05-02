using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerCustomer : NonSeatCustomer
{
    [SerializeField] FakeShadow fakeShadow;
    
    public override void Enter(EnvDoor door, List<Vector3> wayPoints)
    {
        fakeShadow.gameObject.SetActive(true);
        
        List<Vector3> points = new();
        points.Add(wayPoints[0]);
        points.Add(wayPoints[0] + (Vector3.forward * 3.3f));

        base.Enter(door, points);
    }
    
    public override void ForceDestroy()
    {
        if (exitDoor && exitDoor.gameObject.name == "Env_Door (LEFT)")
        {
            exitDoor.Unblock();

            EventManager.GetEvent(EGameEvent.OnKickedLeftDoor).Unsubscribe(OnKicked);
        }

        base.ForceDestroy();
    }

    protected override void OnHit()
    {
        base.OnHit();
        
        fakeShadow.gameObject.SetActive(false);
    }

    void OnKicked()
    {
        if (this.col.enabled)
        {
            this.col.enabled = false;
            fakeShadow.gameObject.SetActive(false);
            
            this.StopAllCoroutines();
            KickedMotion().Start(this);
        }
    }

    IEnumerator KickedMotion()
    {
        motion.OnKicked();
        yield return new WaitForSeconds(0.25f);
        transform.position = wayPoints[1];
        
        yield return new WaitForSeconds(1f);
        ForceDestroy();
        yield break;
    }
    
    protected override IEnumerator HitMotion()
    {
        exitDoor.Unblock();
        exitDoor.Open();

        yield return base.HitMotion();
        yield break;
    }

    protected override IEnumerator EventMotion()
    {
        exitDoor.Block();
        if (exitDoor.gameObject.name == "Env_Door (LEFT)")
        {
            EventManager.GetEvent(EGameEvent.OnKickedLeftDoor).Subscribe(OnKicked);
        }
        
        Vector3 lookAtPos = wayPoints[1];
        lookAtPos.y = transform.position.y;
        transform.LookAt(lookAtPos);
        
        col.enabled = true;
        motion.OnBlocking();
        
        yield break;
    }
}
