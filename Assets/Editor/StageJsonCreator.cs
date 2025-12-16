using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class StageJsonCreator : EditorWindow
{
    List<StageData> stages = new List<StageData>();

    [MenuItem("Tools/JSON/Stage JSON Creator")]
    public static void ShowWindow()
    {
        GetWindow<StageJsonCreator>("Stage Creator");
    }

    [SerializeField] private Vector2 scrollPos = Vector2.zero;
    [SerializeField] bool boolBar = true;

    private void OnGUI()
    {
        GUILayout.Label("Stage 목록", EditorStyles.boldLabel);

        int levelCount = Mathf.Max(0, EditorGUILayout.IntField("Levels Count", stages.Count));

        while (levelCount > stages.Count)
            stages.Add(new StageData());
        while (levelCount < stages.Count)
            stages.RemoveAt(stages.Count - 1);

        GUILayout.Space(8);

        boolBar = EditorGUILayout.Foldout(boolBar, "Stage 리스트");
        if (boolBar)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            for (int i = 0; i < stages.Count; i++)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label($"Stage {i + 1}", EditorStyles.boldLabel);

                stages[i].id = EditorGUILayout.TextField("스테이지 ID", stages[i].id);
                stages[i].stageName = EditorGUILayout.TextField("스테이지 이름", stages[i].stageName);
                stages[i].spritePath = EditorGUILayout.TextField("이미지 경로", stages[i].spritePath);
                stages[i].descript = EditorGUILayout.TextField("스테이지 설명", stages[i].descript);

                GUILayout.Space(5);

                if (GUILayout.Button("Remove This Stage"))
                {
                    stages.RemoveAt(i);
                    break;
                }

                GUILayout.EndVertical();
                GUILayout.Space(4);
            }

            EditorGUILayout.EndScrollView();
        }

        if (GUILayout.Button("Add new Stage"))
        {
            stages.Add(new StageData());
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Json 파일 생성"))
        {
            CreateStageJson();
        }
    }

    private void CreateStageJson()
    {
        StageDatabaseDTO data = new StageDatabaseDTO
        {
            stages = stages
        };

        string json = JsonUtility.ToJson(data, true);

        string folder = "Assets/Resources/Json/Stage";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = $"{folder}/StageDatabase.json";
        File.WriteAllText(filePath, json);

        AssetDatabase.Refresh();
        Debug.Log($"생성 완료: {filePath}");
    }
}
