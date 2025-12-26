using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldDamage : MonoBehaviour
{
    private float damage;
    private float damageInterval;
    private float damageRange;

    // Collider 대신 Enemy 자체를 추적
    private Dictionary<Enemy, Coroutine> damageCoroutines = new Dictionary<Enemy, Coroutine>();

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
        forceFieldCollider.radius = 5f; // 방어막 크기에 맞게 조정
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

    private void FixedUpdate()
    {
        if (!isInitialized) return;

        // 방어막 범위 내의 모든 Collider 찾기
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, forceFieldCollider.radius);

        HashSet<Enemy> currentEnemies = new HashSet<Enemy>();

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    currentEnemies.Add(enemy);

                    // 새로 들어온 적
                    if (!damageCoroutines.ContainsKey(enemy))
                    {

                        // 즉시 첫 데미지
                        DealDamage(enemy, true);

                        // 지속 데미지 코루틴 시작
                        Coroutine damageCoroutine = StartCoroutine(DealDamageOverTime(enemy));
                        damageCoroutines.Add(enemy, damageCoroutine);
                    }
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
    }

    private IEnumerator DealDamageOverTime(Enemy enemy)
    {
        int tickCount = 0;

        while (true)
        {
            yield return new WaitForSeconds(damageInterval);

            // 적이 파괴되었으면 중지
            if (enemy == null)
            {
      
                damageCoroutines.Remove(enemy);
                yield break;
            }

            tickCount++;
            DealDamage(enemy, false, tickCount);
        }
    }

    private void DealDamage(Enemy enemy, bool isFirstHit = false, int tickCount = 0)
    {
        if (enemy == null) return;

        enemy.TakeDamage(damage);

    }

    // Scene 뷰에서 방어막 범위 시각화
    private void OnDrawGizmos()
    {
        if (forceFieldCollider != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f); // 반투명 녹색
            Gizmos.DrawWireSphere(transform.position, forceFieldCollider.radius);

            // 추적 중인 적 표시
            Gizmos.color = Color.red;
            foreach (var enemy in damageCoroutines.Keys)
            {
                if (enemy != null)
                {
                    Gizmos.DrawLine(transform.position, enemy.transform.position);
                }
            }
        }
    }

    // 디버그용: 현재 추적 중인 적 수 확인
    private void OnGUI()
    {
        if (damageCoroutines.Count > 0)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 200, 20),
                $"추적: {damageCoroutines.Count}마리");
        }
    }
}