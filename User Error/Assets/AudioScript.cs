using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip[] musicTracks;
    public int defaultTrackIndex = 0;

    [Header("UI")]
    public Slider musicSlider;

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip[] footstepSounds;
    public AudioClip[] slimeMovementSounds;
    public AudioClip[] slimeAttackSounds;

    [Range(0f, 1f)]
    public float musicVolume = 0.55f;

    [Range(0f, 1f)]
    public float sfxVolume = 0.67f;

    private int lastFootstepIndex = -1;
    private int lastSlimeMoveIndex = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureAudioSources();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        LoadVolumeSettings();
        PlayMusic(defaultTrackIndex);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindMusicSlider();
    }

    void Update()
    {
        if (musicSlider != null && musicSource != null)
        {
            float newVolume = musicSlider.value;
            if (Mathf.Abs(musicSource.volume - newVolume) > 0.01f)
            {
                musicSource.volume = newVolume;
                musicVolume = newVolume;
            }
        }
    }

    public void PlayMusic(int trackIndex)
    {
        if (musicTracks == null || musicTracks.Length == 0) return;
        if (trackIndex < 0 || trackIndex >= musicTracks.Length) return;
        if (musicSource == null) return;

        if (musicSource.isPlaying && musicSource.clip == musicTracks[trackIndex])
            return;

        musicSource.clip = musicTracks[trackIndex];
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PauseMusic()
    {
        if (musicSource != null)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
            musicSource.UnPause();
    }

    public void PlayFootstepSound()
    {
        if (footstepSounds == null || footstepSounds.Length == 0) return;

        int index = GetNextIndex(ref lastFootstepIndex, footstepSounds.Length);
        PlaySFX(footstepSounds[index], sfxVolume * 0.35f);
    }

    public void PlaySlimeMovement()
    {
        if (slimeMovementSounds == null || slimeMovementSounds.Length == 0) return;
        if (sfxSource == null) return;

        int index = GetNextIndex(ref lastSlimeMoveIndex, slimeMovementSounds.Length);
        AudioClip clip = slimeMovementSounds[index];
        if (clip != null)
        {
            float finalVolume = Mathf.Clamp01(sfxVolume * 0.5f);
            sfxSource.PlayOneShot(clip, finalVolume);
        }
    }

    public void PlaySlimeAttack()
    {
        if (slimeAttackSounds == null || slimeAttackSounds.Length == 0) return;
        if (sfxSource == null) return;

        int index = Random.Range(0, slimeAttackSounds.Length);
        AudioClip clip = slimeAttackSounds[index];
        if (clip != null)
        {
            float finalVolume = Mathf.Clamp01(sfxVolume * 0.7f);
            sfxSource.PlayOneShot(clip, finalVolume);
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;

        float finalVolume = Mathf.Clamp01(volume * sfxVolume);
        sfxSource.PlayOneShot(clip, finalVolume);
    }

    public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        float finalVolume = Mathf.Clamp01(volume * sfxVolume);
        AudioSource.PlayClipAtPoint(clip, position, finalVolume);
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("MusicVol", musicVolume);
        PlayerPrefs.SetFloat("SFXVol", sfxVolume);
        PlayerPrefs.Save();
    }

    public void LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey("MusicVol"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVol");
        }
        else
        {
            musicVolume = 0.55f;
        }

        if (PlayerPrefs.HasKey("SFXVol"))
        {
            sfxVolume = PlayerPrefs.GetFloat("SFXVol");
        }
        else
        {
            sfxVolume = 0.67f;
        }

        if (musicSource != null)
            musicSource.volume = musicVolume;

        if (sfxSource != null)
            sfxSource.volume = sfxVolume;

        if (musicSlider != null)
            musicSlider.value = musicVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }

    public void SetFootstepVolume(float volume)
    {
    }

    public void SetSlimeVolume(float volume)
    {
    }

    void FindMusicSlider()
    {
        GameObject sliderObj = GameObject.Find("Slider");
        if (sliderObj != null)
        {
            musicSlider = sliderObj.GetComponent<Slider>();
            if (musicSlider != null)
            {
                musicSlider.value = musicVolume;
            }
        }
    }

    void EnsureAudioSources()
    {
        if (musicSource == null)
        {
            GameObject musicGO = new GameObject("MusicSource");
            musicGO.transform.SetParent(transform);
            musicSource = musicGO.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
        }

        if (sfxSource == null)
        {
            GameObject sfxGO = new GameObject("SFXSource");
            sfxGO.transform.SetParent(transform);
            sfxSource = sfxGO.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
        }
    }

    int GetNextIndex(ref int lastIndex, int arrayLength)
    {
        int nextIndex = (lastIndex + 1) % arrayLength;
        lastIndex = nextIndex;
        return nextIndex;
    }
}
