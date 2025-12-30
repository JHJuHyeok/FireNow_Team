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
    [SerializeField] private float magnetMoveSpeed = 15f; // 자석으로 끌릴 때 더 빠른 속도

    private Transform targetPlayer;
    private PlayerExperience playerExp;
    private bool isMovingToPlayer = false;
    private bool collected = false;
    private bool isMagnetActive = false; // 자석으로 끌리는 중인지

    private void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            PlayerExperience exp = p.GetComponent<PlayerExperience>();
            if (exp != null)
            {
                targetPlayer = p.transform;
                playerExp = exp;
                break;
            }
        }

        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
        }
        col.isTrigger = true;
        col.radius = 0.5f;

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

        // 일반 흡수 범위 체크
        if (distance <= attractRange)
        {
            isMovingToPlayer = true;
        }

        // 플레이어로 이동
        if (isMovingToPlayer || isMagnetActive)
        {
            // 자석 활성화 시 더 빠른 속도 사용
            float currentSpeed = isMagnetActive ? magnetMoveSpeed : moveSpeed;
            transform.position = Vector2.MoveTowards(transform.position, targetPlayer.position, currentSpeed * Time.deltaTime);
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

    /// <summary>
    /// 자석 아이템이 호출하는 메서드 - 모든 경험치 구슬을 끌어당김
    /// </summary>
    public void ActivateMagnet()
    {
        isMagnetActive = true;
        isMovingToPlayer = true;
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
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = Resources.Load<Sprite>("Sprites/Origin_Resources/Texture2D/EXP");
                }
                break;

            case "mid":
                expValue = 30;
                transform.localScale = Vector3.one * 0.3f;
                SpriteRenderer sr2 = GetComponent<SpriteRenderer>();
                if (sr2 != null)
                {
                    sr2.sprite = Resources.Load<Sprite>("Sprites/Origin_Resources/Texture2D/EXP_mid");
                }
                break;

            case "big":
                expValue = 50;
                transform.localScale = Vector3.one * 0.4f;
                SpriteRenderer sr3 = GetComponent<SpriteRenderer>();
                if (sr3 != null)
                {
                    sr3.sprite = Resources.Load<Sprite>("Sprites/Origin_Resources/Texture2D/EXP_big");
                }
                break;
        }
    }
}