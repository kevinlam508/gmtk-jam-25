using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInventory _inventory;

    [SerializeField] private Track[] _tracks;

    private void Start()
    {
        foreach (Track t in _tracks)
        {
            t.CharmFinishedTravel += OnCharmTravelFinished;
        }
    }

    private void OnCharmTravelFinished(Track.TravelData travelData)
    {
        _inventory.ReturnCharm(travelData.Data);
    }
}
