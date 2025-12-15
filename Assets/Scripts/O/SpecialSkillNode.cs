using UnityEngine;
using UnityEngine.UI;

public class SpecialSkillNode : MonoBehaviour
{
    [Header("UI References")]
    public Image nodeBackground;
    public Image icon;
    public Button button;

    [Header("Sprites")]
    public Sprite bgNormal;
    public Sprite bgGray;

    private Sprite unlockedSprite;
    private Sprite lockedSprite;

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

        // specialSkill이 null이어도 괜찮도록 처리
        if (skill != null)
        {
            unlockedSprite = skill.unlockedSprite;
            lockedSprite = skill.lockedSprite;
        }

        UpdateVisual();
    }

    // 스프라이트를 외부에서 설정할 수 있도록 추가
    public void SetSprites(Sprite unlocked, Sprite locked)
    {
        unlockedSprite = unlocked;
        lockedSprite = locked;
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
        // 노드 배경
        if (nodeBackground != null)
        {
            if (isPurchased)
            {
                nodeBackground.sprite = bgNormal;
           
            }
            else if (isUnlocked)
            {
                nodeBackground.sprite = bgGray;

            }
            else
            {
                nodeBackground.sprite = bgGray;
                
            }
        }

        // 아이콘 - unlockedSprite/lockedSprite 사용
        if (icon != null)
        {
            // 구매 여부에 따라 스프라이트 선택
            Sprite targetSprite = isPurchased ? unlockedSprite : lockedSprite;

            // 스프라이트가 없으면 잠금 상태에 따라 선택
            if (targetSprite == null)
            {
                targetSprite = isUnlocked ? unlockedSprite : lockedSprite;
            }

            if (targetSprite != null)
            {
                icon.sprite = targetSprite;
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
            SkillTreeManager.Instance.PurchaseSpecialSkill(specialSkillIndex);
        }
    }
}