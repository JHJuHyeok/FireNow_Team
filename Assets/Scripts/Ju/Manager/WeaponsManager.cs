using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager
{
    private Dictionary<string, Weapon> weaponDict = new();

    /// <summary>
    /// 무기 획득(레벨업) 시 딕셔너리에 추가
    /// </summary>
    /// <param name="data"> 무기의 어빌리티 런타임 데이터 </param>
    public void AddWeapon(AbilityDataRuntime data)
    {
        // 이미 지니고 있는 무기면 레벨업
        if(weaponDict.TryGetValue(data.id, out var weapon))
        {
            weapon.levelUp();
        }
        // 지니고 있지 않으면 추가
        else
        {
            weaponDict[data.id] = new Weapon(data);
        }
    }

    /// <summary>
    /// 딕셔너리에서 무기 삭제
    /// </summary>
    /// <param name="id"> 삭제하려는 무기 ID </param>
    public void RemoveWeapon(string id)
    {
        weaponDict.Remove(id);
    }

    /// <summary>
    /// 모든 무기의 스탯 재계산
    /// </summary>
    /// <param name="battleStat"> 변경된 전투 시 스탯 </param>
    public void RecalculateAll(BattleStat battleStat)
    {
        foreach(var weapon in weaponDict.Values)
        {
            weapon.RecalculateStat(battleStat);
        }
    }
}
