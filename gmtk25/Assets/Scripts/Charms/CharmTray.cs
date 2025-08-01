using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharmTray : MonoBehaviour
{
    [SerializeField] private PlayerInventory _charmInventory;

    [Header("Hand")]
    [SerializeField] private int _startingHandSize = 3;
    [SerializeField] private int _startingMaxHandSize = 5;
    [SerializeField] private int _minMaxHandSize = 1;
    [SerializeField] private int _maxMaxHandSize = 1;
    [SerializeField] private CharmOption _optionTemplate;
    [SerializeField] private Transform _handParent;

    [Header("Draw")]
    [SerializeField] private float _startingDrawCooldown = 2f;
    [SerializeField] private float _minDrawCooldown = .1f;
    [SerializeField] private float _maxDrawCooldown = 3f;
    [SerializeField] private Slider _drawCooldownVisual;


    private readonly List<CharmData> _currentHand = new List<CharmData>();
    private bool _drawOnRefill;
    private bool _drawOnUse;
    private Coroutine _drawCoroutine;

    private float _currentDrawCooldown;
    private int _currentMaxHandSize;

    private void Start()
    {
        ResetHandStats();

        _charmInventory.Init();
        DrawInitialHand();

        _charmInventory.CharmReturned += OnCharmsRefilled;
        StartDrawCooldown();
    }

    public void ResetHandStats()
    {
        _currentDrawCooldown = _startingDrawCooldown;
        _currentMaxHandSize = _startingMaxHandSize;
    }

    public void ModifyDrawCooldown(float amount)
    {
        _currentDrawCooldown += amount;
        _currentDrawCooldown = Mathf.Clamp(_currentDrawCooldown, _minDrawCooldown, _maxDrawCooldown);
    }

    public void ModifyMaxHandSize(int amount)
    {
        _currentMaxHandSize += amount;
        _currentMaxHandSize = Mathf.Clamp(_currentMaxHandSize, _minMaxHandSize, _maxMaxHandSize);

        if (amount > 0 && _drawOnUse)
        {
            TryDrawOne();
            _drawOnUse = false;
        }
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
            TryDrawOne();
            _drawOnUse = false;
        }
    }

    private void OnCharmsRefilled()
    {
        if (!_drawOnRefill)
        {
            return;
        }

        TryDrawOne();
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
        while (timePassed < _currentDrawCooldown)
        {
            timePassed += Time.deltaTime;
            _drawCooldownVisual.value = timePassed / _currentDrawCooldown;
            yield return null;
        }

        // If there's hand space, draw and reset the CD
        _drawCoroutine = null;
        TryDrawOne();
    }

    private void TryDrawOne()
    {
        StartCoroutine(DelayedDraw());

        // Delay actual draw by 1 frame to wait for any cleanup effects
        IEnumerator DelayedDraw()
        {
            yield return null;

            if (_handParent.childCount < _currentMaxHandSize)
            {
                if (DrawOne())
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
}
