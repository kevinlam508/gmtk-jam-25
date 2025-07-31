using UnityEngine;

[CreateAssetMenu(fileName = "DotStatus", menuName = "Scriptable Objects/StatusEffects/DotStatus")]
public class DotStatus : BaseStatusEffect
{
    private class InstanceData
    {
        public float TimePassed;
    }

    [SerializeField] private int _damagePerTick;
    [SerializeField] private float _timeBetweenTicks;

    public override object NewDataStore()
    {
        return new InstanceData();
    }

    public override void Tick(Mob mob, object dataStore)
    {
        InstanceData data = (InstanceData)dataStore;
        data.TimePassed += Time.deltaTime;
        if (data.TimePassed > _timeBetweenTicks)
        {
            mob.TakeDamage(_damagePerTick);
            data.TimePassed -= _timeBetweenTicks;
        }
    }
}
