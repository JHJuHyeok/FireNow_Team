using UnityEngine;
using UnityEngine.UI;

public class SpecialSkillNode : MonoBehaviour
{
    [Header("UI References")]
    public Image nodeBackground;      // 배경
    public Image icon;                // 특수 스킬 아이콘
    public Button button;

    [Header("Sprites")]
    public Sprite bgNormal;           // 활성화 배경
    public Sprite bgGray;             // 비활성화 배경

    private int specialSkillIndex;
    private SpecialSkillData specialSkill;
    private bool isUnlocked;
    private bool isPurchased;

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    public void Initialize(int index, SpecialSkillData skill, bool unlocked, bool purchased)
    {
        specialSkillIndex = index;
        specialSkill = skill;
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
        if (specialSkill == null) return;

        // 노드 배경
        if (nodeBackground != null)
        {
            if (isPurchased)
            {
                nodeBackground.sprite = bgNormal;
                nodeBackground.color = new Color(1f, 0.8f, 0.2f); // 특수 스킬은 금색
            }
            else if (isUnlocked)
            {
                nodeBackground.sprite = bgNormal;
                nodeBackground.color = new Color(1f, 0.8f, 0.2f, 0.6f); // 살짝 투명
            }
            else
            {
                nodeBackground.sprite = bgGray;
                nodeBackground.color = new Color(0.3f, 0.3f, 0.3f); // 잠김
            }
        }

        // 아이콘
        if (icon != null && specialSkill != null)
        {
            icon.sprite = isPurchased ? specialSkill.unlockedSprite : specialSkill.lockedSprite;

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
                icon.color = new Color(0.3f, 0.3f, 0.3f);
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
        
        }
    }
}