using UnityEngine;

[CreateAssetMenu(fileName = "ExplosiveCharmData", menuName = "Scriptable Objects/Charms/ExplosiveCharmData")]
public class ExplosiveCharmData : CharmData
{
    [Header("Explosion")]
    [SerializeField] private float _radius;
    [SerializeField] private int _explosionCount;

    public override void CollisionCallback(Collider other, Vector3 location, TravelState travelStateData)
    {
        if (_explosionCount >= 0 && travelStateData.CollisionCount >= _explosionCount)
        {
            return;
        }

        RaycastHit[] hits = Physics.SphereCastAll(location, _radius, Vector3.up, 0);
        foreach (RaycastHit hit in hits)
        {
            Mob mob = hit.collider.GetComponent<Mob>();
            if (mob == null)
            {
                continue;
            }

            ApplyDamageAndStatusToMob(mob);
        }

        travelStateData.CollisionCount++;
    }
}
