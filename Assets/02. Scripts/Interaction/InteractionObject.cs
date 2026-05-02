using UnityEngine;

public abstract class InteractionObject : MonoBehaviour
{
    protected bool ignoreHover = false;
    protected bool isHover;

    [SerializeField] protected float targetDistance = 2.6f;

    [Space(10)]
    [SerializeField] protected Transform hoverFocusPivot;
    [SerializeField] protected GameObject[] interiorList;

    protected Transform targetTrf;

    void Start()
    {
        ResetObject();
        OnMouseOut();
    }

    public virtual void OnMouseHover()
    {
        isHover = true;
        
        // NOTE : 마우스를 올려놓으면 아웃라인 표시
        foreach(GameObject interior in interiorList)
        {
            interior.layer = LayerMask.NameToLayer("Vector Outline");
        }
    }

    public virtual void OnMouseOut()
    {
        isHover = false;
        
        // NOTE : 마우스 포인터 벗어나면 원상복구
        foreach(GameObject interior in interiorList)
        {
            interior.layer = LayerMask.NameToLayer("Interaction");
        }
    }

    protected void RefreshHover()
    {
        if (isHover)
        {
            OnMouseOut();
            OnMouseHover();
        }
    }
    
    public abstract void OnSelect(Player player);
    
    public abstract void ResetObject();

    public Transform GetHoverFoucsPivot()
    {
        return hoverFocusPivot;
    }
    
    public bool CanSelect(Transform playerTrf)
    {
        return VectorExtensions.IsNearDistance(transform.position, playerTrf.position, targetDistance);
    }
    
    void OnDrawGizmos()
    {
        if(targetTrf != null)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.color = new Color(0.2f, 0.9f, 0.2f, 0.75f);
            Gizmos.matrix = Matrix4x4.TRS(new Vector3(transform.position.x, targetTrf.position.y, transform.position.z), transform.rotation, new Vector3(1, 0.01f, 1));
            Gizmos.DrawSphere(Vector3.zero, targetDistance);
            Gizmos.matrix = oldMatrix;
        }
    }
}
