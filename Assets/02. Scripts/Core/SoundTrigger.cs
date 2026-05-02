using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    [SerializeField] string clipName;

    void OnEnable()
    {
        SoundManager.Instance.PlaySound(clipName);
    }
}
