using UnityEngine;

public class AudioSetup : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Уничтожить этот объект после настройки")]
    public bool destroyAfterSetup = true;

    void Start()
    {
        SetupAudioController();

        if (destroyAfterSetup)
        {
            Destroy(gameObject);
        }
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

        LoadAudioClips(controller);

        Debug.Log("[AudioSetup] AudioController настроен успешно!");
    }

    void LoadAudioClips(AudioController controller)
    {
        AudioClip[] musicTracks = new AudioClip[4];
        musicTracks[0] = Resources.Load<AudioClip>("SomeSounds/Broken Promise Broken Dream 76 BPM Loop");
        musicTracks[1] = Resources.Load<AudioClip>("SomeSounds/Cave of the Sisterhood 131 BPM Loop");
        musicTracks[2] = Resources.Load<AudioClip>("SomeSounds/Silver Creek 117 BPM Loop");

        if (musicTracks[0] == null)
        {
            musicTracks[0] = LoadClipFromPath("Assets/SomeSounds/Broken Promise Broken Dream 76 BPM Loop.wav");
            musicTracks[1] = LoadClipFromPath("Assets/SomeSounds/Cave of the Sisterhood 131 BPM Loop.wav");
            musicTracks[2] = LoadClipFromPath("Assets/SomeSounds/Silver Creek 117 BPM Loop.wav");
        }

        controller.musicTracks = musicTracks;

        AudioClip[] footsteps = new AudioClip[2];
        footsteps[0] = LoadClipFromPath("Assets/SomeSounds/step1.mp3");
        footsteps[1] = LoadClipFromPath("Assets/SomeSounds/step2.mp3");
        controller.footstepSounds = footsteps;

        AudioClip[] slimeMove = new AudioClip[2];
        slimeMove[0] = LoadClipFromPath("Assets/SomeSounds/slime1.mp3");
        slimeMove[1] = LoadClipFromPath("Assets/SomeSounds/slime2.mp3");
        controller.slimeMovementSounds = slimeMove;

        controller.slimeAttackSounds = slimeMove;

        LogMissingClips(musicTracks, "Music");
        LogMissingClips(footsteps, "Footsteps");
        LogMissingClips(slimeMove, "Slime Movement");
    }

    AudioClip LoadClipFromPath(string path)
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(path);
#else
        string resourcePath = path.Replace("Assets/", "").Replace(".wav", "").Replace(".mp3", "");
        return Resources.Load<AudioClip>(resourcePath);
#endif
    }

    void LogMissingClips(AudioClip[] clips, string category)
    {
        int missingCount = 0;
        foreach (var clip in clips)
        {
            if (clip == null) missingCount++;
        }
    }
}
