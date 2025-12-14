using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//장비 슬롯인데..
//아무거나 막 들어가면 또 곤란하잖아
//6종류의 장비가 있어 예상으로는 데이터쪽에서 열거형으로 타입분류를 해놨을거야 아마도 그렇게 믿어야겠지
//1.그러면 장비 부위 타입 설정할 수 있는 어트리뷰트,
//2.인벤토리 슬롯이랑 마찬가지로 보여줄 이미지,
//3.현재 장착된 아이템이 무엇인지 저장용도
//4.슬롯에 아이템 설정할 함수,
//5.슬롯이 비워질때 함수
//6.슬롯을 클릭했을때 함수

//장비 슬롯 6종류에 컴포넌트로 적용
public class EquipSlot : MonoBehaviour
{
    [Header("장비 부위 타입")]
    //이제 받은 데이터 구조의 enum 따라가
    public EquipPart equipPart;

    [Header("슬롯 배경 이미지")]
    public Image slotBgImage;

    //장착시에만 보여질 이미지
    [Header("보여질 장착템 이미지-")]
    public Image equipSlotIcon;

    //슬롯에서 보여줄 아이템 테두리 이미지 추가해야함
    [Header("등급 테두리 이미지")]
    public Image gradeBorderImage;

    //등급맵핑 SO
    [Header("등급 맵핑 DB")]
    public ItemGradeDB gradeDB;

    //현재 장착된 아이템
    private Equip_ItemBase _equipItem; //테스트용 데이터로 임시

    private void Awake()
    {
        //초기 아이템 슬롯 이미지, 테두리 이미지 false
        equipSlotIcon.enabled = false;
        gradeBorderImage.enabled = false;
    }

    /// <summary>
    /// 슬롯에 아이템 설정할 함수 , 장비장착시 호출
    /// </summary>
    /// <param name="item"></param>
    public void Set(Equip_ItemBase item)
    {
        _equipItem = item;
        equipSlotIcon.sprite = item.ItemIcon;

        //슬롯 자체는 남아있고 이미지만 띄울거니까
        equipSlotIcon.enabled = true;

        //등급 테두리 적용
        gradeBorderImage.sprite = gradeDB.GetBorder(item.Grade);
        gradeBorderImage.enabled = true;
    }

    /// <summary>
    /// 슬롯이 비워질때 함수(장착해제), 장착해제시 호출
    /// </summary>
    /// <param name="item"></param>
    public void Clear(Equip_ItemBase item)
    {
        _equipItem = null;
        equipSlotIcon.sprite = null;
        equipSlotIcon.enabled = false;

        //등급 테두리 이미지도
        gradeBorderImage.sprite = null;
        gradeBorderImage.enabled = false;
    }

    /// <summary>
    /// 현재 장착된 아이템을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public Equip_ItemBase GetItem()
    {
        return _equipItem;
    }

    /// <summary>
    /// 장비슬롯을 클릭했을때 함수
    /// </summary>
    public void OnClick()
    {
        if (equipSlotIcon == null) return;

        ItemInfoPanel.Instance.ShowItemInfo(_equipItem, true);
    }
}
