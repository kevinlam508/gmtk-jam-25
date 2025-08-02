using UnityEngine;

public class MenuEnemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Rigidbody _rb;
    [SerializeField][Range(0, 2000f)] float _explosionForce;
    private float _collisionCount = 0;
    private ParticleSystem _ps;
    void Awake()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        _ps = gameObject.GetComponentInChildren<ParticleSystem>();
        _rb.AddTorque(0f, 3f, 0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {

        if (_collisionCount < 1)
        {
            Debug.Log(gameObject.name + " collided with " + collision.gameObject.name);
            _rb.AddExplosionForce(_explosionForce, collision.transform.position, 200f, 25f, ForceMode.Impulse);
            _ps.Play();
            _rb.useGravity = true;
        }
        _collisionCount++;
    }
}
