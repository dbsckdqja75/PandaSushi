using UnityEngine;

public class EnvDoor : MonoBehaviour
{
    bool isBlocking = false;
    
    [SerializeField] Animator animator;

    [SerializeField] Transform fxPoint;
    [SerializeField] GameObject kickFxPrefab;

    public void Open()
    {
        if (isBlocking) return;
        
        animator.SetBool("isOpen" , true);
        
        CancelInvoke();
        Invoke("Close", 2f);
    }

    public void Close()
    {
        animator.SetBool("isOpen" , false);
    }

    public void KeepOpen()
    {
        animator.SetBool("isOpen" , true);
    }
    
    public void Block()
    {
        isBlocking = true;
        
        CancelInvoke();
        animator.SetBool("isOpen" , false);
    }

    public void Unblock()
    {
        isBlocking = false;
    }

    public void Kick()
    {
        CancelInvoke();
        Unblock();
        
        animator.SetBool("isOpen" , false);
        animator.SetTrigger("OnKick");
        
        ObjectPool.Instance.SpawnFX(kickFxPrefab, fxPoint.position, fxPoint.rotation, fxPoint).Hide(2f, true);
    }

    public void ResetMoiton()
    {
        animator.ResetTrigger("OnKick");
        animator.SetTrigger("OnReset");
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }
}
