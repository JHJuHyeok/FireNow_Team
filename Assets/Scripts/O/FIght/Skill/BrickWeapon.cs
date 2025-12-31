using System.Collections;
using UnityEngine;

public class BrickWeapon : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject brickPrefab;

    [Header("Stats")]
    private float damageRate = 1f;
    private int projectileCount = 1;
    private float cooldown = 4f;
    private float speed = 8f;

    [Header("Spawn Settings")]
    private Transform player;
    private PlayerController playerController; // 추가
    private float sequentialDelay = 0.15f;
    private bool isActive = true;
    private bool isEvolved = false;  // 진화여부
    public float LifeTime = 4f;      // 추가
    public float coolTime = 4f;      // 추가
    // 메서드 추가
    public void SetEvolution(bool evolved)
    {
        isEvolved = evolved;

        if (isEvolved)
        {

        }
    }
    private void Start()
    {
        player = transform.parent;
        if (player == null) player = transform;

        // PlayerController 참조 가져오기
        playerController = player.GetComponent<PlayerController>();

        StartCoroutine(FireRoutine());
    }

    public void UpdateStats(AbilityLevelData levelData)
    {
        damageRate = levelData.damageRate;
        projectileCount = levelData.projectileCount;
        cooldown = levelData.cooldown;
        speed = levelData.speed > 0 ? levelData.speed * 3f : 8f;
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
          
            yield break;
        }

        Enemy nearestEnemy = FindNearestEnemy();

        // PlayerController에서 공격력 가져오기
        float baseDamage = playerController != null ? playerController.GetAttackPower() : 10f;
        float finalDamage = baseDamage * damageRate; // baseDamage × damageRate

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

            projectile.SetEvolution(isEvolved);

            // 최종 데미지 전달 (damageRate가 아닌 finalDamage)
            projectile.Initialize(finalDamage, speed, direction);

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