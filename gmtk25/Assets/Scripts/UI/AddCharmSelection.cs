using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

using Random = UnityEngine.Random;

public class AddCharmSelection : MonoBehaviour
{
    public static AddCharmSelection Instance { get; private set; }

    [SerializeField] private string _allCharmsFolder = "Charms";
    [SerializeField] private PlayerInventory _inventory;

    [Header("Add")]
    [SerializeField] private AddCharmButton[] _buttons;

    [Header("Remove")]
    [SerializeField] private Transform _removeParent;
    [SerializeField] private AddCharmButton _removePrefab;
    private readonly List<AddCharmButton> _removeButtonPool = new List<AddCharmButton>();
    [SerializeField] private AnimationCurve _trayMoveInCurve;
    [SerializeField][Range(0, 1)] private float _trayMoveInY;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private CanvasGroup _removeStepCanvasGroup;
    [SerializeField] private CanvasGroup _addStepCanvasGroup;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private RectTransform _trayTransform;
    [SerializeField] private UnityEvent _opened;
    [SerializeField] private UnityEvent _closed;

    private CharmData[] _allCharms;

    private void Awake()
    {
        _allCharms = Resources.LoadAll<CharmData>(_allCharmsFolder);

        Instance = this;

        foreach (AddCharmButton button in _buttons)
        {
            button.ButtonSelected += AddCharmSelected;
        }
        Hide(false);

    }

    public UnityEvent GetClosedEvent()
    {
        return _closed;
    }

    public void ShowAndGenerate()
    {
        // _opened.Invoke();
        // _canvasGroup.DOFade(1f, 3f).SetEase(Ease.OutQuad);
        // // _canvasGroup.alpha = 1;
        // _canvasGroup.blocksRaycasts = true;
        // _canvasGroup.interactable = true;

        // GenerateOptions();
        // EnableAddButtons(true);
        // GenerateRemoveOptions();
        StartCoroutine(LoadRemoveStep());
    }

    private IEnumerator LoadRemoveStep()
    {
        _opened.Invoke();
        GenerateOptions();
        EnableAddButtons(true);
        GenerateRemoveOptions();
        _canvasGroup.alpha = 1;
        // yield return _backgroundImage.DOFade(1f, 1f).SetEase(Ease.OutQuad).WaitForCompletion();
        Sequence TweenInSequence = DOTween.Sequence();
        TweenInSequence.Prepend(_backgroundImage.DOFade(.98f, 1f).SetEase(Ease.OutQuad));
        TweenInSequence.Append(_trayTransform.DOMoveY(Screen.height * _trayMoveInY, 1f).SetEase(_trayMoveInCurve));
        TweenInSequence.Join(_removeStepCanvasGroup.DOFade(1f, 1f).SetEase(Ease.OutQuad));
        yield return TweenInSequence.WaitForCompletion();
        _removeStepCanvasGroup.blocksRaycasts = true;
        _removeStepCanvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        // yield return null;
    }
    public void OnLoadAddStepButtonPress()
    {
        StartCoroutine(LoadAddStep());
    }
    private IEnumerator LoadAddStep()
    {
        _removeStepCanvasGroup.blocksRaycasts = false;
        _removeStepCanvasGroup.interactable = false;
        // yield return _backgroundImage.DOFade(1f, 1f).SetEase(Ease.OutQuad).WaitForCompletion();
        Sequence TweenInSequence = DOTween.Sequence();
        TweenInSequence.Join(_removeStepCanvasGroup.DOFade(0f, 1f).SetEase(Ease.OutQuad));
        TweenInSequence.Append(_addStepCanvasGroup.DOFade(1f, 1f).SetEase(Ease.OutQuad));
        yield return TweenInSequence.WaitForCompletion();
        _addStepCanvasGroup.blocksRaycasts = true;
        _addStepCanvasGroup.interactable = true;

    }

    public void Hide(bool triggerCallback = true)
    {
        StartCoroutine(HideStep(triggerCallback));
    }
    private IEnumerator HideStep(bool triggerCallback)
    {
        Sequence TweenInSequence = DOTween.Sequence();
        TweenInSequence.Prepend(_backgroundImage.DOFade(0f, 1f).SetEase(Ease.OutQuad));
        TweenInSequence.Join(_removeStepCanvasGroup.DOFade(0f, 1f).SetEase(Ease.OutQuad));
        TweenInSequence.Join(_trayTransform.DOMoveY(Screen.height * -2f, 1f).SetEase(_trayMoveInCurve));
        TweenInSequence.Join(_addStepCanvasGroup.DOFade(0f, 1f).SetEase(Ease.OutQuad));
        yield return TweenInSequence.WaitForCompletion();
        _addStepCanvasGroup.blocksRaycasts = false;
        _addStepCanvasGroup.interactable = false;
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
        Hide(true);
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
            int i = Random.Range(0, _allCharms.Length);
            while (drawn.Contains(_allCharms[i]))
            {
                i = Random.Range(0, _allCharms.Length);
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

        OnLoadAddStepButtonPress();
    }
}
