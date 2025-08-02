using UnityEngine;

[CreateAssetMenu(fileName = "SpeedModifierEffect", menuName = "Scriptable Objects/StatusEffects/SpeedModifierEffect")]
public class SpeedModifierEffect : BaseStatusEffect
{
    [SerializeField] private float _speedMultiplier;

    public override void OnApplied(Mob mob, float resistance, object dataStore)
    {
        mob.ApplySpeedMultiplier(_speedMultiplier * (1 - resistance));
    }

    public override void OnRemoved(Mob mob, float resistance, object dataStore)
    {
        mob.ApplySpeedMultiplier(1 / _speedMultiplier * (1 - resistance));
    }
}
