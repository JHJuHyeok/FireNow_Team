using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    //레벨업 상호작용 알림 텍스트
    [SerializeField] private TextMeshProUGUI levelUpAlertText;
    //텍스트 알림 유지시간
    [SerializeField] private float alertTime = 1.0f;

    private Coroutine alertCo;

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

    [Header("HUD 참조")]
    [SerializeField] private HUD hud;

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

        //레벨업 관련
        RefreshLevelUpUI();
        levelUpAlertText.gameObject.SetActive(false);

        SoundManager.Instance.PlaySound("OpenPopup");
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
        attackText.text = item.StatValue.ToString();
    }

    /// <summary>
    /// 아이템 레벨 정보 섹션
    /// </summary>
    /// <param name="item"></param>
    private void LevelInfoSection(Equip_ItemBase item)
    {
        itemLevelText.text = item.Level.ToString();
        itemMaxLevelText.text = item.MaxLevel.ToString();
        //아이템 레벨 변동시 능력치도 변동
        attackText.text = item.StatValue.ToString();
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
    /// 레벨업관련 비용/보유량/버튼 상태 갱신
    /// </summary>
    private void RefreshLevelUpUI()
    {
        EquipLevelUp.CheckLevelUpResult result = EquipLevelUp.Check(_curItem, playerInfo);

        haveGoldText.text = result.haveGold.ToString();
        needGoldText.text = result.needGold.ToString();
        haveStuffText.text = result.haveStuff.ToString();
        needStuffText.text = result.needStuff.ToString();
        //재료 아이콘 갱신
        RefreshNeedStuffIcon();
    }

    /// <summary>
    /// +추가부분 현재 선택된 아이템의 레벨업 필요재료 아이콘 갱신
    /// EquipDataRuntime.requiredStuffId로 StuffData 조회,StuffDataRuntime에서 Icon생성해서 UI에 반영
    /// </summary>
    private void RefreshNeedStuffIcon()
    {
        //부위마다 재료아이디가 다름, 해당 ID의 아이콘 표시
        string requiredStuffId = _curItem.SourceEquipData.requiredStuffId;

        //StuffData 조회
        StuffData stuffdata = StuffDatabase.GetStuff(requiredStuffId);

        //현재 AtlasManager가 StuffDataRuntime 내부에서 호출되고 있으니까,
        //여기서는 Sprite만 받는식으로
        StuffDataRuntime runtimeStuff = new StuffDataRuntime(stuffdata);

        needStuffIcon.sprite = runtimeStuff.icon;
    }

    /// <summary>
    /// 레벨업 알림 텍스트 표시관련
    /// </summary>
    /// <param name="text"></param>
    private void ShowAlert(string text)
    {
        //이전 코루틴 살아있으면 중지
        if (alertCo != null)
        {
            StopCoroutine(alertCo);
            alertCo = null;
        }

        levelUpAlertText.text = text;
        levelUpAlertText.gameObject.SetActive(true);

        SoundManager.Instance.PlaySound("Alert");

        alertCo = StartCoroutine(AlertCO(alertTime));
    }

    /// <summary>
    /// 레벨업 알림 텍스트 코루틴
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator AlertCO(float time)
    {
        yield return new WaitForSeconds(time);
        levelUpAlertText.gameObject.SetActive(false);
        alertCo = null;
    }

    /// <summary>
    /// 레벨업 버튼
    /// </summary>
    public void OnLevelUpClick()
    {
        //지금 상황을 체크하고,
        EquipLevelUp.CheckLevelUpResult check = EquipLevelUp.Check(_curItem, playerInfo);

        //레벨업 할 수 없는 경우
        if (check.canLevelUp == false)
        {
            //상활별 텍스트
            if (check.alertText == "최대레벨알림")
            {
                ShowAlert("이미 최대 레벨입니다!");
            }
            else if (check.alertText == "재화부족알림")
            {
                ShowAlert("재화가 부족합니다!");
            }
            else
            {
                ShowAlert("재료가 부족합니다!");
            }
            RefreshLevelUpUI();
            return;
        }

        string text;
        bool success = EquipLevelUp.TryLevelUp(_curItem, playerInfo, out text);

        if (success)
        {
            ShowAlert("레벨업 성공!");
        }

        SoundManager.Instance.PlaySound("Equip_LevelUp");
        //인포패널의 아이템 레벨표시 갱신
        LevelInfoSection(_curItem);
        //여기서 HUD 갱신
        hud.RefreshHUD(playerInfo);

        //장착 중인 상태라면, 스탯 재적용(재계산)
        if (_isEquipped == true)
        {
            EquipStatSystem.Instance.RecalculateFromEquipSlots(EquipControl.Instance.equipSlots);
        }

        //인벤토리 UI 재갱신(혹시몰라서 전체부분)
        EquipControl.Instance.RefreshInventoryUI();

        RefreshLevelUpUI();
        //저장지점
        SaveManager.Instance.Save();
    }

    /// <summary>
    /// 장착버튼
    /// </summary>
    public void OnEquipClick()
    {
        //장비 슬롯에 더해주고
        EquipControl.Instance.Equip(_curItem);

        SoundManager.Instance.PlaySound("PutOnEquip");
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

        SoundManager.Instance.PlaySound("PutOffEquip");
        //창 닫아주고
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 닫기 창 버튼
    /// </summary>
    public void OnExitClick()
    {
        SoundManager.Instance.PlaySound("ClosePopup");
        gameObject.SetActive(false);
    }
}
