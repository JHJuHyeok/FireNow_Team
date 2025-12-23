using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStat
{
    // 기본 스탯
    public StatValue maxHP = new();             // 최대 체력
    public StatValue attack = new();            // 공격력
    public StatValue defence = new();           // 방어력
    public StatValue moveSpeed = new();         // 움직이는 속도
    public StatValue getHPWithMeat = new();     // 고기 섭취시 회복량

    // 패시브 스탯(기본값은 1)
    public StatValue cooldown = new();          // 무기 쿨다운
    public StatValue duration = new();          // 효과 지속시간
    public StatValue getExp = new();            // EXP 획득량
    public StatValue range = new();             // 공격 범위
    public StatValue projectileSpeed = new();   // 투사체 속도
    public StatValue healHPOnSecond = new();    // 초당 hp 회복량

    /// <summary>
    /// 게임 시작 전 각 스탯의 상승치 초기화
    /// </summary>
    public void ClearRuntimeStats()
    {
        maxHP.ClearStat();
        attack.ClearStat();
        cooldown.ClearStat();
        moveSpeed.ClearStat();
        duration.ClearStat();
        getExp.ClearStat();
        projectileSpeed.ClearStat();
        range.ClearStat();
        healHPOnSecond.ClearStat();
        getHPWithMeat.ClearStat();
    }


}