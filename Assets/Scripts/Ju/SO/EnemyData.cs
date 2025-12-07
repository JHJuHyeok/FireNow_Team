using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public string id;           // 몬스터 식별 ID
    public string name;         // 몬스터 이름
    public int hp;              // 체력
    public float speed;         // 이동 속도
    public int damage;          // 데미지
    // 드랍 경험치/아이템 데이터

    public string idleAnimation;    // 통상 애니메이션 경로
    public string deadAnimation;    // 사망 애니메이션 경로

    public MoveType moveType;       // 움직이는 방식
}

[System.Serializable]
public class EnemyDatabase
{
    public List<EnemyData> list;
}

public enum MoveType
{
    oneDirection,
    chase
}