using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BboyCustomer : NonSeatCustomer
{
    [SerializeField] FakeShadow fakeShadow;
    [SerializeField] Spinner spinner;

    List<Vector3> loopPoints = new();

    protected override void OnEnable()
    {
        base.OnEnable();

        spinner.enabled = false;
        fakeShadow.gameObject.SetActive(true);
    }

    public override void Enter(EnvDoor door, List<Vector3> wayPoints)
    {
        loopPoints = wayPoints;
        loopPoints[0] += (Vector3.forward * 3.3f);
        loopPoints[loopPoints.Count - 1] += (Vector3.forward * 3.3f);
        
        List<Vector3> points = new();
        points.Add(wayPoints[0]);
        points.Add(wayPoints[0] + (Vector3.forward * 3.3f));

        base.Enter(door, points);
    }
    
    public override void ForceStop()
    {
        base.ForceStop();
        
        spinner.enabled = false;
    }

    protected override void OnHit()
    {
        skinHandler.RestoreSkin();
        fakeShadow.gameObject.SetActive(false);
        
        this.StopAllCoroutines();
        HitMotion().Start(this);
        
        OnReward(5);
    }

    IEnumerator HitMotion()
    {
        spinner.enabled = false;

        yield return base.HitMotion();
        yield break;
    }
    
    protected override IEnumerator EventMotion()
    {
        this.col.enabled = true;
        spinner.enabled = true;
        
        motion.OnBboying();
        
        extraSpeed = 9f;
        float moveSpeed = (defaultSpeed + extraSpeed);

        while (this.col.enabled)
        {
            for (int i = 1; i < loopPoints.Count; i++)
            {
                while (Vector3.Distance(transform.position, loopPoints[i]) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, loopPoints[i], moveSpeed * Time.deltaTime);
                    yield return new WaitForEndOfFrame();
                }

                transform.position = loopPoints[i];
                yield return new WaitForEndOfFrame();
            }
            
            loopPoints.Reverse();
            yield return new WaitForEndOfFrame();

            OnPenalty(1);
        }

        yield break;
    }
}
