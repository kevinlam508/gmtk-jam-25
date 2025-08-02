using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler

{
    [SerializeField] private bool isButton;
    [SerializeField] private float hoverScale = 1.07f;
    [SerializeField] private float returnScale = 1f;
    [SerializeField] private float clickScale = 0.98f;
    private float hoverTweenDuration = 0.15f;
    private float clickTweenDuration = 0.05f;
    private float returnToDuration = 0.05f;
    private bool ran = false;
    private Ease hoverEase = Ease.OutExpo;
    private Ease clickEase = Ease.OutBack;

    // hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!ran)
        {
            if (isButton && gameObject.GetComponent<Button>().interactable == true)
            {
                gameObject.transform.DOScale(hoverScale, hoverTweenDuration).SetEase(hoverEase);
            }
            else gameObject.transform.DOScale(hoverScale, hoverTweenDuration).SetEase(hoverEase);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.DOKill();
        gameObject.transform.DOScale(returnScale, returnToDuration);
    }


    // click
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!ran)
        {
            if (isButton && gameObject.GetComponent<Button>().interactable == true)
            {
                gameObject.transform.DOScale(clickScale, clickTweenDuration).SetEase(clickEase);
            }
            else gameObject.transform.DOScale(clickScale, clickTweenDuration).SetEase(clickEase);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.transform.DOKill();
        this.transform.DOScale(returnScale, returnToDuration);
        ran = false;
    }
}