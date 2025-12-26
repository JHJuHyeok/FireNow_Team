using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipLevelUpCost
{
    public int level;           // 각 레벨
    public int stuffCount;      // 필요 재료량
    public int requiredGold;    // 필요 골드
}

[System.Serializable]
public class EquipLevelUpCostTable
{
    public List<EquipLevelUpCost> costs;
}
