using UnityEngine;

public class CharmInstance : MonoBehaviour
{
    private CharmData _data;
    private CharmData.TravelState _travelStateData;

    public void AssignData(CharmData data, CharmData.TravelState travelStateData)
    {
        _data = data;
        _travelStateData = travelStateData;
    }

    private void OnTriggerEnter(Collider other)
    {
        _data.CollisionCallback(other, _travelStateData);
    }
}
