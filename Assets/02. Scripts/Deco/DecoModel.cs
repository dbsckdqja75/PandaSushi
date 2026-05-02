using UnityEngine;

public class DecoModel : DecoPoint
{
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
                    currentModel = Instantiate(prefab, this.transform);
                    currentModel.transform.localPosition = decoData.GetOffsetPos();
                    currentModel.transform.localEulerAngles = decoData.GetOffsetAngle();
                }
            }
        }
    }
    
    protected override void ResetDesign()
    {
        if (currentModel != null)
        {
            currentDesignIdx = -1;
            Destroy(currentModel);
        }
    }
}
