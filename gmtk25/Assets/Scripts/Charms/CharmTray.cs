using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CharmTray : MonoBehaviour
{
    [SerializeField] private PlayerInventory _charmInventory;
    [SerializeField] private UnityEvent _charmSelected;
    [SerializeField] private UnityEvent _charmUnselected;

    [Header("Appearance")]
    [SerializeField] private CharmOption _optionTemplate;
    [SerializeField] private Transform _handParent;
    [SerializeField] private Transform _pileParent;

    [Header("Hand")]
    [SerializeField] private int _startingHandSize = 3;
    [SerializeField] private int _startingMaxHandSize = 5;
    [SerializeField] private int _minMaxHandSize = 1;
    [SerializeField] private int _maxMaxHandSize = 1;

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

    private readonly Dictionary<CharmData, List<CharmOption>> _optionPile = new Dictionary<CharmData, List<CharmOption>>();

    private void Start()
    {
        _charmInventory.CharmReturned += OnCharmsRefilled;
        _charmInventory.Init();
        ResetForNextRound();
    }

    public void ResetForNextRound()
    {
        _currentHand.Clear();
        _optionPile.Clear();
        for (int i = 0; i < _handParent.childCount; i++)
        {
            Destroy(_handParent.GetChild(i).gameObject);
        }
        for (int i = 0; i < _pileParent.childCount; i++)
        {
            Destroy(_pileParent.GetChild(i).gameObject);
        }

        _currentDrawCooldown = _startingDrawCooldown;
        _currentMaxHandSize = _startingMaxHandSize;

        _charmInventory.SetupForRound();
        AddCharmsToPile();
        DrawInitialHand();

        StartDrawCooldown();
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

        List<CharmOption> optionInstances = _optionPile[charm];
        CharmOption option = optionInstances[0];
        optionInstances.RemoveAt(0);

        option.Interactable = true;
        option.transform.SetParent(_handParent, true);

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

    private void AddCharmsToPile()
    {
        Dictionary<CharmData, int> instanceCount = new Dictionary<CharmData, int>();
        foreach (CharmData data in _charmInventory.DrawPile)
        {
            instanceCount.TryGetValue(data, out int count);
            count++;
            instanceCount[data] = count;


            if (!_optionPile.TryGetValue(data, out var list))
            {
                list = new List<CharmOption>();
                _optionPile.Add(data, list);
            }

            if (list.Count < count)
            {
                CharmOption newOption = Instantiate(_optionTemplate, _pileParent);
                newOption.Init(data);
                newOption.CharmPlaced += OnCharmPlaced;
                newOption.CharmHeld += OnCharmHeld;
                newOption.Interactable = false;

                list.Add(newOption);
            }
        }
    }

    private void AddCharmToPile(CharmData data)
    {
        if (!_optionPile.TryGetValue(data, out var list))
        {
            list = new List<CharmOption>();
            _optionPile.Add(data, list);
        }

        CharmOption newOption = Instantiate(_optionTemplate, _pileParent);
        newOption.Init(data);
        newOption.CharmPlaced += OnCharmPlaced;
        newOption.Interactable = false;

        list.Add(newOption);
    }

    private void OnCharmsRefilled(CharmData data)
    {
        AddCharmToPile(data);
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

    private void OnCharmHeld(bool held)
    {
        if (held)
        {
            _charmSelected.Invoke();
        }
        else
        {
            _charmUnselected.Invoke();
        }
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
