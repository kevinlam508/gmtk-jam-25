using System;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public event Action<TravelData> CharmFinishedTravel;

    private class InstanceData
    {
        public CharmData Data;
        public CharmData.TravelState TravelStateData;

        public GameObject VisualRoot;
        public float Distance;
        public float Duration;
    }

    public struct TravelData
    {
        public CharmData Data;
        public CharmData.TravelState TravelStateData;

        public float Duration;
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

            instance.Duration += Time.deltaTime;
            float movement = Time.deltaTime * instance.Data.Speed;
            float nextDistance = instance.Distance + movement;

            if (i > 0)
            {
                // Push charms ahead to match this movement
                if (instance.Data.CanShove)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        InstanceData adjacentInstance = _activeCharms[j];
                        float nextCharmDistance = adjacentInstance.Distance;
                        float adjustedDistance = nextDistance + (_minSeparation * (i - j));

                        // Next charm is too far away to be shoved, so the rest will also be too far
                        if (nextCharmDistance > adjustedDistance)
                        {
                            break;
                        }

                        // Update i to be correctly in sync in the outer loop
                        // but j can stay the same since this loop is always decrementing
                        if (MoveCharm(adjacentInstance, adjustedDistance))
                        {
                            _activeCharms.RemoveAt(j);
                            i--;
                        }
                    }
                }
                // Can't pass through charms ahead
                else
                {
                    float nextCharmDistance = _activeCharms[i - 1].Distance;
                    nextDistance = Mathf.Min(nextCharmDistance - _minSeparation, nextDistance);
                }
            }

            if (MoveCharm(instance, nextDistance))
            {
                _activeCharms.RemoveAt(i);
                i--;
            }
        }

        for (int i = 1; i < _activeCharms.Count; i++)
        {
            InstanceData current = _activeCharms[i];
            InstanceData front = _activeCharms[i - 1];
            if (Mathf.Approximately(front.Distance - current.Distance, _minSeparation))
            {
                front.TravelStateData.BackNeighbor = current.Data;
                current.TravelStateData.FrontNeighbor = front.Data;
            }
        }
    }

    public void Clear()
    {
        foreach (InstanceData data in _activeCharms)
        {
            Destroy(data.VisualRoot);
        }
        _activeCharms.Clear();
    }

    // returns if charm was removed
    private bool MoveCharm(InstanceData instance, float newDistance)
    {
        instance.Distance = newDistance;

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
            Destroy(instance.VisualRoot);
            CharmFinishedTravel?.Invoke(
                    new TravelData
                    {
                        Data = instance.Data,
                        TravelStateData = instance.TravelStateData,

                        Duration = instance.Duration
                    }
                );
            return true;
        }

        // Compute new position
        Vector3 segmentStart = _line.GetPosition(closestSegment);
        Vector3 segmentEnd = _line.GetPosition(closestSegment + 1);

        float distanceInSegment = instance.Distance - _segmentDistances[closestSegment];
        Vector3 alongSegment = segmentEnd - segmentStart;
        Vector3 newPosition = segmentStart + (alongSegment.normalized * distanceInSegment);

        instance.VisualRoot.transform.localPosition = newPosition;
        instance.VisualRoot.transform.rotation = Quaternion.LookRotation(alongSegment, Vector3.up);

        return false;
    }

    public void AddCharm(CharmData charm)
    {
        GameObject newInstance = Instantiate(charm.Prefab, transform);
        CharmData.TravelState travelState = charm.NewTravelStateData();

        CharmInstance charmInstance = newInstance.GetComponent<CharmInstance>();
        charmInstance.AssignData(charm, travelState);

        _activeCharms.Add(new InstanceData
        { 
            Data = charm,
            TravelStateData = travelState,

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
