using UnityEngine;

[CreateAssetMenu(fileName = "DrawBoostReturnEffect", menuName = "Scriptable Objects/ReturnEffects/DrawTimerReturnEffect")]
public class DrawTimerReturnEffect : BaseReturnEffect
{
    [SerializeField] private int _minContactsToTrigger = 3;
    [SerializeField] private float _drawTimerChange = -.2f;

    public override void Apply(Player player, Track.TravelData data)
    {
        if (data.TravelStateData.HitCount < _minContactsToTrigger)
        {
            return;
        }

        player.ModifyDrawCooldown(_drawTimerChange);
    }
}
