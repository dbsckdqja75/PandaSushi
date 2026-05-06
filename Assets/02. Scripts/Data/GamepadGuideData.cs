using UnityEngine;

[CreateAssetMenu(fileName = "GamepadGuidePreset", menuName = "Scriptable Object/GamepadGuidePreset")]
public class GamepadGuideData : ScriptableObject
{
    [SerializeField] GameObject guidePrefab;
    // [SerializeField] Sprite[] inputSprites;

    public GameObject GetPrefab()
    {
        return guidePrefab;
    }
}