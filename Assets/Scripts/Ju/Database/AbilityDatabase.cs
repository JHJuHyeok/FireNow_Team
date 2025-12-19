using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDatabase
{
    private static Dictionary<string, AbilityData> AbilityDict;

    /// <summary>
    /// 게임 시작 시 능력 데이터베이스 불러오기
    /// </summary>
    public static void Initialize()
    {
        AbilityDict = new Dictionary<string, AbilityData>();

        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Json/Ability");

        foreach (var file in jsonFiles)
        {
            AbilityData data = JsonUtility.FromJson<AbilityData>(file.text);
            AbilityDict[data.id] = data;
        }
    }

    /// <summary>
    /// 고를 수 있는 능력 목록 불러오기
    /// </summary>
    /// <param name="id"> 불러올 목록 ID </param>
    /// <returns> ID값에 대응하는 AbilityData 값 (없으면 null 반환) </returns>
    public static AbilityData GetAbility(string id)
    {
        if (AbilityDict.TryGetValue(id, out var ability))
            return ability;

        return null;
    }

}
