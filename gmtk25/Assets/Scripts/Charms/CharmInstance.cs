using UnityEngine;

public class CharmInstance : MonoBehaviour
{
    [SerializeField] private GameObject[] _states;

    private CharmData _data;
    private CharmData.TravelState _travelStateData;

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
