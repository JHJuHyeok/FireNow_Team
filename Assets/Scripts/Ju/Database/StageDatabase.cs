using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스테이지 데이터베이스
/// </summary>
public class StageDatabase
{
    private static Dictionary<string, StageData> stageDict;

    /// <summary>
    /// 게임 시작 시 스테이지 데이터 불러오기
    /// </summary>
    public static void Initialize()
    {
        stageDict = new Dictionary<string, StageData>();
        
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Stage/StageDatabase");
        StageDatabaseDTO stageDatabase = JsonUtility.FromJson<StageDatabaseDTO>(jsonFile.text);

        for (int i = 0; i < stageDatabase.stages.Count; i++)
        {
            StageData data = stageDatabase.stages[i];
            stageDict[data.id] = data;
        }
    }

    /// <summary>
    /// SaveManager에서 스테이지 불러오기
    /// </summary>
    /// <param name="id"> 찾아오는 스테이지 ID </param>
    /// <returns> ID값에 맞는 스테이지 데이터(없을 경우 null) </returns>
    public static StageData GetStage(string id)
    {
        if (stageDict.TryGetValue(id, out var stage))
            return stage;

        return null;
    }
}
