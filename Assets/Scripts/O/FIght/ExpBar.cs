using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    [Header("경험치 바 설정")]
    [SerializeField] private Image expFillImage; 

    private float currentExp = 0f;
    private float maxExp = 100f;

    void Start()
    {
        // ExpFill 찾기 (자동 할당되지 않은 경우)
        if (expFillImage == null)
        {
            Transform expFill = transform.Find("ExpFill");
            if (expFill != null)
            {
                expFillImage = expFill.GetComponent<Image>();
            }
        }

        if (expFillImage != null)
        {
            expFillImage.type = Image.Type.Filled;
            expFillImage.fillMethod = Image.FillMethod.Horizontal;
            expFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;

        }


        UpdateExpBar();
    }

    // 경험치 추가
    public void AddExp(float amount)
    {
        currentExp += amount;

        if (currentExp >= maxExp)
        {
            LevelUp();
        }

        UpdateExpBar();
    }

    // 경험치 설정
    public void SetExp(float current, float max)
    {
        currentExp = current;
        maxExp = max;
        UpdateExpBar();
    }

    // 경험치 바 업데이트
    private void UpdateExpBar()
    {
        if (expFillImage != null)
        {
            float fillRatio = Mathf.Clamp01(currentExp / maxExp);
            expFillImage.fillAmount = fillRatio;
        }
    }

    // 레벨업 처리
    private void LevelUp()
    {
        currentExp -= maxExp;
        maxExp *= 1.2f; // 다음 레벨 필요 경험치 증가

       
    }

    // 애니메이션과 함께 경험치 증가
    public void AddExpSmooth(float amount, float duration = 0.5f)
    {
        StartCoroutine(AddExpCoroutine(amount, duration));
    }

    private System.Collections.IEnumerator AddExpCoroutine(float amount, float duration)
    {
        float startExp = currentExp;
        float targetExp = currentExp + amount;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentExp = Mathf.Lerp(startExp, targetExp, elapsed / duration);
            UpdateExpBar();
            yield return null;
        }

        currentExp = targetExp;

        if (currentExp >= maxExp)
        {
            LevelUp();
        }

        UpdateExpBar();
    }
}