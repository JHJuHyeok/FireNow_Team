using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 장비 슬롯 관련
/// 슬롯 6종류에 컴포넌트로 적용
/// </summary>
public class EquipSlot : MonoBehaviour
{
    [Header("장비 부위 타입")]
    public EquipPart equipPart;

    [Header("슬롯 배경 이미지")]
    public Image slotBgImage;

    [Header("보여질 장착템 이미지-")]
    public Image equipSlotIcon;

    [Header("등급 테두리 이미지")]
    public Image gradeBorderImage;

    [Header("등급 맵핑 DB")]
    public ItemGradeDB gradeDB;

    //현재 장착된 아이템 저장용
    private Equip_ItemBase _equipItem;

    private void Awake()
    {
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

        //등급 테두리 이미지도 해제
        gradeBorderImage.sprite = null;
        gradeBorderImage.enabled = false;
    }

    /// <summary>
    /// 현재 장착된 아이템을 반환
    /// </summary>
    /// <returns></returns>
    public Equip_ItemBase GetItem()
    {
        return _equipItem;
    }

    /// <summary>
    /// 장비슬롯을 클릭시,
    /// 해당 아이템 정보기반의 인포패널 활성화 
    /// </summary>
    public void OnClick()
    {
        if (_equipItem == null) return;
        if (equipSlotIcon == null) return;

        ItemInfoPanel.Instance.ShowItemInfo(_equipItem, true);
    }
}
