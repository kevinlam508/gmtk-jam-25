using UnityEngine;

[CreateAssetMenu(fileName = "CharmData", menuName = "Scriptable Objects/CharmData")]
public class CharmData : ScriptableObject
{
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

    public virtual void CollisionCallback(Collider other)
    {
        Mob mob = other.GetComponent<Mob>();
        if (mob != null)
        {
            mob.TakeDamage(_damage);
            if (_onHitStatus != null)
            {
                mob.AddStatus(_onHitStatus);
            }
        }
    }
}
