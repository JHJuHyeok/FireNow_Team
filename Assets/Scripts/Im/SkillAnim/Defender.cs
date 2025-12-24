using DG.Tweening;
using UnityEngine;

public class Defender : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 0.5f;
    [SerializeField] Sprite nomalSprite;
    [SerializeField] Sprite evolutionSprite;

    [Header("Damage Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageInterval = 0.5f;


    private Tween _spintween;
    private SpriteRenderer _spriteRenderer;


    private CircleCollider2D _collider;


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();




        _collider = GetComponent<CircleCollider2D>();
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<CircleCollider2D>();

        }
        _collider.isTrigger = true;
        _collider.radius = 0.5f;
    }
    private void OnEnable()
    {
        Spin();
        
    }
    private void Spin()
    {
        
        _spintween = transform.DORotate(new Vector3(0, 0, -360), spinSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);

    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
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
