using UnityEngine;

public class EnvDoor : MonoBehaviour
{
    bool isBlocking = false;
    
    [SerializeField] Animator animator;
    
    // FIXME: FxPool로 관리
    [SerializeField] GameObject testFX1;
    [SerializeField] GameObject testFX2;

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
        
        testFX1.SetActive(true);
        testFX2.SetActive(true);
    }

    public void ResetMoiton()
    {
        animator.ResetTrigger("OnKick");
        animator.SetTrigger("OnReset");
        
        testFX1.SetActive(false);
        testFX2.SetActive(false);
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }
}
