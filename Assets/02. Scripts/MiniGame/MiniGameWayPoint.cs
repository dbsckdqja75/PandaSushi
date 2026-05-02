using System;
using UnityEngine;

public class MiniGameWayPoint : MonoBehaviour
{
    [SerializeField] MiniGameWayPoint[] nextPoints = new MiniGameWayPoint[4]; // 상하좌우

    RectTransform rectTrf;
    
    void Awake()
    {
        rectTrf = this.GetComponent<RectTransform>();
    }

    public Vector3 GetPosition()
    {
        #if UNITY_EDITOR
        if (rectTrf == null)
        {
            rectTrf = this.GetComponent<RectTransform>();
        }
        #endif
        
        return rectTrf.position;
    }

    public bool HasDirectionPoint(int direction)
    {
        return (nextPoints[direction] != null);
    }

    public MiniGameWayPoint GetNextPoint(int direction)
    {
        return nextPoints[direction];
    }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (nextPoints[0])
        {
            Gizmos.DrawLine(GetPosition(), nextPoints[0].GetPosition());
        }

        Gizmos.color = Color.green;
        if (nextPoints[1])
        {
            Gizmos.DrawLine(GetPosition(), nextPoints[1].GetPosition());
        }
        
        Gizmos.color = Color.yellow;
        if (nextPoints[2])
        {
            Gizmos.DrawLine(GetPosition(), nextPoints[2].GetPosition());
        }
        
        Gizmos.color = Color.magenta;
        if (nextPoints[3])
        {
            Gizmos.DrawLine(GetPosition(), nextPoints[3].GetPosition());
        }
    }
    #endif
}
