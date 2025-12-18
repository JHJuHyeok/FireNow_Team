using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDatabase
{
    private static Dictionary<string, EnemyDatabaseDTO> enemiesDict;

    /// <summary>
    /// 게임 시작 시 적 데이터베이스 불러오기
    /// </summary>
    public static void Initialize()
    {
        enemiesDict = new Dictionary<string, EnemyDatabaseDTO>();

        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Json/Enemy");

        foreach (var file in jsonFiles)
        {
            EnemyDatabaseDTO data = JsonUtility.FromJson<EnemyDatabaseDTO>(file.text);
            enemiesDict[data.id] = data;
        }
    }

    /// <summary>
    /// 각 스테이지 적 목록 불러오기
    /// </summary>
    /// <param name="id"> 불러올 목록 ID </param>
    /// <returns> ID값에 대응하는 EnemyDatabaseDTO 값 (없으면 null 반환) </returns>
    public static EnemyDatabaseDTO GetEnemies(string id)
    {
        if (enemiesDict.TryGetValue(id, out var enemies))
            return enemies;

        return null;
    }

    /// <summary>
    /// 목록에서 적 단일 데이터 불러오기
    /// </summary>
    /// <param name="baseId"> 탐색할 목록의 ID </param>
    /// <param name="enemyId"> 찾을 단일 적 ID </param>
    /// <returns> 찾는 적 ID가 있으면 ID에 맞는 EnemyData return </returns>
    public static EnemyData GetEnemy(string baseId, string enemyId)
    {
        if (enemiesDict.TryGetValue(baseId, out var enemies))
        {
            foreach (var enemy in enemies.enemyList)
            {
                if (enemy.id == enemyId)
                    return enemy;
                else continue;
            }
        }

        return null;
    }
}
