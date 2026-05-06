public enum EGameEvent
{
    OnChangedPlayerPlate,
    OnHoldPlayerDish,
    OnHoverPlate,
    
    OnTimeOutOrder,
    OnTimeOutDeliveryOrder,
    
    OnCleanedSeat,
    
    OnCustomerPaid,
    OnUpdateCurrency,
    OnUpdateReview,
    OnUpdateStamina,
    
    OnSelectFridge,
    OnStoreFridge,
    OnClickFridgeSlot,

    OnHighlightSink,
    OnHoverFocus, // 상호작용 오브젝트 호버
    OnCloserHoverObject, // 상호작용 가능 거리 호버 투명도 변화
    
    // RoundAnalytics
    OnRewardedOrder,
    OnSeatedGuest,
    OnFinishedDelivery,
    OnFinishedOrder,
    
    OnSoldOutOrder, // 재고 소진
    OnGameSaved,
    
    OnKickedLeftDoor,
    
    OnChangeGamepadFoucs,
    OnControlChange,
}