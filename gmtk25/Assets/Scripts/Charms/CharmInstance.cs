using UnityEngine;

public class CharmInstance : MonoBehaviour
{
    [SerializeField] private GameObject[] _states;

    [SerializeField] private AudioSource _hitSource;
    [SerializeField] private AudioSource _chainSource;
    [SerializeField] private AudioSource _splashSource;

    private CharmData _data;
    private CharmData.TravelState _travelStateData;

    public void PlayHit(AudioClip clip)
    {
        _hitSource.clip = clip;
        _hitSource.Play();
    }

    public void PlayChain(AudioClip clip)
    {
        _chainSource.clip = clip;
        _chainSource.Play();
    }

    public void PlaySplash(AudioClip clip)
    {
        _splashSource.clip = clip;
        _splashSource.Play();
    }

    public void AssignData(CharmData data, CharmData.TravelState travelStateData)
    {
        _data = data;
        _travelStateData = travelStateData;
    }

    private void OnTriggerEnter(Collider other)
    {
        _data.CollisionCallback(other, _travelStateData, this, SwapToState);
    }

    private void SwapToState(int index)
    {
        for(int i = 0; i < _states.Length; i++)
        {
            _states[i].SetActive(i == index);
        }
    }
}
