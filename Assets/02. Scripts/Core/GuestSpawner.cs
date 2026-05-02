using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuestSpawner : MonoBehaviour
{
    bool isStarted;
    
    float spawnTimer;
    int specialSpawnTrigger;
    
    [SerializeField] float spawnInterval = 20;
    [SerializeField] int minSpecialSpawnInterval = 3;
    [SerializeField] int maxSpecialSpawnInterval = 6;
    [SerializeField] GlobalState globalState;
    
    [SerializeField] List<GameObject> customerPrefabs;
    [SerializeField] List<GameObject> specialCustomerPrefabs;

    [Space(10)] 
    [SerializeField] List<EnvDoor> doors = new();
    [SerializeField] List<Transform> wayPoints = new();
    [SerializeField] List<ServeTable> tables = new();

    void OnEnable()
    {
        EventManager.GetEvent(EGameEvent.OnSoldOutOrder).Subscribe(ResetSpawnTimer);
    }

    void OnDisable()
    {
        EventManager.GetEvent(EGameEvent.OnSoldOutOrder).Unsubscribe(ResetSpawnTimer);
    }
    
    void Update()
    {
        if (isStarted)
        {
            spawnTimer -= Time.deltaTime;
            
            if (spawnTimer <= 0)
            {
                if (SpawnGuest())
                {
                    spawnTimer = spawnInterval * (2f - globalState.spawnTimeBonus);
                }
                else
                {
                    spawnTimer = 1f;
                }
            }
        }
    }

    public void StartSpawnTimer()
    {
        isStarted = true;
        
        spawnTimer = 0;
        specialSpawnTrigger = Random.Range(minSpecialSpawnInterval, maxSpecialSpawnInterval);
    }
    
    public void ResetSpawnTimer()
    {
        isStarted = false;
        
        spawnTimer = 0;
        specialSpawnTrigger = maxSpecialSpawnInterval;
    }

    public void ResetSpawner()
    {
        ResetSpawnTimer();
    }

    public bool SpawnEvent(GameObject prefab, int direction = -1)
    {
        if (direction == -1)
        {
            direction = GetEnterableRandomDoorIndex();
        }
        
        if (direction != -1)
        {
            List<Vector3> points = GenerateWayPoints(direction);
            NonSeatCustomer customer = ObjectPool.Instance.Spawn(prefab, points[0], Quaternion.identity).GetComponent<NonSeatCustomer>();

            doors[direction].Open();
            customer.Enter(doors[direction], points);
            return true;
        }

        return false;
    }
    
    public bool SpawnGuest()
    {
        int direction = GetEnterableRandomDoorIndex(); // NOTE : 왼쪽 또는 오른쪽 문
        var targetTable = GetEmptyTable();
        if (targetTable != null && direction != -1)
        {
            targetTable.ReserveTable();
            
            List<Vector3> points = GenerateWayPoints(direction, targetTable.transform);

            doors[direction].Open();

            GameObject prefab = null;
            if(specialSpawnTrigger <= 0) // if (DEBUG_spawnCount >= DEBUG_targetEventSpawnCount)
            {
                prefab = specialCustomerPrefabs[Random.Range(0, specialCustomerPrefabs.Count)];
                
                specialSpawnTrigger = Random.Range(minSpecialSpawnInterval, maxSpecialSpawnInterval);
            }
            else
            {
                prefab = customerPrefabs[Random.Range(0, customerPrefabs.Count)];
                
                specialSpawnTrigger -= 1;
            }

            Customer customer = ObjectPool.Instance.Spawn(prefab, points[0], Quaternion.identity).GetComponent<Customer>();
            customer.Enter(targetTable, doors[direction], points);
            
            return true;
        }

        return false;
    }
    
    public ServeTable GetEmptyTable()
    {
        return tables.Where(x => x.CanSit()).FirstOrDefault();
    }

    public List<Vector3> GenerateWayPoints(int direction, Transform endPoint)
    {
        List<Vector3> points = new();
        
        int startIdx = (direction == 0 ? 0 : (wayPoints.Count / 2));
        int endIdx = (direction == 0 ? (wayPoints.Count / 2) : wayPoints.Count);
        
        for (int i = startIdx; i < endIdx; i++)
        {
            points.Add(wayPoints[i].position);
        }
        
        Vector3 endPos = endPoint.position;
        endPos.SetY(points[points.Count - 1].y);
        endPos.SetZ(points[points.Count - 1].z);
        
        points.Add(endPos);
        return points;
    }
    
    public List<Vector3> GenerateWayPoints(int direction)
    {
        List<Vector3> points = new();
        
        int startIdx = (direction == 0 ? 0 : (wayPoints.Count / 2));
        int endIdx = (direction == 0 ? (wayPoints.Count / 2) : wayPoints.Count);
        for (int i = startIdx; i < endIdx; i++)
        {
            points.Add(wayPoints[i].position);
        }
        
        startIdx = (direction == 1 ? 0 : (wayPoints.Count / 2));
        endIdx = (direction == 1 ? (wayPoints.Count / 2) : wayPoints.Count);
        for (int i = endIdx-1; i >= startIdx; i--)
        {
            points.Add(wayPoints[i].position);
        }

        return points;
    }

    int GetEnterableRandomDoorIndex()
    {
        List<int> numbers = new(); 
        for (int i = 0; i < doors.Count; i++)
        {
            if (doors[i].IsBlocking() == false)
            {
                numbers.Add(i);
            }
        }

        if (numbers.Count > 0)
        {
            return numbers[Random.Range(0, numbers.Count)];
        }
        
        return -1;
    }
}
