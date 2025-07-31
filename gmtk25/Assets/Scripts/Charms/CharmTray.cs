using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharmTray : MonoBehaviour
{
    [SerializeField] private PlayerInventory _charmInventory;

    [Header("Hand")]
    [SerializeField] private int _startingHandSize = 3;
    [SerializeField] private int _maxHandSize = 5;
    [SerializeField] private CharmOption _optionTemplate;
    [SerializeField] private Transform _handParent;

    [Header("Draw")]
    [SerializeField] private float _drawCooldown = 2f;
    [SerializeField] private Slider _drawCooldownVisual;


    private readonly List<CharmData> _currentHand = new List<CharmData>();
    private bool _drawOnRefill;
    private bool _drawOnUse;
    private Coroutine _drawCoroutine;

    private void Start()
    {
        _charmInventory.Init();
        DrawInitialHand();

        _charmInventory.CharmReturned += OnCharmsRefilled;
        StartDrawCooldown();
    }

    private void DrawInitialHand()
    {
        for (int i = 0; i < _startingHandSize; i++)
        {
            DrawOne();
        }
    }

    // True if successfully drew
    private bool DrawOne()
    {
        CharmData charm = _charmInventory.DrawCharm();
        if (charm == null)
        {
            return false;
        }

        CharmOption newOption = Instantiate(_optionTemplate, _handParent);
        newOption.Init(charm);
        newOption.CharmPlaced += OnCharmPlaced;

        _currentHand.Add(charm);

        return true;
    }

    private void OnCharmPlaced(int index)
    {
        Transform child = _handParent.GetChild(index);
        Destroy(child.gameObject);

        if (_drawOnUse)
        {
            DrawOne();
            StartDrawCooldown();
            _drawOnUse = false;
        }
    }

    private void OnCharmsRefilled()
    {
        if (!_drawOnRefill)
        {
            return;
        }

        DrawOne();
        StartDrawCooldown();
        _drawOnRefill = false;
    }

    private void StartDrawCooldown()
    {
        if (_drawCoroutine != null)
        {
            return;
        }

        _drawCoroutine = StartCoroutine(DrawCooldown());
    }

    private IEnumerator DrawCooldown()
    {
        float timePassed = 0;
        while (timePassed < _drawCooldown)
        {
            timePassed += Time.deltaTime;
            _drawCooldownVisual.value = timePassed / _drawCooldown;
            yield return null;
        }

        // If there's hand space, draw and reset the CD
        _drawCoroutine = null;
        if (_handParent.childCount < _maxHandSize)
        {
            if(DrawOne())
            {
                StartDrawCooldown();
            }
            // Out of draw, draw when one becomes available
            else
            {
                _drawOnRefill = true;
            }
        }
        // Hand full, draw immediately on use
        else
        {
            _drawOnUse = true;
        }
    }
}
