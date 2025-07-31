using UnityEngine;

[CreateAssetMenu(fileName = "HealReturnEffect", menuName = "Scriptable Objects/ReturnEffects/HealReturnEffect")]
public class HealReturnEffect : BaseReturnEffect
{
    [SerializeField] private float _healPerSecondTraveled = 1;

    public override void Apply(Player player, Track.TravelData data)
    {
        player.Heal((int)Mathf.Ceil(_healPerSecondTraveled * data.Duration));
    }
}
