using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 소지품 데이터베이스
/// </summary>
public class StuffDatabase : MonoBehaviour
{
    // 소지품 목록 저장용 딕셔너리
    private Dictionary<string, StuffDataRuntime> stuffDict = new();

    /// <summary>
    /// JSON 데이터에서 소지품 데이터베이스 불러오기
    /// </summary>
    /// <param name="json"> 불러올 소지품 데이터베이스 Json 데이터</param>
    public void LoadFromJson(string json)
    {
        StuffDatabaseDTO db = JsonUtility.FromJson<StuffDatabaseDTO>(json);

        // 불러오기 전 소지품 정리
        stuffDict.Clear();

        // [ID, 소지품 데이터] 형태로 저장된 목록 불러오기
        foreach(var data in db.stuffs)
        {
            StuffDataRuntime stuff = new StuffDataRuntime(data);
            stuffDict[stuff.id] = stuff;
        }

        // 총 몇 개 불러왔는지 확인용 로그
        Debug.Log($"StuffDatabase Loaded: {stuffDict.Count} stuff");
    }

    /// <summary>
    /// SaveManager에서 소지품 불러오기 함수
    /// </summary>
    /// <paramname="id"> 불러올 소지품 ID </param>
    /// <returns> ID 값에 대응하는 StuffDataRuntime 데이터 </returns>
    public StuffDataRuntime GetStuff(string id)
    {
        stuffDict.TryGetValue(id, out var stuff);
        return stuff;
    }
}
