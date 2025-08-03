using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class SlideInContainer : MonoBehaviour
{
    [SerializeField] private RectTransform _trackEnd;
    [SerializeField] private RectTransform _trackBegin;

    [SerializeField] private Ease _toBeginEasing;
    [SerializeField] private Ease _toDestinationEasing;
    [SerializeField] private float _speed = 6;
    [SerializeField] private float _spacing = 10;
    [SerializeField] private float _delay = 0.25f;
    [SerializeField] private float _drawToTrackTime = .1f;

    private Dictionary<Transform, Sequence> _tweens = new Dictionary<Transform, Sequence>();

    private void OnTransformChildrenChanged()
    {
        foreach ((_, Sequence s) in _tweens)
        {
            s.Kill();
        }
        _tweens.Clear();

        Vector2 alongTrack = (_trackEnd.anchoredPosition - _trackBegin.anchoredPosition).normalized;
        for (int i = 0; i < transform.childCount; i++)
        {
            Vector2 targetLocation = _trackEnd.anchoredPosition - (alongTrack * i * _spacing);
            RectTransform child = (RectTransform)transform.GetChild(i);

            Sequence slideIn = DOTween.Sequence();
            // Behind beginning, need to tween to that first
            Vector3 beginToCurrent = child.position - _trackBegin.position;
            slideIn.PrependInterval(i * _delay);
            if (Vector3.Dot(alongTrack, beginToCurrent) < 0)
            {
                slideIn.Append(child.DOAnchorPos(_trackBegin.anchoredPosition, _drawToTrackTime)
                        .SetEase(_toBeginEasing));
            }

            slideIn.Append(child.DOAnchorPos(targetLocation, (targetLocation - child.anchoredPosition).magnitude / _speed)
                    .SetEase(_toDestinationEasing));

            _tweens.Add(child, slideIn);
        }
    }
}
