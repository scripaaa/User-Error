using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [Header("Музыка")]
    public AudioSource musicSource; 
    public Slider musicSlider;

    [Header("Звуки кнопок")]
    public AudioSource sfxSource;
    public AudioClip clickSound;

    void Start()
    {
        // Проверяем, есть ли сохраненная громкость
        if (PlayerPrefs.HasKey("MusicVol"))
        {
            float vol = PlayerPrefs.GetFloat("MusicVol");
            musicSlider.value = vol;
            musicSource.volume = vol;
        }
        
        if (musicSource != null && !musicSource.isPlaying)
            musicSource.Play();
    }

    void Update()
    {
        // Привязываем громкость музыки к ползунку
        if (musicSource != null && musicSlider != null)
        {
            musicSource.volume = musicSlider.value;
        }
    }

    public void PlayClick()
    {
        if (sfxSource != null && clickSound != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("MusicVol", musicSlider.value);
        PlayerPrefs.Save();
    }
}