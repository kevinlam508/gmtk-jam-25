using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiDangle : MonoBehaviour
{
    [SerializeField] private Transform[] _links;
    [SerializeField] private float[] _linksLength;

    [Header("Sim Settings")]
    [SerializeField] private float _gravity = -10f;

    private readonly List<Vector3> _startPositions = new List<Vector3>();

    private Vector3 _previousPosition;
    private Coroutine _dangleCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform link in _links)
        {
            _startPositions.Add(link.localPosition);
        }
    }

    private IEnumerator ComputeDangle()
    {
        while(true)
        {
            // Simulate move of the parent dragging but children staying in place
            Vector3 movement = transform.position - _previousPosition;
            for (int i = 1; i < _links.Length; i++)
            {
                _links[i].position -= movement;
            }

            // Gravity
            for (int i = 1; i < _links.Length; i++)
            {
                _links[i].position += Vector3.down * _gravity * _gravity / 2 * Time.deltaTime;
            }

            ClampDistanceContraints();

            _previousPosition = transform.position;
            yield return null;
        }
    }

    private void ClampDistanceContraints()
    {
        for (int i = 0; i < _links.Length - 1; i++)
        {
            Vector3 toNextLink = _links[i + 1].localPosition - _links[i].localPosition;
            float maxDistance = (_linksLength[i] + _linksLength[i + 1]) / 2;
            if (toNextLink.magnitude > maxDistance)
            {
                toNextLink = toNextLink.normalized * maxDistance;
                _links[i + 1].localPosition = _links[i].localPosition + toNextLink;
            }
        }
    }

    public void BeginDangle()
    {
        _previousPosition = transform.position;
        _dangleCoroutine = StartCoroutine(ComputeDangle());
    }

    public void EndDangle()
    {
        StopCoroutine(_dangleCoroutine);
        _dangleCoroutine = null;

        ResetPositions();
    }

    private void ResetPositions()
    {
        for(int i = 0; i < _links.Length; i++)
        {
            _links[i].localPosition = _startPositions[i];
        }
    }    
}
