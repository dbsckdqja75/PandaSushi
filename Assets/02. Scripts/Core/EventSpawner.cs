using System.Collections.Generic;
using UnityEngine;

public class EventSpawner : MonoBehaviour
{
    bool isStarted;

    float spawnTimer;
    
    [SerializeField] float minSpawnInterval = 25f;
    [SerializeField] float maxSpawnInterval = 35f;
    [SerializeField] GlobalState globalState;
    
    [SerializeField] GuestSpawner guestSpawner;
    [SerializeField] List<GameObject> extraEventCustomerPrefabs;
    [SerializeField] List<GameObject> uniqueEventCustomerPrefabs;

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
                if (Spawn())
                {
                    spawnTimer = (Random.Range(minSpawnInterval, maxSpawnInterval) * (2f - globalState.eventTimeBonus));
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
        
        spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
    }
    
    public void ResetSpawnTimer()
    {
        isStarted = false;
        
        spawnTimer = 0;
    }
    
    bool Spawn()
    {
        bool isUnique = RandomExtensions.RandomBool();
        int direction = isUnique ? 1 : -1;

        GameObject prefab = isUnique
            ? uniqueEventCustomerPrefabs[Random.Range(0, uniqueEventCustomerPrefabs.Count)]
            : extraEventCustomerPrefabs[Random.Range(0, extraEventCustomerPrefabs.Count)];

        return guestSpawner.SpawnEvent(prefab, direction);
    }
}
