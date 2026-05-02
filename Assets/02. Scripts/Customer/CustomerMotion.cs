using UnityEngine;

public class CustomerMotion : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void OnIdle(bool onReset = false)
    {
        animator.SetBool("isMoving", false);
        animator.SetInteger("MoveType", -1);

        if (onReset)
        {
            animator.Rebind();
            animator.Update(0f);
            animator.enabled = false;
            animator.enabled = true;
        }
    }

    public void OnMove(int walkType = 0)
    {
        animator.SetBool("isMoving", true);
        animator.SetInteger("MoveType", walkType);
    }

    public void OnFail()
    {
        animator.SetTrigger("OnFail");
        animator.SetBool("isMoving", false);
    }

    public void OnSit()
    {
        animator.SetTrigger("OnSitDown");
        animator.SetBool("isMoving", false);
    }

    public void OnStaySit()
    {
        animator.SetInteger("MoveType", -1);
    }

    public void OnEat()
    {
        animator.SetInteger("MoveType", 0);
        animator.SetTrigger("OnSitEat");
        animator.SetBool("isMoving", false);
    }

    public void OnStandUp()
    {
        animator.SetTrigger("OnStandUp");
        animator.SetBool("isMoving", false);
    }
    
    public void OnTantrum()
    {
        animator.SetTrigger("OnTantrum");
        animator.SetBool("isMoving", false);
    }

    public void OnHijack()
    {
        animator.SetTrigger("OnHijack");
        animator.SetBool("isMoving", false);
    }

    public void OnPuke()
    {
        animator.SetTrigger("OnPuke");
        animator.SetBool("isMoving", false);
    }

    public void OnSteal()
    {
        animator.SetTrigger("OnSteal");
        animator.SetBool("isMoving", false);
    }

    public void OnBboying()
    {
        animator.SetTrigger("OnBboying");
        animator.SetBool("isMoving", false);
    }

    public void OnBlocking()
    {
        animator.SetTrigger("OnBlocking");
        animator.SetBool("isMoving", false);
    }
    
    public void OnKicked()
    {
        animator.SetTrigger("OnKicked");
        animator.SetBool("isMoving", false);
    }
    
    public void OnHit(bool isSeated = false)
    {
        animator.SetTrigger(isSeated ? "OnSitHit" : "OnHit");
        animator.SetBool("isMoving", false);
    }

    public void OnDead()
    {
        animator.SetTrigger("OnDead");
        animator.SetBool("isMoving", false);
    }

    public bool IsPlaying(string clipName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(clipName) &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
    }
}
