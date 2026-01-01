using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ForceFieldSponer : MonoBehaviour
{
    [SerializeField] private GameObject spreadPrefab;

    private int count = 4;
    private float interval = 2f;
    [SerializeField] private float maxScale = 0.7f;
    [SerializeField] private bool _Eveloution = true;

    [Header("Damage Settings")]
    [SerializeField] private float damageRate = 1f; // damage -> damageRate로 변경
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private float damageRange = 1.0f;

    private Coroutine _sequentialCoroutine;
    private PlayerController playerController; // 추가

    private void Awake() // 추가
    {
        playerController = GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    private void Start()
    {
        _sequentialCoroutine = StartCoroutine(Sequential());
    }

    public void ReStart(float currentScale, bool Evelutuon, float newDamageRate, float newRange)
    {
        damageRate = newDamageRate; // damageRate로 변경
        damageRange = newRange;
        maxScale = currentScale;
        _Eveloution = Evelutuon;

        if (_sequentialCoroutine != null)
        {
            StopCoroutine(_sequentialCoroutine);
            _sequentialCoroutine = null;
        }
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        DOTween.PauseAll();
        endSequential();
        yield return null;
        DOTween.PlayAll();
        _sequentialCoroutine = StartCoroutine(Sequential());
    }

    public void endSequential()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator Sequential()
    {
        for (int i = 0; i < count; i++)
        {
            GameObject field = Instantiate(spreadPrefab, transform.position, Quaternion.identity, transform);
            SpriteRenderer spriteTenderer = field.GetComponent<SpriteRenderer>();

            if (_Eveloution == true)
            {
                spriteTenderer.color = new Color32(234, 116, 52, 255);
            }
            else
            {
                spriteTenderer.color = new Color32(77, 188, 79, 255);
            }

            if (i % 2 == 1)
            {
                field.transform.localRotation = Quaternion.Euler(0, 0, 22.5f);
            }

            Forcefield forcefield = field.GetComponent<Forcefield>();
            if (forcefield != null)
            {
                float visualScale = 0.3f + (damageRange * 0.4f);
                forcefield.spreadField(visualScale);
            }

            // 최종 데미지 계산 추가
            float baseDamage = playerController != null ? playerController.GetAttackPower() : 10f;
            float finalDamage = baseDamage * damageRate;

            ForceFieldDamage damageComponent = field.GetComponent<ForceFieldDamage>();
            if (damageComponent != null)
            {
                damageComponent.Initialize(finalDamage, damageInterval, damageRange);
            }

            yield return new WaitForSeconds(interval);
        }
    }
}