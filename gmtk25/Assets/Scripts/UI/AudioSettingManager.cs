using UnityEngine;
using UnityEngine.UI;
public class AudioSettingManager : MonoBehaviour
{
    public AudioClip MainMusicTrack;
    public AudioSource MainMusicAudioSource;
    public Slider attachedSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (MainMusicTrack != null)
            PlayMusic();
    }

    private void OnEnable()
    {
        attachedSlider.value = gameSettings.audioVolume;
    }

    void PlayMusic()
    {
        MainMusicAudioSource.clip = MainMusicTrack;
        MainMusicAudioSource.volume = gameSettings.audioVolume;
        MainMusicAudioSource.Play();
    }

    public void ChangeAudioVolume(float value)
    {
        MainMusicAudioSource.volume = value;
        gameSettings.audioVolume = value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public static class gameSettings
{
    public static float audioVolume = .7f;
}

