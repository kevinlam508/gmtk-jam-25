using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private Health _health;

    [SerializeField] private Track[] _tracks;

    private void Start()
    {
        foreach (Track t in _tracks)
        {
            t.CharmFinishedTravel += OnCharmTravelFinished;
        }
    }

    public void TakeDamage(int amount) => _health.TakeDamage(amount);
    public void Heal(int amount) => _health.Heal(amount);

    private void OnCharmTravelFinished(Track.TravelData travelData)
    {
        _inventory.ReturnCharm(travelData.Data);

        BaseReturnEffect returnEffect = travelData.Data.ReturnEffect;
        if (returnEffect != null)
        {
            returnEffect.Apply(this, travelData);
        }
    }
}
