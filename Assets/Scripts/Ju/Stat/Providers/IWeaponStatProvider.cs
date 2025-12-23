using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponStatProvider
{
    void Apply(WeaponStat weaponStat, BattleStat battleStat);
}
