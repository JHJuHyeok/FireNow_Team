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
    [SerializeField] private float magnetMoveSpeed = 15f;

    private Transform targetPlayer;
    private PlayerExperience playerExp;
    private bool isMovingToPlayer = false;
    private bool collected = false;
    private bool isMagnetActive = false;

    private void Start()
    {

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        Debug.Log($"Player 태그를 가진 오브젝트 수: {players.Length}");

        foreach (GameObject p in players)
        {
            Debug.Log($"찾은 플레이어: {p.name}");

            PlayerExperience exp = p.GetComponent<PlayerExperience>();
            if (exp != null)
            {
                targetPlayer = p.transform;
                playerExp = exp;
                Debug.Log("PlayerExperience 컴포넌트 찾음!");
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

        if (distance <= attractRange)
        {
            isMovingToPlayer = true;
        }

        if (isMovingToPlayer || isMagnetActive)
        {
            float currentSpeed = isMagnetActive ? magnetMoveSpeed : moveSpeed;
            transform.position = Vector2.MoveTowards(transform.position, targetPlayer.position, currentSpeed * Time.deltaTime);
        }

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
            // 경험치 배율 적용
            PlayerController player = targetPlayer.GetComponent<PlayerController>();
            float expMultiplier = 1f;

            if (player != null && player.GetBattleStat() != null)
            {
                expMultiplier = player.GetBattleStat().finalGetExp;
            }

            // 최종 경험치 = 기본 경험치 × 배율
            int finalExp = Mathf.RoundToInt(expAmount * expMultiplier);

            playerExp.AddExperience(finalExp);
            Destroy(gameObject);
        }
    }

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
                expAmount = 10;  // expValue → expAmount로 변경
                transform.localScale = Vector3.one * 0.2f;
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = Resources.Load<Sprite>("Sprites/Origin_Resources/Texture2D/EXP");
                }
                break;

            case "mid":
                expAmount = 300;  // expValue → expAmount로 변경
                transform.localScale = Vector3.one * 0.2f;
                SpriteRenderer sr2 = GetComponent<SpriteRenderer>();
                if (sr2 != null)
                {
                    sr2.sprite = Resources.Load<Sprite>("Sprites/Origin_Resources/Texture2D/EXP_mid");
                }
                break;

            case "big":
                expAmount = 1000;  // expValue → expAmount로 변경
                transform.localScale = Vector3.one * 0.2f;
                SpriteRenderer sr3 = GetComponent<SpriteRenderer>();
                if (sr3 != null)
                {
                    sr3.sprite = Resources.Load<Sprite>("Sprites/Origin_Resources/Texture2D/EXP_big");
                }
                break;
        }
    }
}