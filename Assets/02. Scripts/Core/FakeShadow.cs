using UnityEngine;

public class FakeShadow : MonoBehaviour
{
    [SerializeField] Transform targetTrf;

    float originHeight = 0;
    
    Color originalColor;
    MeshRenderer meshRenderer;

    void Awake()
    {
        UpdateOriginHeight();
        
        meshRenderer = this.GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.GetColor("_BaseColor");
    }

    void FixedUpdate()
    {
        if(targetTrf)
        {
            float heightDistance = (1.5f - Mathf.Clamp(targetTrf.GetChild(0).position.y, 0f, 1.5f));

            Color shadowColor = originalColor;
            shadowColor.a = Mathf.Lerp(0, originalColor.a, heightDistance);

            meshRenderer.material.SetColor("_BaseColor", shadowColor);

            transform.position = new Vector3(transform.position.x, originHeight, transform.position.z);
        }
    }

    public void UpdateOriginHeight()
    {
        originHeight = transform.position.y;
    }
}
