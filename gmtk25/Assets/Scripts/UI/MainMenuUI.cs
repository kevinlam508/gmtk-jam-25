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
    [SerializeField][Range(0,1)] private float _settingsUiY;
    [SerializeField][Range(0,2)] private float _settingsUiTweenTime;
    void Awake()
    {
        _settingsWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSettingsOpened()
    {
        _settingsWindow.SetActive(true);
        _settingsUiBG.DOFade(1f, 1f).SetEase(_settingsUiElementsFadeIn);
        TweenIn_Settings();
    }

    public void OnSettingsClosed()
    {
        FadeOutCoroutine();
    }
    private IEnumerator FadeOutCoroutine()
    {
        Tween myTween = _settingsUiBG.DOFade(0f, 1f).SetEase(_settingsUiElementsFadeIn);
        
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
}
