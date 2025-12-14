using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어 장비 관련 능력치 - 임시- 최종 데이터와 호환 생각해둘것
public class PlayerEquip_Stat : MonoBehaviour
{
    public static PlayerEquip_Stat Instance;

    public int baseAttack;
    private int equipAttack;

    public int TotalAttack
    {
        get { return baseAttack + equipAttack; }
    }

    private void Awake()
    {
        Instance = this;
    }

    //장비 장착
    public void AddEquipStat(Equip_ItemBase item)
    {
        equipAttack += item.AttackPower;
    }

    //장비 해제
    public void RemoveEquipStat(Equip_ItemBase item)
    {
        equipAttack -= item.AttackPower;
    }
}
