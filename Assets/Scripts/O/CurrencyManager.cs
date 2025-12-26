using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("보유 재화")]
    public int gold = 10000;        
    public int goldDNA = 100;      

    void Awake()
    {
        Instance = this;
    }

    public bool HasEnoughCurrency(int requiredGold, int requiredGoldDNA)
    {
        return gold >= requiredGold && goldDNA >= requiredGoldDNA;
    }

    public bool SpendCurrency(int requiredGold, int requiredGoldDNA)
    {
        if (!HasEnoughCurrency(requiredGold, requiredGoldDNA))
        {
            Debug.Log("재화가 부족합니다!");
            return false;
        }

        gold -= requiredGold;
        goldDNA -= requiredGoldDNA;

        Debug.Log($"소모: 골드 {requiredGold}, DNA {requiredGoldDNA}");
        Debug.Log($"남은 재화: 골드 {gold}, DNA {goldDNA}");

        return true;
    }

    public void AddCurrency(CurrencyType type, int amount)
    {
        if (type == CurrencyType.Gold)
        {
            gold += amount;
            Debug.Log($"골드 +{amount} (현재: {gold})");
        }
        else
        {
            goldDNA += amount;
            Debug.Log($"골드 DNA +{amount} (현재: {goldDNA})");
        }
    }
}