using DG.Tweening;
using UnityEngine;

public class Forcefield : MonoBehaviour
{
    [SerializeField] private Ease fadeEase = Ease.Linear;
    public float fieldScale = 0.5f;
    private float _spreadSpeed = 8f;
    private SpriteRenderer spriteRenderer;

    private Tween _scaleTween;
    private Tween _fadeTween;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        transform.localScale = Vector3.zero;
        

    }


    public void spreadField(float fieldScale)
    {
        _scaleTween?.Kill();
        _fadeTween?.Kill();

        _fadeTween = spriteRenderer.DOFade(0f, _spreadSpeed).SetEase(fadeEase).SetLoops(-1, LoopType.Restart);
        _scaleTween = transform.DOScale(Vector3.one * fieldScale, _spreadSpeed).SetEase(Ease.Linear).SetLoops(-1,LoopType.Restart).OnComplete(() =>
        {
            transform.localScale = Vector3.zero;
        });
       
    }
    private void OnDestroy()
    {
        _scaleTween?.Kill();
        _fadeTween?.Kill();
    }
    private void OnDisable()
    {
        _scaleTween?.Kill();
        _fadeTween?.Kill();
    }
}
