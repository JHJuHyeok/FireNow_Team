using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[System.Serializable]
public class AbilityData
{
    public string id;                           // 아이템 식별 ID
    public string name;                         // UI에 표시될 이름
    public string spriteName;                   // 아틀라스에서 불러올 스프라이트 이름
    public int maxLevel;                        // 최대 레벨

    [JsonConverter(typeof(StringEnumConverter))]
    public AbilityType type;                    // 아이템의 타입

    public List<AbilityLevelData> levels;       // 각 레벨의 데이터
    
    public WeaponEvolution evolution;           // 진화 관련 데이터
}

[System.Serializable]
public class AbilityLevelData       // 레벨 데이터
{
    // 타입이 무기일 때
    public float damage;            // 데미지
    public float cooldown;          // 쿨다운
    public int projectileCount;     // 투사체 개수
    public float range;             // 범위
    public float speed;             // 속도
    public float duration;          // 지속 시간

    // 타입이 패시브일 때
    public float rangeIncrease;     // 범위 상승
    public float speedIncrease;     // 투사체 속도 상승
    public float maxHPIncrease;     // 최대 체력 상승
    public float healHPIncrease;    // 초당 회복량 상승
    public float durationDecrease;  // 효과 지속시간 감소
    public float cooldownDecrease;  // 공격 간격 감소
    public float getEXPIncrease;    // 경험치 획득량 상승

    // 업그레이드 시 표시될 설명
    public string description;
}

[System.Serializable]
public class WeaponEvolution        // 진화 데이터
{
    public string requireItem;      // 진화 시 필요한 패시브 아이템
    public string result;           // 진화 후 무기
}

public enum AbilityType             // 능력 타입
{
    weapon,
    passive,
    evolution
}
