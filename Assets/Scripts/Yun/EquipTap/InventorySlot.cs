using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//장비탭에서 인벤토리 아이템을 보여줄 슬롯UI
//해당 슬롯 클릭하면, 정보 패널 띄워주고, 패널안에 해당 슬롯 아이템 데이터 대입

//인벤토리 슬롯에 컴포넌트로 적용, 슬롯프리팹으로 만들어야함
public class InventorySlot : MonoBehaviour
{
    [Header("보여질 인벤템 이미지")]
    public Image inventorySlotIcon;

    //슬롯에서 보여줄 아이템 테두리 이미지 추가해야함!!
    [Header("등급 테두리 이미지")]
    public Image gradeBorderImage;

    //등급맵핑 SO
    [Header("등급 맵핑 DB")]
    public ItemGradeDB gradeDB;

    //정보패널에서 보여줄 아이템 데이터 
    private Equip_ItemBase _item; //테스트용 데이터로 임시

    /// <summary>
    /// 슬롯에 아이템을 설정하고 UI를 업데이트 해줄 함수
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
    /// 슬롯 비워질때 (장착할때)
    /// </summary>
    public void Clear()
    {
        //아이템 데이터는 널이고
        _item = null;

        //아이콘 남아있으면 없애주고,
        if (inventorySlotIcon != null)
        {
            inventorySlotIcon.sprite = null;
            inventorySlotIcon.enabled = false;
        }

        //테두리 이미지도 남아있으면 없애주고
        if (gradeBorderImage != null)
        {
            gradeBorderImage.sprite = null;
            gradeBorderImage.enabled = false;
        }

        //아니면 걍 오브젝트 비활성화
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 인벤토리 슬롯을 클릭했을때, 인포패널 띄우기
    /// </summary>
    public void OnClick()
    {
        //없으면 아무것도 하지말고
        if (_item == null) return;
        //아이템 정보를 담은 패널을 띄워
        ItemInfoPanel.Instance.ShowItemInfo(_item, false);
    }
}
