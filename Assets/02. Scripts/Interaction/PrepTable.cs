using System.Linq;
using UnityEngine;

public class PrepTable : InteractionObject
{
    RecipePlate recipePlate;

    [SerializeField] GameObject cutTable;
    
    [SerializeField] Vector3 assemblePosOffset;
    [SerializeField] Vector3 assembleRotOffset;
    [SerializeField] Vector3 guideOffset;
    
    [Space(10)]
    [SerializeField] MiniGameTrigger miniGameTrigger;

    Player player;
    GuideElement guide;

    void Awake()
    {
        player = GameObject.FindAnyObjectByType<Player>();
        recipePlate = this.GetComponent<RecipePlate>();
    }

    void Update()
    {
        if (guide != null)
        {
            if(VectorExtensions.IsNearDistance(transform.position, player.transform.position, targetDistance) && player.IsIdle())
            {
                guide.gameObject.SetActive(true);
            }
            else
            {
                guide.gameObject.SetActive(false);
            }
        }
    }

    public void TryPrep()
    {
        if (recipePlate.IsAnyPlated())
        {
            if (recipePlate.CanPrep(ECookType.Assemble) || recipePlate.CanPrep(ECookType.AssembleOrPan) || recipePlate.CanPrep(ECookType.AssembleOrPot))
            {
                if (recipePlate.IsCooking())
                {
                    Prep();
                    return;
                }
                
                if (miniGameTrigger.TryTrigger(recipePlate.ForcePrep, Prep) == false)
                {
                    Prep();
                }
            }
            else if (recipePlate.CanFinishDish(out RecipeData recipe))
            {
                recipePlate.UpdateRecipe(recipePlate.GetPlatedIngredientID(), recipe);
            }
        }
    }

    void Prep()
    {
        GameObject plateModel = recipePlate.GetPlateModel();
        GameObject ingredientModel = null;
        if (plateModel.transform.childCount > 0)
        {
            ingredientModel = plateModel.transform.GetChild(0).gameObject;
                    
            cutTable.SetActive(true);
            ingredientModel.transform.SetParent(cutTable.transform);
        }

        plateModel.SetActive(false);
        player.StartHandling((transform.position + assemblePosOffset), assembleRotOffset);
        recipePlate.StartPrep((isSuccess) =>
        {
            if (isSuccess)
            {
                if (ingredientModel)
                {
                    Destroy(ingredientModel);
                }
                    
                cutTable.SetActive(false);
            }
            
            miniGameTrigger.AccrueStack();
                
            player.FinishHandling();
        });
        
        EventManager.GetEvent<HoverPlateInfo>(EGameEvent.OnHoverPlate).Invoke(null);
        EventManager.GetEvent<Transform>(EGameEvent.OnHoverFocus).Invoke(null);
        
        SoundManager.Instance.PlaySound("SFX_Chop", 1, 4);
            
        OnMouseOut();
    }

    public override void OnMouseHover()
    {
        isHover = true;
        
        foreach(var interior in interiorList)
        {
            interior.SetActive(true);
        }

        if(recipePlate.IsAnyPlated())
        {
            if (recipePlate.CanPrep(ECookType.Assemble) || recipePlate.CanFinishDish(out RecipeData recipe)
                || recipePlate.CanPrep(ECookType.AssembleOrPan) || recipePlate.CanPrep(ECookType.AssembleOrPot))
            {
                if (guide == null)
                {
                    guide = GuideUI.Instance.ShowGuide(EGuideType.Interaction_F, this.transform, guideOffset);
                }
            }

            EventManager.GetEvent<HoverPlateInfo>(EGameEvent.OnHoverPlate).Invoke(new HoverPlateInfo(recipePlate.GetPlatedIngredientID(), this.transform, recipePlate.GetDishID()));
        }
        else
        {
            EventManager.GetEvent<HoverPlateInfo>(EGameEvent.OnHoverPlate).Invoke(null);
        }
    }

    public override void OnMouseOut()
    {
        isHover = false;
        
        foreach(var interior in interiorList)
        {
            interior.SetActive(false);
        }

        if (guide != null)
        {
            GuideUI.Instance.HideGuide(EGuideType.Interaction_F);
            guide = null;
        }
    }

    public override void OnSelect(Player player)
    {
        if(VectorExtensions.IsNearDistance(transform.position, player.transform.position, targetDistance))
        {
            if (recipePlate.IsCooking())
            {
                return;
            }
            
            if(player.IsHolding()) // NOTE : 손에 재료가 있는 경우
            {
                var holdDishID = player.GetHoldDishID();
                var holdIngredientID = player.GetHoldIngredientID().ToList();

                if (holdDishID != RecipeID.NULL)
                {
                    player.ReleaseIngredient();
                    
                    if (recipePlate.IsAnyPlated()) // NOTE: 손에 든 요리랑 교체
                    {
                        var plateIngredientID = recipePlate.GetPlatedIngredientID().ToList();
                        player.HoldIngredient(plateIngredientID, recipePlate.GetDishID());
                    }
                    
                    recipePlate.Plate(holdDishID);
                }
                else if(recipePlate.CanPlate(holdIngredientID)) // NOTE : 조합 시도
                {
                    recipePlate.Plate(holdIngredientID);
                    
                    player.ReleaseIngredient();
                }
                else if (recipePlate.IsAnyPlated()) // NOTE : 손에 든 재료와 교체
                {
                    var plateIngredientID = recipePlate.GetPlatedIngredientID().ToList();
                    player.HoldIngredient(plateIngredientID, recipePlate.GetDishID());

                    recipePlate.Plate(holdIngredientID, true);
                }
            }
            else if(recipePlate.IsAnyPlated()) // NOTE : 빈손인 경우
            {
                player.HoldIngredient(recipePlate.GetPlatedIngredientID(), recipePlate.GetDishID());
                
                recipePlate.ResetPlate();
            }

            RefreshHover();
        }
    }
    
    public override void ResetObject()
    {
        recipePlate.ResetPlate();
        
        cutTable.SetActive(false);
        miniGameTrigger.ResetStack();
    }
}
