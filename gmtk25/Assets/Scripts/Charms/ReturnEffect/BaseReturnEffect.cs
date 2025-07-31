using UnityEngine;

public abstract class BaseReturnEffect : ScriptableObject
{
    public abstract void Apply(Player player, Track.TravelData data);
}
