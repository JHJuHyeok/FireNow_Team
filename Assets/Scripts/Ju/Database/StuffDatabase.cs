using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 소지품 데이터베이스
/// </summary>
public class StuffDatabase
{
    // 소지품 목록 저장용 딕셔너리
    private static Dictionary<string, StuffData> stuffDict;

    /// <summary>
    /// 게임 시작 시 소지품 데이터베이스 불러오기
    /// </summary>
    public static void Initialize()
    {
        stuffDict = new Dictionary<string, StuffData>();

        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Json/StuffData");

        foreach (var file in jsonFiles)
        {
            StuffData data = JsonUtility.FromJson<StuffData>(file.text);
            stuffDict[data.id] = data;
        }
    }

    /// <summary>
    /// SaveManager에서 보유 소지품 불러오기
    /// </summary>
    /// <paramname="id"> 세이브 데이터로 불러올 소지품 ID </param>
    /// <returns> ID 값에 대응하는 StuffData 데이터 </returns>
    public static StuffData GetStuff(string id)
    {
        if(stuffDict.TryGetValue(id, out var stuff))
            return stuff;

        return null;
    }
}
