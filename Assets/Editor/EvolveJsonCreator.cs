using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EvolveJsonCreator : EditorWindow
{
    List<EvolveData> evolves = new List<EvolveData>();

    [MenuItem("Tools/JSON/Evolve JSON Creator")]
    public static void ShowWindow()
    {
        GetWindow<EvolveJsonCreator>("Evolve Creator");
    }

    [SerializeField] private Vector2 scrollPos = Vector2.zero;
    [SerializeField] bool boolBar = true;

    private void OnGUI()
    {
        GUILayout.Label("Evolve JSON", EditorStyles.boldLabel);
        GUILayout.Space(10);

        int levelCount = Mathf.Max(0, EditorGUILayout.IntField("Levels Count", evolves.Count));

        while (levelCount > evolves.Count)
            evolves.Add(new EvolveData());
        while (levelCount < evolves.Count)
            evolves.RemoveAt(evolves.Count - 1);

        boolBar = EditorGUILayout.Foldout(boolBar, "진화 노드 목록");

        // 노드 목록 박스
        if(boolBar)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            for(int i = 0; i < evolves.Count; i++)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label($"{i + 1}번째 노드", EditorStyles.boldLabel);

                evolves[i].id = EditorGUILayout.TextField("노드 ID", evolves[i].id);
                evolves[i].evolveName = EditorGUILayout.TextField("노드 이름", evolves[i].evolveName);
                evolves[i].iconPath = EditorGUILayout.TextField("아이콘 경로", evolves[i].iconPath);
                evolves[i].descript = EditorGUILayout.TextField("노드 설명", evolves[i].descript);

                GUILayout.Space(5);

                if (GUILayout.Button("Remove This Evolve"))
                {
                    evolves.RemoveAt(i);
                    break;
                }

                GUILayout.EndVertical();
                GUILayout.Space(4);
            }
            EditorGUILayout.EndScrollView();
        }

        if (GUILayout.Button("Add new Evolve"))
        {
            evolves.Add(new EvolveData());
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Json 파일 생성"))
        {
            CreateEvolveJson();
        }
    }

    private void CreateEvolveJson()
    {
        EvolveDatabase database = new()
        {
            evolves = evolves
        };

        string json = JsonUtility.ToJson(database, true);

        string folder = "Assets/Resources/Json/Evolve";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = $"{folder}/evolveDatabase.json";
        File.WriteAllText(filePath, json);

        AssetDatabase.Refresh();
        Debug.Log($"생성 완료: {filePath}");
    }
}
