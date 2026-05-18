using UnityEngine;

[CreateAssetMenu(fileName = "GamepadGuidePreset", menuName = "Scriptable Object/GamepadGuidePreset")]
public class GamepadGuideData : ScriptableObject
{
    [SerializeField] GameObject guidePrefab;

    public GameObject GetPrefab()
    {
        return guidePrefab;
    }
}