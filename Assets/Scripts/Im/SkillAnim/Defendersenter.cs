using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defendersenter : MonoBehaviour
{
    [SerializeField] private GameObject prefabDefender;
    [SerializeField] private int MaxDefenderCount = 6;
    [Range(2, 6)] public int defenderCount = 2;
    public float revolutionSpeed = 1f;
    public float spinradius = 1.5f;
    public float LifeTime = 4f;
    public float coolTime = 4f;
    public bool _eveloution = false;

    [Header("Damage Settings")]
    [SerializeField] private float damageRate = 1f; // 배율로 변경
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private float damageRange = 1.0f;
    private float damage = 10f; // 데미지 변수 추가

    private List<GameObject> _pool = new();
    private Tween _revolutiontween;
    private Coroutine _lifeCoroutine;
    private PlayerController playerController; // 추가

    private void Awake()
    {
        CreatePool();

        // PlayerController 참조 가져오기
        playerController = GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
    }
    public void SetDamage(float newDamage)
    {
        damage = newDamage;

        // 이미 생성된 Defender들에게도 데미지 적용
        foreach (Transform child in transform)
        {
            Defender defender = child.GetComponent<Defender>();
            if (defender != null)
            {
                defender.SetDamage(newDamage);
            }
        }
    }
    private void OnEnable()
    {
        ActivateDefenders();
        revolves();
        _lifeCoroutine = StartCoroutine(LifeCycle());
    }

    private void OnDisable()
    {
        _revolutiontween?.Kill();
        if (_lifeCoroutine != null)
            StopCoroutine(_lifeCoroutine);
        DeactivationDefenders();
    }

    private void CreatePool()
    {
        _pool.Clear();
        for (int i = 0; i < MaxDefenderCount; i++)
        {
            GameObject obj = Instantiate(prefabDefender, transform);

            DefenderDamage damageComponent = obj.GetComponent<DefenderDamage>();
            if (damageComponent == null)
            {
                damageComponent = obj.AddComponent<DefenderDamage>();
            }

            obj.SetActive(false);
            _pool.Add(obj);
        }
    }

    private void ActivateDefenders()
    {
        // 최종 데미지 계산
        float baseDamage = playerController != null ? playerController.GetAttackPower() : 10f;
        float finalDamage = baseDamage * damageRate;

        for (int i = 0; i < _pool.Count; i++)
        {
            GameObject Obj = _pool[i];
            if (i < defenderCount)
            {
                float angel = i * Mathf.PI * 2f / defenderCount;
                Vector2 offset = new Vector2(Mathf.Cos(angel), Mathf.Sin(angel)) * spinradius;

                Obj.transform.localPosition = offset;

                Defender defender = Obj.GetComponent<Defender>();
                if (defender != null)
                {
                    defender.setEvolution(_eveloution);
                }

                // 최종 데미지 전달
                DefenderDamage damageComponent = Obj.GetComponent<DefenderDamage>();
                if (damageComponent != null)
                {
                    damageComponent.Initialize(finalDamage, damageInterval, damageRange);
                }

                Obj.SetActive(true);
            }
            else
            {
                Obj.SetActive(false);
            }
        }
    }

    // 스탯 업데이트 메서드 추가 (AbilitySelectionManager에서 호출)
    public void UpdateStats(float newDamageRate)
    {
        damageRate = newDamageRate;
        ActivateDefenders(); // 재활성화로 데미지 갱신
    }

    private void DeactivationDefenders()
    {
        foreach (var obj in _pool)
        {
            obj.SetActive(false);
        }
    }

    private IEnumerator LifeCycle()
    {
        while (!_eveloution)
        {
            yield return new WaitForSeconds(LifeTime);
            DeactivationDefenders();
            yield return new WaitForSeconds(coolTime);
            ActivateDefenders();
        }
    }

    private void revolves()
    {
        _revolutiontween?.Kill();
        _revolutiontween = transform.DOLocalRotate(new Vector3(0, 0, -360), revolutionSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
    }

    public void SetEvolutution(bool value)
    {
        _eveloution = value;
        foreach (var obj in _pool)
        {
            if (!obj.activeSelf) continue;

            Defender defender = obj.GetComponent<Defender>();
            if (defender != null)
            {
                defender.setEvolution(_eveloution);
            }
        }
    }

    public void SetDefenderCount(int count)
    {
        defenderCount = count;
        ActivateDefenders();
    }
}