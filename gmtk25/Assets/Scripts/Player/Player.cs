using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private Health _health;
    [SerializeField] private CharmTray _tray;

    [SerializeField] private Track[] _tracks;

    private void Start()
    {
        foreach (Track t in _tracks)
        {
            t.CharmFinishedTravel += OnCharmTravelFinished;
        }
    }

    public void ClearForRound()
    {
        foreach (Track t in _tracks)
        {
            t.Clear();
        }
    }

    public void TakeDamage(int amount) => _health.TakeDamage(amount);
    public void Heal(int amount) => _health.Heal(amount);
    public void ModifyDrawCooldown(float amount) => _tray.ModifyDrawCooldown(amount);
    public void ModifyMaxHandSize(int amount) => _tray.ModifyMaxHandSize(amount);

    private void OnCharmTravelFinished(Track.TravelData travelData)
    {
        _inventory.ReturnCharm(travelData.Data);

        BaseReturnEffect[] returnEffects = travelData.Data.ReturnEffects;
        if (returnEffects != null)
        {
            foreach (BaseReturnEffect effect in returnEffects)
            {
                effect.Apply(this, travelData);
            }
        }
    }
}
