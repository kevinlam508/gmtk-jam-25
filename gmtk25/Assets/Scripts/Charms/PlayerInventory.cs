using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Scriptable Objects/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{
    public event Action CharmReturned;

    [SerializeField] private List<CharmData> _deck;

    private readonly List<CharmData> _drawPile = new List<CharmData>();
    private readonly List<CharmData> _discardPile = new List<CharmData>();

    public void Init()
    {
        _discardPile.AddRange(_deck);
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
        CharmReturned?.Invoke();
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
