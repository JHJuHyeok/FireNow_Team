using UnityEngine;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour
{
    [Header("UI References")]
    public Image nodeBackground;      // Talent_Slot_Normal_BG
    public Image icon;                // 스킬 아이콘
    public Button button;

    [Header("Sprites")]
    public Sprite bgNormal;           // Talent_Slot_Normal_BG
    public Sprite bgGray;             // Talent_Slot_Normal_BG_gray

    private int level;
    private int skillTypeIndex;
    private SkillTypeData skillType;
    private int cost;
    private bool isUnlocked;
    private bool isPurchased;

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    public void Initialize(int lvl, int typeIndex, SkillTypeData type, int skillCost, bool unlocked, bool purchased)
    {
        level = lvl;
        skillTypeIndex = typeIndex;
        skillType = type;
        cost = skillCost;
        isUnlocked = unlocked;
        isPurchased = purchased;

        UpdateVisual();
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
        UpdateVisual();
    }

    public void SetPurchased(bool purchased)
    {
        isPurchased = purchased;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (skillType == null) return;

        // 노드 배경
        if (nodeBackground != null)
        {
            if (isPurchased)
            {
                nodeBackground.sprite = bgNormal;
                nodeBackground.color = Color.white; // 구매 완료
            }
            else if (isUnlocked)
            {
                nodeBackground.sprite = bgNormal;
                nodeBackground.color = new Color(1f, 1f, 1f, 0.8f); // 살짝 투명
            }
            else
            {
                nodeBackground.sprite = bgGray;
                nodeBackground.color = new Color(0.5f, 0.5f, 0.5f); // 잠김
            }
        }

        // 아이콘
        if (icon != null && skillType != null)
        {
            icon.sprite = isPurchased ? skillType.unlockedSprite : skillType.lockedSprite;

            // 아이콘 색상
            if (isPurchased)
            {
                icon.color = Color.white;
            }
            else if (isUnlocked)
            {
                icon.color = new Color(1f, 1f, 1f, 0.9f);
            }
            else
            {
                icon.color = new Color(0.4f, 0.4f, 0.4f);
            }
        }

        // 버튼
        if (button != null)
        {
            button.interactable = isUnlocked && !isPurchased;
        }
    }

    void OnClick()
    {
        if (!isUnlocked || isPurchased) return;

    

        if (SkillTreeManager.Instance != null)
        {
            SkillTreeManager.Instance.PurchaseSkill(level, skillTypeIndex, cost, skillType.requiredCurrency);
        }
    }
}