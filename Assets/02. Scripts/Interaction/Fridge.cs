using System;
using UnityEngine;

public class Fridge : InteractionObject
{
    Animator animator;

    [SerializeField] GlobalState globalState;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    public void OnOpen()
    {
        animator.SetBool("isOpen", true);
        
        hoverFocusPivot.gameObject.SetActive(false);
    }

    public void OnClose()
    {
        animator.SetBool("isOpen", false);
        
        hoverFocusPivot.gameObject.SetActive(true);
    }

    public override void OnSelect(Player player)
    {
        if(VectorExtensions.IsNearDistance(transform.position, player.transform.position, targetDistance))
        {
            if (globalState.isSinkDishFull)
            {
                EventManager.GetEvent(EGameEvent.OnHighlightSink).Invoke();
                return;
            }

            if (player.IsHolding())
            {
                var dish = player.GetHoldDishID();
                var ingredient = player.GetHoldIngredientID();
                if (dish != RecipeID.NULL || ingredient == null || ingredient.Count != 1)
                {
                    if ((int)dish >= 1000) // NOTE: 완제 별도 처리
                    {
                        player.ReleaseIngredient();
                        EventManager.GetEvent<IngredientID>(EGameEvent.OnStoreFridge).Invoke((IngredientID)((int)dish));
                        
                        player.OnSearching();
                        EventManager.GetEvent<bool>(EGameEvent.OnSelectFridge).Invoke(true);
                    }
                    
                    return;
                }
                
                // NOTE : 손에 들고있는 재료 보관
                player.ReleaseIngredient();
                EventManager.GetEvent<IngredientID>(EGameEvent.OnStoreFridge).Invoke(ingredient[0]);
            }

            player.OnSearching();
            
            EventManager.GetEvent<bool>(EGameEvent.OnSelectFridge).Invoke(true);
        }
    }

    public override void ResetObject()
    {
        OnClose();
    }
}
