using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private Health _health;
    [SerializeField] private CharmTray _tray;

    [SerializeField] private Track[] _tracks;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _hitState = "Starboy_Armature|Starboy_Hurt";
    [SerializeField] private string _hitTrigger = "Hit";
    [SerializeField] private string _deadParam = "Dead";

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

    public void Kill()
    {
        if (_animator != null)
        {
            _animator.SetBool(_deadParam, true);
        }
    }

    public void TakeDamage(int amount)
    {
        _health.TakeDamage(amount);

        if (_animator != null)
        {
            var state = _animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName(_hitState) && !_animator.IsInTransition(0))
            {
                _animator.SetTrigger(_hitTrigger);
            }
        }
    }

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

    public void ShowDropHighlights(bool show)
    {
        foreach (Track t in _tracks)
        {
            t.ShowDropHighlight(show);
        }
    }
}
