using System;
using UnityEngine;

public class HoverFocuser : MonoBehaviour
{
    [SerializeField] float translucentAlpha = 18;
    [SerializeField] float opaqueAlpha = 2;
    [SerializeField] GameObject hoverPrefab;

    bool isOpaque = false;
    
    GameObject currentHover;
    Material hoverMaterial;

    void Awake()
    {
        currentHover = Instantiate(hoverPrefab, Vector3.zero, Quaternion.identity);
        currentHover.SetActive(false);
        
        hoverMaterial = currentHover.GetComponentInChildren<MeshRenderer>().material;
        hoverMaterial.SetFloat("_LightingPower", translucentAlpha);
    }

    void OnEnable()
    {
        EventManager.GetEvent<Transform>(EGameEvent.OnHoverFocus).Subscribe(UpdateFocus);
        EventManager.GetEvent<bool>(EGameEvent.OnCloserHoverObject).Subscribe(UpdateFoucsTransparent);
    }
    
    void OnDisable()
    {
        EventManager.GetEvent<Transform>(EGameEvent.OnHoverFocus).Unsubscribe(UpdateFocus);
        EventManager.GetEvent<bool>(EGameEvent.OnCloserHoverObject).Unsubscribe(UpdateFoucsTransparent);
    }

    void UpdateFocus(Transform targetPivot)
    {
        if (targetPivot != null)
        {
            currentHover.transform.SetParent(targetPivot);
            currentHover.transform.localPosition = Vector3.zero;
            currentHover.transform.localRotation = Quaternion.identity;
            currentHover.transform.localScale = Vector3.one;
            
            hoverMaterial.SetFloat("_LightingPower", isOpaque ? opaqueAlpha : translucentAlpha);
        }
        
        currentHover.SetActive(targetPivot != null);
    }

    void UpdateFoucsTransparent(bool isOpaque)
    {
        this.isOpaque = isOpaque;
        
        hoverMaterial.SetFloat("_LightingPower", isOpaque ? opaqueAlpha : translucentAlpha);
    }
}
