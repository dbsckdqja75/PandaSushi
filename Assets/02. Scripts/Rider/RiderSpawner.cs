using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RiderSpawner : MonoBehaviour
{
    bool isStarted;
    bool isWaiting = false;
    
    float orderTimer;
    [SerializeField] float orderInterval = 40;

    [Space(10)]
    [SerializeField] GameObject orderPrefab;
    [SerializeField] GameObject riderPrefab;
    
    [Space(10)]
    [SerializeField] PickupZone pickUpZone;
    [SerializeField] EnvDoor entryDoor;
    [SerializeField] GlobalState globalState;
    
    [Space(10)]
    [SerializeField] List<Transform> wayPoints = new();

    OrderManager orderManager;

    void Awake()
    {
        orderManager = GameObject.FindAnyObjectByType<OrderManager>();
    }

    void OnEnable()
    {
        EventManager.GetEvent(EGameEvent.OnSoldOutOrder).Subscribe(ResetOrderTimer);
    }

    void OnDisable()
    {
        EventManager.GetEvent(EGameEvent.OnSoldOutOrder).Unsubscribe(ResetOrderTimer);
    }

    void Update()
    {
        if (isStarted)
        {
            orderTimer -= Time.deltaTime;
            
            if (orderTimer <= 0)
            {
                OrderInfo orderInfo = orderManager.AddDeliveryOrder();
                if (orderInfo != null)
                {
                    DeliveryTicket ticket = Instantiate(orderPrefab).GetComponent<DeliveryTicket>();
                    ticket.Init(orderInfo, orderManager);
                    
                    pickUpZone.OnAddOrder(ticket);
                }
                
                orderTimer = orderInterval;
            }
        }
    }
    
    public void StartOrderTimer()
    {
        orderTimer = orderInterval * (2f - globalState.deliveryTimeBonus);
        isStarted = true;
    }
    
    public void ResetOrderTimer()
    {
        this.StopAllCoroutines();
        
        orderTimer = orderInterval;
        isStarted = false;
        isWaiting = false;
    }

    public void ResetSpawner()
    {
        ResetOrderTimer();
    }

    public void CallRider()
    {
        if (isWaiting == false)
        {
            this.StopAllCoroutines();
            Spawn().Start(this);
        }
    }

    IEnumerator Spawn()
    {
        isWaiting = true;
        
        // NOTE: 기사가 가게로 오는 속도도 함께 이속 버프 적용
        float turnaroundTime = (Random.Range(2.0f, 4.0f) / globalState.riderSpeedMultiple);
        yield return new WaitForSeconds(turnaroundTime);
        
        Rider rider = ObjectPool.Instance.Spawn(riderPrefab, wayPoints[0].position, Quaternion.identity).GetComponent<Rider>();
        rider.Enter(pickUpZone, entryDoor, GenerateWayPoints());
        
        isWaiting = false;
        
        yield break;
    }
    
    public List<Vector3> GenerateWayPoints()
    {
        List<Vector3> points = new();
        foreach (Transform wayPoint in wayPoints)
        {
            points.Add(wayPoint.position);
        }

        return points;
    }
}
