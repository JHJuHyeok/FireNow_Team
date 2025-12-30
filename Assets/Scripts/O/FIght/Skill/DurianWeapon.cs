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

    // 현재 활성화된 단 하나의 두리안
    private GameObject currentDurian;

    private void Start()
    {
        player = transform.parent;
        if (player == null) player = transform;

        // 처음 한 번 발사
        FireDurian();
    }

    public void UpdateStats(AbilityLevelData levelData)
    {
        damageRate = levelData.damageRate;
        projectileCount = levelData.projectileCount;
        cooldown = levelData.cooldown;
        speed = levelData.speed;

        Debug.Log($"두리안 스탯 업데이트 - 데미지: {damageRate}, 개수: {projectileCount}, 속도: {speed}");

        // 기존 두리안 제거
        if (currentDurian != null)
        {
            Destroy(currentDurian);
            currentDurian = null;
        }

        // 새로 발사
        FireDurian();
    }

    private void FireDurian()
    {
        if (durianPrefab == null)
        {
            Debug.LogError("두리안 프리팹이 할당되지 않았습니다!");
            return;
        }

        Enemy nearestEnemy = FindNearestEnemy();

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

        currentDurian = Instantiate(durianPrefab, player.position, Quaternion.identity);

        DurianProjectile projectile = currentDurian.GetComponent<DurianProjectile>();
        if (projectile == null)
        {
            projectile = currentDurian.AddComponent<DurianProjectile>();
        }

        projectile.Initialize(damageRate, speed, direction);
    }

    private Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Enemy nearest = null;
        float minDistance = float.MaxValue;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;

            nearest = enemy;
        }

        return nearest;
    }

    private void OnDestroy()
    {
        if (currentDurian != null)
        {
            Destroy(currentDurian);
        }
    }
}