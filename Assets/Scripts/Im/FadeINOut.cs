using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeINOut : MonoBehaviour
{

    [SerializeField] private RectTransform transitionImage;
    [SerializeField] private GameObject Blackout;
    [SerializeField] private Ease scaleEase = Ease.InOutQuad;
    [SerializeField] private Ease MoveEase = Ease.InOutQuad;

    private Vector2 startingPoint;
    private Tween _tween;
    private Tween _moveTween;
    private void Awake()
    {
        startingPoint = transitionImage.anchoredPosition;
        transitionImage.gameObject.SetActive(false);
        Blackout.gameObject.SetActive(false);
    }

    public void FadeIn()
    {
        _tween?.Kill();
        _moveTween?.Kill();
        transitionImage.localScale = Vector3.one*0.1f;
        Blackout.gameObject.SetActive(false);
        transitionImage.gameObject.SetActive(true);
        _tween = transitionImage.DOScale(15f, 2f).SetEase(scaleEase);

        _moveTween = transitionImage.DOAnchorPos(startingPoint + Vector2.down * 1600f, 2f).SetEase(MoveEase).OnComplete(() =>
        {
            
            transitionImage.gameObject.SetActive(false);
            transitionImage.anchoredPosition = startingPoint; ;
            
        });


    }
    public void FadeOut()
    {
        _tween?.Kill();
        _moveTween?.Kill();
        transitionImage.localScale = Vector3.one * 15f;
        transitionImage.gameObject.SetActive(true);
        _tween = transitionImage.DOScale(0.1f, 2f).SetEase(scaleEase).OnComplete(() =>
        {
            Blackout.gameObject.SetActive(true);

        });
    }
}
