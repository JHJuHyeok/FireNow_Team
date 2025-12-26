using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CostTableGenerator : MonoBehaviour
{
    private void Start()
    {
        EquipLevelUpCostTable table = new EquipLevelUpCostTable();
        List<EquipLevelUpCost> tableList = new();

        for(int i = 1; i <= 50; i++)
        {
            EquipLevelUpCost cost = new EquipLevelUpCost();
            cost.level = i;

            if (i < 10)
            {
                cost.stuffCount = i / 3 + 1;
                cost.requiredGold = 1000 * i;
            }
            else if (i < 15)
            {
                cost.stuffCount = 4;
                cost.requiredGold = 10000;
            }
            else if (i < 20)
            {
                cost.stuffCount = 5;
                cost.requiredGold = 15000;
            }
            else if (i < 25)
            {
                cost.stuffCount = 10;
                cost.requiredGold = 20000;
            }
            else if (i < 30)
            {
                cost.stuffCount = 15;
                cost.requiredGold = 25000;
            }
            else if (i < 35)
            {
                cost.stuffCount = 20;
                cost.requiredGold = 30000;
            }
            else if (i < 40)
            {
                cost.stuffCount = 25;
                cost.requiredGold = 40000;
            }
            else if (i < 50)
            {
                cost.stuffCount = 50;
                cost.requiredGold = 50000;
            }
            else
            {
                cost.stuffCount = 60;
                cost.requiredGold = 70000;
            }

            tableList.Add(cost);
        }

        table.costs = tableList;

        string json = JsonUtility.ToJson(table, true);

        string folder = "Assets/Resources/Json/CostTable";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = $"{folder}/EquipLevelUpCostTable.json";
        File.WriteAllText(filePath, json);

        Debug.Log("코스트 테이블 생성 완료");
    }
}
