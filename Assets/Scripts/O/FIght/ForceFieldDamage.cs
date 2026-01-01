using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldDamage : MonoBehaviour
{
    private float damage;
    private float damageInterval;
    private float damageRange;

    private Dictionary<Enemy, Coroutine> damageCoroutines = new Dictionary<Enemy, Coroutine>();
    private Dictionary<BossEnemy, Coroutine> bossCoroutines = new Dictionary<BossEnemy, Coroutine>(); // 추가

    private CircleCollider2D forceFieldCollider;
    private bool isInitialized = false;

    private void Awake()
    {
        forceFieldCollider = GetComponent<CircleCollider2D>();
        if (forceFieldCollider == null)
        {
            forceFieldCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        forceFieldCollider.isTrigger = true;
        forceFieldCollider.radius = 5f;
    }

    public void Initialize(float dmg, float interval, float range)
    {
        damage = dmg;
        damageInterval = interval;
        damageRange = range;

        if (forceFieldCollider != null)
        {
            forceFieldCollider.radius = damageRange;
        }

        isInitialized = true;
    }

    private HashSet<BreakableBox> destroyedBoxes = new HashSet<BreakableBox>();

    private void FixedUpdate()
    {
        if (!isInitialized) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, forceFieldCollider.radius);
        HashSet<Enemy> currentEnemies = new HashSet<Enemy>();
        HashSet<BossEnemy> currentBosses = new HashSet<BossEnemy>(); // 추가

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    currentEnemies.Add(enemy);
                    if (!damageCoroutines.ContainsKey(enemy))
                    {
                        DealDamage(enemy, true);
                        Coroutine damageCoroutine = StartCoroutine(DealDamageOverTime(enemy));
                        damageCoroutines.Add(enemy, damageCoroutine);
                    }
                }

                // 보스 처리 추가
                BossEnemy boss = hit.GetComponent<BossEnemy>();
                if (boss != null)
                {
                    currentBosses.Add(boss);
                    if (!bossCoroutines.ContainsKey(boss))
                    {
                        DealBossDamage(boss, true);
                        Coroutine bossCoroutine = StartCoroutine(DealBossDamageOverTime(boss));
                        bossCoroutines.Add(boss, bossCoroutine);
                    }
                }
            }

            if (hit.CompareTag("Box"))
            {
                BreakableBox box = hit.GetComponent<BreakableBox>();
                if (box != null && !destroyedBoxes.Contains(box))
                {
                    destroyedBoxes.Add(box);
                    box.Break();
                }
            }
        }

        // Enemy 제거
        List<Enemy> enemiesToRemove = new List<Enemy>();
        foreach (var enemy in damageCoroutines.Keys)
        {
            if (enemy == null || !currentEnemies.Contains(enemy))
            {
                enemiesToRemove.Add(enemy);
            }
        }

        foreach (var enemy in enemiesToRemove)
        {
            if (damageCoroutines.ContainsKey(enemy))
            {
                StopCoroutine(damageCoroutines[enemy]);
                damageCoroutines.Remove(enemy);
            }
        }

        // Boss 제거
        List<BossEnemy> bossesToRemove = new List<BossEnemy>();
        foreach (var boss in bossCoroutines.Keys)
        {
            if (boss == null || !currentBosses.Contains(boss))
            {
                bossesToRemove.Add(boss);
            }
        }

        foreach (var boss in bossesToRemove)
        {
            if (bossCoroutines.ContainsKey(boss))
            {
                StopCoroutine(bossCoroutines[boss]);
                bossCoroutines.Remove(boss);
            }
        }

        destroyedBoxes.RemoveWhere(box => box == null);
    }

    private IEnumerator DealDamageOverTime(Enemy enemy)
    {
        int tickCount = 0;

        while (true)
        {
            yield return new WaitForSeconds(damageInterval);

            if (enemy == null)
            {
                damageCoroutines.Remove(enemy);
                yield break;
            }

            tickCount++;
            DealDamage(enemy, false, tickCount);
        }
    }

    private IEnumerator DealBossDamageOverTime(BossEnemy boss)
    {
        int tickCount = 0;

        while (true)
        {
            yield return new WaitForSeconds(damageInterval);

            if (boss == null)
            {
                bossCoroutines.Remove(boss);
                yield break;
            }

            tickCount++;
            DealBossDamage(boss, false, tickCount);
        }
    }

    private void DealDamage(Enemy enemy, bool isFirstHit = false, int tickCount = 0)
    {
        if (enemy == null) return;
        enemy.TakeDamage(damage);
    }

    private void DealBossDamage(BossEnemy boss, bool isFirstHit = false, int tickCount = 0)
    {
        if (boss == null) return;
        boss.TakeDamage(damage);
    }

    private void OnDrawGizmos()
    {
        if (forceFieldCollider != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawWireSphere(transform.position, forceFieldCollider.radius);

            // Enemy 추적 표시
            Gizmos.color = Color.red;
            foreach (var enemy in damageCoroutines.Keys)
            {
                if (enemy != null)
                {
                    Gizmos.DrawLine(transform.position, enemy.transform.position);
                }
            }

            // Boss 추적 표시
            Gizmos.color = Color.yellow;
            foreach (var boss in bossCoroutines.Keys)
            {
                if (boss != null)
                {
                    Gizmos.DrawLine(transform.position, boss.transform.position);
                }
            }
        }
    }
}