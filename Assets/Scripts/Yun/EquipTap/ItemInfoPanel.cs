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
    [SerializeField] private PlayerInfoSO playerInfo;
    
    [Header("기본 텍스트 UI")] //담당섹션-BasicInfoSection
    //아이템 이름
    [SerializeField] private TextMeshProUGUI itemNameText;
    //아이템 설명
    [SerializeField] private TextMeshProUGUI itemDesciptionText;
    //등급 이름
    [SerializeField] private TextMeshProUGUI gradeText;

    [Header("아이템 이미지/등급 이미지")] //담당섹션-BasicInfoSection
    //아이템 이미지
    [SerializeField] private Image itemIcon;
    //아이템 등급 테두리 이미지
    [SerializeField] private Image gradeBorderImage;
    //아이템 등급(상단) 이미지
    [SerializeField] private Image topGradeImage;

    [Header("아이템 레벨 텍스트 UI")] //담당섹션-LevelInfoSection
    //아이템 레벨 <-이부분도 시작 레벨이라 변수 이름이 좀 맘에 안드는데-바꿀의향on
    [SerializeField] private TextMeshProUGUI itemLevelText;
    //아이템 맥스레벨
    [SerializeField] private TextMeshProUGUI itemMaxLevelText;

    [Header("기본 능력치 관련")] //담당섹션-StatIconInfoSection
    //부위별로 변경된 기본능력치 아이콘
    [SerializeField] private Image statIconImage;
    //공격력 아이콘(무기,목걸이,장갑)
    [SerializeField] private Sprite attackIconSprite;
    //체력 아이콘(아머,벨트,부츠)
    [SerializeField] private Sprite hpIconSprite;
    //아이템 공격력 <-이부분 공용 능력치로(체력/공격력) 변수이름 바꿔야되고**
    [SerializeField] private TextMeshProUGUI attackText;

    #region 등급별 스킬설명-담당섹션-GradeSkillInfoSection
    [Header("등급 스킬 설명 - 그린(노말등급)")]
    //등급 변경될 해금이미지
    [SerializeField] private Image greenUnlockImage;
    //등급 해금 아이콘
    [SerializeField] private Sprite greenUnlockedSprite;
    //등급 미해금 아이콘
    [SerializeField] private Sprite greenLockedSprite;
    //등급 텍스트
    [SerializeField] private TextMeshProUGUI greenGradeText;

    [Header("등급 스킬 설명 - 퍼플(레어등급)")]
    [SerializeField] private Image puppleUnlockImage;
    [SerializeField] private Sprite puppleUnlockedSprite;
    [SerializeField] private Sprite puppleLockedSprite;
    [SerializeField] private TextMeshProUGUI puppleGradeText;

    [Header("등급 스킬 설명 - 옐로우(전설등급)")]
    [SerializeField] private Image yellowUnlockImage;
    [SerializeField] private Sprite yellowUnlockedSprite;
    [SerializeField] private Sprite yellowLockedSprite;
    [SerializeField] private TextMeshProUGUI yellowGradeText;

    //등급별 텍스트 색상-잠금상태, 잠금해제 상태 표시용
    [Header("등급스킬 텍스트 색상")]
    [SerializeField] private Color activeGradeTextColor = new Color(1.0f, 0.9f, 0.2f, 1.0f); //옐로우
    [SerializeField] private Color lockedGradeTextColor = new Color(0.2f, 0.2f, 0.2f, 0.9f); //어두운색
    #endregion

    [Header("레벨업 관련 UI")] //담당섹션
    //레벨업 버튼
    [SerializeField] private Button levelUpButton;
    //보유코인
    [SerializeField] private TextMeshProUGUI haveGoldText;
    //레벨업 필요 코인
    [SerializeField] private TextMeshProUGUI needGoldText;
    //보유 재료 갯수
    [SerializeField] private TextMeshProUGUI haveStuffText;
    //필요 재료 갯수
    [SerializeField] private TextMeshProUGUI needStuffText;
    //레벨업 재료 이미지
    [SerializeField] private Image needStuffIcon;
    //레벨업 불가 알림 텍스트
    [SerializeField] private TextMeshProUGUI levelUpAlertText;

    //등급맵핑 SO
    [Header("등급 맵핑 DB")]
    [SerializeField] private ItemGradeDB gradeDB;

    [Header("버튼 UI")]
    //장착 버튼
    [SerializeField] private GameObject equipButton;
    //장착해제 버튼
    [SerializeField] private GameObject unEquipButton;
    //Exit버튼
    [SerializeField] private GameObject exitButton;

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

        //상황별 버튼 설정
        equipButton.SetActive(!_isEquipped && item.CanEquip);
        unEquipButton.SetActive(_isEquipped);

        //레벨업 관련 갱신 함수 호출 
        RefreshLevelUpUI();

        gameObject.SetActive(true);
    }

    //현재 선택된 아이템 기준으로
    //다음 레벨업 비용 표시, 보유재화 표시, 버튼 상호작용,알림 텍스트 갱신
    private void RefreshLevelUpUI()
    {
        ////EquipInfoBridge가 아니면 실제 level을 올릴 수 없음(시스템 설계상)
        //EquipInfoBridge bridge = _curItem as EquipInfoBridge;
        //if (bridge == null)
        //{
        //    levelUpButton.interactable = false;
        //    SetAlertText("레벨업 불가 대상입니다");
        //    return;
        //}

        //만렙 체크
        if (_curItem.Level >= _curItem.MaxLevel)
        {
            levelUpButton.interactable = false;
            SetAlertText("더 이상 레벨을 올릴 수 없습니다.");
            SetCostTexts("-", "-");
            SetHaveTexts(GetGold().ToString(), GetStuffAmount(GetLevelUpStuffId()).ToString());
            return;
        }

        //다음 레벨업 비용 가져오기
        int nextLevel = _curItem.Level + 1;
        EquipLevelUpCost cost = CostTable.GetCost(nextLevel);

        if (cost == null)
        {
            levelUpButton.interactable = false;
            SetAlertText("비용 테이블 없음");
            SetCostTexts("-", "-");
            return;
        }

        //재료 ID는 EquipDataRuntime.levelUpStuffId에서 가져옴(부위별 차등 가능하게)
        string stuffId = GetLevelUpStuffId();
        int haveGold = GetGold();
        int haveStuff = GetStuffAmount(stuffId);

        SetCostTexts(cost.requiredGold.ToString(), cost.stuffCount.ToString());
        SetHaveTexts(haveGold.ToString(), haveStuff.ToString());

        bool canLevelUp = (haveGold >= cost.requiredGold) && (haveStuff >= cost.stuffCount);
        levelUpButton.interactable = canLevelUp;

        if (canLevelUp)
        {
            SetAlertText("");
        }
        else
        {
            int lackGold = cost.requiredGold - haveGold;
            int lackStuff = cost.stuffCount - haveStuff;

            if (lackGold < 0) lackGold = 0;
            if (lackStuff < 0) lackStuff = 0;

            SetAlertText("부족: 골드 " + lackGold + ", 재료 " + lackStuff);
        }
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

    /// <summary>
    /// 레벨업 버튼
    /// </summary>
    public void OnClick_LevelUp()
    {
        if (_curItem == null)
        {
            return;
        }

        EquipInfoBridge bridge = _curItem as EquipInfoBridge;
        if (bridge == null)
        {
            SetAlertText("레벨업 불가 대상");
            RefreshLevelUpUI();
            return;
        }

        // 만렙 방어
        if (_curItem.Level >= _curItem.MaxLevel)
        {
            SetAlertText("MAX");
            RefreshLevelUpUI();
            return;
        }

        int nextLevel = _curItem.Level + 1;
        EquipLevelUpCost cost = CostTable.GetCost(nextLevel);
        if (cost == null)
        {
            SetAlertText("비용 테이블 없음");
            RefreshLevelUpUI();
            return;
        }

        //재료 ID는 장비 정의 데이터에서 가져옴(부위별)
        string stuffId = GetLevelUpStuffId();

        int haveGold = GetGold();
        int haveStuff = GetStuffAmount(stuffId);

        //보유 재화 부족하면 중단
        if (haveGold < cost.requiredGold || haveStuff < cost.stuffCount)
        {
            SetAlertText("재화가 부족합니다");
            RefreshLevelUpUI();
            return;
        }

        //보유골드 차감
        playerInfo.gold = playerInfo.gold - cost.requiredGold;

        //보유재료 차감
        bool stuffSpent = TrySpendStuff(stuffId, cost.stuffCount);
        if (stuffSpent == false)
        {
            //재료 차감이 실패하면 골드 다시 돌려주기
            playerInfo.gold = playerInfo.gold + cost.requiredGold;
            SetAlertText("재료 차감 실패");
            RefreshLevelUpUI();
            return;
        }

        //레벨 증가 (진짜 데이터는 EquipInfo.level)
        bridge.ItemBaseSourceInfo.level = bridge.ItemBaseSourceInfo.level + 1;

        //장착 중이라면 스탯 갱신(현재 AttackPower가 레벨 반영 안 해도, 추후 반영 대비) -이부분 아직 임시-플레이어 스탯관련이 없음
        if (_isEquipped == true)
        {
            PlayerEquip_Stat.Instance.RemoveEquipStat(_curItem);
            PlayerEquip_Stat.Instance.AddEquipStat(_curItem);
        }

        //패널 UI 다시 갱신
        LevelInfoSection(_curItem);
        StatIconInfoSection(_curItem);

        //인벤토리 UI도 갱신(슬롯에 레벨 표기 생길 수 있으므로 안전하게)
        if (EquipControl.Instance != null)
        {
            EquipControl.Instance.RefreshInventoryUI();
        }

        SetAlertText("해당 장비 레벨업!");
        RefreshLevelUpUI();
    }

    /// <summary>
    /// 현재 선택 장비의 레벨업에 사용되는 재료 ID를 반환하는 함수
    /// (부위별 강화재료는 데이터가 결정)
    /// </summary>
    private string GetLevelUpStuffId()
    {        
        return _curItem.SourceEquipData.levelUpStuffId;
    }

    /// <summary>
    /// 보유 골드 반환
    /// </summary>
    private int GetGold()
    {
        return playerInfo.gold;
    }

    /// <summary>
    /// 특정 재료ID의 보유 개수를 반환
    /// PlayerInfoSO.stuffs에서 찾는다.
    /// </summary>
    private int GetStuffAmount(string stuffId)
    {
        for (int i = 0; i < playerInfo.stuffs.Count; i++)
        {
            StuffStack stack = playerInfo.stuffs[i];
            if (stack == null || stack.stuff == null)
            {
                continue;
            }

            if (stack.stuff.id == stuffId)
            {
                return stack.amount;
            }
        }

        return 0;
    }

    /// <summary>
    /// 특정 재료ID를 amount만큼 차감시도 하는 함수
    /// 성공하면 true, 부족/미보유면 false 반환
    /// </summary>
    private bool TrySpendStuff(string stuffId, int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        for (int i = 0; i < playerInfo.stuffs.Count; i++)
        {
            StuffStack stack = playerInfo.stuffs[i];
            if (stack == null || stack.stuff == null)
            {
                continue;
            }

            if (stack.stuff.id == stuffId)
            {
                if (stack.amount < amount)
                {
                    return false;
                }

                stack.amount = stack.amount - amount;

                return true;
            }
        }
        return false;
    }
    #region 레벨업 관련 유틸성 함수

    /// <summary>
    /// 레벨업 관련 주의 알림텍스트 설정 
    /// </summary>
    /// <param name="message"></param>
    private void SetAlertText(string message)
    {
        if (levelUpAlertText == null)
        {
            return;
        }
        levelUpAlertText.text = message;
    }

    /// <summary>
    /// 필요 재화 갯수(텍스트) 설정
    /// </summary>
    /// <param name="gold"></param>
    /// <param name="stuff"></param>
    private void SetCostTexts(string gold, string stuff)
    {
        if (needGoldText != null) needGoldText.text = gold;
        if (needStuffText != null) needStuffText.text = stuff;
    }

    /// <summary>
    /// 보유 재화 갯수(텍스트) 설정
    /// </summary>
    /// <param name="gold"></param>
    /// <param name="stuff"></param>
    private void SetHaveTexts(string gold, string stuff)
    {
        if (haveGoldText != null) haveGoldText.text = gold;
        if (haveStuffText != null) haveStuffText.text = stuff;
    }
    #endregion
}
