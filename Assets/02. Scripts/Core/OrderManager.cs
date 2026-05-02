using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderInfo
{
    public int number = -1;
    public RecipeID dish = RecipeID.NULL;
    public float reward;
    public float starReward;

    public IndicatorElement orderSheet;
    public OrderTicket orderTicket;
}

public class OrderManager : MonoBehaviour
{
    int latestOrderNumber = 0;
    int latestPickUpNumber = 1000;

    Dictionary<int, OrderInfo> orders = new(); // NOTE : <주문번호, 완성 레시피 번호>

    [SerializeField] GameObject[] fxFacePrefabs;
    
    [Space(10)]
    [SerializeField] GlobalState globalState;

    OrderUI orderUI;
    FridgeStorage fridgeStorage;
    
    void Awake()
    {
        orderUI = this.GetComponent<OrderUI>();
        fridgeStorage = FindAnyObjectByType<FridgeStorage>();
    }

    public OrderInfo AddGeneralOrder(Transform indicatorTarget, Vector3 indicatorOffset)
    {
        OrderInfo info = GenerateOrderInfo();
        if (info != null)
        {
            info.number = AddOrderNumber();
            info.orderSheet = orderUI.SpawnOrderSheet(indicatorTarget, indicatorOffset);
            info.orderTicket = orderUI.SpawnOrderTicket();

            latestOrderNumber = info.number;
            orders.Add(latestOrderNumber, info);
            
            SoundManager.Instance.PlaySound("SFX_NewOrder");
        }
        
        return info;
    }

    public OrderInfo AddExtraOrder(Transform indicatorTarget, Vector3 indicatorOffset)
    {
        OrderInfo info = GenerateExtraOrderInfo();
        if (info != null)
        {
            info.number = AddOrderNumber();
            info.orderSheet = orderUI.SpawnOrderSheet(indicatorTarget, indicatorOffset);
            info.orderTicket = orderUI.SpawnOrderTicket();

            latestOrderNumber = info.number;
            orders.Add(latestOrderNumber, info);
            
            SoundManager.Instance.PlaySound("SFX_NewOrder");
        }
        
        return info;
    }
    
    public OrderInfo AddDeliveryOrder()
    {
        OrderInfo info = GenerateOrderInfo();
        if (info != null)
        {
            info.number = AddPickUpOrder();
            info.orderTicket = orderUI.SpawnOrderTicket();
        
            latestPickUpNumber = info.number;
            orders.Add(latestPickUpNumber, info);
            
            SoundManager.Instance.PlaySound("SFX_NewOrder");
        }
        
        return info;
    }

    int AddOrderNumber()
    {
        int addedNumber = (latestOrderNumber + 1);
        while(orders.ContainsKey(addedNumber))
        {
            addedNumber += 1;
        }

        return addedNumber;
    }
    
    int AddPickUpOrder()
    {
        int addedNumber = (latestPickUpNumber + 1);
        while(orders.ContainsKey(addedNumber))
        {
            addedNumber += 1;
        }

        return addedNumber;
    }

    public bool ProcessOrder(int orderNumber, RecipeID result)
    {
        if (orders.TryGetValue(orderNumber, out var orderInfo))
        {
            if (orderInfo.dish == result) // NOTE : 올바른 주문 서빙 완료
            {
                CurrencyManager.Instance.RewardCurrency(ECurrencyType.Money, orderInfo.reward);
                CurrencyManager.Instance.RewardCurrency(ECurrencyType.Star, orderInfo.starReward);
                
                EventManager.GetEvent<float>(EGameEvent.OnRewardedOrder).Invoke(orderInfo.reward);
                EventManager.GetEvent<int>(EGameEvent.OnUpdateReview).Invoke((int)orderInfo.starReward);

                orders.Remove(orderNumber);
                return true;
            }
            else
            {
                OnPenalty((int)(orders[orderNumber].starReward * 0.1f));
                orders.Remove(orderNumber);
            }
        }

        return false;
    }

    public int ProcessPickUpOrder(RecipeID result)
    {
        int pickUpNumber = -1;
        foreach (KeyValuePair<int, OrderInfo> pair in orders)
        {
            if (pair.Key > 1000)
            {
                if (pair.Value.dish == result)
                {
                    CurrencyManager.Instance.RewardCurrency(ECurrencyType.Money, pair.Value.reward);
                    CurrencyManager.Instance.RewardCurrency(ECurrencyType.Star, pair.Value.starReward);
                    
                    EventManager.GetEvent<float>(EGameEvent.OnRewardedOrder).Invoke(pair.Value.reward);
                    EventManager.GetEvent<int>(EGameEvent.OnUpdateReview).Invoke((int)pair.Value.starReward);

                    pickUpNumber = pair.Key;
                    break;
                }
                else
                {
                    break;
                }
            }
        }

        if (pickUpNumber > 1000)
        {
            orders.Remove(pickUpNumber);
        }

        return pickUpNumber;
    }

    public bool HasPickUpOrder()
    {
        return orders.ContainsKey(latestPickUpNumber);
    }

    public void TimeOutOrder(int orderNumber)
    {
        if (orders.ContainsKey(orderNumber))
        {
            OnPenalty((int)(orders[orderNumber].starReward * 0.5f));
            orders.Remove(orderNumber);

            EventManager.GetEvent(EGameEvent.OnTimeOutOrder).Invoke();
        }
    }

    void OnPenalty(int value)
    {
        Debug.LogWarningFormat("Penalty Value {0}", value);
        
        CurrencyManager.Instance.Purchase(ECurrencyType.Star, value);
        EventManager.GetEvent<int>(EGameEvent.OnUpdateReview).Invoke(-value);
    }
    
    public void ShowFaceFX(int faceIdx, Vector3 spawnPos)
    {
        // NOTE : 0 - 화남 | 1 - 행복 | 2 - 실망 | 3 - 구토
        ObjectPool.Instance.SpawnFX(fxFacePrefabs[faceIdx], spawnPos).Hide(1.5f, true);
    }

    OrderInfo GenerateOrderInfo()
    {
        RecipeData recipeData = GetOrderableRandomRecipe();
        if (recipeData != null)
        {
            OrderInfo orderInfo = new();
            orderInfo.dish = recipeData.GetID();
            orderInfo.reward = recipeData.GetPrice() * globalState.tipBonus;
            orderInfo.starReward = recipeData.GetStarReward() * globalState.reviewBonus;
            return orderInfo;
        }

        return null;
    }
    
    OrderInfo GenerateExtraOrderInfo()
    {
        RecipeData recipeData = GetOrderableRandomExtraRecipe();
        if (recipeData != null)
        {
            OrderInfo orderInfo = new();
            orderInfo.dish = recipeData.GetID();
            orderInfo.reward = recipeData.GetPrice() * globalState.tipBonus;
            orderInfo.starReward = recipeData.GetStarReward() * globalState.reviewBonus;

            Debug.LogWarningFormat("GenerateExtraOrderInfo - {0}", orderInfo.dish);
            
            return orderInfo;
        }

        return null;
    }

    RecipeData GetOrderableRandomRecipe()
    {
        List<RecipeID> allRecipeKeys = PandaResources.Instance.GetRecipeKeys().Where(x => (int)x < 1000).ToList();
        List<RecipeID> orderableKeys = new();
        
        foreach (var recipeID in allRecipeKeys)
        {
            Debug.LogWarningFormat("레시피ID {0}", recipeID);
                
            bool canMix = true;
            foreach (var ingredientID in PandaResources.Instance.GetRecipeData(recipeID).GetIngredients())
            {
                IngredientID targetID = ingredientID;
                if((int)ingredientID >= 100)
                {
                    bool isRootID = false;
                    while(isRootID == false)
                    {
                        IngredientData ingredientData = PandaResources.Instance.GetIngredientData(targetID);
                        if (ingredientData.GetParentID() != targetID)
                        {
                            targetID = ingredientData.GetParentID();
                            Debug.LogWarningFormat("상위 재료ID : {0}", targetID);
                        }
                        else
                        {
                            isRootID = true;
                            Debug.LogWarningFormat("상위 재료 파악 완료 : {0}", targetID);
                        }
                    }
                }

                if (fridgeStorage.CanReservable(targetID) == false) // NOTE: 활용 가능한 재료 소진
                {
                    canMix = false;
                    Debug.LogFormat("{0} 재료 소진", ingredientID);
                    break;
                }
            }

            if (canMix)
            {
                orderableKeys.Add(recipeID);
            }
        }
        
        Debug.LogWarningFormat("GetOrderableRecipeList -> ListCount {0}", orderableKeys.Count);

        if (orderableKeys.Count > 0)
        {
            return PandaResources.Instance.GetRecipeData(orderableKeys[Random.Range(0, orderableKeys.Count)]);
        }

        EventManager.GetEvent(EGameEvent.OnSoldOutOrder).Invoke();
        
        Debug.LogWarning("재고 소진 (조기 종료 버튼 활성화) - 손님 더 안오고 바로 퇴장함 (앉지 않음)");

        return null;
    }
    
    RecipeData GetOrderableRandomExtraRecipe()
    {
        List<RecipeID> allRecipeKeys = PandaResources.Instance.GetRecipeKeys().Where(x => (int)x >= 1000).ToList();
        List<RecipeID> orderableKeys = new();
        
        foreach (var recipeID in allRecipeKeys)
        {
            bool canOrder = true;
            foreach (var ingredientID in PandaResources.Instance.GetRecipeData(recipeID).GetIngredients())
            {
                if (fridgeStorage.CanReservable(ingredientID) == false) // NOTE: 완제 소진
                {
                    Debug.LogFormat("{0} 완제 소진", ingredientID);
                    canOrder = false;
                    break;
                }
            }

            if (canOrder)
            {
                orderableKeys.Add(recipeID);
            }
        }
        
        Debug.LogWarningFormat("GetOrderableRecipeList -> ListCount {0}", orderableKeys.Count);

        if (orderableKeys.Count > 0)
        {
            return PandaResources.Instance.GetRecipeData(orderableKeys[Random.Range(0, orderableKeys.Count)]);
        }
        
        return null;
    }
}