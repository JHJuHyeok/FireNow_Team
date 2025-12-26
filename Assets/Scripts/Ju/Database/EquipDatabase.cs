using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipDatabase
{
    private static Dictionary<string, EquipData> equipDict;

    /// <summary>
    /// 게임 시작 시 장비 데이터베이스 불러오기
    /// </summary>
    public static void Initialize()
    {
        equipDict = new Dictionary<string, EquipData>();

        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Json/Equip");

        foreach (var file in jsonFiles)
        {
            EquipData data = JsonUtility.FromJson<EquipData>(file.text);
            equipDict[data.id] = data;
        }
    }

    /// <summary>
    /// SaveManager에서 보유한 장비 불러오기
    /// </summary>
    /// <param name="id"> 불러올 장비 ID </param>
    /// <returns> ID값에 대응하는 EquipData 값 (없으면 null 반환) </returns>
    public static EquipData GetEquip(string id)
    {
        if (equipDict.TryGetValue(id, out var equip))
            return equip;

        return null;
    }
}
