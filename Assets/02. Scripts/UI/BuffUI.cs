using UnityEngine;

public class BuffUI : MonoBehaviour
{
    [SerializeField] RectTransform listTrf;
    [SerializeField] GameObject iconPrefab;
        
    [Space(10)]
    [SerializeField] Sprite[] iconSprites;

    public BuffIcon AddIcon(int iconIdx)
    {
        BuffIcon icon = ObjectPool.Instance.Spawn(iconPrefab, listTrf, false).GetComponent<BuffIcon>();
        icon.UpdateIcon(iconSprites[iconIdx]);
        icon.gameObject.SetActive(true);

        return icon;
    }
}