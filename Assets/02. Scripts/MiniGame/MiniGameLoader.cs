using System;
using System.Linq;
using UnityEngine;

public class MiniGameLoader : MonoBehaviour
{
    [SerializeField] RectTransform panelTrf;
    [SerializeField] MiniGameData[] miniGameData;

    MiniGame currentGame = null;

    public void Load(EMiniGameType gameType, Action<bool> onFinish)
    {
        if (currentGame != null)
        {
            return;
        }
        
        MiniGameData data = miniGameData.Where(x => x.IsType(gameType)).FirstOrDefault();
        if (data != null)
        {
            GameObject prefab = data.GetPrefab();
            currentGame = Instantiate(prefab, panelTrf).GetComponent<MiniGame>();
            currentGame.Init(onFinish);
        }
    }

    public void ForceReset()
    {
        if (currentGame != null)
        {
            currentGame.ForceCancel();
            currentGame = null;
        }
    }

    public bool IsPlaying()
    {
        return (currentGame != null);
    }
}
