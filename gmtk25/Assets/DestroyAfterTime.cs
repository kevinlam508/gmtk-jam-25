using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    private float _timePassed = 0f;
    [SerializeField] private float _lifeTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _timePassed += Time.deltaTime;
        if (_timePassed > _lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
