using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoSingleton<ObjectPool>
{
    Dictionary<GameObject, List<GameObject>> objectPool = new();
    Dictionary<GameObject, List<WorldFX>> effectPool = new();

    public GameObject Spawn(GameObject prefab, Transform parent, bool onActive = true)
    {
        var obj = Spawn(prefab, parent.position, Quaternion.identity, onActive);
        obj.transform.SetParent(parent);
        obj.transform.SetScale(prefab.transform.localScale);
        return obj;
    }

    public T Spawn<T>(GameObject prefab, Transform parent, bool onActive = true)
    {
        return Spawn(prefab, parent, onActive).GetComponent<T>();
    }
    
    public GameObject Spawn(GameObject prefab, Vector3 spawnPos)
    {
        return Spawn(prefab, spawnPos, Quaternion.identity);
    }

    public WorldFX SpawnFX(GameObject prefab, Vector3 spawnPos, Transform parent = null)
    {
        return SpawnFX(prefab, spawnPos, Quaternion.identity, parent);
    }

    public GameObject Spawn(GameObject prefab, Vector3 spawnPos, Quaternion spawnRot, bool onActive = true)
    {
        GameObject obj = null;
        if (objectPool.ContainsKey(prefab))
        {
            int poolCount = objectPool[prefab].Count;
            if (poolCount > 0)
            {
                for (int i = 0; i < poolCount; i++)
                {
                    if (objectPool[prefab][i] != null && objectPool[prefab][i].activeSelf == false)
                    {
                        obj = objectPool[prefab][i];
                        break;
                    }
                }
            }
        }
        else
        {
            objectPool.Add(prefab, new List<GameObject>());
        }
        
        if (obj == null)
        {
            obj = Instantiate(prefab, spawnPos, spawnRot);
            objectPool[prefab].Add(obj);
        }
        else
        {
            obj.transform.position = spawnPos;
            obj.transform.rotation = spawnRot;
        }

        obj.SetActive(onActive);
        return obj;
    }

    public WorldFX SpawnFX(GameObject prefab, Vector3 spawnPos, Quaternion spawnRot, Transform parent)
    {
        WorldFX fx = null;
        if (effectPool.ContainsKey(prefab))
        {
            int poolCount = effectPool[prefab].Count;
            if (poolCount > 0)
            {
                for (int i = 0; i < poolCount; i++)
                {
                    if (effectPool[prefab][i] != null && effectPool[prefab][i].gameObject.activeSelf == false)
                    {
                        fx = effectPool[prefab][i];
                        fx.transform.SetParent(parent);
                        break;
                    }
                }
            }
        }
        else
        {
            effectPool.Add(prefab, new List<WorldFX>());
        }
        
        if (fx == null)
        {
            fx = Instantiate(prefab, spawnPos, spawnRot, parent).GetComponent<WorldFX>();
            effectPool[prefab].Add(fx);
        }
        else
        {
            fx.transform.position = spawnPos;
            fx.transform.rotation = spawnRot;
        }

        fx.gameObject.SetActive(true);
        return fx;
    }

    public void AllClear()
    {
        foreach (var poolPrefab in objectPool.Keys)
        {
            ClearPool(poolPrefab);
        }
        
        foreach (var poolPrefab in effectPool.Keys)
        {
            ClearFxPool(poolPrefab);
        }
        
        objectPool.Clear();
        effectPool.Clear();
    }

    public void ClearPool(GameObject prefab)
    {
        if (objectPool.ContainsKey(prefab))
        {
            foreach (GameObject obj in objectPool[prefab])
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            
            objectPool[prefab].Clear();
        }
    }

    public void ClearFxPool(GameObject prefab)
    {
        if (effectPool.ContainsKey(prefab))
        {
            foreach (WorldFX obj in effectPool[prefab])
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            
            effectPool[prefab].Clear();
        }
    }
}
