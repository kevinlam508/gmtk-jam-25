using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharmOption : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action<int> CharmPlaced;

    private CharmData _data;

    [SerializeField] private LayerMask _dropLayers;

    [SerializeField] private GameObject _dragParent;
    private Vector3 _dragOffset;
    private Coroutine _dragRoutine;
    private Vector3 _dragParentOriginalPosition;

    public void Init(CharmData data)
    {
        _data = data;
        // TODO: setup appearance for charm data
    }

    private void Awake()
    {
        _dragParentOriginalPosition = _dragParent.transform.localPosition;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _dragOffset = _dragParent.transform.position - Input.mousePosition;
        _dragRoutine = StartCoroutine(DragCoroutine());
    }
    
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {

        if (_dragRoutine == null)
        {
            return;
        }

        StopCoroutine(_dragRoutine);
        _dragRoutine = null;

        // TODO: Tween back into place instead of snapping
        _dragParent.transform.localPosition = _dragParentOriginalPosition;

        if (TryPlaceCharm())
        {
            CharmPlaced?.Invoke(transform.GetSiblingIndex());
        }
    }

    private IEnumerator DragCoroutine()
    {
        while(true)
        {
            _dragParent.transform.position = Input.mousePosition + _dragOffset;
            yield return null;
        }
    }

    private bool TryPlaceCharm()
    {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 1;
        Camera cam = Camera.main;
        Vector3 worldPoint = cam.ScreenToWorldPoint(screenPoint);

        Ray testRay = new Ray
        {
            origin = cam.transform.position,
            direction = worldPoint - cam.transform.position
        };
        RaycastHit[] hits = Physics.RaycastAll(testRay,
            float.MaxValue,
            _dropLayers.value,
            QueryTriggerInteraction.Collide);

        bool hasDropped = false;
        foreach (RaycastHit hit in hits)
        {
            // Not a track, don't care
            Transform hitParnet = hit.transform.parent;
            Track track = hitParnet?.GetComponent<Track>();
            if (track == null)
            {
                continue;
            }

            track.AddCharm(_data);
            hasDropped = true;
            break;
        }

        return hasDropped;
    }
}
