using UnityEngine;

public class DeliveryTicket : MonoBehaviour
{
    OrderInfo orderInfo;
    float remainTimer = 0;

    OrderManager orderManager;
    OrderTicket orderTicket;

    void Update()
    {
        if (orderTicket != null)
        {
            if (remainTimer > 0)
            {
                remainTimer -= Time.deltaTime;
                orderTicket.UpdateTimerText(remainTimer);
            }
            else
            {
                TimeOutOrder();
            }
        }
    }

    public void Init(OrderInfo orderInfo, OrderManager orderManager)
    {
        this.orderManager = orderManager;
        this.orderInfo = orderInfo;
        orderTicket = orderInfo.orderTicket;

        remainTimer = 60;
        orderTicket.UpdateInfo(orderInfo.dish, (orderInfo.number - 1000), true);
        orderTicket.UpdateTimerText(remainTimer);
    }

    public void Release()
    {
        orderTicket.Hide();
        orderTicket = null;
        
        Destroy(this.gameObject);
    }

    public void MissOrder()
    {
        TimeOutOrder();
    }

    void TimeOutOrder()
    {
        orderTicket.Hide();
        orderTicket = null;
        
        orderManager.TimeOutOrder(orderInfo.number);
        
        EventManager.GetEvent<DeliveryTicket>(EGameEvent.OnTimeOutDeliveryOrder).Invoke(this);
        
        Destroy(this.gameObject);
    }

    public RecipeID GetOrderDishID()
    {
        return orderInfo.dish;
    }

    public int GetOrderNumber()
    {
        return orderInfo.number;
    }
}
