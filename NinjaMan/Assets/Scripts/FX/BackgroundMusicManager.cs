using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] backgroundTracks;
    private AudioSource audioSource;

    private void Awake()
    {
        // Create a new GameObject with an AudioSource component
        GameObject audioSourceObject = new GameObject("BackgroundMusic");
        audioSource = audioSourceObject.AddComponent<AudioSource>();
        audioSource.loop = true;

        // Load the first background track and play it
        if (backgroundTracks.Length > 0)
        {
            audioSource.clip = backgroundTracks[0];
            audioSource.Play();
        }
    }

    private void Start()
    {
        // Stop the music if the scene is not active
        if (!gameObject.activeInHierarchy)
        {
            StopAudio();
        }
    }

    private void OnEnable()
    {
        // Resume playing the music when the script is enabled (scene becomes active)
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void OnDisable()
    {
        // Pause or stop the music when the script is disabled (scene becomes inactive)
        if (audioSource != null)
        {
            PauseAudio();
        }
    }

    private void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void PauseAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }
}
