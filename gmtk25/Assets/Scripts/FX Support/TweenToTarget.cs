using UnityEngine;

public class TweenToTarget : MonoBehaviour
{
    [SerializeField] private float _killTime = 1;

    private Vector3 _target;
    private float _duration;

    private float _timePassed;
    private Vector3 _start;

    public void Init(Vector3 target, float duration)
    {
        _target = target;
        _duration = duration;
    }

    private void Start()
    {
        _start = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(_start, _target, _timePassed / _duration);

        _timePassed += Time.deltaTime;

        if (_timePassed > _duration + _killTime)
        {
            Destroy(gameObject);
        }
    }
}
