using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickupZone : InteractionObject
{
    GameObject plateModel;
    
    RecipeID dish = RecipeID.NULL;
    List<IngredientID> ingredientList = new();

    DeliverySheet deliverySheet;
    Dictionary<int, DeliveryTicket> pickUpTicketList = new();

    [SerializeField] GameObject platePrefab; // NOTE: 포장된 상태 모델로 통일
    [SerializeField] GameObject wipeFX;

    [SerializeField] MiniGameTrigger miniGameTrigger;
    [SerializeField] GlobalState globalState;

    OrderManager orderManager;
    OrderUI orderUI;
    
    void Awake()
    {
        orderManager = GameObject.FindAnyObjectByType<OrderManager>();
        orderUI = GameObject.FindAnyObjectByType<OrderUI>();
    }
    
    void OnEnable()
    {
        EventManager.GetEvent<DeliveryTicket>(EGameEvent.OnTimeOutDeliveryOrder).Subscribe(OnRemoveOrder);
    }
    
    void OnDisable()
    {
        EventManager.GetEvent<DeliveryTicket>(EGameEvent.OnTimeOutDeliveryOrder).Unsubscribe(OnRemoveOrder);
    }

    public override void OnMouseHover()
    {
        isHover = true;
        
        foreach(var interior in interiorList)
        {
            interior.SetActive(true);
        }

        if (plateModel != null)
        {
            EventManager.GetEvent<HoverPlateInfo>(EGameEvent.OnHoverPlate).Invoke(new HoverPlateInfo(ingredientList, this.transform, dish));
        }
    }
    
    public override void OnMouseOut()
    {
        isHover = false;
        
        foreach(var interior in interiorList)
        {
            interior.SetActive(false);
        }
    }
    
    public override void OnSelect(Player player)
    {
        if (VectorExtensions.IsNearDistance(transform.position, player.transform.position, targetDistance))
        {
            if (orderManager.HasPickUpOrder())
            {
                if (plateModel == null && player.IsHolding()) // NOTE: 서빙 가능
                {
                    dish = player.GetHoldDishID();
                    ingredientList = player.GetHoldIngredientID();

                    plateModel = Instantiate(platePrefab, transform.position, platePrefab.transform.rotation, transform);
                    
                    player.ReleaseIngredient();
                    
                    OnServe();
                    RefreshHover();
                }
            }
            else
            {
                if (plateModel != null && player.IsHolding() == false) // NOTE: 치워야 하는 경우
                {
                    if (globalState.isSinkDishFull)
                    {
                        EventManager.GetEvent(EGameEvent.OnHighlightSink).Invoke();
                        return;
                    }

                    WipeZone();
                    EventManager.GetEvent(EGameEvent.OnCleanedSeat).Invoke();
                }
            }
        }
    }

    public void OnAddOrder(DeliveryTicket ticket)
    {
        pickUpTicketList.Add(ticket.GetOrderNumber(), ticket);

        if (deliverySheet == null)
        {
            deliverySheet = orderUI.SpawnDeliverySheet(this.transform, Vector3.up * 1.75f);
        }
        
        deliverySheet.UpdateInfo(pickUpTicketList.Values.First().GetOrderDishID(), pickUpTicketList.Count - 1);
    }

    public void OnRemoveOrder(DeliveryTicket ticket)
    {
        if (pickUpTicketList.Count > 0 && pickUpTicketList.ContainsKey(ticket.GetOrderNumber()))
        {
            pickUpTicketList.Remove(ticket.GetOrderNumber());
        }
        
        UpdateSheet();
    }

    public void OnPickUp()
    {
        int processedNumber = orderManager.ProcessPickUpOrder(dish);
        if (processedNumber > 0)
        {
            Debug.LogFormat("배달기사 주문픽업 : {0}", processedNumber);
            
            if (pickUpTicketList.ContainsKey(processedNumber))
            {
                pickUpTicketList[processedNumber].Release();
                pickUpTicketList.Remove(processedNumber);
                
                EventManager.GetEvent(EGameEvent.OnFinishedDelivery).Invoke();
                EventManager.GetEvent(EGameEvent.OnFinishedOrder).Invoke();
                EventManager.GetEvent<Vector3>(EGameEvent.OnCustomerPaid).Invoke(transform.position + (Vector3.up * 3));
            }
        }
        else
        {
            // NOTE: 주문 리스트 중에 매치되는게 없는 경우
            if (pickUpTicketList.Count > 0)
            {
                int firstOrder = pickUpTicketList.Keys.ToList()[0];
                pickUpTicketList[firstOrder].MissOrder(); // NOTE: 첫 번째 주문을 취소 및 페널티 처리
                pickUpTicketList.Remove(firstOrder);
            }
        }

        UpdateSheet();
        WipeZone();
        RefreshHover();
        
        Debug.LogFormat("OnPickUp : {0}", processedNumber);
    }
    
    public override void ResetObject()
    {
        ClearSheet();
        ClearPlate();
        miniGameTrigger.ResetStack();
    }

    void UpdateSheet()
    {
        if (pickUpTicketList.Count > 0)
        {
            deliverySheet.UpdateInfo(pickUpTicketList.Values.First().GetOrderDishID(), pickUpTicketList.Count - 1);
        }
        else
        {
            ClearSheet();
        }
    }

    void OnServe()
    {
        miniGameTrigger.AccrueStack();
        if (miniGameTrigger.TryTrigger(StageManager.Instance.CallDeliveryRider, null) == false)
        {
            StageManager.Instance.CallDeliveryRider();
        }
    }

    void ClearSheet()
    {
        if (deliverySheet != null)
        {
            Destroy(deliverySheet.gameObject);
        }
    }

    void ClearPlate()
    {
        if (plateModel != null)
        {
            dish = RecipeID.NULL;
            ingredientList.Clear();
            
            Destroy(plateModel);
        }
    }
    
    void WipeZone()
    {
        ClearPlate();
        
        ObjectPool.Instance.SpawnFX(wipeFX, transform.position).Hide(1);
    }
}
