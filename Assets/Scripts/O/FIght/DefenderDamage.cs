using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderDamage : MonoBehaviour
{
    private float damage;
    private float damageInterval;
    private float damageRange;

    private Dictionary<Enemy, Coroutine> damageCoroutines = new Dictionary<Enemy, Coroutine>();
    private Dictionary<BossEnemy, Coroutine> bossCoroutines = new Dictionary<BossEnemy, Coroutine>();

    private CircleCollider2D defenderCollider;
    private bool isInitialized = false;

    [Header("Sound")]
    private string hitSoundName = "Gurdian"; // 추가

    private void Awake()
    {
        defenderCollider = GetComponent<CircleCollider2D>();
        if (defenderCollider == null)
        {
            defenderCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        defenderCollider.isTrigger = true;
        defenderCollider.radius = 0.5f;
    }

    public void Initialize(float dmg, float interval, float range)
    {
        damage = dmg;
        damageInterval = interval;
        damageRange = range;

        if (defenderCollider != null)
        {
            defenderCollider.radius = damageRange;
        }

        isInitialized = true;
    }

    private HashSet<BreakableBox> destroyedBoxes = new HashSet<BreakableBox>();

    private void FixedUpdate()
    {
        if (!isInitialized) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, defenderCollider.radius);
        HashSet<Enemy> currentEnemies = new HashSet<Enemy>();
        HashSet<BossEnemy> currentBosses = new HashSet<BossEnemy>();

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
                        DealDamage(enemy);
                        Coroutine damageCoroutine = StartCoroutine(DealDamageOverTime(enemy));
                        damageCoroutines.Add(enemy, damageCoroutine);
                    }
                }

                BossEnemy boss = hit.GetComponent<BossEnemy>();
                if (boss != null)
                {
                    currentBosses.Add(boss);
                    if (!bossCoroutines.ContainsKey(boss))
                    {
                        DealBossDamage(boss);
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
        while (true)
        {
            yield return new WaitForSeconds(damageInterval);

            if (enemy == null)
            {
                damageCoroutines.Remove(enemy);
                yield break;
            }

            DealDamage(enemy);
        }
    }

    private IEnumerator DealBossDamageOverTime(BossEnemy boss)
    {
        while (true)
        {
            yield return new WaitForSeconds(damageInterval);

            if (boss == null)
            {
                bossCoroutines.Remove(boss);
                yield break;
            }

            DealBossDamage(boss);
        }
    }

    private void DealDamage(Enemy enemy)
    {
        if (enemy == null) return;
        enemy.TakeDamage(damage);
        PlayHitSound(); // 사운드 재생 추가
    }

    private void DealBossDamage(BossEnemy boss)
    {
        if (boss == null) return;
        boss.TakeDamage(damage);
        PlayHitSound(); // 사운드 재생 추가
    }

    // 히트 사운드 재생 메서드 추가
    private void PlayHitSound()
    {
        if (string.IsNullOrEmpty(hitSoundName)) return;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(hitSoundName, 0f, false, SoundType.effect);
        }
    }

    private void OnDrawGizmos()
    {
        if (defenderCollider != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawWireSphere(transform.position, defenderCollider.radius);
        }
    }
}