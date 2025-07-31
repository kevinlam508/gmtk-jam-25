using System;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public event Action<TravelData> CharmFinishedTravel;

    private class InstanceData
    {
        public CharmData Data;

        public GameObject VisualRoot;
        public float Distance;
    }

    public struct TravelData
    {
        public CharmData Data;
    }

    [SerializeField] private LineRenderer _line;
    [SerializeField] private float _minSeparation = .5f;
    private readonly List<float> _segmentDistances = new List<float>();

    [SerializeField] private CharmInstance _charmPrefab;
    private readonly List<InstanceData> _activeCharms = new List<InstanceData>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ComputeSegmentLengths();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _activeCharms.Count; i++)
        {
            InstanceData instance = _activeCharms[i];

            float movement = Time.deltaTime * instance.Data.Speed;
            float nextDistance = instance.Distance + movement;

            // Can't pass through charms ahead
            if (i > 0)
            {
                float nextCharmDistance = _activeCharms[i - 1].Distance;
                nextDistance = Mathf.Min(nextCharmDistance - _minSeparation, nextDistance);
            }
            instance.Distance = nextDistance;

            // Negative is where it would need to be to insert to maintain order,
            // so one earlier is the value that's lower
            int closestSegment = _segmentDistances.BinarySearch(instance.Distance);
            if (closestSegment < 0)
            {
                closestSegment = ~closestSegment;
                closestSegment--;
            }

            // Passed the end, get rid of it
            if (closestSegment == _line.positionCount - 1)
            {
                _activeCharms.RemoveAt(i);

                Destroy(instance.VisualRoot);
                CharmFinishedTravel?.Invoke(
                        new TravelData
                        {
                            Data = instance.Data
                        }
                    );
                continue;
            }

            // Compute new position
            Vector3 segmentStart = _line.GetPosition(closestSegment);
            Vector3 segmentEnd = _line.GetPosition(closestSegment + 1);
            
            float distanceInSegment = instance.Distance - _segmentDistances[closestSegment];
            Vector3 alongSegment = segmentEnd - segmentStart;
            Vector3 newPosition = segmentStart + (alongSegment.normalized * distanceInSegment);

            instance.VisualRoot.transform.localPosition = newPosition;
        }
    }

    public void AddCharm(CharmData charm)
    {
        GameObject newInstance = Instantiate(charm.Prefab, transform);

        CharmInstance charmInstance = newInstance.GetComponent<CharmInstance>();
        charmInstance.AssignData(charm);

        _activeCharms.Add(new InstanceData
        { 
            Data = charm,
            VisualRoot = newInstance
        });
    }

    private void ComputeSegmentLengths()
    {
        _segmentDistances.Add(0);
        for (int i = 1; i < _line.positionCount; i++)
        {
            float length = (_line.GetPosition(i) - _line.GetPosition(i - 1)).magnitude;
            _segmentDistances.Add(_segmentDistances[_segmentDistances.Count - 1] + length);
        }
    }
}
