using UnityEngine;

public class AudioSetupRuntime : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip[] musicTracksManual;
    public AudioClip[] footstepSoundsManual;
    public AudioClip[] slimeMovementSoundsManual;
    public AudioClip[] slimeAttackSoundsManual;

    [Header("Settings")]
    public bool destroyAfterSetup = true;

    void Awake()
    {
        SetupAudioController();
    }

    void SetupAudioController()
    {
        AudioController controller = FindObjectOfType<AudioController>();

        if (controller == null)
        {
            GameObject audioGO = new GameObject("AudioController");
            controller = audioGO.AddComponent<AudioController>();
            DontDestroyOnLoad(audioGO);
        }

        SetupMusic(controller);
        SetupFootsteps(controller);
        SetupSlimeSounds(controller);

        if (destroyAfterSetup)
        {
            Destroy(gameObject);
        }
    }

    void SetupMusic(AudioController controller)
    {
        if (musicTracksManual != null && musicTracksManual.Length > 0 && musicTracksManual[0] != null)
        {
            controller.musicTracks = musicTracksManual;
            return;
        }

        controller.musicTracks = new AudioClip[0];
    }

    void SetupFootsteps(AudioController controller)
    {
        if (footstepSoundsManual != null && footstepSoundsManual.Length > 0 && footstepSoundsManual[0] != null)
        {
            controller.footstepSounds = footstepSoundsManual;
            return;
        }

        controller.footstepSounds = new AudioClip[0];
    }

    void SetupSlimeSounds(AudioController controller)
    {
        if (slimeMovementSoundsManual != null && slimeMovementSoundsManual.Length > 0 && slimeMovementSoundsManual[0] != null)
        {
            controller.slimeMovementSounds = slimeMovementSoundsManual;
        }
        else
        {
            controller.slimeMovementSounds = new AudioClip[0];
        }

        if (slimeAttackSoundsManual != null && slimeAttackSoundsManual.Length > 0 && slimeAttackSoundsManual[0] != null)
        {
            controller.slimeAttackSounds = slimeAttackSoundsManual;
        }
        else
        {
            controller.slimeAttackSounds = controller.slimeMovementSounds;
        }
    }
}
