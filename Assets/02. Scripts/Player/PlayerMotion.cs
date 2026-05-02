using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = this.GetComponent<Animator>();
    }

    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     anim.SetInteger("State", 0);
        //     anim.SetTrigger("OnUpdate");
        //     
        //     OnIdle();
        // }
        //
        // if(Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     OnHold();
        // }
        //
        // if(Input.GetKeyDown(KeyCode.Alpha3))
        // {
        //     anim.SetInteger("State", 2); // NOTE : Assembly
        //     anim.SetTrigger("OnUpdate");
        // }
        //
        // if(Input.GetKeyDown(KeyCode.Alpha4))
        // {
        //     anim.SetInteger("State", 3); // NOTE : Chop
        //     anim.SetTrigger("OnUpdate");
        // }
        //
        // if(Input.GetKeyDown(KeyCode.Alpha5))
        // {
        //     anim.SetInteger("State", 4); // NOTE : Pan
        //     anim.SetTrigger("OnUpdate");
        // }
    }

    public void OnWalk()
    {
        anim.SetBool("isWalking", true);
    }

    public void OnIdle()
    {
        anim.SetBool("isWalking", false);
    }

    public void OnHold()
    {
        anim.SetInteger("State", 1); // NOTE : Holding
        anim.SetTrigger("OnUpdate");
    }

    public void OnRelease()
    {
        anim.SetInteger("State", 0);
        anim.SetTrigger("OnUpdate");
    }

    public void OnAssemble()
    {
        anim.SetInteger("State", 2); // NOTE : Assembly
        anim.SetTrigger("OnUpdate");
    }

    public void OnAim()
    {
        anim.SetInteger("State", 5);
        anim.SetTrigger("OnUpdate");
    }

    public void OnThrow()
    {
        anim.SetInteger("State", 0);
        anim.SetTrigger("OnThrow");
    }
}
