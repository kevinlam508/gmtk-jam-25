using UnityEngine;

[CreateAssetMenu(fileName = "SpeedModifierEffect", menuName = "Scriptable Objects/StatusEffects/SpeedModifierEffect")]
public class SpeedModifierEffect : BaseStatusEffect
{
    [SerializeField] private float _speedMultiplier;

    public override void OnApplied(Mob mob, object dataStore)
    {
        mob.ApplySpeedMultiplier(_speedMultiplier);
    }

    public override void OnRemoved(Mob mob, object dataStore)
    {
        mob.ApplySpeedMultiplier(1 / _speedMultiplier);
    }
}
