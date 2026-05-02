using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PlayerState
{
    Idle = 0, // 이동 가능 상태
    Hold = 1, // 들고 있는 상태
    Aim = 2, // 조준 상태
    Assembly = 100, // 손질 혹은 상호작용 중 상태
    Lock = 1000, // 조작 불가
}

public class Player : MonoBehaviour
{
    PlayerState currentState;

    [SerializeField] int maxStamina = 20;
    int stamina = 0;
    
    RecipeID curDish = RecipeID.NULL;
    List<IngredientID> curIngredientID = new();

    [SerializeField] GameObject ingredientPoint;
    
    [SerializeField] GameObject extraHealFxPrefab;
    [SerializeField] GameObject normalHealFxPrefab;

    PlayerController controller;
    PlayerWeapon weapon;
    PlayerHandSlot handSlot;
    PlayerMotion playerMotion;

    void Awake()
    {
        currentState = PlayerState.Idle;

        controller = this.GetComponent<PlayerController>();
        weapon = this.GetComponent<PlayerWeapon>();
        handSlot = this.GetComponent<PlayerHandSlot>();
        playerMotion = this.GetComponent<PlayerMotion>();
        
        ResetStamina();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            DrainStamina();
        }
    }

    void UpdateState(PlayerState newState)
    {
        currentState = newState;
    }
    
    public void OnSearching()
    {
        UpdateState(PlayerState.Assembly);

        playerMotion.OnAssemble();
    }

    public void FinishSearching()
    {
        if(IsHolding() == false)
        {
            UpdateState(PlayerState.Idle);

            playerMotion.OnRelease();
        }
    }

    public void StartHandling(Vector3 fixedPos, Vector3 fixedRot)
    {
        if(IsHolding() == false)
        {
            UpdateState(PlayerState.Assembly);
            
            controller.ForceUpdateTransform(fixedPos, fixedRot);

            playerMotion.OnAssemble();

            // stage.StartMiniGame(platedID);
        }
    }

    public void FinishHandling()
    {
        DrainStamina();
        UpdateState(PlayerState.Idle);

        playerMotion.OnRelease();
    }

    public void StartAim()
    {
        UpdateState(PlayerState.Aim);
        
        playerMotion.OnAim();
    }

    public void FinishAim(Vector3 throwPoint)
    {
        UpdateState(PlayerState.Idle);

        if (throwPoint != Vector3.zero && stamina > 0)
        {
            weapon.ShootUtensil(throwPoint);
            
            playerMotion.OnThrow();
            
            DrainStamina();
            
            SoundManager.Instance.PlaySound("SFX_Throw", 1, 4);
        }
        else
        {
            playerMotion.OnRelease();            
        }
    }

    bool SurpriseEat()
    {
        if (stamina <= 0 && RandomExtensions.RandomBool())
        {
            ManuallyEat();
            return true;
        }

        return false;
    }

    public void ManuallyEat()
    {
        int fullnessValue = GetHoldIngredientLevel();

        if (fullnessValue >= 30)
        {
            ObjectPool.Instance.SpawnFX(extraHealFxPrefab, transform.position, transform).Hide(1f, true);
            
            SoundManager.Instance.PlaySound("SFX_Drink", 1, 2);
            SoundManager.Instance.PlaySound("SFX_Buff");
        }
        else if (fullnessValue >= 15)
        {
            ObjectPool.Instance.SpawnFX(normalHealFxPrefab, transform.position, transform).Hide(1f, true);
            
            SoundManager.Instance.PlaySound("SFX_Drink", 1, 2);
            SoundManager.Instance.PlaySound("SFX_Buff");
        }

        SoundManager.Instance.PlaySound("SFX_Tap1");
        
        stamina = Mathf.Min(stamina + fullnessValue, maxStamina);
        EventManager.GetEvent<float>(EGameEvent.OnUpdateStamina).Invoke((float)stamina/(float)maxStamina);

        ReleaseIngredient();
    }

    public void DrainStamina()
    {
        stamina = Mathf.Max(0, stamina-1);
        EventManager.GetEvent<float>(EGameEvent.OnUpdateStamina).Invoke((float)stamina/(float)maxStamina);
    }

    public void ResetStamina()
    {
        stamina = PlayerPrefsManager.LoadSlotData("Stamina", maxStamina);
        EventManager.GetEvent<float>(EGameEvent.OnUpdateStamina).Invoke((float)stamina/(float)maxStamina);
    }

    public void SaveStamina()
    {
        PlayerPrefsManager.SaveSlotData("Stamina", stamina);
    }

    public void HoldIngredient(IngredientID targetID, RecipeID dishID)
    {
        List<IngredientID> newIngredients = new List<IngredientID>();
        newIngredients.Add(targetID);
        
        HoldIngredient(newIngredients, dishID);
    }
    
    public void HoldIngredient(List<IngredientID> targetID, RecipeID dishID)
    {
        curDish = dishID;
        curIngredientID = targetID.ToList();
        
        if (SurpriseEat())
        {
            return;
        }
        
        UpdateState(PlayerState.Hold);

        if (curDish != RecipeID.NULL)
        {
            handSlot.OnHold(curDish);
        }
        else
        {
            handSlot.OnHold(curIngredientID);
        }

        playerMotion.OnHold();

        ingredientPoint.SetActive(true);

        if (curDish == RecipeID.NULL)
        {
            EventManager.GetEvent<List<IngredientID>>(EGameEvent.OnChangedPlayerPlate).Invoke(curIngredientID);
        }
        else
        {
            EventManager.GetEvent<RecipeID>(EGameEvent.OnHoldPlayerDish).Invoke(curDish);
        }
        
        Debug.LogFormat("HoldIngredient {0}/{1}", dishID.ToString(), curIngredientID.Count);
    }

    public void ReleaseIngredient()
    {
        UpdateState(PlayerState.Idle);

        curDish = RecipeID.NULL;
        curIngredientID.Clear();

        handSlot.OnRelease();
        playerMotion.OnRelease();

        ingredientPoint.SetActive(false);
        
        EventManager.GetEvent<List<IngredientID>>(EGameEvent.OnChangedPlayerPlate).Invoke(null);
    }

    public void Freeze()
    {
        ReleaseIngredient();
        
        controller.ForceClearHoverObject();

        UpdateState(PlayerState.Lock);
        playerMotion.OnIdle();
    }

    public void Unfreeze()
    {
        UpdateState(PlayerState.Idle);
        playerMotion.OnIdle();
    }

    public bool CanMove()
    {
        return (int)currentState <= 2;
    }
    
    public bool CanEat()
    {
        // return (stamina < maxStamina);
        return true;
    }

    public bool IsIdle()
    {
        return (currentState == PlayerState.Idle);
    }

    public bool IsHolding()
    {
        return (currentState == PlayerState.Hold);
    }

    public bool IsAiming()
    {
        return (currentState == PlayerState.Aim);
    }

    public RecipeID GetHoldDishID()
    {
        return curDish;
    }

    public List<IngredientID> GetHoldIngredientID()
    {
        return curIngredientID.ToList();
    }
    
    public int GetHoldIngredientLevel()
    {
        if (curDish != RecipeID.NULL)
        {
            return PandaResources.Instance.GetRecipeData(curDish).GetTargetLevel();
        }
        else
        {
            if (curIngredientID.Count == 1)
            {
                if (curIngredientID[0] == IngredientID.Engery_Drink)
                {
                    return 15;
                }
                
                if (curIngredientID[0] == IngredientID.Engery_DrinkGold)
                {
                    return 30;
                }
            }
            
            return curIngredientID.Count;
        }
    }
}
