using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rider : MonoBehaviour
{
    [SerializeField] float defaultSpeed = 6f;
    [SerializeField] GlobalState globalState;

    PickupZone targetZone;
    EnvDoor exitDoor;
    
    List<Vector3> wayPoints = new List<Vector3>();

    void OnDisable()
    {
        targetZone = null;
    }

    public void Enter(PickupZone targetTable, EnvDoor door, List<Vector3> wayPoints)
    {
        if (targetTable != null)
        {
            this.targetZone = targetTable;
            this.exitDoor = door;

            door.Kick();
            EntranceMotion(wayPoints).Start(this);
            
            EventManager.GetEvent(EGameEvent.OnKickedLeftDoor).Invoke();
        }
    }

    public void ForceDestroy()
    {
        this.StopAllCoroutines();
        
        exitDoor?.ResetMoiton();
        this.gameObject.SetActive(false);
    }
    
    IEnumerator EntranceMotion(List<Vector3> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].Set(points[i].x, transform.position.y, points[i].z);
        }
        
        transform.position = points[0];

        for (int i = 1; i < points.Count; i++)
        {
            while (Vector3.Distance(transform.position, points[i]) > 0.1f)
            {
                transform.LookAt(points[i]);
                transform.position = Vector3.MoveTowards(transform.position, points[i], defaultSpeed * globalState.riderSpeedMultiple * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            transform.position = points[i];
            yield return new WaitForEndOfFrame();
        }

        wayPoints = points;
        wayPoints.Reverse();
        
        targetZone.OnPickUp();
        yield return LeaveMotion(wayPoints);
        yield break;
    }
    
    IEnumerator LeaveMotion(List<Vector3> points)
    {
        transform.position = points[0];

        for (int i = 1; i < points.Count; i++)
        {
            while (Vector3.Distance(transform.position, points[i]) > 0.1f)
            {
                transform.LookAt(points[i]);
                transform.position = Vector3.MoveTowards(transform.position, points[i], defaultSpeed * globalState.riderSpeedMultiple * Time.smoothDeltaTime);

                yield return new WaitForEndOfFrame();
            }

            transform.position = points[i];
            yield return new WaitForEndOfFrame();
        }
        
        exitDoor.ResetMoiton();
        this.gameObject.SetActive(false);
        
        yield break;
    }
}
