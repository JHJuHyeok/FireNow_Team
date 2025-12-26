using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderDamage : MonoBehaviour
{
    private float damage;
    private float damageInterval;
    private float damageRange;

    private Dictionary<Enemy, Coroutine> damageCoroutines = new Dictionary<Enemy, Coroutine>();
    private CircleCollider2D defenderCollider;
    private bool isInitialized = false;

    private void Awake()
    {
        defenderCollider = GetComponent<CircleCollider2D>();
        if (defenderCollider == null)
        {
            defenderCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        defenderCollider.isTrigger = true;
        defenderCollider.radius = 0.5f; // 기본 크기
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

        foreach (var hit in hits)
        {
            // Enemy 처리
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
            }

            // Box 처리
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

        // 범위를 벗어난 적들 제거
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

            DealDamage(enemy, false);
        }
    }

    private void DealDamage(Enemy enemy, bool isFirstHit = false)
    {
        if (enemy == null) return;
        enemy.TakeDamage(damage);
    }

    private void OnDrawGizmos()
    {
        if (defenderCollider != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f); // 반투명 빨간색
            Gizmos.DrawWireSphere(transform.position, defenderCollider.radius);
        }
    }
}