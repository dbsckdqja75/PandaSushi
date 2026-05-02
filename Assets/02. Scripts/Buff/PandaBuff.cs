using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PandaBuff : MonoBehaviour
{
    [SerializeField] BuffUI buffUI;
    [SerializeField] GlobalState globalState;

    [SerializeField] GameObject[] buffPrefabs;
    Dictionary<EBuffType, Buff> buffList = new();

    public void Grant(EBuffType buffType)
    {
        Debug.LogWarning("버프 부여 => " + buffType.ToString());
        
        if (buffList.ContainsKey(buffType))
        {
            if (buffList[buffType] != null)
            {
                buffList[buffType].Extend();
                return;
            }

            buffList.Remove(buffType);
        }

        Buff newBuff = Instantiate(buffPrefabs[(int)buffType]).GetComponent<Buff>();
        newBuff.Init(buffUI.AddIcon((int)buffType));
        
        buffList.Add(buffType, newBuff);
        
        SoundManager.Instance.PlaySound("SFX_Buff");
    }

    public void Expire(EBuffType buffType)
    {
        if (buffList.ContainsKey(buffType))
        {
            if (buffList[buffType] != null)
            {
                buffList[buffType].Finish();
            }

            buffList.Remove(buffType);
        }
    }

    public void ForceClear()
    {
        foreach (EBuffType buffType in buffList.Keys.ToList())
        {
            Expire(buffType);
        }
        
        buffList.Clear();
    }
}
