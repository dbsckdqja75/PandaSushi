using System.Collections.Generic;
using UnityEngine;

public class GuideUI : MonoSingleton<GuideUI>
{
    [SerializeField] Transform listTrf;
    [SerializeField] List<GameObject> originalGuidePrefabs = new List<GameObject>();

    Dictionary<EGuideType, GameObject> guidePrefabs = new Dictionary<EGuideType, GameObject>();
    Dictionary<EGuideType, GuideElement> guideList = new Dictionary<EGuideType, GuideElement>();

    protected override void Init()
    {
        guidePrefabs.Clear();

        for(int i = 0; i < originalGuidePrefabs.Count; i++)
        {
            guidePrefabs.Add((EGuideType)i, originalGuidePrefabs[i]);
        }
    }

    public GuideElement ShowGuide(EGuideType type, Transform target, Vector3 offset)
    {
        if(guideList.ContainsKey(type) == false)
        {
            guideList.Add(type, null);
        }

        Vector3 spawnPos = Camera.main.WorldToScreenPoint(target.transform.position);

        if(guideList[type] == null)
        {
            guideList[type] = Instantiate(guidePrefabs[type], spawnPos, Quaternion.identity, listTrf).GetComponent<GuideElement>();
        }

        guideList[type].transform.position = spawnPos;
        guideList[type].UpdateTarget(target);
        guideList[type].UpdateOffset(offset);
        
        guideList[type].gameObject.SetActive(true);
        return guideList[type];
    }

    public void HideGuide(EGuideType type)
    {
        if(guideList.ContainsKey(type))
        {
            if(guideList[type] != null)
            {
                guideList[type].gameObject.SetActive(false);
            }
        }
    }
}
