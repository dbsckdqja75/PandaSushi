using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Oven : InteractionObject
{
    bool isCooking = false;
    int curIngredient = -1;
    
    [SerializeField] Utensil pot;
    [SerializeField] Utensil pan;

    [Space(10)]
    [SerializeField] GameObject fireFX;
    [SerializeField] GameObject blazeFX;
    [SerializeField] GameObject dustFX;
    
    [Space(10)]
    [SerializeField] Animator ovenAnim;
    [SerializeField] AttachedAudioSource uniqueAudioSource;
    [SerializeField] GlobalState globalState;

    Utensil curUtensil;
    Mixer mixer;
    ProcessHandler processHandler;

    void Awake()
    {
        mixer = FindAnyObjectByType<Mixer>();
        processHandler = this.GetComponent<ProcessHandler>();
    }

    public void StartCook(IngredientID prepareTarget, float cookTime, ECookType cookType)
    {
        isCooking = true;
        ignoreHover = true;
        curIngredient = -1;
        
        hoverFocusPivot.gameObject.SetActive(false);

        UpdateUtensil(cookType);

        cookTime *= (2f - globalState.cookTimeBonus);

        this.StopAllCoroutines();
        processHandler.StartProcess(cookTime, (isSuccess) =>
        {
            isCooking = false;
            ignoreHover = false;
            curIngredient = (int)prepareTarget;
            
            hoverFocusPivot.gameObject.SetActive(true);
            
            GuideUI.Instance.ShowGuide(EGuideType.Finish_Green, this.transform, Vector3.up * 3);

            OvercookProcess().Start(this);
            RefreshHover();
        });

        RefreshHover();
    }

    void UpdateUtensil(ECookType cookType)
    {
        if (cookType == ECookType.HeatPan || cookType == ECookType.AssembleOrPan) // TODO: 올릴 재료 모델이 필요한 경우에 반영
        {
            curUtensil = pan;
            
            SoundManager.Instance.PlaySound("SFX_StovePan", uniqueAudioSource);
        }

        if (cookType == ECookType.HeatPot || cookType == ECookType.AssembleOrPot)
        {
            curUtensil = pot;
            
            SoundManager.Instance.PlaySound("SFX_StovePot", uniqueAudioSource);
        }
        
        curUtensil.Show();
        OnBurn();
    }

    void OnBurn()
    {
        fireFX.SetActive(true);
        blazeFX.SetActive(false);
        dustFX.SetActive(false);
    }

    void OnBlaze()
    {
        fireFX.SetActive(false);
        blazeFX.SetActive(true);
        dustFX.SetActive(false);
        
        curUtensil.PlayShake();
        ovenAnim.SetTrigger("OnShake");
        
        SoundManager.Instance.PlaySound("SFX_BigBurn", uniqueAudioSource);
        
        GuideUI.Instance.HideGuide(EGuideType.Finish_Green);
        GuideUI.Instance.ShowGuide(EGuideType.Warning_Red, this.transform, Vector3.up * 3);
    }

    void ClearFX()
    {
        fireFX.SetActive(false);
        blazeFX.SetActive(false);
    }
    
    public override void OnMouseHover()
    {
        isHover = true;

        if (ignoreHover == false)
        {
            interiorList[0].layer = LayerMask.NameToLayer("Vector Outline");

            if (isCooking == false && curIngredient != -1)
            {
                EventManager.GetEvent<HoverPlateInfo>(EGameEvent.OnHoverPlate).
                    Invoke(new HoverPlateInfo(new List<IngredientID>(){(IngredientID)curIngredient}, pot.transform));
            }
            else
            {
                EventManager.GetEvent<HoverPlateInfo>(EGameEvent.OnHoverPlate).Invoke(null);
            }
        }
    }
    
    public override void OnMouseOut()
    {
        isHover = false;
        
        interiorList[0].layer = LayerMask.NameToLayer("Interaction");
    }

    public override void OnSelect(Player player)
    {
        if(VectorExtensions.IsNearDistance(transform.position, player.transform.position, targetDistance) && isCooking == false)
        {
            if (player.IsHolding() && curIngredient == -1)
            {
                var holdIngredientID = player.GetHoldIngredientID().ToList();
                if (holdIngredientID.Count == 1)
                {
                    var ingredientData = PandaResources.Instance.GetIngredientData(holdIngredientID[0]);
                    ECookType cookType = ingredientData.GetCookType();
                    if ((int)cookType >= 2)
                    {
                        StartCook(ingredientData.GetOvenTarget(), ingredientData.GetOvenTime(), cookType);
                        
                        player.DrainStamina();
                        player.ReleaseIngredient();
                        
                        RefreshHover();
                    }
                }
            }
            else if(player.IsHolding() == false && curIngredient != -1)
            {
                RecipeID recipeID = mixer.TrySingleMix((IngredientID)curIngredient);
                player.HoldIngredient((IngredientID)curIngredient, recipeID);
                
                ResetObject();
            }
        }
    }

    public override void ResetObject()
    {
        processHandler?.StopProcess();
        this.StopAllCoroutines();

        isCooking = false;
        ignoreHover = false;
        curIngredient = -1;
        curUtensil = null;

        pan.Hide();
        pot.Hide();
        
        if (blazeFX.activeSelf)
        {
            GuideUI.Instance.HideGuide(EGuideType.Warning_Red);
        }

        ClearFX();
        RefreshHover();
        ovenAnim.SetTrigger("OnReset");
        
        SoundManager.Instance.Stop(uniqueAudioSource);
    }

    IEnumerator OvercookProcess()
    {
        float overcookTimer = 0;
        while (overcookTimer < 6)
        {
            overcookTimer += Time.deltaTime;
            
            if (overcookTimer > 3)
            {
                if (blazeFX.activeSelf == false)
                {
                    OnBlaze();
                }
                
                curUtensil.SetBurnIntensity(overcookTimer/6);
            }

            yield return new WaitForEndOfFrame();
        }

        dustFX.SetActive(true);
        ResetObject();
        yield break;
    }
}
