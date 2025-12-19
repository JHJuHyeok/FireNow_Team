using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDatabase
{
    private static Dictionary<string, WaveData> waveDict;

    /// <summary>
    /// 게임 시작 시 웨이브 데이터베이스 불러오기
    /// </summary>
    public static void Initialize()
    {
        waveDict = new Dictionary<string, WaveData>();

        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Json/Wave");

        foreach (var file in jsonFiles)
        {
            WaveData data = JsonUtility.FromJson<WaveData>(file.text);
            waveDict[data.id] = data;
        }
    }

    /// <summary>
    /// 웨이브 데이터 불러오기
    /// </summary>
    /// <param name="id"> 불러올 웨이브 ID </param>
    /// <returns> ID값에 대응하는 WaveData 값 (없으면 null 반환) </returns>
    public static WaveData GetAbility(string id)
    {
        if (waveDict.TryGetValue(id, out var wave))
            return wave;

        return null;
    }
}
