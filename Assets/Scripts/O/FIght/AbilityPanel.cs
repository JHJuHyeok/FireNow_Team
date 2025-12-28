using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class AbilityPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public Image abiliIcon;           // AbiliIcon
    public TMP_Text nameText;         // NameText
    public TMP_Text exText;           // ExText (설명)
    public GameObject starLinear;     // StarLinear (별 표시)
    public Image[] stars;             // FirstStarOne ~ Five
    public GameObject evol;           // Evol (진화 표시)
    public TMP_Text evolText;         // EvolText
    public Image evolImage;           // EvolImage
    public Button button;             // 버튼 (패널 자체)


    [Header("Background")]
    public Image backgroundImage;  // 배경 이미지 

    [Header("Background Sprites")]
    public Sprite weaponBackground;     // 무기용 배경
    public Sprite passiveBackground;    // 패시브용 배경
    public Sprite evolutionBackground;  // 진화용 배경


    [Header("Star Sprites")]
    public Sprite emptyStarSprite;    // 빈 별 
    public Sprite filledStarSprite;   // 레벨 별 
    public Sprite redStarSprite;      // 빨간 별 


    private AbilityData currentAbility;
    private int displayLevel;
    private AbilitySelectionManager manager;
    private Coroutine blinkCoroutine;


    public void Setup(AbilityData ability, int level, AbilitySelectionManager selectionManager)
    {
        Debug.Log($"[AbilityPanel] Setup 시작 - {ability.name} Lv.{level}");

        currentAbility = ability;
        displayLevel = level;
        manager = selectionManager;


        // 이전 깜빡임 중지
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }


        // 배경 스프라이트 변경
        SetBackground(ability.type);

        // 아이콘
   Sprite sprite = Resources.Load<Sprite>($"Sprites/Classified/Ability_Icon/{ability.spriteName}");
        if (sprite != null && abiliIcon != null)
        {
            abiliIcon.sprite = sprite;
       
        }
     

        // 이름
        if (nameText != null)
        {
            nameText.text = ability.name;
    
        }
   

        // 설명
        if (exText != null && level <= ability.levels.Count)
        {
            exText.text = ability.levels[level - 1].description;
  
        }
  
        
        if (exText != null && level <= ability.levels.Count)
        {
            exText.text = ability.levels[level - 1].description;
 
            
        }


        // 별 표시
        if (ability.type == AbilityType.evolution)
        {
            // 진화 무기
            if (starLinear != null) starLinear.SetActive(true);
            SetEvolutionStars();
        }
        else
        {
            // 일반 무기/패시브
            if (starLinear != null) starLinear.SetActive(true);

            // 진화 조건이 있을 때만 표시
            if (!string.IsNullOrEmpty(ability.evolution.requireItem))
            {
                if (evol != null) evol.SetActive(true);

                AbilityData requestItem = AbilityDatabase.GetAbility(ability.evolution.requireItem);
                if (requestItem != null && evolImage != null)
                {
                    Sprite sprite1 = Resources.Load<Sprite>($"Sprites/Classified/Ability_Icon/{requestItem.spriteName}");
                    if (sprite1 != null)
                    {
                        evolImage.sprite = sprite1;
                    }
                    else
                    {
                        Debug.LogWarning($"진화 아이템 스프라이트를 찾을 수 없습니다: {requestItem.spriteName}");
                    }
                }
                else
                {
                    if (evol != null) evol.SetActive(false);
                }
            }
            else
            {
                // 진화 조건이 없으면 evol UI 비활성화
                if (evol != null) evol.SetActive(false);
            }

            SetNormalStars(level);
        }

        // 버튼 이벤트
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => manager.SelectAbility(currentAbility));
        }
    }



    // 일반 무기/패시브 별 설정
    void SetNormalStars(int level)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null) continue;

            // 모든 별 활성화
            stars[i].gameObject.SetActive(true);

            if (i < level - 1)
            {
                // 이미 획득한 레벨 - 채워진 별 (노란색)
                stars[i].sprite = filledStarSprite;
                stars[i].color = Color.white;
            }
            else if (i == level - 1)
            {
                // 현재 추가될 레벨 - 깜빡이는 별
                stars[i].sprite = filledStarSprite;
                blinkCoroutine = StartCoroutine(BlinkStar(stars[i]));
            }
            else
            {
                // 아직 획득 안한 레벨 - 빈 별 (회색)
                stars[i].sprite = emptyStarSprite;
                stars[i].color = Color.white;
            }
        }
    }

    // 진화 무기 별 설정
    void SetEvolutionStars()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null) continue;

            if (i == 2) // 가운데 별 (인덱스 2)
            {
                stars[i].gameObject.SetActive(true);
                stars[i].sprite = redStarSprite;
                stars[i].color = Color.white;

                // 가운데 별도 깜빡임
                blinkCoroutine = StartCoroutine(BlinkStar(stars[i]));
            }
            else
            {
                // 나머지 별들은 비활성화
                stars[i].gameObject.SetActive(false);
            }
        }
    }

    // 별 깜빡임 코루틴
    IEnumerator BlinkStar(Image star)
    {
        while (true)
        {
            // 페이드 아웃
            float alpha = 1f;
            while (alpha > 0.3f)
            {
                alpha -= Time.unscaledDeltaTime * 2f; // unscaledDeltaTime 사용 (Time.timeScale = 0일 때도 작동)
                star.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }

            // 페이드 인
            while (alpha < 1f)
            {
                alpha += Time.unscaledDeltaTime * 2f;
                star.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
        }
    }



    // 배경 설정 메서드
    void SetBackground(AbilityType type)
    {
        if (backgroundImage == null) return;

        switch (type)
        {
            case AbilityType.weapon:
                if (weaponBackground != null)
                {
                    backgroundImage.sprite = weaponBackground;
                }
                break;

            case AbilityType.passive:
                if (passiveBackground != null)
                {
                    backgroundImage.sprite = passiveBackground;
                }
                break;

            case AbilityType.evolution:
                if (evolutionBackground != null)
                {
                    backgroundImage.sprite = evolutionBackground;
                }
                break;
        }
    }
}