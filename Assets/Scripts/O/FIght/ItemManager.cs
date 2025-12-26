using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("아이템 효과 설정")]
    [SerializeField] private int moneyAmount = 100; // 돈 획득량
    [SerializeField] private float magnetDuration = 10f; // 자석 지속시간
    [SerializeField] private float magnetRange = 15f; // 자석 범위
    [SerializeField] private float bombRadius = 10f; // 폭탄 범위
    [SerializeField] private int meatHealAmount = 30; // 고기 회복량

    [Header("폭탄 이펙트")]
    [SerializeField] private GameObject bombEffectPrefab;

    private float magnetTimer = 0f;
    private bool isMagnetActive = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 자석 효과 타이머
        if (isMagnetActive)
        {
            magnetTimer -= Time.deltaTime;
            if (magnetTimer <= 0)
            {
                isMagnetActive = false;
                Debug.Log("자석 효과 종료");
            }
            else
            {
                // 자석 효과 활성화 중 - 경험치 오브들을 끌어당김
                PullExperienceOrbs();
            }
        }
    }

    public void OnItemCollected(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Money:
                CollectMoney();
                break;
            case ItemType.Magnet:
                ActivateMagnet();
                break;
            case ItemType.Bomb:
                ActivateBomb();
                break;
            case ItemType.Meat:
                HealPlayer();
                break;
        }
    }

    private void CollectMoney()
    {
        // 돈 획득
        Debug.Log($"돈 {moneyAmount} 획득!");

        // GameManager나 PlayerData에 돈 추가
        // 예: GameManager.Instance.AddMoney(moneyAmount);

        // UI 업데이트 등
    }

    private void ActivateMagnet()
    {
        Debug.Log($"자석 효과 활성화! ({magnetDuration}초)");
        isMagnetActive = true;
        magnetTimer = magnetDuration;

        // 즉시 모든 경험치 오브 끌어당기기 시작
        PullExperienceOrbs();
    }

    private void PullExperienceOrbs()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // 모든 경험치 오브 찾기
        ExpOrb[] allOrbs = FindObjectsOfType<ExpOrb>();

        foreach (ExpOrb orb in allOrbs)
        {
            float distance = Vector2.Distance(orb.transform.position, player.transform.position);

            // 자석 범위 내에 있으면 강제로 끌어당김
            if (distance <= magnetRange)
            {
                //orb.StartCollection(); // ExpOrb에 이 메서드 추가 필요
            }
        }
    }

    private void ActivateBomb()
    {
        Debug.Log($"폭탄 폭발! 범위: {bombRadius}");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 bombPosition = player.transform.position;

        // 폭탄 이펙트
        if (bombEffectPrefab != null)
        {
            Instantiate(bombEffectPrefab, bombPosition, Quaternion.identity);
        }

        // 범위 내 모든 적과 박스 찾아서 파괴
        Collider2D[] hits = Physics2D.OverlapCircleAll(bombPosition, bombRadius);

        foreach (Collider2D hit in hits)
        {
            // 적 파괴
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(9999); // 즉사
                }
            }

            // 박스 파괴 (HP 없으니 직접 Break 호출)
            if (hit.CompareTag("Box"))
            {
                BreakableBox box = hit.GetComponent<BreakableBox>();
                if (box != null)
                {
                    box.Break(); // public으로 만든 Break() 메서드 호출
                }
            }
        }
    }

    private void HealPlayer()
    {
        Debug.Log($"체력 {meatHealAmount} 회복!");

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            //player.Heal(meatHealAmount);
        }
    }

    public bool IsMagnetActive()
    {
        return isMagnetActive;
    }

    public float GetMagnetRange()
    {
        return magnetRange;
    }
}