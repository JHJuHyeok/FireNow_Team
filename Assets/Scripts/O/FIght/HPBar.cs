using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [Header("자동 할당")]
    private Slider hpSlider;
    private Image fillImage;

    [Header("체력 설정")]
    public float maxHP = 100f;
    private float currentHP;

    [Header("UI 참조")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text killText;
    [SerializeField] private TMP_Text failTimeText;
    [SerializeField] private TMP_Text failKillText;
    [SerializeField] private GameObject failUI;

    void Start()
    {
        // Slider 자동 찾기
        hpSlider = GetComponent<Slider>();

        // Fill Image 자동 찾기
        Transform fillArea = transform.Find("Fill Area");
        if (fillArea != null)
        {
            Transform fill = fillArea.Find("Fill");
            if (fill != null)
            {
                fillImage = fill.GetComponent<Image>();
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

        UpdateHPBar();
    }

    // 체력 초기화 (PlayerController에서 호출)
    public void SetHP(float current, float max)
    {
        currentHP = current;
        maxHP = max;

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }

        UpdateHPBar();
    }

    // 데미지 받기
    public void TakeDamage(float damage)
    {

        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);


        UpdateHPBar();
    }


    // 체력 회복
    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
        UpdateHPBar();
    }

    // 체력 바 업데이트
    private void UpdateHPBar()
    {
        if (hpSlider != null)
        {
            hpSlider.value = currentHP;
        }

     
    }

    // 사망
    public void Die()
    {
        ActivateUI();
  
    }

    private void ActivateUI()
    {
        if (failUI != null)
        {
            failUI.SetActive(true);

            // 시간과 킬 수 복사
            if (timeText != null && failTimeText != null)
            {
                failTimeText.text = timeText.text;
            }

            if (killText != null && failKillText != null)
            {
                failKillText.text = killText.text;
            }

            Time.timeScale = 0f;
        }
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

    // 테스트용
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(200);
          
        }
    }
}