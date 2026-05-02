using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AttachedAudioSource : MonoBehaviour
{
    AudioSource audioSource;

    void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public void Play(AudioClip clip, float volume)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }
}
