using UnityEngine;
using Random = UnityEngine.Random;

public class PukeDirtyFX : WorldFX
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] pukeSprites;

    void OnEnable()
    {
        spriteRenderer.sprite = pukeSprites[Random.Range(0, pukeSprites.Length)];
        spriteRenderer.flipX = RandomExtensions.RandomBool();
        spriteRenderer.flipY = RandomExtensions.RandomBool();
    }
}
