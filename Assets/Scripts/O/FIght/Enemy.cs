using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyData data;
    private Transform player;
    private float currentHealth;

    public void Initialize(EnemyData enemyData)
    {
        data = enemyData;
        currentHealth = data.hp;

        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        // MoveType에 따라 이동
        if (data.moveType == MoveType.chase)
        {
            // 플레이어 추적
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * data.speed * Time.deltaTime;
        }
        else if (data.moveType == MoveType.oneDirection)
        {
            // 플레이어 방향으로 직진만
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * data.speed * Time.deltaTime;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 사망 처리
        Destroy(gameObject);
    }
}