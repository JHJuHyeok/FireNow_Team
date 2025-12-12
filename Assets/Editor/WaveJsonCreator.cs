using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class WaveJsonCreator : EditorWindow
{
    string id;
    int startTime;
    int endTime;
    string enemiesPath;
    float spawnRate;
    int spawnCount;

    [MenuItem("Tools/Wave JSON Creator")]
    public static void ShowWindow()
    {
        GetWindow<WaveJsonCreator>("Item Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Wave JSON 기본 정보", EditorStyles.boldLabel);

        id = EditorGUILayout.TextField("웨이브 ID", id);
        startTime = EditorGUILayout.IntField("웨이브 시작 시간", startTime);
        endTime = EditorGUILayout.IntField("웨이브 종료 시간", endTime);
        enemiesPath = EditorGUILayout.TextField("몬스터 JSON 경로", enemiesPath);

        GUILayout.Space(8);
        GUILayout.Label("Wave 스폰 정보", EditorStyles.boldLabel);

        spawnRate = EditorGUILayout.FloatField("스폰 간격", spawnRate);
        spawnCount = EditorGUILayout.IntField("스폰 횟수", spawnCount);

        GUILayout.Space(15);

        if (GUILayout.Button("Json 파일 생성"))
        {
            CreateWaveJson();
        }
    }

    private void CreateWaveJson()
    {
        WaveData data = new WaveData
        {
            id = id,
            startTime = startTime,
            endTime = endTime,
            enemiesPath = enemiesPath,
            spawnRate = spawnRate,
            spawnCount = spawnCount
        };

        string json = JsonUtility.ToJson(data, true);

        string folder = "Assets/Resources/Json";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = $"{folder}/{id}.json";
        File.WriteAllText(filePath, json);

        AssetDatabase.Refresh();
        Debug.Log($"생성 완료: {filePath}");
    }
}
