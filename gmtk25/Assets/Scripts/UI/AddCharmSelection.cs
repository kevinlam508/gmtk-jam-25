using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AddCharmSelection : MonoBehaviour
{
    [SerializeField] private string _allCharmsFolder = "Charms";
    [SerializeField] private PlayerInventory _inventory;

    [SerializeField] private AddCharmButton[] _buttons;
    [SerializeField] private CanvasGroup _canvasGroup;

    private CharmData[] _allCharms;

    private void Awake()
    {
        _allCharms = Resources.LoadAll<CharmData>(_allCharmsFolder);


        foreach (AddCharmButton button in _buttons)
        {
            button.ButtonSelected += CharmSelected;
        }
        Hide();
    }

    public void ShowAndGenerate()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        GenerateOptions();
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    private void CharmSelected(CharmData data)
    {
        _inventory.AddCharm(data);
        Hide();
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
}
