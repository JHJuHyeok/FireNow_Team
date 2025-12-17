using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolveDatabase : MonoBehaviour
{
    private static Dictionary<string, EvolveData> evolveDict;

    /// <summary>
    /// 게임 시작 시 진화 데이터베이스 불러오기
    /// </summary>
    public static void Initialize()
    {
        evolveDict = new Dictionary<string, EvolveData>();

        TextAsset jsonFiles = Resources.Load<TextAsset>("Json/Evolve");
        EvolveDatabaseDTO database = JsonUtility.FromJson<EvolveDatabaseDTO>(jsonFiles.text);

        foreach (var evolve in database.evolves)
        {
            EvolveData data = evolve;
            evolveDict[evolve.id] = evolve;
        }
    }

    /// <summary>
    /// SaveManager에서 마지막으로 해금한 진화 불러오기
    /// </summary>
    /// <param name="id"> 불러올 진화 ID </param>
    /// <returns> ID값에 대응하는 EvolveData 값 (없으면 null 반환) </returns>
    public static EvolveData GetEvolve(string id)
    {
        if (evolveDict.TryGetValue(id, out var evolve))
            return evolve;

        return null;
    }
}
