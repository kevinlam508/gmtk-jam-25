using UnityEngine;

[CreateAssetMenu(fileName = "CharmData", menuName = "Scriptable Objects/CharmData")]
public class CharmData : ScriptableObject
{
    [SerializeField] private float _speed;
    [SerializeField] private int _damage;

    [SerializeField] private GameObject _prefab;

    public float Speed => _speed;

    public GameObject Prefab => _prefab;

    public virtual void CollisionCallback(Collider other)
    {

    }
}
