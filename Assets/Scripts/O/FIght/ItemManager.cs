using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("아이템 효과 설정")]
    [SerializeField] private int moneyAmount = 250;
    [SerializeField] private float magnetDuration = 10f;
    [SerializeField] private float magnetRange = 15f;
    [SerializeField] private float bombRadius = 10f;
    [SerializeField] private int meatHealAmount = 30;

    [Header("폭탄 이펙트")]
    [SerializeField] private GameObject bombEffectPrefab;
    [SerializeField] private GameObject enemyExplosionPrefab;

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
        if (isMagnetActive)
        {
            magnetTimer -= Time.deltaTime;
            if (magnetTimer <= 0)
            {
                isMagnetActive = false;
            }
            else
            {
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

    private void ActivateMagnet()
    {
        ExpOrb[] allOrbs = FindObjectsOfType<ExpOrb>();
       
        foreach (ExpOrb orb in allOrbs)
        {
            orb.ActivateMagnet();
        }
    }

    private void ActivateBomb()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
     
            return;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
     

        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null)
            {
           
                enemy.Die();
            }
        }
    }

    private void HealPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // PlayerController의 Heal은 이제 BattleStat의 finalGetHP를 사용함
            playerController.Heal(meatHealAmount); // 기본값만 전달, 실제 회복량은 내부에서 계산
        }
    }

    private void CollectMoney()
    {
         BattleManager.Instance.AddMoney(moneyAmount);
    }

   

    private void PullExperienceOrbs()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        ExpOrb[] allOrbs = FindObjectsOfType<ExpOrb>();

        foreach (ExpOrb orb in allOrbs)
        {
            float distance = Vector2.Distance(orb.transform.position, player.transform.position);

            if (distance <= magnetRange)
            {
                orb.ActivateMagnet();
            }
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

    private void OnDrawGizmosSelected()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.transform.position, bombRadius);
        }
    }
}