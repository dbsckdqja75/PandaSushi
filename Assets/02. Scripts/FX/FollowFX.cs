using UnityEngine;

public class FollowFX : WorldFX
{
    Transform targetTrf;

    void Update()
    {
        if (targetTrf != null)
        {
            UpdatePositionToUGUI(targetTrf.position);
        }
    }

    public void SetTarget(Transform target)
    {
        targetTrf = target;
        Update();
    }
}
