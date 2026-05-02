using UnityEngine;

public class RevenueUI : CurrencyLabel
{
    [SerializeField] Transform moneyFxPoint;
    [SerializeField] Transform revenueFxPoint;

    [Space(10)]
    [SerializeField] GameObject moneyFxPrefab;
    [SerializeField] GameObject flashFxPrefab;
    [SerializeField] GameObject revenueTextFxPrefab;
    
    protected override void OnEnable()
    {
        EventManager.GetEvent<Vector3>(EGameEvent.OnCustomerPaid).Subscribe(OnCustomerPaid);
        EventManager.GetEvent(EGameEvent.OnUpdateCurrency).Subscribe(RefreshUI);

        RefreshUI();
    }

    protected override void OnDisable()
    {
        EventManager.GetEvent<Vector3>(EGameEvent.OnCustomerPaid).Unsubscribe(OnCustomerPaid);
        EventManager.GetEvent(EGameEvent.OnUpdateCurrency).Unsubscribe(RefreshUI);
    }
    
    public void OnCustomerPaid(Vector3 effectSpawnPos)
    {
        MoneyFX fx = (MoneyFX)ObjectPool.Instance.SpawnFX(moneyFxPrefab, effectSpawnPos);
        fx.MoveTo(moneyFxPoint, () =>
        {
            RefreshUI();
            animator.ResetTrigger("OnProfit");
            animator.SetTrigger("OnProfitFX");
            
            FollowFX flashFX = (FollowFX)ObjectPool.Instance.SpawnFX(flashFxPrefab, fx.transform.position);
            flashFX.SetTarget(moneyFxPoint);
            flashFX.Hide(1);
        });
    }
}
