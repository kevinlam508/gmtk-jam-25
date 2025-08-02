using UnityEngine;

public abstract class BaseReturnEffect : ScriptableObject
{
    public abstract void Apply(Player player, Track.TravelData data);

    public abstract bool ShouldSwitchState(CharmData.TravelState data);
}
