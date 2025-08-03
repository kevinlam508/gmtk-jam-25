using UnityEngine;
using UnityEngine.UI;
using System;

public class AddCharmButton : MonoBehaviour
{
    public event Action<CharmData> ButtonSelected;

    [SerializeField] public Button _addButton;
    [SerializeField] private Image _iamge;

    private CharmData _data;

    public bool Interactable
    {
        get => _addButton.interactable;
        set => _addButton.interactable = value;
    }

    private void Start()
    {
        _addButton.onClick.AddListener(OnButtonPressed);
    }

    public void Assign(CharmData data)
    {
        _data = data;
        _iamge.sprite = data.UiSprite;
    }

    private void OnButtonPressed()
    {
        ButtonSelected?.Invoke(_data);
    }
}
