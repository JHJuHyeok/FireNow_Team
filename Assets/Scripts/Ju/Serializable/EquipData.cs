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
    public string spriteName;               // 아틀라스에서 불러올 스프라이트 이름

    public string requiredStuffId;          // 장비 레벨업에 필요한 소모품 ID

    [JsonConverter(typeof(StringEnumConverter))]
    public EquipPart part;                  // 장비의 부위
    [JsonConverter(typeof(StringEnumConverter))]
    public StatType stat;

    public List<EquipGrade> equipGrades;    // 각 등급의 데이터
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
    uncommon,
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

public enum StatType
{
    attack,
    health
}
