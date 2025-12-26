using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 레벨테이블 역할인 evolveLevelConfig.json 로드,
/// 레벨별 비용과 슬롯 설정 제공위함
/// 
/// EvolveDatabase는 정의 데이터,
/// 여기는 데이터베이스의 level을 레벨규칙과 테이블로 제공
/// </summary>
public static class EvolveLevelConfigDatabase
{
    private static Dictionary<int, EvolveLevelConfig> _levelDict;
    private static int _maxLevel;

    /// <summary>
    /// 게임 시작 시 호출하는거 잊지 말것
    /// </summary>
    public static void Initialize()
    {
        _levelDict = new Dictionary<int, EvolveLevelConfig>();
        _maxLevel = 0;

        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Evolve/evolveLevelConfigs");
        EvolveLevelConfigs dto = JsonUtility.FromJson<EvolveLevelConfigs>(jsonFile.text);

        for (int i = 0; i < dto.levels.Count; i++)
        {
            EvolveLevelConfig levelData = dto.levels[i];
            if (levelData == null) continue;

            _levelDict[levelData.level] = levelData;

            if (levelData.level > _maxLevel)
            {
                _maxLevel = levelData.level;
            }
        }
    }

    /// <summary>
    /// 레벨 테이블 기준 최대 레벨 반환함수
    /// </summary>
    public static int GetMaxLevel()
    {
        return _maxLevel;
    }

    /// <summary>
    /// level에 해당하는 설정 반환함수 (없으면 일단null)
    /// </summary>
    public static EvolveLevelConfig GetLevel(int level)
    {
        if (_levelDict == null)
        {
            return null;
        }

        EvolveLevelConfig result;
        if (_levelDict.TryGetValue(level, out result))
        {
            return result;
        }

        return null;
    }
}
