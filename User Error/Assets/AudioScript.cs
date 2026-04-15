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

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.67f;

    [Range(0f, 1f)]
    public float sfxVolume = 0.72f;

    [Range(0f, 1f)]
    public float slimeVolume = 0.3f;

    [Header("Multipliers")]
    [Range(0f, 1f)]
    public float musicMultiplier = 0.2f;

    [Tooltip("Множитель громкости шагов игрока")]
    [Range(0f, 2f)]
    public float footstepMultiplier = 1.4f;

    [Tooltip("Множитель громкости слаймов")]
    [Range(0f, 1f)]
    public float slimeMultiplier = 0.5f;

    [Tooltip("Максимальное расстояние для звуков слаймов")]
    public float slimeSoundMaxDistance = 15f;

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
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Запускаем музыку для текущей сцены
        PlayMusicForCurrentScene();

        // Пробуем найти ползунок сразу при старте
        FindMusicSlider();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindMusicSlider();

        // При загрузке новой сцены - переключаем музыку
        PlayMusicForCurrentScene();
    }

    void PlayMusicForCurrentScene()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        bool isMenuScene = sceneName.Contains("Menu") || sceneName.Contains("menu");

        if (isMenuScene)
        {
            // В меню - проверяем что первый трек менюшный
            if (musicTracks == null || musicTracks.Length == 0) return;
            Debug.Log("[AudioController] МЕНЮ - играем трек: " + (musicTracks.Length > 0 ? musicTracks[0].name : "нет"));
            PlayMusic(0);
        }
        else
        {
            // В игре - проверяем что треки игровые (не менюшные)
            if (musicTracks == null || musicTracks.Length == 0) return;

            // Если треков меньше 2 значит это менюшные - нужно загрузить игровые
            if (musicTracks.Length < 2)
            {
                LoadGameTracks();
            }

            Debug.Log("[AudioController] ИГРА - играем трек индекс: " + defaultTrackIndex);
            PlayMusic(defaultTrackIndex);
        }
    }

    void LoadGameTracks()
    {
        // Загружаем игровую музыку
        AudioClip[] tracks = new AudioClip[4];
        tracks[0] = LoadClipFromResources("SomeSounds/Broken Promise Broken Dream 76 BPM Loop");
        tracks[1] = LoadClipFromResources("SomeSounds/Cave of the Sisterhood 131 BPM Loop");
        tracks[2] = LoadClipFromResources("SomeSounds/Silver Creek 117 BPM Loop");
        tracks[3] = LoadClipFromResources("SomeSounds/Far From Home 112 BPM Loop");

        if (tracks[0] == null)
        {
            tracks[0] = LoadClipFromPath("Assets/SomeSounds/Broken Promise Broken Dream 76 BPM Loop.wav");
            tracks[1] = LoadClipFromPath("Assets/SomeSounds/Cave of the Sisterhood 131 BPM Loop.wav");
            tracks[2] = LoadClipFromPath("Assets/SomeSounds/Silver Creek 117 BPM Loop.wav");
            tracks[3] = LoadClipFromPath("Assets/SomeSounds/Far From Home 112 BPM Loop.wav");
        }

        musicTracks = tracks;
        Debug.Log("[AudioController] Загружены игровые треки: " + tracks.Length + " шт.");
    }

    AudioClip LoadClipFromResources(string resourceName)
    {
        AudioClip clip = Resources.Load<AudioClip>(resourceName);
        if (clip == null)
        {
            Debug.LogWarning("[AudioController] Не найден трек в Resources: " + resourceName);
        }
        return clip;
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

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (musicSlider != null && musicSource != null)
        {
            float newVolume = musicSlider.value;
            if (Mathf.Abs(musicVolume - newVolume) > 0.01f)
            {
                SetMusicVolume(newVolume);
                SaveVolume();
            }
        }
    }

    public void PlayMusic(int trackIndex)
    {
        if (musicTracks == null || musicTracks.Length == 0) return;
        if (trackIndex < 0 || trackIndex >= musicTracks.Length) return;
        if (musicSource == null) return;

        // Останавливаем текущую музыку
        musicSource.Stop();

        musicSource.clip = musicTracks[trackIndex];
        musicSource.volume = musicVolume * musicMultiplier;
        musicSource.loop = true;
        musicSource.Play();

        Debug.Log("[AudioController] Играет трек: " + musicTracks[trackIndex].name + " (индекс: " + trackIndex + ")");
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
        PlaySFX(footstepSounds[index], footstepMultiplier);
    }

    public void PlaySlimeMovement(Vector3 slimePosition)
    {
        if (slimeMovementSounds == null || slimeMovementSounds.Length == 0) return;
        if (sfxSource == null) return;

        // Проверяем расстояние до игрока
        if (Hero.Instance != null)
        {
            float distanceToPlayer = Vector3.Distance(slimePosition, Hero.Instance.transform.position);
            if (distanceToPlayer > slimeSoundMaxDistance)
                return; // Не воспроизводим звук если слайм далеко

            // Уменьшаем громкость в зависимости от расстояния
            float distanceFactor = 1f - (distanceToPlayer / slimeSoundMaxDistance);
            distanceFactor = Mathf.Clamp01(distanceFactor);

            int index = GetNextIndex(ref lastSlimeMoveIndex, slimeMovementSounds.Length);
            AudioClip clip = slimeMovementSounds[index];
            if (clip != null)
            {
                float finalVolume = slimeVolume * slimeMultiplier * distanceFactor;
                sfxSource.PlayOneShot(clip, finalVolume);
            }
        }
    }

    public void PlaySlimeMovement()
    {
        if (slimeMovementSounds == null || slimeMovementSounds.Length == 0) return;
        if (sfxSource == null) return;

        int index = GetNextIndex(ref lastSlimeMoveIndex, slimeMovementSounds.Length);
        AudioClip clip = slimeMovementSounds[index];
        if (clip != null)
        {
            float finalVolume = slimeVolume * slimeMultiplier;
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
            float finalVolume = slimeVolume * slimeMultiplier;
            sfxSource.PlayOneShot(clip, finalVolume);
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;

        float finalVolume = volume * sfxVolume;
        sfxSource.PlayOneShot(clip, finalVolume);
    }

    public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        float finalVolume = volume * sfxVolume;
        AudioSource.PlayClipAtPoint(clip, position, finalVolume);
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("MusicVol", musicVolume);
        PlayerPrefs.SetFloat("SFXVol", sfxVolume);
        PlayerPrefs.SetFloat("SlimeVol", slimeVolume);
        PlayerPrefs.Save();
    }

    public void SetSlimeVolume(float volume)
    {
        slimeVolume = Mathf.Clamp01(volume);
    }

    public void LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey("MusicVol"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVol");
        }
        else
        {
            musicVolume = 0.67f;
        }

        if (PlayerPrefs.HasKey("SFXVol"))
        {
            sfxVolume = PlayerPrefs.GetFloat("SFXVol");
        }
        else
        {
            sfxVolume = 0.72f;
        }

        if (PlayerPrefs.HasKey("SlimeVol"))
        {
            slimeVolume = PlayerPrefs.GetFloat("SlimeVol");
        }
        else
        {
            slimeVolume = 0.3f;
        }

        if (musicSource != null)
            musicSource.volume = musicVolume * musicMultiplier;

        if (sfxSource != null)
            sfxSource.volume = sfxVolume;

        if (musicSlider != null)
            musicSlider.value = musicVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume * musicMultiplier;
        if (musicSlider != null)
            musicSlider.value = musicVolume;
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

    // Метод для вызова из UI Slider (OnValueChanged)
    public void OnMusicSliderChanged(float value)
    {
        SetMusicVolume(value);
        SaveVolume();
    }

    void FindMusicSlider()
    {
        // Если ползунок уже назначен в инспекторе - используем его
        if (musicSlider != null)
        {
            musicSlider.value = musicVolume;
            return;
        }

        // Ищем по имени "Slider"
        GameObject sliderObj = GameObject.Find("Slider");
        if (sliderObj != null)
        {
            musicSlider = sliderObj.GetComponent<Slider>();
            if (musicSlider != null)
            {
                musicSlider.value = musicVolume;
                Debug.Log("[AudioController] Ползунок найден по имени: " + sliderObj.name);
                return;
            }
        }

        // Ищем среди всех Canvas
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (var canvas in canvases)
        {
            Slider[] sliders = canvas.GetComponentsInChildren<Slider>(true);
            if (sliders.Length > 0)
            {
                musicSlider = sliders[0];
                musicSlider.value = musicVolume;
                Debug.Log("[AudioController] Ползунок найден в Canvas: " + musicSlider.gameObject.name);
                return;
            }
        }

        Debug.LogWarning("[AudioController] Ползунок НЕ найден! Назначьте его вручную в AudioController.MusicSlider");
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
            musicSource.volume = musicVolume * musicMultiplier;
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
