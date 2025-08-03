using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingManager : MonoBehaviour
{
    public AudioClip MainMusicTrack;
    public AudioSource MainMusicAudioSource;
    public Slider attachedSlider;
    public AudioMixer mixer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (MainMusicTrack != null)
            PlayMusic();
    }

    private void OnEnable()
    {
        if (attachedSlider != null)
        {
            attachedSlider.value = gameSettings.audioVolume;
            attachedSlider.onValueChanged.AddListener(ChangeAudioVolume);
        }
        mixer.SetFloat("MasterVolume", LinearToDecibel(gameSettings.audioVolume));
    }

    void PlayMusic()
    {
        MainMusicAudioSource.clip = MainMusicTrack;
        MainMusicAudioSource.volume = gameSettings.audioVolume;
        MainMusicAudioSource.Play();
    }

    public void ChangeAudioVolume(float value)
    {
        mixer.SetFloat("MasterVolume", LinearToDecibel(value));
        gameSettings.audioVolume = value;
    }
    private float LinearToDecibel(float linear)
    {
        float dB;

        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;

        return dB;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public static class gameSettings
{
    public static float audioVolume = .1f;
}

