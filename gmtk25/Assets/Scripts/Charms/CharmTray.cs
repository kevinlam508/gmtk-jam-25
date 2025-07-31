using System.Collections.Generic;
using UnityEngine;

public class CharmTray : MonoBehaviour
{
    [SerializeField] private PlayerInventory _charmInventory;
    [SerializeField] private int _startingHandSize = 3;
    [SerializeField] private int _maxHandSize = 5;

    [SerializeField] private CharmOption _optionTemplate;
    [SerializeField] private Transform _handParent;

    private readonly List<CharmData> _currentHand = new List<CharmData>();

    private void Start()
    {
        _charmInventory.Init();
        DrawInitialHand();
    }

    private void DrawInitialHand()
    {
        for (int i = 0; i < _startingHandSize; i++)
        {
            DrawOne();
        }
    }

    private void DrawOne()
    {
        CharmData charm = _charmInventory.DrawCharm();
        if (charm == null)
        {
            return;
        }

        CharmOption newOption = Instantiate(_optionTemplate, _handParent);
        newOption.Init(charm);
        newOption.CharmPlaced += OnCharmPlaced;

        _currentHand.Add(charm);
    }

    private void OnCharmPlaced(int index)
    {
        Transform child = _handParent.GetChild(index);
        Destroy(child.gameObject);
    }
}
