using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    public void PlayAttackSound(AudioClip[] clips)
    {
        if (clips.Length == 0)
            return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        _audioSource.PlayOneShot(clip);
    }

    public void PlayHurtSound(AudioClip[] clips)
    {
        if (clips.Length == 0)
            return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        _audioSource.PlayOneShot(clip);
    }

    public void PlayDeathSound(AudioClip[] clips)
    {
        if (clips.Length == 0)
            return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        _audioSource.PlayOneShot(clip);
    }

    public void PlayJumpSound(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}
