using UnityEngine;

public class WorldLayoutElement : MonoBehaviour
{
    Transform targetTrf;
    Vector3 targetOffset = Vector3.zero;

    void Update()
    {
        if(targetTrf != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(targetTrf.position + targetOffset);
        }
    }

    public void UpdateTarget(Transform newTarget)
    {
        targetTrf = newTarget;
    }

    public void UpdateOffset(Vector3 newOffset)
    {
        targetOffset = newOffset;
    }
}
