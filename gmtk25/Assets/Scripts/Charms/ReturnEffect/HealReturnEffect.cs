using UnityEngine;

[CreateAssetMenu(fileName = "HealReturnEffect", menuName = "Scriptable Objects/ReturnEffects/HealReturnEffect")]
public class HealReturnEffect : BaseReturnEffect
{
    [SerializeField] private float _healPerSecondTraveled = 1;

    public override void Apply(Player player, Track.TravelData data)
    {
        // Can only heal if it made it through without touching anything
        if (data.TravelStateData.CollisionCount > 0)
        {
            return;
        }

        player.Heal((int)Mathf.Ceil(_healPerSecondTraveled * data.Duration));
    }

    public override bool ShouldSwitchState(CharmData.TravelState data)
    {
        return data.CollisionCount > 0;
    }
}
