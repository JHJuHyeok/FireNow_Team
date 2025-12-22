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
}
