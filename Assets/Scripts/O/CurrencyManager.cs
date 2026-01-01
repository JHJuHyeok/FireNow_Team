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
          
            return false;
        }

        gold -= requiredGold;
        goldDNA -= requiredGoldDNA;

     

        return true;
    }

    public void AddCurrency(CurrencyType type, int amount)
    {
        if (type == CurrencyType.Gold)
        {
            gold += amount;
           
        }
        else
        {
            goldDNA += amount;
  
        }
    }
}