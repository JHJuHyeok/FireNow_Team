using System.Collections;
using UnityEngine;

public class BrickWeapon : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject brickPrefab;

    [Header("Stats")]
    private float damageRate = 1f;
    private int projectileCount = 1;
    private float cooldown = 1f;
    private float speed = 8f;

    [Header("Spawn Settings")]
    private Transform player;
    private float sequentialDelay = 0.15f; // 연속 발사 딜레이

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
        speed = levelData.speed > 0 ? levelData.speed * 3f : 8f;

        Debug.Log($"벽돌 스탯 업데이트 - 데미지: {damageRate}, 개수: {projectileCount}, 쿨다운: {cooldown}");
    }

    private IEnumerator FireRoutine()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(cooldown);
            yield return StartCoroutine(FireBricksSequentially());
        }
    }

    private IEnumerator FireBricksSequentially()
    {
        if (brickPrefab == null)
        {
            Debug.LogError("벽돌 프리팹이 할당되지 않았습니다!");
            yield break;
        }

        Enemy nearestEnemy = FindNearestEnemy();

        for (int i = 0; i < projectileCount; i++)
        {
            float angleOffset = 0f;
            if (projectileCount > 1)
            {
                float totalSpread = 60f;
                angleOffset = (i - (projectileCount - 1) / 2f) * (totalSpread / Mathf.Max(1, projectileCount - 1));
            }

            Vector2 direction;
            if (nearestEnemy != null)
            {
                direction = (nearestEnemy.transform.position - player.position).normalized;
            }
            else
            {
                direction = Vector2.right;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle += angleOffset;
            float rad = angle * Mathf.Deg2Rad;
            direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject brick = Instantiate(brickPrefab, player.position, Quaternion.identity);
            BrickProjectile projectile = brick.GetComponent<BrickProjectile>();

            if (projectile == null)
            {
                projectile = brick.AddComponent<BrickProjectile>();
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