using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class StuffJsonCreator : EditorWindow
{
    string id;
    string stuffName;
    string iconPath;
    string descript;

    Grade grade = Grade.normal;

    [MenuItem("Tools/Stuff JSON Creator")]
    public static void ShowWindow()
    {
        GetWindow<StuffJsonCreator>("Stuff Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Stuff JSON 기본 정보", EditorStyles.boldLabel);

        id = EditorGUILayout.TextField("소지품 ID", id);
        stuffName = EditorGUILayout.TextField("소지품 이름", stuffName);
        iconPath = EditorGUILayout.TextField("이미지 경로", iconPath);
        descript = EditorGUILayout.TextField("소지품 설명", descript);

        GUILayout.Space(8);
        GUILayout.Label("Stuff 등급", EditorStyles.boldLabel);

        grade = (Grade)EditorGUILayout.EnumPopup("소지품 등급", grade);

        GUILayout.Space(15);

        if (GUILayout.Button("Json 파일 생성"))
        {
            CreateWaveJson();
        }
    }

    private void CreateWaveJson()
    {
        StuffData data = new StuffData
        {
            id = id,
            stuffName = stuffName,
            iconPath = iconPath,
            descript = descript,
            grade = grade
        };

        string json = JsonUtility.ToJson(data, true);

        string folder = "Assets/Resources/Json/Stuff";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = $"{folder}/{id}.json";
        File.WriteAllText(filePath, json);

        AssetDatabase.Refresh();
        Debug.Log($"생성 완료: {filePath}");
    }
}
