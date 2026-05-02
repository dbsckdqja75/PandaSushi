using UnityEngine;

public class OrderUI : MonoBehaviour
{
    [SerializeField] RectTransform orderSheetPivot;
    [SerializeField] GameObject deliverySheetPrefab;
    [SerializeField] GameObject orderSheetPrefab;
    
    [Space(10)]
    [SerializeField] RectTransform orderTicketPivot;
    [SerializeField] GameObject orderTicketPrefab;
    
    public OrderSheet SpawnOrderSheet(Transform followTarget, Vector3 offset)
    {
        OrderSheet orderSheet = ObjectPool.Instance.Spawn(orderSheetPrefab, orderSheetPivot, false).GetComponent<OrderSheet>();
        
        WorldLayoutElement layout = orderSheet.GetComponent<WorldLayoutElement>();
        layout.UpdateTarget(followTarget);
        layout.UpdateOffset(offset);
        
        orderSheet.gameObject.SetActive(true);
        return orderSheet;
    }
    
    public DeliverySheet SpawnDeliverySheet(Transform followTarget, Vector3 offset)
    {
        DeliverySheet deliverySheet = ObjectPool.Instance.Spawn(deliverySheetPrefab, orderSheetPivot, false).GetComponent<DeliverySheet>();
        
        WorldLayoutElement layout = deliverySheet.GetComponent<WorldLayoutElement>();
        layout.UpdateTarget(followTarget);
        layout.UpdateOffset(offset);
        
        deliverySheet.gameObject.SetActive(true);
        return deliverySheet;
    }
    
    public OrderTicket SpawnOrderTicket()
    {
        OrderTicket orderTicket = ObjectPool.Instance.Spawn(orderTicketPrefab, orderTicketPivot, false).GetComponent<OrderTicket>();
        orderTicket.transform.SetAsFirstSibling();
        orderTicket.gameObject.SetActive(true);
        return orderTicket;
    }
}
