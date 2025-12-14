using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//슬롯 누르면 열릴 인포패널
//인벤토리 슬롯 에서 클릭하면 패널의 장착버튼이 나오게,
//장비슬롯 에서 클릭하면 패널의 장착해제 버튼이 나오게,

//인포패널에 필요한 정보가 무엇이냐
//아이템 이름, 등급이름, 공격력(int), 아이템 설명, 아이템 이미지,
//레벨(int) 등급별 설명, 보유코인, 필요코인, 필요재료 이미지, 재료필요갯수, 재료 보유갯수
//텍스트랑 이미지 정수형 버튼 전부 카테고리로 일단 나눠봐

//상호작용되는건 무엇이냐
//버튼 레벨업까지 포함? 일단 장착버튼, exit 버튼은 확실하게 yes

//장착 상태인지? 아닌지? 플래그

//인포패널에 컴포넌트로 적용
public class ItemInfoPanel : MonoBehaviour
{
    public static ItemInfoPanel Instance;

    [Header("텍스트 UI")]
    //아이템 이름
    public TextMeshProUGUI itemNameText;
    //아이템 설명
    public TextMeshProUGUI itemDesciptionText;
    //등급 이름
    public TextMeshProUGUI gradeText;

    //아이템 상세 (등급스킬)설명 빠짐

    [Header("수치 UI")]
    //아이템 공격력
    public TextMeshProUGUI attackText;
    //아이템 레벨
    public TextMeshProUGUI itemLevelText;
    //보유코인
    public TextMeshProUGUI haveCoinText;
    //레벨업 필요 코인
    public TextMeshProUGUI needCoinText;
    //보유 재료 갯수
    public TextMeshProUGUI havePartText;
    //필요 재료 갯수
    public TextMeshProUGUI needPartText;

    [Header("이미지 UI")]
    //아이템 이미지
    public Image itemIcon;
    //레벨업 재료 이미지
    public Image needPartIcon;

    //아이템 등급 테두리 이미지
    public Image gradeBorderImage;
    //아이템 등급(상단) 이미지
    public Image topGradeImage;

    //등급맵핑 SO
    [Header("등급 맵핑 DB")]
    public ItemGradeDB gradeDB;

    [Header("버튼 UI")]
    //장착 버튼
    public GameObject equipButton;
    //장착해제 버튼
    public GameObject unEquipButton;
    //Exit버튼
    public GameObject exitButton;

    //현재 아이템 저장용
    private Equip_ItemBase _curItem; //임시 데이터

    //장착 상태인지 플래그
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

        //텍스트 채우기
        itemNameText.text = item.ItemName;
        gradeText.text = item.Grade.ToString();
        itemDesciptionText.text = item.Description;

        //수치 채우기
        attackText.text = item.AttackPower.ToString();
        itemLevelText.text = item.Level.ToString();
        //haveCoinText.text = item.HaveCoin.ToString();
        //needCoinText.text = item.NeedCoin.ToString();
        //havePartText.text = item.HavePartCount.ToString();
        //needPartText.text = item.NeedPartCount.ToString();

        //이미지 채우기
        itemIcon.sprite = item.ItemIcon;
        //needPartIcon.sprite = item.NeedPartIcon;
        gradeBorderImage.sprite = gradeDB.GetBorder(item.Grade);
        topGradeImage.sprite = gradeDB.GetTopImage(item.Grade);

        //버튼 설정
        equipButton.SetActive(!_isEquipped && item.CanEquip);
        unEquipButton.SetActive(_isEquipped);

        gameObject.SetActive(true);
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
