using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 장비탭에서 인벤토리 아이템을 보여줄 슬롯UI
/// 인벤토리 슬롯에 컴포넌트로 적용
/// 슬롯 클릭시, 해당 슬롯의 정보 기반 인포패널 활성화 
/// </summary>
public class InventorySlot : MonoBehaviour
{
    [Header("보여질 인벤템 이미지")]
    public Image inventorySlotIcon;

    [Header("등급 테두리 이미지")]
    public Image gradeBorderImage;

    [Header("등급 맵핑 DB")]
    public ItemGradeDB gradeDB;

    //인포패널에서 보여줄 아이템 데이터 
    private Equip_ItemBase _item;

    /// <summary>
    /// 슬롯에 아이템 설정, 매핑된 스프라이트 적용
    /// </summary>
    public void Set(Equip_ItemBase item)
    {
        _item = item;
        //아이콘 표시해주고,
        inventorySlotIcon.sprite = item.ItemIcon;
        inventorySlotIcon.enabled = true;

        //등급별 테두리이미지 적용
        gradeBorderImage.sprite = gradeDB.GetBorder(item.Grade);
        gradeBorderImage.enabled = true;

        //해당오브젝트 활성화
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 슬롯 장착할때 이전 슬롯 비활성화
    /// </summary>
    public void Clear()
    {
        _item = null;

        if (inventorySlotIcon != null)
        {
            inventorySlotIcon.sprite = null;
            inventorySlotIcon.enabled = false;
        }

        if (gradeBorderImage != null)
        {
            gradeBorderImage.sprite = null;
            gradeBorderImage.enabled = false;
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 인벤토리 슬롯을 클릭시, 인포패널 활성화
    /// </summary>
    public void OnClick()
    {
        if (_item == null) return;
        ItemInfoPanel.Instance.ShowItemInfo(_item, false);
    }
}
