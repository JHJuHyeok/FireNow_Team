using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    [Header("자동 할당")]
    private Slider hpSlider;
    private Image fillImage;
    private Image backgroundImage;

    [Header("체력 설정")]
    public float maxHP = 1000f;
    private float currentHP;

    [Header("UI 참조 - BossHPBar 하위 오브젝트")]
    [SerializeField] private GameObject slider; // Slider 오브젝트
    [SerializeField] private TMP_Text bossNameText; // Text (TMP) - 보스 이름

    [Header("HP바 색상")]
    [SerializeField] private Color normalColor = Color.red;
    [SerializeField] private Color lowHPColor = new Color(0.5f, 0f, 0f);
    [SerializeField] private float lowHPThreshold = 0.3f;

    void Start()
    {
        // Slider 컴포넌트 찾기
        if (slider != null)
        {
            hpSlider = slider.GetComponent<Slider>();
        }

        // Fill Image 찾기 (Slider > Fill Area > Fill)
        if (hpSlider != null)
        {
            Transform fillArea = hpSlider.transform.Find("Fill Area");
            if (fillArea != null)
            {
                Transform fill = fillArea.Find("Fill");
                if (fill != null)
                {
                    fillImage = fill.GetComponent<Image>();
                }
            }

            // Background Image 찾기
            Transform background = hpSlider.transform.Find("Background");
            if (background != null)
            {
                backgroundImage = background.GetComponent<Image>();
            }
        }

        // 초기 설정
        if (hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = maxHP;
            currentHP = maxHP;
            hpSlider.value = currentHP;
        }

        // 초기에는 BossHPBar 전체 비활성화
        //gameObject.SetActive(false);
    }

    // 보스 HP 초기화 및 HP바 활성화
    public void InitializeBoss(float maxHealth, string bossName = "BOSS")
    {
        maxHP = maxHealth;
        currentHP = maxHP;

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }

        // 보스 이름 설정
        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }

        // BossHPBar 전체 활성화
        gameObject.SetActive(true);

        UpdateHPBar();
    }

    // 데미지 받기
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);
        UpdateHPBar();

        // HP가 0이 되면 사망 처리
        if (currentHP <= 0)
        {
            Die();
        }
    }

    // 체력 회복
    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
        UpdateHPBar();
    }

    // HP바 업데이트
    private void UpdateHPBar()
    {
        if (hpSlider != null)
        {
            hpSlider.value = currentHP;
        }

        // HP 비율에 따라 Fill 색상 변경
        if (fillImage != null)
        {
            float hpRatio = currentHP / maxHP;
            if (hpRatio <= lowHPThreshold)
            {
                fillImage.color = lowHPColor;
            }
            else
            {
                fillImage.color = normalColor;
            }
        }
    }

    // 사망
    public void Die()
    {

        // 2초 후 HP바 비활성화
        Invoke("HideBossHPBar", 2f);
    }

    // HP바 비활성화
    private void HideBossHPBar()
    {
        gameObject.SetActive(false);
    }

    // 현재 체력 반환
    public float GetCurrentHP()
    {
        return currentHP;
    }

    // 최대 체력 반환
    public float GetMaxHP()
    {
        return maxHP;
    }

    // HP 비율 반환 (0~1)
    public float GetHPRatio()
    {
        return currentHP / maxHP;
    }

    // 보스 생존 여부
    public bool IsAlive()
    {
        return currentHP > 0;
    }

    // 테스트용
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            // 보스 HP바 테스트 활성화
            InitializeBoss(1000f, "BOSS");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(100);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(200);
        }
    }
}