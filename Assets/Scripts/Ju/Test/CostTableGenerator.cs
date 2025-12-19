using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CostTableGenerator : MonoBehaviour
{
    private void Start()
    {
        EquipLevelUpCostTable table = new EquipLevelUpCostTable();

        for(int i = 1; i <= 50; i++)
        {
            table.costs[i - 1].level = i;
            if (i < 10)
            {
                table.costs[i - 1].stuffCount = i / 3 + 1;
                table.costs[i - 1].requiredGold = 1000 * i;
            }
            else if (10 <= i && i < 15)
            {
                table.costs[i - 1].stuffCount = 4;
                table.costs[i - 1].requiredGold = 10000;
            }
            else if (15 <= i && i < 20)
            {
                table.costs[i - 1].stuffCount = 5;
                table.costs[i - 1].requiredGold = 15000;
            }
            else if (20 <= i && i < 25)
            {
                table.costs[i - 1].stuffCount = 10;
                table.costs[i - 1].requiredGold = 20000;
            }
            else if (25 <= i && i < 30)
            {
                table.costs[i - 1].stuffCount = 15;
                table.costs[i - 1].requiredGold = 25000;
            }
            else if (30 <= i && i < 35)
            {
                table.costs[i - 1].stuffCount = 20;
                table.costs[i - 1].requiredGold = 30000;
            }
            else if (35 <= i && i < 40)
            {
                table.costs[i - 1].stuffCount = 25;
                table.costs[i - 1].requiredGold = 40000;
            }
            else if (40 <= i && i < 50)
            {
                table.costs[i - 1].stuffCount = 50;
                table.costs[i - 1].requiredGold = 50000;
            }
        }

        string json = JsonUtility.ToJson(table, true);

        string folder = "Assets/Resources/Json/CostTable";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = $"{folder}/EquipLevelUpCostTable.json";
        File.WriteAllText(filePath, json);

        Debug.Log("코스트 테이블 생성 완료");
    }
}
