using UnityEngine;

public class DecoColor : DecoPoint
{
    [SerializeField] MeshRenderer[] targetMeshRenderers;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material[] materials;

    public override void UpdateDesign(int idx)
    {
        ResetDesign();
        
        if (idx >= 0)
        {
            currentDesignIdx = idx;
            if (materials[currentDesignIdx] != null)
            {
                foreach (var renderer in targetMeshRenderers)
                {
                    renderer.sharedMaterial = materials[currentDesignIdx];
                }
            }
        }
    }
    
    protected override void ResetDesign()
    {
        if (defaultMaterial != null)
        {
            currentDesignIdx = -1;
            foreach (var renderer in targetMeshRenderers)
            {
                renderer.sharedMaterial = defaultMaterial;
            }
        }
    }

    public override int GetDesignCount()
    {
        return materials.Length;
    }
}
