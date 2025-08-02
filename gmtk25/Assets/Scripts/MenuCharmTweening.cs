using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuCharmTweening : MonoBehaviour
{
    [SerializeField][Range(0, 2)] private float ButtonHoverScale;
    [SerializeField][Range(0, 2)] private float ButtonHoverScaleTime;
    [SerializeField] private AnimationCurve ButtonHoverScaleCurve;
    [SerializeField][Range(0, 1f)] private float _waitTimeMax;
    [SerializeField][Range(0, 1f)] private float _waitTimeMin;
    public AudioSource source;
    public AudioClip hover, click;
    // [SerializeField] private TMPro.TMP_Text _text;
    private DOTweenTMPAnimator _animator;
    [SerializeField] private Transform _charmMesh;
    private Vector3 _initialScale;
    private float _waitTime;
    void Awake()
    {
        _initialScale = _charmMesh.localScale;
        _waitTime = Random.Range(_waitTimeMin, _waitTimeMax);
    }
    void Start()
    {

    }

    void Update()
    {

    }

    public void OnHoverEnterTween(Button button)
    {
        Sequence hoverEnterSequence = DOTween.Sequence();
        hoverEnterSequence.AppendInterval(_waitTime);
        hoverEnterSequence.Append(button.GetComponent<RectTransform>().DOScale(ButtonHoverScale, ButtonHoverScaleTime).SetEase(ButtonHoverScaleCurve));
        hoverEnterSequence.Join(_charmMesh.DOScale(ButtonHoverScale * _initialScale, ButtonHoverScaleTime).SetEase(ButtonHoverScaleCurve));
        hoverEnterSequence.Play();
        
    }

    public void OnHoverExitTween(Button button)
    {
        Sequence hoverExitSequence = DOTween.Sequence();
        _waitTime = Random.Range(_waitTimeMin, _waitTimeMax);
        hoverExitSequence.AppendInterval(_waitTime);
        hoverExitSequence.Append(button.GetComponent<RectTransform>().DOScale(1, ButtonHoverScaleTime).SetEase(ButtonHoverScaleCurve));
        hoverExitSequence.Join(_charmMesh.DOScale(_initialScale, ButtonHoverScaleTime).SetEase(ButtonHoverScaleCurve));
        hoverExitSequence.Play();
    }
    
    
    public void OnClickPlay()
    {
        // GameManager.Instance.Reset();
    }

    public void OnClickHowToPlay()
    {
        // SceneManager.LoadScene( "HowToPlay" );
    }

    public void OnClickQuit()
    {
        // Application.Quit();
    }

    public void OnHoverSFX()
    {
        if(hover!=null)
            source.PlayOneShot(hover);
    }

    public void OnClickSFX()
    {
        if(click!=null)
            source.PlayOneShot(click);
    }

    
}
