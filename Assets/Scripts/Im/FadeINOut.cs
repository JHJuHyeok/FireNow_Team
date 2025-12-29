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

    [Header("연출시간")]//침범-윤성원
    [SerializeField] private float fadeOutDuration = 1.5f;
    [SerializeField] private float fadeInDuration = 0.5f;

    //연출시간 외부 접근용-침범 윤성원
    public float FadeOutDuration { get { return fadeOutDuration; } }
    public float FadeInDuration { get { return fadeInDuration; } }

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
        _tween = transitionImage.DOScale(30f, fadeInDuration).SetEase(scaleEase);

        _moveTween = transitionImage.DOAnchorPos(startingPoint + Vector2.right * 1000f, fadeInDuration).SetEase(MoveEase).OnComplete(() =>
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
        _tween = transitionImage.DOScale(0.1f, fadeOutDuration).SetEase(scaleEase).OnComplete(() =>
        {
            //씬 전환 구간 보험역할 =블랙이 꺼지고 켜지면서 공백 보임-강제로 블랙 켜버리기-윤성원
            if (Blackout != null) Blackout.SetActive(true);

            Blackout.gameObject.SetActive(true);

        });
    }
}
