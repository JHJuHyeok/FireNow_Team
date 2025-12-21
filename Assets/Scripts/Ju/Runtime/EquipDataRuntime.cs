using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipDataRuntime
{
    public string id;                       // 장비 ID
    public string equipName;                // 장비 명칭
    public string descript;                 // 장비 설명
    public Sprite icon;                     // 아이콘

    public string requiredStuffId;          // 레벨업에 필요한 소모품 ID

    public EquipPart part;                  // 장비의 부위

    public List<EquipGrade> equipGrades;    // 각 등급의 데이터

    public EquipDataRuntime(EquipData data)
    {
        id = data.id;
        equipName = data.equipName;
        descript = data.descript;
        icon = AtlasManager.GetSprite("Equip_Item_Atlas", data.spriteName);

        requiredStuffId = data.requiredStuffId;

        part = data.part;

        equipGrades = data.equipGrades;
    }
}