using UnityEngine;

public class EventCustomer : Customer
{
    protected Coroutine eventLogic = null;

    protected override void OnDisable()
    {
        this.StopAllCoroutines();
        if (eventLogic != null)
        {
            eventLogic.Stop(this);
            eventLogic = null;
        }
        
        base.OnDisable();
    }
}
