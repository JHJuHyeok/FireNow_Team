using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[System.Serializable]
public class EnemyData
{
    public string id;           // 몬스터 식별 ID
    public int hp;              // 체력
    public float speed;         // 이동 속도
    public float damage;        // 데미지
    public string dropItem;     // 드랍 경험치/아이템 ID

    public string idleAnimation;    // 통상 애니메이션 경로
    public string deadAnimation;    // 사망 애니메이션 경로

    [JsonConverter(typeof(StringEnumConverter))]
    public MoveType moveType;       // 움직이는 방식
}

[System.Serializable]
public class EnemyDatabaseDTO
{
    public string id;
    public List<EnemyData> enemyList;
}

public enum MoveType
{
    oneDirection,
    chase
}