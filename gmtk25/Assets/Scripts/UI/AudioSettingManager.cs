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
    System.Collections.IEnumerator Start()
    {
        yield return null;

        mixer.SetFloat("MasterVolume", LinearToDecibel(gameSettings.audioVolume));
        if (MainMusicTrack != null)
            PlayMusic();

    }

    private void Awake()
    {
        if (attachedSlider != null)
        {
            attachedSlider.value = gameSettings.audioVolume;
            attachedSlider.onValueChanged.AddListener(ChangeAudioVolume);
        }
    }

    void PlayMusic()
    {
        MainMusicAudioSource.clip = MainMusicTrack;
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
            dB = (float)8.6858896380650365530225783783321 * Mathf.Log(linear);
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

