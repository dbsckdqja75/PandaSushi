using System.Collections;
using UnityEngine;

public class PandaSkinHandler : MonoBehaviour
{
    SkinnedMeshRenderer[] skinnedMeshRenderers;
    
    void Awake()
    {
        skinnedMeshRenderers = this.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    void OnDisable()
    {
        RestoreSkin();    
    }

    public void RestoreSkin(bool onFade = false)
    {
        this.StopAllCoroutines();
        
        if (onFade)
        {
            FadeMotion(Color.white, 0f).Start(this);
            return;
        }

        ChangeColor(Color.white, 0f);
    }

    public void ChangeSkinColor(Color targetColor, bool onFade = false, float colorIntensity = 0.5f, float duration = 0.25f)
    {
        this.StopAllCoroutines();
        
        if (onFade)
        {
            FadeMotion(targetColor, 0.5f, duration).Start(this);
            return;
        }

        ChangeColor(targetColor, colorIntensity);
    }

    void ChangeColor(Color targetColor, float colorIntensity)
    {
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            renderer.material.SetColor("_ExtraColor", targetColor);
            renderer.material.SetFloat("_ExtraIntensity", colorIntensity);
        }
    }
    
    IEnumerator FadeMotion(Color targetColor, float targetValue, float duration = 0.25f)
    {
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            renderer.material.SetColor("_ExtraColor", targetColor);
        }

        float startValue = 0f;
        if (skinnedMeshRenderers.Length > 0)
        {
            startValue = skinnedMeshRenderers[0].material.GetFloat("_ExtraIntensity");
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.smoothDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
            {
                renderer.material.SetFloat("_ExtraIntensity", Mathf.Lerp(startValue, targetValue, t));
            }

            yield return new WaitForEndOfFrame();
        }
        
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            renderer.material.SetFloat("_ExtraIntensity", targetValue);
        }

        yield break;
    }
}
