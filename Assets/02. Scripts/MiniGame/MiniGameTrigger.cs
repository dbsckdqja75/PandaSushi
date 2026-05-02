using Action = System.Action;
using UnityEngine;

public class MiniGameTrigger : MonoBehaviour
{
    [SerializeField] EMiniGameType[] targetGameType = new [] { EMiniGameType.DeliveryRide };
    
    [Space(10)]
    [SerializeField] int triggerStack = 4;
    [SerializeField] float triggerChance = 0.3f;

    int stack = 0;
    
    public void AccrueStack()
    {
        stack += 1;
    }

    public bool TryTrigger(Action onSuccess, Action onFail)
    {
        if (stack >= triggerStack)
        {
            stack = 0;
            
            if (RandomExtensions.ProbabilityBool(triggerChance))
            {
                stack = 0;
                
                StageManager.Instance.StartMiniGame(targetGameType[Random.Range(0, targetGameType.Length)], onSuccess, onFail);
                return true;
            }
        }

        return false;
    }

    public void ResetStack()
    {
        stack = 0;
    }
}
