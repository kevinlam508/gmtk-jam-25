using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;


public class MainMenuUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject _settingsWindow;
    [SerializeField] private AnimationCurve _settingsUiElementsCurveIn;
    [SerializeField] private AnimationCurve _settingsUiElementsFadeIn;
    [SerializeField] private RectTransform _settingsUiElements;
    [SerializeField] private Image _settingsUiBG;
    [SerializeField] private GameObject _creditsWindow;
    [SerializeField] private RectTransform _creditsUiElements;
    [SerializeField] private Image _creditsUiBG;
    [SerializeField][Range(0, 1)] private float _settingsUiY;
    [SerializeField][Range(0, 2)] private float _settingsUiTweenTime;
    void Awake()
    {
        _settingsWindow.SetActive(false);
        _creditsWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSettingsOpened()
    {
        _settingsWindow.SetActive(true);
        _settingsUiBG.DOFade(.4f, 1f).SetEase(_settingsUiElementsFadeIn);
        TweenIn_Settings();
    }

    public void OnSettingsClosed()
    {
        StartCoroutine(FadeOutCoroutine());
    }
    private IEnumerator FadeOutCoroutine()
    {
        Tween myTween = _settingsUiBG.DOFade(0f, 1f).SetEase(_settingsUiElementsFadeIn);
        _settingsUiElements.DOMoveY(Screen.height * 1.5f, _settingsUiTweenTime).SetEase(_settingsUiElementsCurveIn);
        yield return myTween.WaitForCompletion();
        _settingsWindow.SetActive(false);
        // This log will happen after the tween has completed
        Debug.Log("Tween completed!");
    }
    public void TweenIn_Settings()
    {
        Sequence TweenInSequence = DOTween.Sequence();
        TweenInSequence.AppendInterval(.5f);
        TweenInSequence.Append(_settingsUiElements.DOMoveY(Screen.height * _settingsUiY, _settingsUiTweenTime).SetEase(_settingsUiElementsCurveIn));
        TweenInSequence.Play();
    }
    public void OnCreditsOpened()
    {
        _creditsWindow.SetActive(true);
        _creditsUiBG.DOFade(.4f, 1f).SetEase(_settingsUiElementsFadeIn);
        TweenIn_Credits();
    }

    public void OnCreditsClosed()
    {
        StartCoroutine(FadeOutCreditsCoroutine());
    }
    private IEnumerator FadeOutCreditsCoroutine()
    {
        Tween myTween = _creditsUiBG.DOFade(0f, 1f).SetEase(_settingsUiElementsFadeIn);
        _creditsUiElements.DOMoveY(Screen.height * 1.5f, _settingsUiTweenTime).SetEase(_settingsUiElementsCurveIn);
        yield return myTween.WaitForCompletion();
        _creditsWindow.SetActive(false);
        // This log will happen after the tween has completed
        Debug.Log("Tween completed!");
    }
    public void TweenIn_Credits()
    {
        Sequence TweenInSequence = DOTween.Sequence();
        TweenInSequence.AppendInterval(.5f);
        TweenInSequence.Append(_creditsUiElements.DOMoveY(Screen.height * _settingsUiY, _settingsUiTweenTime).SetEase(_settingsUiElementsCurveIn));
        TweenInSequence.Play();
    }
}
