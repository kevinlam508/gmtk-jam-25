using UnityEngine;

public class AudioSettingManager : MonoBehaviour
{
    public AudioClip MainMusicTrack;
    public AudioSource MainMusicAudioSource;
    public float defaultMusicValue = .7f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayMusic();
    }

    void PlayMusic()
    {
        MainMusicAudioSource.clip = MainMusicTrack;
        MainMusicAudioSource.volume = defaultMusicValue;
        MainMusicAudioSource.Play();
    }

    public void ChangeAudioVolume(float value)
    {
        MainMusicAudioSource.volume = value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
