using UnityEngine;

public class ServeTable : InteractionObject
{
    bool canSit = true;

    Customer seatedCustomer;
    RecipePlate recipePlate;

    [SerializeField] GameObject wipeFX;
    
    [SerializeField] Transform seatPoint;
    [SerializeField] Transform extraDirtyPoint;
    
    [SerializeField] GlobalState globalState;

    OrderManager orderManager;
    WorldFX extraDirtyFX;
    
    void Awake()
    {
        recipePlate = this.GetComponent<RecipePlate>();
        orderManager = GameObject.FindAnyObjectByType<OrderManager>();
    }

    public override void OnMouseHover()
    {
        foreach(var interior in interiorList)
        {
            interior.SetActive(true);
        }
    }
    
    public override void OnMouseOut()
    {
        foreach(var interior in interiorList)
        {
            interior.SetActive(false);
        }
    }

    public override void OnSelect(Player player)
    {
        if(VectorExtensions.IsNearDistance(transform.position, player.transform.position, targetDistance))
        {
            if (seatedCustomer != null && player.IsHolding())
            {
                if (recipePlate.IsAnyPlated())
                {
                    return;
                }
                
                bool isPerfect = false;
                RecipeID dish = player.GetHoldDishID();
                if (dish == RecipeID.NULL)
                {
                    var ingredients = player.GetHoldIngredientID();
                    recipePlate.Plate(player.GetHoldIngredientID());
                }
                else
                {
                    isPerfect = orderManager.ProcessOrder(seatedCustomer.GetOrderNumber(), dish);
                    recipePlate.Plate(dish);
                    
                    SoundManager.Instance.PlaySound("SFX_Serve", 1, 2);
                }

                seatedCustomer.OnServe(isPerfect);
                player.ReleaseIngredient();

                Debug.LogFormat("서빙 {0} / {1}", dish.ToString(), isPerfect);

                return;
            }

            if (player.IsHolding() == false)
            {
                if (recipePlate.IsDirty() == false && extraDirtyFX != null)
                {
                    WipeExtraDirty();
                    
                    SoundManager.Instance.PlaySound("SFX_Stab", 1, 2);
                    return;
                }
                
                if (recipePlate.IsDirty() || (seatedCustomer == null && recipePlate.IsAnyPlated()))
                {
                    if (globalState.isSinkDishFull)
                    {
                        EventManager.GetEvent(EGameEvent.OnHighlightSink).Invoke();
                        return;
                    }
                    
                    WipePlate();
                    
                    SoundManager.Instance.PlaySound("SFX_Stab", 1, 2);
                }
            }
        }
    }
    
    public override void ResetObject()
    {
        canSit = true;
        
        recipePlate.ResetPlate();
        WipeExtraDirty();
    }
    
    public bool SeatCustomer(Customer customer)
    {
        if (seatedCustomer == null)
        {
            seatedCustomer = customer;
            seatedCustomer.transform.position = seatPoint.transform.position;
            seatedCustomer.transform.LookAt(seatedCustomer.transform.position + Vector3.back);
            return true;
        }

        return false;
    }

    public void UnseatCustomer()
    {
        seatedCustomer = null;
        
        if (recipePlate.IsEmpty())
        {
            canSit = true;
        }
    }

    public void ReserveTable()
    {
        canSit = false;
    }

    public void ForceDirty()
    {
        recipePlate.DirtyPlate();
    }

    public void SetExtraDirty(GameObject prefab)
    {
        WipeExtraDirty();
        
        extraDirtyFX = ObjectPool.Instance.SpawnFX(prefab, extraDirtyPoint.position);
        extraDirtyFX.transform.rotation = extraDirtyPoint.rotation;
    }

    public void WipeExtraDirty()
    {
        if (extraDirtyFX != null)
        {
            extraDirtyFX.Hide(true);
            extraDirtyFX = null;
            
            ObjectPool.Instance.SpawnFX(wipeFX, recipePlate.transform.position).Hide(1);
        }
    }

    public void WipePlate()
    {
        canSit = (seatedCustomer == null);
        recipePlate.ResetPlate();
        
        ObjectPool.Instance.SpawnFX(wipeFX, recipePlate.transform.position).Hide(1);
        
        EventManager.GetEvent(EGameEvent.OnCleanedSeat).Invoke();
    }

    public bool CanSit()
    {
        return (seatedCustomer == null) && canSit && (extraDirtyFX == null);
    }
}
