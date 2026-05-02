using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefCustomer : NonSeatCustomer
{
    [SerializeField] float stealTime = 6f;
    [SerializeField] FakeShadow fakeShadow;
    [SerializeField] GameObject bag;

    ProcessHandler processHandler;
    
    Fridge fridge;
    FridgeManager fridgeManager;
    
    protected override void OnEnable()
    {
        base.OnEnable();

        processHandler = this.GetComponent<ProcessHandler>();
        fridgeManager = FindAnyObjectByType<FridgeManager>();
        fakeShadow.gameObject.SetActive(true);
    }

    public override void Enter(EnvDoor door, List<Vector3> wayPoints)
    {
        fridge = FindAnyObjectByType<Fridge>();

        List<Vector3> points = new();
        points.Add(wayPoints[0]);
        points.Add(wayPoints[0] + (Vector3.forward * 2));
        points.Add(fridge.transform.position + new Vector3(-2, 0, 2));
        points[points.Count - 1] = new Vector3(points[2].x, wayPoints[0].y, points[2].z);

        base.Enter(door, points);
    }
    
    public override void ForceStop()
    {
        base.ForceStop();
        
        processHandler.StopProcess();
    }

    void OnExpireStealTime(bool isTimeOver)
    {
        this.col.enabled = false;
        this.StopAllCoroutines();
        processHandler.StopProcess();
        
        if (isTimeOver)
        {
            fridgeManager.RandomTake();
            
            OnPenalty(10);

            extraSpeed = 6;
            LeaveMotion(wayPoints).Start(this);
        }
        else
        {
            fridgeManager.GetStorage().Add(IngredientID.Engery_Drink);

            fakeShadow.gameObject.SetActive(false);
            HitMotion().Start(this);
        }
    }

    protected override void OnHit()
    {
        base.OnHit();
        
        OnExpireStealTime(false);
    }

    protected override IEnumerator EventMotion()
    {
        this.col.enabled = true;

        Vector3 lookAtPos = fridge.transform.position;
        lookAtPos.y = transform.position.y;
        transform.LookAt(lookAtPos);
        
        motion.OnSteal();

        processHandler.StartProcess(stealTime, OnExpireStealTime);
        yield break;
    }
}
