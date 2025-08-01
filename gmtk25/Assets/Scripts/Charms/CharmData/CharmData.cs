using UnityEngine;


[CreateAssetMenu(fileName = "CharmData", menuName = "Scriptable Objects/Charms/CharmData")]
public class CharmData : ScriptableObject
{
    public class TravelState
    {
        public int CollisionCount;
    }

    [Header("Effects")]
    [SerializeField] private float _speed;
    [SerializeField] private int _damage;
    [SerializeField] private bool _canShove;
    [SerializeField] private BaseStatusEffect _onHitStatus;
    [SerializeField] private BaseReturnEffect _returnEffect;

    [Header("Appearance")]
    [SerializeField] private GameObject _prefab;

    public float Speed => _speed;
    public bool CanShove => _canShove;
    public BaseReturnEffect ReturnEffect => _returnEffect;

    public GameObject Prefab => _prefab;

    public virtual TravelState NewTravelStateData() => new TravelState();

    public virtual void CollisionCallback(Collider other, Vector3 location, TravelState travelStateData)
    {
        Mob mob = other.GetComponent<Mob>();
        if (mob == null)
        {
            return;
        }

        travelStateData.CollisionCount++;
        ApplyDamageAndStatusToMob(mob);
    }

    protected void ApplyDamageAndStatusToMob(Mob mob)
    {
        mob.TakeDamage(_damage);
        if (_onHitStatus != null)
        {
            mob.AddStatus(_onHitStatus);
        }
    }
}
