using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class AddCharmSelection : MonoBehaviour
{
    [SerializeField] private string _allCharmsFolder = "Charms";
    [SerializeField] private PlayerInventory _inventory;

    [Header("Add")]
    [SerializeField] private AddCharmButton[] _buttons;

    [Header("Remove")]
    [SerializeField] private Transform _removeParent;
    [SerializeField] private AddCharmButton _removePrefab;
    private readonly List<AddCharmButton> _removeButtonPool = new List<AddCharmButton>();

    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private UnityEvent _opened;
    [SerializeField] private UnityEvent _closed;

    private CharmData[] _allCharms;

    private void Awake()
    {
        _allCharms = Resources.LoadAll<CharmData>(_allCharmsFolder);

        foreach (AddCharmButton button in _buttons)
        {
            button.ButtonSelected += AddCharmSelected;
        }
        Hide(false);
    }

    public void ShowAndGenerate()
    {
        _opened.Invoke();

        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        GenerateOptions();
        EnableAddButtons(true);
        GenerateRemoveOptions();
    }

    public void Hide(bool triggerCallback = true)
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        if (triggerCallback)
        {
            _closed.Invoke();
        }
    }

    private void AddCharmSelected(CharmData data)
    {
        _inventory.AddCharm(data);
        EnableAddButtons(false);
    }

    private void EnableAddButtons(bool enable)
    {
        foreach (AddCharmButton button in _buttons)
        {
            button.Interactable = enable;
        }
    }

    private void GenerateOptions()
    {
        HashSet<CharmData> drawn = new HashSet<CharmData>();
        foreach (AddCharmButton button in _buttons)
        {
            int i = Random.Range(0, _allCharms.Length - 1);
            while(drawn.Contains(_allCharms[i]))
            {
                i = Random.Range(0, _allCharms.Length - 1);
            }

            CharmData draw = _allCharms[i];
            button.Assign(draw);
            drawn.Add(draw);
        }
    }

    private void GenerateRemoveOptions()
    {
        List<CharmData> charms = _inventory.EntireDeck;

        for (int i = 0; i < charms.Count; i++)
        {
            if (i < _removeButtonPool.Count)
            {
                _removeButtonPool[i].Interactable = true;
                _removeButtonPool[i].gameObject.SetActive(true);
                _removeButtonPool[i].Assign(charms[i]);
            }
            else
            {
                AddCharmButton newButton = Instantiate(_removePrefab, _removeParent);
                newButton.ButtonSelected += RemoveCharmSelected;
                newButton.Assign(charms[i]);
                _removeButtonPool.Add(newButton);
            }
        }

        for (int i = charms.Count; i < _removeButtonPool.Count; i++)
        {
            _removeButtonPool[i].gameObject.SetActive(false);
        }
    }

    private void RemoveCharmSelected(CharmData data)
    {
        _inventory.EntireDeck.Remove(data);

        foreach (AddCharmButton button in _removeButtonPool)
        {
            button.Interactable = false;
        }
    }
}
