using System;
using UnityEngine;

public class Utensil : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] MeshRenderer meshRenderer;
    
    public void Show()
    {
        SetBurnIntensity(0);
        this.gameObject.SetActive(true);
        
        anim.ResetTrigger("OnShake");
        anim.ResetTrigger("OnReset");
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void SetBurnIntensity(float amount)
    {
        meshRenderer.material.SetFloat("_ExtraIntensity", amount);
    }

    public void PlayShake()
    {
        anim.SetTrigger("OnShake");
    }
}
