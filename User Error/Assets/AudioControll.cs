using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public Slider slider;       // Ссылка на ваш ползунок
    public AudioSource audio;    // Ссылка на источник звука

    void Start()
    {
        // Если музыка не должна прерываться при старте, проверяем настройки
        if (audio != null)
        {
            audio.playOnAwake = true; 
            if (!audio.isPlaying) audio.Play();
            
            // Устанавливаем громкость из слайдера сразу при старте
            if (slider != null)
            {
                audio.volume = slider.value;
            }
        }
    }

    void Update()
    {
        // Постоянно обновляем громкость в зависимости от положения ползунка
        if (audio != null && slider != null)
        {
            audio.volume = slider.value;
        }
    }
}