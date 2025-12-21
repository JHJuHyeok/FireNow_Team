using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipDataRuntime
{
    public string id;                       // 장비 ID
    public string equipName;                // 장비 명칭
    public string descript;                 // 장비 설명
    public Sprite icon;                     // 아이콘

    public EquipPart part;                  // 장비의 부위

    public List<EquipGrade> equipGrades;    // 각 등급의 데이터

    public string levelUpStuffId; //레벨업에 필요한 재료 id -병합전에 토의하고 지울것-윤성원>

    public EquipDataRuntime(EquipData data)
    {
        id = data.id;
        equipName = data.equipName;
        descript = data.descript;
        icon = Resources.Load<Sprite>(data.iconPath);

        part = data.part;

        equipGrades = data.equipGrades;

        //어떤 강화재료를 쓰는지 런타임에서도 보관 - 병합전에 토의후 지울것 -윤성원>
        levelUpStuffId = data.levelUpStuffId;
    }
}