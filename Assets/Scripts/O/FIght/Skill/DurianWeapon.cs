using System.Collections;
using UnityEngine;

public class DurianWeapon : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject durianPrefab;

    [Header("Stats")]
    private float damageRate = 1f;
    private int projectileCount = 1;
    private float cooldown = 1f;
    private float speed = 1f;

    [Header("Spawn Settings")]
    private Transform player;
    private float sequentialDelay = 0.15f;

    private bool isActive = true;

    private void Start()
    {
        player = transform.parent;
        if (player == null) player = transform;

        StartCoroutine(FireRoutine());
    }

    public void UpdateStats(AbilityLevelData levelData)
    {
        damageRate = levelData.damageRate;
        projectileCount = levelData.projectileCount;
        cooldown = levelData.cooldown;
        speed = levelData.speed;

        Debug.Log($"두리안 스탯 업데이트 - 데미지: {damageRate}, 개수: {projectileCount}, 속도: {speed}");
    }

    private IEnumerator FireRoutine()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(cooldown);
            yield return StartCoroutine(FireDuriansSequentially());
        }
    }

    private IEnumerator FireDuriansSequentially()
    {
        if (durianPrefab == null)
        {
            Debug.LogError("두리안 프리팹이 할당되지 않았습니다!");
            yield break;
        }

        Enemy nearestEnemy = FindNearestEnemy();

        for (int i = 0; i < projectileCount; i++)
        {
            float angleOffset = 0f;
            if (projectileCount > 1)
            {
                float totalSpread = 360f / projectileCount;
                angleOffset = i * totalSpread;
            }

            Vector2 direction;
            if (nearestEnemy != null)
            {
                direction = (nearestEnemy.transform.position - player.position).normalized;
            }
            else
            {
                float randomAngle = Random.Range(0f, 360f);
                direction = new Vector2(
                    Mathf.Cos(randomAngle * Mathf.Deg2Rad),
                    Mathf.Sin(randomAngle * Mathf.Deg2Rad)
                );
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle += angleOffset;
            float rad = angle * Mathf.Deg2Rad;
            direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject durian = Instantiate(durianPrefab, player.position, Quaternion.identity);
            DurianProjectile projectile = durian.GetComponent<DurianProjectile>();

            if (projectile == null)
            {
                projectile = durian.AddComponent<DurianProjectile>();
            }

            projectile.Initialize(damageRate, speed, direction);

            // 다음 발사까지 대기
            if (i < projectileCount - 1)
            {
                yield return new WaitForSeconds(sequentialDelay);
            }
        }
    }

    private Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Enemy nearest = null;
        float minDistance = float.MaxValue;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector2.Distance(player.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private void OnDestroy()
    {
        isActive = false;
        StopAllCoroutines();
    }
}