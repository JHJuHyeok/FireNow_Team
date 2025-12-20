using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostTable
{
    private static Dictionary<int, EquipLevelUpCost> costTable;

    /// <summary>
    /// 게임 시작 시 장비 레벨업 시 재화 테이블 호출
    /// </summary>
    public static void Initialize()
    {
        costTable = new Dictionary<int, EquipLevelUpCost>();
        
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/CostTable/EquipLevelUpCostTable");
        EquipLevelUpCostTable table = JsonUtility.FromJson<EquipLevelUpCostTable>(jsonFile.text);

        for (int i = 0; i < table.costs.Count; i++)
        {
            EquipLevelUpCost cost = table.costs[i];
            costTable[cost.level] = cost;
        }
    }

    /// <summary>
    /// 장비 레벨업 시 필요한 재화 불러오기
    /// </summary>
    /// <param name="currentLevel"> 현재 레벨 또는 호출하고자 하는 레벨 </param>
    /// <returns> 각 레벨에 맞는 필요 재화량 정보 </returns>
    public EquipLevelUpCost GetCost(int currentLevel)
    {
        if (costTable.TryGetValue(currentLevel, out var cost))
            return cost;

        return null;
    }
}
