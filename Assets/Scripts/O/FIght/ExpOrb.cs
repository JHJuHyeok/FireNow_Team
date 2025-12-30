using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [Header("Exp Settings")]
    [SerializeField] private int expAmount = 10;
    [SerializeField] private int expValue = 10;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float attractRange = 3f;
    [SerializeField] private float collectRange = 1.0f;

    private Transform targetPlayer; // 추적할 실제 player
    private PlayerExperience playerExp;
    private bool isMovingToPlayer = false;
    private bool collected = false;

    private void Start()
    {
        // "Player" 태그 모두 찾기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            // PlayerExperience가 있는 객체 찾기
            PlayerExperience exp = p.GetComponent<PlayerExperience>();
            if (exp != null)
            {
                targetPlayer = p.transform;
                playerExp = exp;
                break;
            }
        }

        // Collider 설정
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
        }
        col.isTrigger = true;
        col.radius = 0.5f;

        // Rigidbody2D 설정
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    private void Update()
    {
        if (targetPlayer == null || collected) return;

        float distance = Vector2.Distance(transform.position, targetPlayer.position);

        if (distance <= attractRange)
        {
            isMovingToPlayer = true;
        }

        if (isMovingToPlayer)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);
        }

        // 거리로 강제 수집
        if (distance <= collectRange)
        {
            CollectExp();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollectExp();
        }
    }

    private void CollectExp()
    {
        if (collected) return;
        collected = true;

        if (playerExp != null)
        {
            playerExp.AddExperience(expAmount);
            Destroy(gameObject);
        }
    }

    public void SetExpAmount(int amount)
    {
        expAmount = amount;
    }

    public void SetExpType(string expType)
    {
        switch (expType)
        {
            case "small":
                expValue = 10;
                transform.localScale = Vector3.one * 0.2f;
                // 스프라이트 변경
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = Resources.Load<Sprite>("Sprites/Origin_Resources/Texture2D/EXP");
                    //sr.color = Color.green; // 초록색
                }
                break;

            case "mid":
                expValue = 30;
                transform.localScale = Vector3.one * 0.3f;
                SpriteRenderer sr2 = GetComponent<SpriteRenderer>();
                if (sr2 != null)
                {
                    sr2.sprite = Resources.Load<Sprite>("Sprites/Origin_Resources/Texture2D/EXP_mid");
                    //sr2.color = Color.blue; // 파란색
                }
                break;

            case "big":
                expValue = 50;
                transform.localScale = Vector3.one * 0.4f;
                SpriteRenderer sr3 = GetComponent<SpriteRenderer>();
                if (sr3 != null)
                {
                    sr3.sprite = Resources.Load<Sprite>("Sprites/Origin_Resources/Texture2D/EXP_big");
                    //sr3.color = Color.yellow; // 주황?
                }
                break;
        }
    }
}