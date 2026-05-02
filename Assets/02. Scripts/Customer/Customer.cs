using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] float defaultSpeed = 6f;
    [SerializeField] float orderTime = 60;
    [SerializeField] float mealTime = 10;
    [SerializeField] Vector3 orderSheetOffset;
    
    [SerializeField] GlobalState globalState;

    protected OrderInfo orderInfo;
    OrderSheet orderSheet;
    OrderTicket orderTicket;

    protected CustomerMotion motion;
    protected ServeTable targetTable;
    protected OrderManager orderManager;

    EnvDoor exitDoor;
    List<Vector3> wayPoints = new();

    float remainTimer = 0;
    protected float extraSpeed = 0f;
    protected int extraOrderCount = 0;

    protected virtual void Awake()
    {
        motion = this.GetComponent<CustomerMotion>();
        orderManager = GameObject.FindAnyObjectByType<OrderManager>();
    }

    protected virtual void OnDisable()
    {
        extraSpeed = 0;
        extraOrderCount = 0;

        orderInfo = null;
        orderSheet = null;
        orderTicket = null;
        targetTable = null;
    }

    void Update()
    {
        if (targetTable != null)
        {
            if (orderSheet != null && orderTicket != null)
            {
                UpdateOrderTime();
                return;
            }

            if (orderInfo != null)
            {
                UpdateMealTime();
            }
        }
    }

    void UpdateOrderTime()
    {
        if (remainTimer > 0)
        {
            remainTimer -= Time.deltaTime;
                
            orderSheet.UpdateFill(remainTimer / orderTime);
            orderTicket.UpdateTimerText(remainTimer);
        }
        else
        {
            extraOrderCount = 0;
            
            Leave(true);
                
            ReleaseIndicator();
            
            orderManager.TimeOutOrder(orderInfo.number);
            orderManager.ShowFaceFX(0, transform.position + (Vector3.up * 4)); // 화남
        }
    }

    void UpdateMealTime()
    {
        if (remainTimer > 0)
        {
            remainTimer -= Time.deltaTime;
        }
        else
        {
            if (extraOrderCount > 0) // NOTE : 추가 주문
            {
                extraOrderCount -= 1;
                motion.OnStaySit();
                
                orderInfo = orderManager.AddExtraOrder(this.transform, orderSheetOffset);
                if (orderInfo != null)
                {
                    RefreshOrderInfo();
                    
                    targetTable.ForceDirty();
                    return;
                }
            }

            OnExpireMealTime();
        }
    }

    protected virtual void OnExpireMealTime()
    {
        Leave(false);
    }

    void RefreshOrderInfo()
    {
        orderSheet = (OrderSheet)orderInfo.orderSheet;
        orderTicket = orderInfo.orderTicket;
        
        remainTimer = orderTime;
        orderSheet.UpdateFill(1f);
        orderSheet.UpdateInfo(orderInfo.number);
        orderTicket.UpdateInfo(orderInfo.dish, orderInfo.number, false);
        orderTicket.UpdateTimerText(remainTimer);
    }

    void Seat()
    {
        if (targetTable.SeatCustomer(this))
        {
            orderInfo = orderManager.AddGeneralOrder(this.transform, orderSheetOffset);
            if (orderInfo != null)
            {
                extraOrderCount = Random.Range(0, 2+1);
                RefreshOrderInfo();
                
                motion.OnSit();
            
                EventManager.GetEvent(EGameEvent.OnSeatedGuest).Invoke();
            }
            else
            {
                motion.OnSit();
                
                Leave(true);
            }
        }
        else
        {
            ForceDestroy();
        }
    }

    public void Enter(ServeTable targetTable, EnvDoor door, List<Vector3> wayPoints)
    {
        if (targetTable != null)
        {
            this.targetTable = targetTable;
            this.exitDoor = door;
            
            EntranceMotion(wayPoints).Start(this);
        }
    }

    public virtual void OnServe(bool isPerfect)
    {
        if (isPerfect)
        {
            remainTimer = GetMealTime();
            
            motion.OnEat();

            orderManager.ShowFaceFX(1, transform.position + (Vector3.up * 4)); // 행복

            if (RandomExtensions.RandomBool())
            {
                SoundManager.Instance.PlaySound("SFX_Meal1");
            }
            
            EventManager.GetEvent(EGameEvent.OnFinishedOrder).Invoke();
            EventManager.GetEvent<Vector3>(EGameEvent.OnCustomerPaid).Invoke(transform.position + (Vector3.up * 3));
        }
        else
        {
            orderManager.ShowFaceFX(2, transform.position + (Vector3.up * 4)); // 실망
            
            Leave(true);
        }
        
        ReleaseIndicator();
    }

    public void Leave(bool playFailMotion)
    {
        if (playFailMotion == false)
        {
            targetTable.ForceDirty();
        }
        
        Unseat();
        
        LeaveMotion(playFailMotion, wayPoints).Start(this);
    }

    protected void ReleaseIndicator()
    {
        if (orderSheet != null)
        {
            orderSheet.Hide();
            orderSheet = null;
        }
        
        if (orderTicket != null)
        {
            orderTicket.Hide();
            orderTicket = null;
        }
    }

    public virtual void ForceStop()
    {
        this.StopAllCoroutines();
        ReleaseIndicator();

        orderInfo = null;
        motion.OnIdle(targetTable == null);
        
        Unseat();
    }

    public void ForceDestroy()
    {
        this.StopAllCoroutines();
        ReleaseIndicator();

        Unseat();

        this.gameObject.SetActive(false);
    }

    void Unseat()
    {
        if (targetTable != null)
        {
            targetTable.UnseatCustomer();
            targetTable = null;
        }
    }

    public int GetOrderNumber()
    {
        if (orderInfo != null)
        {
            return orderInfo.number;
        }
        
        return 0;
    }

    protected float GetMealTime()
    {
        return mealTime * (2f - globalState.mealTimeBonus);
    }

    IEnumerator EntranceMotion(List<Vector3> points)
    {
        float moveSpeed = (defaultSpeed + extraSpeed);
        motion.OnMove();
        
        for (int i = 0; i < points.Count; i++)
        {
            points[i].Set(points[i].x, transform.position.y, points[i].z);
        }
        
        transform.position = points[0];

        Quaternion lookRotation = transform.rotation;
        for (int i = 1; i < points.Count; i++)
        {
            lookRotation = Quaternion.LookRotation(points[i] - transform.position);
            while (Vector3.Distance(transform.position, points[i]) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, points[i], moveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, moveSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            transform.position = points[i];
            yield return new WaitForEndOfFrame();
        }

        wayPoints = points;
        wayPoints.Reverse();
        
        Seat();
        
        yield break;
    }

    IEnumerator LeaveMotion(bool onPlayFail, List<Vector3> points)
    {
        if (onPlayFail)
        {
            motion.OnFail();
            yield return new WaitForSeconds(1.5f);
        }

        float moveSpeed = (defaultSpeed + extraSpeed);
        motion.OnMove(moveSpeed > defaultSpeed ? 1 : 0);
        
        transform.position = points[0];
        Quaternion lookRotation = transform.rotation;

        bool isOpened = false;
        for (int i = 1; i < points.Count; i++)
        {
            lookRotation = Quaternion.LookRotation(points[i] - transform.position);
            while (Vector3.Distance(transform.position, points[i]) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, points[i], moveSpeed * Time.smoothDeltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, moveSpeed * Time.deltaTime);
                
                if (VectorExtensions.IsNearDistance(transform.position, exitDoor.transform.position, 2) && isOpened == false)
                {
                    if (exitDoor.IsBlocking())
                    {
                        motion.OnIdle();
                        yield return new WaitUntil(() => exitDoor.IsBlocking() == false);
                        yield return new WaitForEndOfFrame();
                        motion.OnMove(moveSpeed > defaultSpeed ? 1 : 0);
                    }
                    
                    isOpened = true;
                    
                    exitDoor.Open();
                }
                
                yield return new WaitForEndOfFrame();
            }

            transform.position = points[i];
            yield return new WaitForEndOfFrame();
        }

        ForceDestroy();
        
        yield break;
    }
}
