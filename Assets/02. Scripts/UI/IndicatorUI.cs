using System.Collections.Generic;
using UnityEngine;

public enum IndicatorType
{
    CookProgress = 0,
    OrderSheet = 1,
    EventProgress = 2,
}

public class IndicatorUI : MonoSingleton<IndicatorUI>
{
    [SerializeField] Transform listTrf;
    [SerializeField] List<GameObject> originalIndicatorPrefabs = new List<GameObject>();

    Dictionary<IndicatorType, GameObject> indicatorPrefabs = new Dictionary<IndicatorType, GameObject>();

    void Awake()
    {
        indicatorPrefabs.Clear();

        for(int i = 0; i < originalIndicatorPrefabs.Count; i++)
        {
            indicatorPrefabs.Add((IndicatorType)i, originalIndicatorPrefabs[i]);
        }
    }

    public IndicatorElement ShowIndicator(IndicatorType type, Transform target, Vector3 offset)
    {
        if(indicatorPrefabs.ContainsKey(type))
        {
            IndicatorElement indicator = ObjectPool.Instance.Spawn(indicatorPrefabs[type], listTrf, false).GetComponent<IndicatorElement>();
            indicator.UpdateTarget(target);
            indicator.UpdateOffset(offset);
            indicator.gameObject.SetActive(true);
            
            return indicator;
        }

        return null;
    }
}
