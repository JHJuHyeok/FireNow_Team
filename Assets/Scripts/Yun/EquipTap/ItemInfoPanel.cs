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

    [Header("플레이어 데이터")]
    [SerializeField] private PlayerInfoSO playerInfo;
    
    [Header("기본 텍스트 UI")] //담당섹션-BasicInfoSection
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDesciptionText;
    [SerializeField] private TextMeshProUGUI gradeText;

    [Header("아이템 이미지/등급 이미지")] //담당섹션-BasicInfoSection
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image gradeBorderImage;
    [SerializeField] private Image topGradeImage;

    [Header("아이템 레벨 텍스트 UI")] //담당섹션-LevelInfoSection
    [SerializeField] private TextMeshProUGUI itemLevelText;
    [SerializeField] private TextMeshProUGUI itemMaxLevelText;

    [Header("기본 능력치 관련")] //담당섹션-StatIconInfoSection
    [SerializeField] private Image statIconImage;
    [SerializeField] private Sprite attackIconSprite;
    [SerializeField] private Sprite hpIconSprite;
    [SerializeField] private TextMeshProUGUI baseStatText;

    #region 등급별 스킬설명-담당섹션-GradeSkillInfoSection
    [Header("등급 스킬 설명 - 그린(노말등급)")]
    [SerializeField] private Image greenUnlockImage;
    [SerializeField] private Sprite greenUnlockedSprite;
    [SerializeField] private Sprite greenLockedSprite;
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

    [Header("레벨업 관련 UI")]
    [SerializeField] private Button levelUpButton;
    [SerializeField] private TextMeshProUGUI haveGoldText;
    [SerializeField] private TextMeshProUGUI needGoldText;
    [SerializeField] private TextMeshProUGUI haveStuffText;
    [SerializeField] private TextMeshProUGUI needStuffText;
    [SerializeField] private Image needStuffIcon;
    [SerializeField] private TextMeshProUGUI levelUpAlertText;
    [SerializeField] private float alertTime = 1.0f;

    private Coroutine alertCo;

    [Header("등급 맵핑 DB")]
    [SerializeField] private ItemGradeDB gradeDB;

    [Header("버튼 UI")]
    [SerializeField] private GameObject equipButton;
    [SerializeField] private GameObject unEquipButton;
    [SerializeField] private GameObject exitButton;

    [Header("HUD 참조")]
    [SerializeField] private HUD hud;

    //현재 아이템 저장용
    private Equip_ItemBase _curItem; 

    //장착 상태 플래그
    private bool _isEquipped;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 아이템 정보를 인포패널에 표시할 함수
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
    /// 선택된 아이템의 정보를 기반으로 텍스트,이미지 변경
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
    /// 부위별 능력치 타입 아이콘 변경
    /// </summary>
    /// <param name="item"></param>
    private void StatIconInfoSection(Equip_ItemBase item)
    {
        //무기,목걸이,장갑=>어택파츠
        bool isAttackPart = (item.EquipPart == EquipPart.weapon)||(item.EquipPart == EquipPart.necklace)||(item.EquipPart == EquipPart.glove);
        //어택 파츠는 어택아이콘으로 변경
        statIconImage.sprite = isAttackPart ? attackIconSprite : hpIconSprite;
        //기본 능력치 텍스트
        baseStatText.text = item.StatValue.ToString();
    }

    /// <summary>
    /// 아이템 레벨 정보 섹션
    /// 선택된 아이템의 레벨에 맞게 레벨텍스트 변경
    /// </summary>
    /// <param name="item"></param>
    private void LevelInfoSection(Equip_ItemBase item)
    {
        itemLevelText.text = item.Level.ToString();
        itemMaxLevelText.text = item.MaxLevel.ToString();
        //아이템 레벨 변동시 능력치도 변동
        baseStatText.text = item.StatValue.ToString();
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

        //각 등급별 처리 구간
        ApplyGradeUnlockState(greenUnlockImage, greenUnlockedSprite, greenLockedSprite, greenGradeText, greenUnlocked);
        ApplyGradeUnlockState(puppleUnlockImage, puppleUnlockedSprite, puppleLockedSprite, puppleGradeText, puppleUnlocked);
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
    /// 현재 선택된 아이템의 레벨업 필요재료 아이콘 갱신
    /// EquipDataRuntime.requiredStuffId로 StuffData 조회,StuffDataRuntime에서 Icon생성해서 UI에 반영
    /// </summary>
    private void RefreshNeedStuffIcon()
    {
        //해당 ID의 재료 아이콘 표시
        string requiredStuffId = _curItem.SourceEquipData.requiredStuffId;

        //StuffData 조회
        StuffData stuffdata = StuffDatabase.GetStuff(requiredStuffId);

        StuffDataRuntime runtimeStuff = new StuffDataRuntime(stuffdata);

        needStuffIcon.sprite = runtimeStuff.icon;
    }

    /// <summary>
    /// 장비 레벨업시 알림 텍스트 표시관련 
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
        //HUD 갱신
        hud.RefreshHUD(playerInfo);

        //장착 중인 상태라면, 스탯 재적용(재계산)
        if (_isEquipped == true)
        {
            EquipStatSystem.Instance.RecalculateFromEquipSlots(EquipControl.Instance.equipSlots);
        }

        //인벤토리 UI 재갱신
        EquipControl.Instance.RefreshInventoryUI();

        RefreshLevelUpUI();
        
        //저장지점
        SaveManager.Instance.Save();
    }

    /// <summary>
    /// 장착버튼 클릭시, 장비슬롯에 현재 아이템 추가
    /// </summary>
    public void OnEquipClick()
    {
        EquipControl.Instance.Equip(_curItem);

        SoundManager.Instance.PlaySound("PutOnEquip");
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 장착해제 버튼 클릭시, 장비슬롯에 현재 아이템 제거
    /// </summary>
    public void OnUnEquipClick()
    {
        EquipControl.Instance.UnEquip(_curItem);

        SoundManager.Instance.PlaySound("PutOffEquip");
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 인포패널내의 닫기 창 버튼-패널비활성화
    /// </summary>
    public void OnExitClick()
    {
        SoundManager.Instance.PlaySound("ClosePopup");
        gameObject.SetActive(false);
    }
}
