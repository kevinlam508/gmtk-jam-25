using UnityEngine;

[CreateAssetMenu(fileName = "HandSizeReturnEffect", menuName = "Scriptable Objects/ReturnEffects/HandSizeReturnEffect")]
public class HandSizeReturnEffect : BaseReturnEffect
{
    [SerializeField] private int _minContactsToTrigger = 3;
    [SerializeField] private int _handSizeChange = 1;

    public override void Apply(Player player, Track.TravelData data)
    {
        if (data.TravelStateData.HitCount < _minContactsToTrigger)
        {
            return;
        }

        player.ModifyMaxHandSize(_handSizeChange);
    }

    public override bool ShouldSwitchState(CharmData.TravelState data)
    {
        return data.CollisionCount >= _minContactsToTrigger;
    }
}
