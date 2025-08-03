using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class SlideInContainer : MonoBehaviour
{
    [SerializeField] private Transform _trackEnd;
    [SerializeField] private Transform _trackBegin;

    [SerializeField] private Ease _toBeginEasing;
    [SerializeField] private Ease _toDestinationEasing;
    [SerializeField] private float _speed = 6;
    [SerializeField] private float _spacing = 10;
    [SerializeField] private float _delay = 0.25f;

    private Dictionary<Transform, Sequence> _tweens = new Dictionary<Transform, Sequence>();

    private void OnTransformChildrenChanged()
    {
        foreach ((_, Sequence s) in _tweens)
        {
            s.Kill();
        }
        _tweens.Clear();

        Vector3 alongTrack = (_trackEnd.position - _trackBegin.position).normalized;
        for (int i = 0; i < transform.childCount; i++)
        {
            Vector3 targetLocation = _trackEnd.position - (alongTrack * i * _spacing);
            Transform child = transform.GetChild(i);

            Sequence slideIn = DOTween.Sequence();
            // Behind beginning, need to tween to that first
            Vector3 beginToCurrent = child.position - _trackBegin.position;
            if (Vector3.Dot(alongTrack, beginToCurrent) < 0)
            {
                slideIn.Insert(i * _delay, child.DOMove(_trackBegin.position, beginToCurrent.magnitude / _speed).SetEase(_toBeginEasing));
            }

            slideIn.Insert(i * _delay, child.DOMove(targetLocation, (child.position - targetLocation).magnitude / _speed).SetEase(_toDestinationEasing));

            _tweens.Add(child, slideIn);
        }
    }
}
