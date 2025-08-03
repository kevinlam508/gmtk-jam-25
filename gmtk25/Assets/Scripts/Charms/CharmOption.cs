using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField] private GameObject _beadRoot;
    [SerializeField] private Image _beadImage;
    [SerializeField] private GameObject _charmRoot;
    [SerializeField] private UiDangle _charmDangle;
    [SerializeField] private Image _charmImage;
    [SerializeField] private Image _shadow;

    [SerializeField] private TooltipTrigger _tooltip;

    public bool Interactable { get; set; } = true;

    private Track _hoveredTrack;

    public void Init(CharmData data)
    {
        _data = data;

        if (data.IsBead)
        {
            _beadImage.sprite = data.UiSprite;
            _charmRoot.SetActive(false);
        }
        else
        {
            _charmImage.sprite = data.UiSprite;
            _beadRoot.SetActive(false);
        }

        if (_tooltip != null)
        {
            _tooltip.header = data.TooltipName;
            _tooltip.content = data.TooltipDescription;
        }
    }

    private void Awake()
    {
        _dragParentOriginalPosition = _dragParent.transform.localPosition;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (!Interactable)
        {
            return;
        }

        _dragOffset = _dragParent.transform.position - Input.mousePosition;
        _dragRoutine = StartCoroutine(DragCoroutine());
        _shadow.enabled = false;

        if (!_data.IsBead)
        {
            _charmDangle.BeginDangle();
        }
    }
    
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (_dragRoutine == null)
        {
            return;
        }

        StopCoroutine(_dragRoutine);
        _dragRoutine = null;
        _shadow.enabled = true;

        // TODO: Tween back into place instead of snapping
        _dragParent.transform.localPosition = _dragParentOriginalPosition;

        if (TryPlaceCharm())
        {
            CharmPlaced?.Invoke(transform.GetSiblingIndex());
        }

        if (!_data.IsBead)
        {
            _charmDangle.EndDangle();
        }
    }

    private IEnumerator DragCoroutine()
    {
        while(true)
        {
            _dragParent.transform.position = Input.mousePosition + _dragOffset;

            Track t = GetTrackUnderMouse();
            if (t != _hoveredTrack)
            {
                if (_hoveredTrack != null)
                {
                    _hoveredTrack.ShowHighlight(false);
                }

                if (t != null)
                {
                    t.ShowHighlight(true);
                }

                _hoveredTrack = t;
            }
            yield return null;
        }
    }

    private Track GetTrackUnderMouse()
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

        foreach (RaycastHit hit in hits)
        {
            // Not a track, don't care
            Transform hitParnet = hit.transform.parent;
            Track track = hitParnet?.GetComponent<Track>();
            if (track == null)
            {
                continue;
            }

            return track;
        }

        return null;
    }

    private bool TryPlaceCharm()
    {
        if (_hoveredTrack != null)
        {
            _hoveredTrack.ShowHighlight(false);
        }

        Track t = GetTrackUnderMouse();
        t?.AddCharm(_data);
        return t != null;
    }
}
