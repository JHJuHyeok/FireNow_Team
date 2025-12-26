using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShurikenShooter : MonoBehaviour
{
    [Header("Shuriken Settings")]
    [SerializeField] private GameObject shurikenPrefab;
    [SerializeField] private ShurikenData shurikenData;

    [Header("Shoot Settings")]
    [SerializeField] private int shootCount = 1; // 한 번에 발사할 수리검 개수
    [SerializeField] private float shootInterval = 1f; // 발사 간격
    [SerializeField] private float spreadAngle = 30f; // 여러 개 발사 시 퍼지는 각도

    [Header("Target Settings")]
    [SerializeField] private float detectionRange = 10f; // 적 탐지 범위
    [SerializeField] private LayerMask enemyLayer;

    private bool isEvolution = false;
    private Coroutine shootCoroutine;

    private void OnEnable()
    {
        shootCoroutine = StartCoroutine(ShootRoutine());
    }

    private void OnDisable()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            ShootShuriken();
            yield return new WaitForSeconds(shootInterval);
        }
    }
 

    // PlayerAbility에서 호출할 메서드
    public void SetShurikenData(ShurikenData data)
    {
        shurikenData = data;
    }

    public void UpgradeShootCount(int count)
    {
        shootCount = count;
    }

    public void UpgradeShootSpeed(float interval)
    {
        shootInterval = interval;
    }
    private void ShootShuriken()
    {
        // 가장 가까운 적 찾기
        Transform target = FindNearestEnemy();

        if (target == null)
        {
            // 적이 없으면 랜덤 방향으로 발사
            ShootInRandomDirection();
            return;
        }

        Vector2 baseDirection = (target.position - transform.position).normalized;

        // 여러 개 발사
        for (int i = 0; i < shootCount; i++)
        {
            Vector2 direction = baseDirection;

            // 여러 개 발사 시 각도 분산
            if (shootCount > 1)
            {
                float angle = spreadAngle * (i - (shootCount - 1) / 2f) / (shootCount - 1);
                direction = Quaternion.Euler(0, 0, angle) * baseDirection;
            }

            SpawnShuriken(direction);
        }
    }

    private void ShootInRandomDirection()
    {
        for (int i = 0; i < shootCount; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            Vector2 direction = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad),
                                           Mathf.Sin(randomAngle * Mathf.Deg2Rad));
            SpawnShuriken(direction);
        }
    }

    private void SpawnShuriken(Vector2 direction)
    {
        GameObject shurikenObj = Instantiate(shurikenPrefab, transform.position, Quaternion.identity);
        Shuriken shuriken = shurikenObj.GetComponent<Shuriken>();

        if (shuriken != null)
        {
            shuriken.Initialize(shurikenData, direction);
        }
    }

    private Transform FindNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);

        if (enemies.Length == 0)
            return null;

        Transform nearest = null;
        float minDistance = float.MaxValue;

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }

    public void SetEvolution(bool value)
    {
        isEvolution = value;
        // 진화 시 데미지나 다른 스탯 증가
        if (isEvolution)
        {
            shurikenData.damage *= 1.5f;
            shurikenData.pierceCount += 1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 탐지 범위 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}