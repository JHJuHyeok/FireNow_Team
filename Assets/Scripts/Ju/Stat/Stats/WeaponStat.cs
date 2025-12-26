using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStat
{
    public StatValue damage = new();
    public StatValue cooldown = new();
    public StatValue projectileCount = new();
    public StatValue range = new();
    public StatValue speed = new();
    public StatValue duration = new();

    public void ClearWeaponStats()
    {
        damage.ClearStat();
        cooldown.ClearStat();
        projectileCount.ClearStat();
        range.ClearStat();
        speed.ClearStat();
        duration.ClearStat();
    }

    // 최종 계산 결과 변수
    public float finalDamage;
    public float finalCoolDown;
    public int finalProjectileCount;
    public float finalRange;
    public float finalSpeed;
    public float finalDuration;

    /// <summary>
    /// 최종 계산값 산출
    /// </summary>
    public void CalculateFinalValues()
    {
        finalDamage = damage.calculateValue;
        finalCoolDown = Mathf.Max(0.05f, cooldown.calculateValue);
        finalProjectileCount = Mathf.Max(1, Mathf.RoundToInt(projectileCount.calculateValue));
        finalRange = range.calculateValue;
        finalSpeed = speed.calculateValue;
        finalDuration = duration.calculateValue;
    }
}
