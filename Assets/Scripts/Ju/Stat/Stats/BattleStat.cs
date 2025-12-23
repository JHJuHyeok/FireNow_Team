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

    // 최종 계산된 스탯
    public float finalMaxHP;
    public float finalAttack;
    public float finalDefence;
    public float finalMoveSpeed;
    public float finalGetHP;
    public float finalCooldown;
    public float finalDuration;
    public float finalGetExp;
    public float finalRange;
    public float finalProjectileSpeed;
    public float finalHealHpOnSecond;

    /// <summary>
    /// 게임 시작 전 각 스탯의 상승치 초기화
    /// </summary>
    public void ClearRuntimeStats()
    {
        maxHP.ClearStat();
        attack.ClearStat();
        defence.ClearStat();
        cooldown.ClearStat();
        moveSpeed.ClearStat();
        duration.ClearStat();
        getExp.ClearStat();
        projectileSpeed.ClearStat();
        range.ClearStat();
        healHPOnSecond.ClearStat();
        getHPWithMeat.ClearStat();
    }

    /// <summary>
    /// 최종 계산값 산출
    /// </summary>
    public void Refresh()
    {
        finalMaxHP = maxHP.calculateValue;
        finalAttack = attack.calculateValue;
        finalDefence = defence.calculateValue;
        finalCooldown = cooldown.calculateValue;
        finalMoveSpeed = moveSpeed.calculateValue;
        finalDuration = duration.calculateValue;
        finalGetExp = getExp.calculateValue;
        finalProjectileSpeed = projectileSpeed.calculateValue;
        finalRange = range.calculateValue;
        finalHealHpOnSecond = healHPOnSecond.calculateValue;
        finalGetHP = getHPWithMeat.calculateValue;
    }
}