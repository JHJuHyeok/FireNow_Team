using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// 인벤토리,장착슬롯 누르면 열릴 인포패널
/// 각 필드요소들 섹션별로 나누어서 기능 호출
/// 인포패널에 컴포넌트로 적용
/// </summary>
public class ItemInfoPanel : MonoBehaviour
{
    public static ItemInfoPanel Instance;

    //플레이어 데이터 들어와야 하고(보유량 체크)
    [Header("플레이어 데이터")]
    public PlayerInfoSO playerInfo;
    
    [Header("기본 텍스트 UI")] //담당섹션-BasicInfoSection
    //아이템 이름
    public TextMeshProUGUI itemNameText;
    //아이템 설명
    public TextMeshProUGUI itemDesciptionText; 
    //등급 이름
    public TextMeshProUGUI gradeText;

    [Header("아이템 이미지/등급 이미지")] //담당섹션-BasicInfoSection
    //아이템 이미지
    public Image itemIcon;
    //아이템 등급 테두리 이미지
    public Image gradeBorderImage;
    //아이템 등급(상단) 이미지
    public Image topGradeImage;

    [Header("아이템 레벨 텍스트 UI")] //담당섹션-LevelInfoSection
    //아이템 레벨 <-이부분도 시작 레벨이라 변수 이름이 좀 맘에 안드는데-바꿀의향on
    public TextMeshProUGUI itemLevelText;
    //아이템 맥스레벨
    public TextMeshProUGUI itemMaxLevelText;

    [Header("기본 능력치 관련")] //담당섹션-StatIconInfoSection
    //부위별로 변경된 기본능력치 아이콘
    public Image statIconImage;
    //공격력 아이콘(무기,목걸이,장갑)
    public Sprite attackIconSprite;
    //체력 아이콘(아머,벨트,부츠)
    public Sprite hpIconSprite;
    //아이템 공격력 <-이부분 공용 능력치로(체력/공격력) 변수이름 바꿔야되고**
    public TextMeshProUGUI attackText;

    #region 등급별 스킬설명-담당섹션-GradeSkillInfoSection
    [Header("등급 스킬 설명 - 그린(노말등급)")]
    //등급 변경될 해금이미지
    public Image greenUnlockImage;
    //등급 해금 아이콘
    public Sprite greenUnlockedSprite;
    //등급 미해금 아이콘
    public Sprite greenLockedSprite;
    //등급 텍스트
    public TextMeshProUGUI greenGradeText;

    [Header("등급 스킬 설명 - 퍼플(레어등급)")]
    public Image puppleUnlockImage; 
    public Sprite puppleUnlockedSprite;
    public Sprite puppleLockedSprite;
    public TextMeshProUGUI puppleGradeText;

    [Header("등급 스킬 설명 - 옐로우(전설등급)")]
    public Image yellowUnlockImage;
    public Sprite yellowUnlockedSprite;
    public Sprite yellowLockedSprite;
    public TextMeshProUGUI yellowGradeText;

    //등급별 텍스트 색상-잠금상태, 잠금해제 상태 표시용
    [Header("등급스킬 텍스트 색상")]
    public Color activeGradeTextColor = new Color(1.0f, 0.9f, 0.2f, 1.0f); //옐로우
    public Color lockedGradeTextColor = new Color(0.2f, 0.2f, 0.2f, 0.9f); //어두운색
    #endregion

    ////레벨업시 필요 비용 관련
    //[Header("레벨업시 필요 비용 관련")] //데이터 추가로 들어오면, HUD 쪽 작업이랑 연동하는게 나을듯 한데,-**일단 보류
    ////보유코인
    //public TextMeshProUGUI haveCoinText;
    ////레벨업 필요 코인
    //public TextMeshProUGUI needCoinText;
    ////보유 재료 갯수
    //public TextMeshProUGUI havePartText;
    ////필요 재료 갯수
    //public TextMeshProUGUI needPartText;
    ////레벨업 재료 이미지
    //public Image needPartIcon;

    //등급맵핑 SO
    [Header("등급 맵핑 DB")]
    public ItemGradeDB gradeDB;

    [Header("버튼 UI")] //추후 레벨업버튼, 레벨업All 버튼 추가해야함 -**레벨업 시스템 구현때 같이 진행
    //장착 버튼
    public GameObject equipButton;
    //장착해제 버튼
    public GameObject unEquipButton;
    //Exit버튼
    public GameObject exitButton;

    //현재 아이템 저장용
    private Equip_ItemBase _curItem; 

    //장착 상태인지 플래그
    private bool _isEquipped;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 아이템 정보를 인포패널에 표시할 함수 --섹션별로 나눠서 리팩토링 할것
    /// </summary>
    /// <param name="item"></param>
    /// <param name="isEquipped"></param>
    public void ShowItemInfo(Equip_ItemBase item, bool isEquipped)
    {
        _curItem = item;
        _isEquipped = isEquipped;

        BasicInfoSection(item);
        StatIconInfoSection(item);
        LevelInfoSection(item);
        GradeSkillInfoSection(item);

        //**
        //필요재화 재료 부분은 아직 수정x-아래 부분도 섹션 나눠서 처리할것
        //haveCoinText.text = item.HaveCoin.ToString();
        //needCoinText.text = item.NeedCoin.ToString();
        //havePartText.text = item.HavePartCount.ToString();
        //needPartText.text = item.NeedPartCount.ToString();
        //needPartIcon.sprite = item.NeedPartIcon;
        //**

        //버튼 설정
        equipButton.SetActive(!_isEquipped && item.CanEquip);
        unEquipButton.SetActive(_isEquipped);

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 아이템 기본 정보 섹션
    /// </summary>
    /// <param name="item"></param>
    private void BasicInfoSection(Equip_ItemBase item)
    {
        itemNameText.text = item.ItemName;
        gradeText.text = item.Grade.ToString();
        itemDesciptionText.text = item.Description;
        itemIcon.sprite = item.ItemIcon;
        gradeBorderImage.sprite = gradeDB.GetBorder(item.Grade);
        topGradeImage.sprite = gradeDB.GetTopImage(item.Grade);
    }

    /// <summary>
    /// 기본 능력치 아이콘 변경 섹션
    /// </summary>
    /// <param name="item"></param>
    private void StatIconInfoSection(Equip_ItemBase item)
    {
        //무기, 목걸이, 장갑은 어택파츠
        bool isAttackPart = (item.EquipPart == EquipPart.weapon)||(item.EquipPart == EquipPart.necklace)||(item.EquipPart == EquipPart.glove);
        //어택 파츠면 어택아이콘으로 변경
        statIconImage.sprite = isAttackPart ? attackIconSprite : hpIconSprite;
        //기본 능력치 텍스트
        attackText.text = item.AttackPower.ToString();
    }

    /// <summary>
    /// 아이템 레벨 정보 섹션
    /// </summary>
    /// <param name="item"></param>
    private void LevelInfoSection(Equip_ItemBase item)
    {
        itemLevelText.text = item.Level.ToString();
        itemMaxLevelText.text = item.MaxLevel.ToString();
    }  

    /// <summary>
    /// 등급 스킬 설명 갱신 섹션
    /// </summary>
    /// <param name="item"></param>
    private void GradeSkillInfoSection(Equip_ItemBase item)
    {
        //각 줄마다 기준이 되는 등급
        Grade greenGrade = Grade.normal;
        Grade puppleGrade = Grade.rare;
        Grade yellowGrade = Grade.legend;

        //해금상태 판단
        bool greenUnlocked = IsGradeUnlocked(item.Grade, greenGrade);
        bool puppleUnlocked = IsGradeUnlocked(item.Grade, puppleGrade);
        bool yellowUnlocked = IsGradeUnlocked(item.Grade, yellowGrade);

        //텍스트 내용 처리
        greenGradeText.text = GetGradeDescript(item, Grade.normal);
        puppleGradeText.text = GetGradeDescript(item, Grade.rare);
        yellowGradeText.text = GetGradeDescript(item, Grade.legend);

        //여기서 종합처리
        //그린등급 처리
        ApplyGradeUnlockState(greenUnlockImage, greenUnlockedSprite, greenLockedSprite, greenGradeText, greenUnlocked);
        //퍼플등급
        ApplyGradeUnlockState(puppleUnlockImage, puppleUnlockedSprite, puppleLockedSprite, puppleGradeText, puppleUnlocked);
        //옐로등급
        ApplyGradeUnlockState(yellowUnlockImage, yellowUnlockedSprite, yellowLockedSprite, yellowGradeText, yellowUnlocked);
    }

    /// <summary>
    /// Grade Enum 순서대로 해금상태 판단
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool IsGradeUnlocked(Grade current, Grade target)
    {
        return (int)current >= (int)target;
    }

    /// <summary>
    /// 해금 상태에 따라 등급별스킬 해금아이콘,텍스트 색상 변경
    /// </summary>
    /// <param name="image"></param>
    /// <param name="unLockedSprite"></param>
    /// <param name="lockedSprite"></param>
    /// <param name="text"></param>
    /// <param name="unlocked"></param>
    private void ApplyGradeUnlockState(Image image, Sprite unLockedSprite, Sprite lockedSprite, TextMeshProUGUI text, bool unlocked)
    {
        image.sprite = unlocked ? unLockedSprite : lockedSprite;
        text.color = unlocked ? activeGradeTextColor : lockedGradeTextColor;
    }

    /// <summary>
    /// 특정 등급의 등급설명 읽어오기
    /// </summary>
    /// <param name="item"></param>
    /// <param name="grade"></param>
    /// <returns></returns>
    private string GetGradeDescript(Equip_ItemBase item, Grade grade)
    {
        List<EquipGrade> grades = item.SourceEquipData.equipGrades;

        for (int i = 0; i < grades.Count; i++)
        {
            if (grades[i].grade == grade)
            {
                return grades[i].descript;
            }
        }
        return string.Empty;
    }

    /// <summary>
    /// 장착버튼
    /// </summary>
    public void OnEquipClick()
    {
        //장비 슬롯에 더해주고
        EquipControl.Instance.Equip(_curItem);

        //창 닫아주고
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 장착해제 버튼
    /// </summary>
    public void OnUnEquipClick()
    {
        //장비슬롯에서 빼줘야 되고,
        EquipControl.Instance.UnEquip(_curItem);

        //창 닫아주고
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 닫기 창 버튼
    /// </summary>
    public void OnExitClick()
    {
        gameObject.SetActive(false);
    }
}
