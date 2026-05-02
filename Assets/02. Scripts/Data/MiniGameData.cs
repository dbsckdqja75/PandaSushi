using UnityEngine;

[CreateAssetMenu(fileName = "MiniGameData", menuName = "Scriptable Object/MiniGameData")]
public class MiniGameData : ScriptableObject
{
    [SerializeField] EMiniGameType gameType;
    [SerializeField] GameObject prefab;

    public bool IsType(EMiniGameType targetType)
    {
        return (gameType == targetType);
    }
    
    public GameObject GetPrefab()
    {
        return prefab;
    }
}
