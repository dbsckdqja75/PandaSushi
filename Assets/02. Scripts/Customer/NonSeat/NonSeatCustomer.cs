using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonSeatCustomer : MonoBehaviour
{
    [SerializeField] protected float defaultSpeed = 6f;
    
    [SerializeField] protected CustomerMotion motion;
    [SerializeField] protected BoxCollider col;
    
    [SerializeField] protected GameObject hitFxPrefab;
    
    protected EnvDoor exitDoor;
    protected List<Vector3> wayPoints = new();
    
    protected float extraSpeed = 0f;
    protected PandaSkinHandler skinHandler;

    private void Awake()
    {
        skinHandler = this.GetComponent<PandaSkinHandler>();
    }

    protected virtual void OnEnable()
    {
        col.enabled = false;
    }

    protected virtual void OnDisable()
    {
        skinHandler.RestoreSkin();
        
        exitDoor = null;
            
        extraSpeed = 0;
    }
    
    public virtual void Enter(EnvDoor door, List<Vector3> wayPoints)
    {
        this.exitDoor = door;
        
        EntranceMotion(wayPoints).Start(this);
    }

    public virtual void ForceStop()
    {
        this.StopAllCoroutines();
        
        motion.OnIdle(true);
    }
    
    public virtual void ForceDestroy()
    {
        this.StopAllCoroutines();
        this.gameObject.SetActive(false);
    }

    protected void OnReward(int starValue)
    {
        CurrencyManager.Instance.RewardCurrency(ECurrencyType.Star, starValue);
        EventManager.GetEvent<int>(EGameEvent.OnUpdateReview).Invoke(starValue);
    }

    protected void OnPenalty(int penaltyValue)
    {
        CurrencyManager.Instance.Purchase(ECurrencyType.Star, penaltyValue);
        EventManager.GetEvent<int>(EGameEvent.OnUpdateReview).Invoke(-penaltyValue);
    }
    
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            this.col.enabled = false;
            
            OnHit();
        }
    }
    
    protected virtual void OnHit()
    {
        skinHandler.RestoreSkin();
        
        this.StopAllCoroutines();
        HitMotion().Start(this);
        
        OnReward(1);
    }
    
    protected virtual IEnumerator HitMotion()
    {
        motion.OnHit();
        
        skinHandler.ChangeSkinColor(Color.white, false, 0.8f);
        skinHandler.RestoreSkin(true);

        ObjectPool.Instance.SpawnFX(hitFxPrefab, transform.position).Hide(1f);
        
        SoundManager.Instance.PlaySound("SFX_Hit", 1, 2);
        
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => motion.IsPlaying("Hit") == false);
        yield return new WaitForSeconds(0.25f);

        motion.OnDead();
        yield return new WaitForSeconds(3f);

        transform.position = wayPoints[0];
        ForceDestroy();
        
        yield break;
    }

    protected virtual IEnumerator EventMotion()
    {
        yield break;
    }

    protected IEnumerator EntranceMotion(List<Vector3> points)
    {
        float moveSpeed = (defaultSpeed + extraSpeed);
        motion.OnMove();
        
        for (int i = 0; i < points.Count; i++)
        {
            points[i].Set(points[i].x, transform.position.y, points[i].z);
        }
        
        transform.position = points[0];

        Quaternion lookRotation = transform.rotation;
        for (int i = 1; i < points.Count; i++)
        {
            lookRotation = Quaternion.LookRotation(points[i] - transform.position);
            while (Vector3.Distance(transform.position, points[i]) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, points[i], moveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, moveSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            transform.position = points[i];
            yield return new WaitForEndOfFrame();
        }

        wayPoints = points;
        wayPoints.Reverse();

        EventMotion().Start(this);
        yield break;
    }
    
    protected IEnumerator LeaveMotion(List<Vector3> points)
    {
        float moveSpeed = (defaultSpeed + extraSpeed);
        motion.OnMove(moveSpeed > defaultSpeed ? 1 : 0);
        
        transform.position = points[0];
        Quaternion lookRotation = transform.rotation;

        bool isOpened = false;
        for (int i = 1; i < points.Count; i++)
        {
            lookRotation = Quaternion.LookRotation(points[i] - transform.position);
            while (Vector3.Distance(transform.position, points[i]) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, points[i], moveSpeed * Time.smoothDeltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, moveSpeed * Time.deltaTime);
                
                if (VectorExtensions.IsNearDistance(transform.position, exitDoor.transform.position, 2) && isOpened == false)
                {
                    isOpened = true;
                    
                    exitDoor.Open();
                }
                
                yield return new WaitForEndOfFrame();
            }

            transform.position = points[i];
            yield return new WaitForEndOfFrame();
        }

        ForceDestroy();
        yield break;
    }
}
