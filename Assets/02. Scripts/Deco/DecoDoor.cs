using UnityEngine;

public class DecoDoor : DecoPoint
{
    [SerializeField] Transform[] targetPivots;
    GameObject[] currentModels = new GameObject[2];

    public override void UpdateDesign(int idx)
    {
        ResetDesign();
        
        if (idx >= 0)
        {
            DecoData decoData = PandaResources.Instance.GetDecoData(targetID, idx);

            currentDesignIdx = idx;
            if (decoData != null)
            {
                GameObject prefab = decoData.GetModelPrefab();
                if (prefab != null)
                {
                    for (int i = 0; i < targetPivots.Length; i++)
                    {
                        currentModels[i] = Instantiate(prefab, targetPivots[i]);
                        currentModels[i].transform.localPosition = decoData.GetOffsetPos();
                        currentModels[i].transform.localEulerAngles = decoData.GetOffsetAngle();
                    }
                }
            }
        }
    }
    
    protected override void ResetDesign()
    {
        if (currentModels.Length <= 0)
        {
            currentModels = new GameObject[targetPivots.Length];
        }
        
        currentDesignIdx = -1;            
        foreach (GameObject model in currentModels)
        {
            if (model != null)
            {
                Destroy(model);
            }
        }
    }
}
