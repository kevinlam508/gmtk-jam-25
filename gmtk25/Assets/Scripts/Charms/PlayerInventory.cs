using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Scriptable Objects/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{
    public event Action<CharmData> CharmReturned;

    [SerializeField] private List<CharmData> _deck;

    private readonly List<CharmData> _entireDeck = new List<CharmData>();
    private readonly List<CharmData> _drawPile = new List<CharmData>();
    private readonly List<CharmData> _discardPile = new List<CharmData>();

    public List<CharmData> EntireDeck => _entireDeck;
    public List<CharmData> DrawPile => _drawPile;

    public void Init()
    {
        _entireDeck.AddRange(_deck);
    }

    public void SetupForRound()
    {
        _discardPile.Clear();
        _drawPile.Clear();

        _discardPile.AddRange(_entireDeck);
        ShuffleDiscardIntoDraw();
    }

    public CharmData DrawCharm()
    {
        if (_drawPile.Count == 0)
        {
            ShuffleDiscardIntoDraw();

            // Nothing to shuffle back in, empty draw
            if (_drawPile.Count == 0)
            {
                return null;
            }
        }

        CharmData data = _drawPile[0];
        _drawPile.RemoveAt(0);
        return data;
    }


    public void ReturnCharm(CharmData data)
    {
        _discardPile.Add(data);
        CharmReturned?.Invoke(data);
    }

    public void AddCharm(CharmData data)
    {
        _entireDeck.Add(data);
    }

    private void ShuffleDiscardIntoDraw()
    {
        for(int i = 0; i < _discardPile.Count; i++)
        {
            int randomIndex = Random.Range(0, _discardPile.Count - 1);

            var temp = _discardPile[i];
            _discardPile[i] = _discardPile[randomIndex];
            _discardPile[randomIndex] = temp;
        }

        _drawPile.AddRange(_discardPile);
        _discardPile.Clear();
    }
}
