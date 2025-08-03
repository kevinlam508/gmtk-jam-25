using UnityEngine;
using UnityEngine.Events;

public class TweenToTarget : MonoBehaviour
{
    [SerializeField] private float _killTime = 1;
    [SerializeField] private UnityEvent _killTimerBegan;

    private Vector3 _target;
    private float _duration;

    private float _timePassed;
    private Vector3 _start;

    private bool _calledKillBegin;

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

        if (!_calledKillBegin && _timePassed > _duration)
        {
            _killTimerBegan.Invoke();
            _calledKillBegin = true;
        }

        if (_timePassed > _duration + _killTime)
        {
            Destroy(gameObject);
        }
    }
}
