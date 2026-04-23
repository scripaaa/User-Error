using UnityEngine;

public class AudioInitializer : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip[] musicTracks;
    public AudioClip[] footstepSounds;
    public AudioClip[] slimeMovementSounds;
    public AudioClip[] slimeAttackSounds;

    [Header("Settings")]
    public int defaultTrackIndex = 0;
    public bool destroyAfterSetup = true;

    void Awake()
    {
        InitializeAudio();
    }

    void InitializeAudio()
    {
        if (AudioController.Instance != null)
        {
            AudioController.Instance.PlayMusic(defaultTrackIndex);

            if (destroyAfterSetup)
            {
                Destroy(gameObject);
            }
            return;
        }

        GameObject audioGO = new GameObject("AudioController");
        AudioController controller = audioGO.AddComponent<AudioController>();
        DontDestroyOnLoad(audioGO);

        controller.musicTracks = musicTracks;
        controller.footstepSounds = footstepSounds;
        controller.slimeMovementSounds = slimeMovementSounds;
        controller.slimeAttackSounds = slimeAttackSounds;
        controller.defaultTrackIndex = defaultTrackIndex;

        controller.LoadVolumeSettings();
        controller.PlayMusic(defaultTrackIndex);

        if (destroyAfterSetup)
        {
            Destroy(gameObject);
        }
    }
}
