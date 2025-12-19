using DG.Tweening;
using UnityEngine;

public class Defender : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 0.5f;
    [SerializeField] Sprite nomalSprite;
    [SerializeField] Sprite evolutionSprite;
    private Tween _spintween;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        Spin();
    }
    private void Spin()
    {
        
        _spintween = transform.DORotate(new Vector3(0, 0, -360), spinSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);

    }
    public void setEvolution(bool isEvolution )
    {
        _spriteRenderer.sprite = isEvolution ? evolutionSprite : nomalSprite;
    }
    private void OnDisable()
    {
        _spintween?.Kill();
    }
}
