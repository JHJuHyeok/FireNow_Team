using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("현재 스탯")]
    public int attack = 0;      // 공격력
    public int hp = 100;        // 체력
    public int defense = 0;     // 방어구
    public int meatRegen = 0;   // 고기회복

    void Awake()
    {
        Instance = this;
    }

    public void ApplySkillUpgrade(int skillTypeIndex, int amount)
    {
        switch (skillTypeIndex)
        {
            case 0: // 힘
                attack += amount;
                Debug.Log($"공격력 +{amount} → 현재: {attack}");
                break;
            case 1: // 체력
                hp += amount;
                Debug.Log($"HP +{amount} → 현재: {hp}");
                break;
            case 2: // 끈기
                defense += amount;
                Debug.Log($"방어구 +{amount} → 현재: {defense}");
                break;
            case 3: // 회복
                meatRegen += amount;
                Debug.Log($"고기회복 +{amount} → 현재: {meatRegen}");
                break;
        }
    }
}