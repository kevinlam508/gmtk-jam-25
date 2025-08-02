using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    public AudioSource source;
    public AudioClip hover, click;
    [SerializeField] private TMPro.TMP_Text _text;
    // private DOTweenTMPAnimator _animator;
    void Awake()
    {
        // _animator = new DOTweenTMPAnimator(_text);
    }
    private void Start()
    {
        LetterLoadIn();
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
        source.PlayOneShot(hover);
    }

    public void OnClickSFX()
    {
        source.PlayOneShot(click);
    }

    public void LetterLoadIn()
    {
        
        // Sequence sequence = DOTween.Sequence();
        // sequence.AppendInterval(1.5f);
        // for (int i = 0; i < _animator.textInfo.characterCount; ++i)
        // {
        //     if (!_animator.textInfo.characterInfo[i].isVisible) continue;
        //     Vector3 currCharOffset = _animator.GetCharOffset(i);
        //     // sequence.AppendInterval(0.05f);
        //     sequence.Insert((float)i * .25f,_animator.DOOffsetChar(i, currCharOffset + new Vector3(2000, 0, 0), 1).From());
        // }
        // sequence.Play();
    }


}
