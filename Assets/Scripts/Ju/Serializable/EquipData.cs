using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[System.Serializable]
public class EquipData
{
    public string id;                       // 장비 ID
    public string equipName;                // 장비 명칭
    public string descript;                 // 장비 설명
    public string iconPath;                 // 아이콘 경로

    [JsonConverter(typeof(StringEnumConverter))]
    public EquipPart part;                  // 장비의 부위

    public List<EquipGrade> equipGrades;    // 각 등급의 데이터

    public string levelUpStuffId; //레벨업에 필요한 재료 id -병합전에 토의하고 지울것-윤성원>
}
[System.Serializable]
public class EquipGrade
{
    [JsonConverter(typeof(StringEnumConverter))]
    public Grade grade;             // 장비 등급
    public int maxLevel;            // 최대 레벨
    public int startValue;          // 레벨업 안 한 능력치
    public string descript;         // 등급별 설명
}

public enum Grade
{
    normal,
    rare,
    elite,
    epic,
    legend
}
public enum EquipPart
{
    weapon,
    necklace,
    glove,
    armor,
    belt,
    shoes
}
