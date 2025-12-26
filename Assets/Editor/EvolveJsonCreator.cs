using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EvolveJsonCreator : EditorWindow
{
    List<EvolveData> evolves = new List<EvolveData>();
    List<EvolveLevelConfig> levelConfigs = new List<EvolveLevelConfig>();

    [MenuItem("Tools/JSON/Evolve JSON Creator")]
    public static void ShowWindow()
    {
        GetWindow<EvolveJsonCreator>("Evolve Creator");
    }

    [SerializeField] private Vector2 scrollPos = Vector2.zero;
    [SerializeField] private Vector2 levelScrollPos = Vector2.zero;
    [SerializeField] bool evolveBar = false;
    [SerializeField] bool levelBar = false;

    private void OnGUI()
    {
        GUILayout.Label("Evolve JSON", EditorStyles.boldLabel);
        GUILayout.Space(10);

        int nodeCount = Mathf.Max(0, EditorGUILayout.IntField("Nodes Count", evolves.Count));

        while (nodeCount > evolves.Count)
            evolves.Add(new EvolveData());
        while (nodeCount < evolves.Count)
            evolves.RemoveAt(evolves.Count - 1);

        evolveBar = EditorGUILayout.Foldout(evolveBar, "진화 노드 목록");

        // 노드 목록 박스
        if(evolveBar)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            for(int i = 0; i < evolves.Count; i++)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label($"{i + 1}번째 노드", EditorStyles.boldLabel);

                evolves[i].id = EditorGUILayout.TextField("노드 ID", evolves[i].id);
                evolves[i].evolveName = EditorGUILayout.TextField("노드 이름", evolves[i].evolveName);
                evolves[i].nodeType = (EvolveNodeType)EditorGUILayout.EnumPopup("노드 타입", evolves[i].nodeType);
                evolves[i].gainStat = EditorGUILayout.TextField("상승 능력치", evolves[i].gainStat);
                evolves[i].activeSpriteName = EditorGUILayout.TextField("스프라이트 명칭", evolves[i].activeSpriteName);
                evolves[i].deactiveSpriteName = EditorGUILayout.TextField("비활성 스프라이트", evolves[i].deactiveSpriteName);
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

        GUILayout.Space(20);
        GUILayout.Label("Evolve Configs", EditorStyles.boldLabel);
        GUILayout.Space(10);

        int levelCount = Mathf.Max(0, EditorGUILayout.IntField("Levels Count", levelConfigs.Count));

        while (levelCount > levelConfigs.Count)
            levelConfigs.Add(new EvolveLevelConfig { configs = new List<EvolveConfig>() });
        while (levelCount < levelConfigs.Count)
            levelConfigs.RemoveAt(levelConfigs.Count - 1);

        levelBar = EditorGUILayout.Foldout(levelBar, "각 레벨별 노드");

        if(levelBar)
        {
            levelScrollPos = EditorGUILayout.BeginScrollView(levelScrollPos);

            for (int i = 0; i < levelConfigs.Count; i++)
            {
                GUILayout.BeginVertical("Box");

                DrawLevelConfig(levelConfigs[i], i);

                GUILayout.EndVertical();
                GUILayout.Space(4);
            }
            EditorGUILayout.EndScrollView();
        }

        GUILayout.Space(15);

        if (GUILayout.Button("레벨 노드 목록 생성"))
        {
            CreateLevelConfigs();
        }
    }

    private void DrawLevelConfig(EvolveLevelConfig levelConfig, int index)
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label($"Level {index + 1}", EditorStyles.boldLabel);

        levelConfig.level = EditorGUILayout.IntField("Level", levelConfig.level);
        levelConfig.cost = EditorGUILayout.IntField("Cost", levelConfig.cost);

        GUILayout.Space(5);

        // 내부 리스트
        DrawEvolveConfigs(levelConfig.configs);

        GUILayout.EndVertical();
        GUILayout.Space(6);
    }

    private void DrawEvolveConfigs(List<EvolveConfig> configs)
    {
        GUILayout.Label("Evolve Nodes", EditorStyles.miniBoldLabel);

        int newCount = Mathf.Max(0,
            EditorGUILayout.IntField("Node Count", configs.Count));

        while (newCount > configs.Count)
            configs.Add(new EvolveConfig());

        while (newCount < configs.Count)
            configs.RemoveAt(configs.Count - 1);

        for (int i = 0; i < configs.Count; i++)
        {
            GUILayout.BeginVertical("Box");

            configs[i].evolveId =
                EditorGUILayout.TextField("Evolve ID", configs[i].evolveId);

            configs[i].value =
                EditorGUILayout.IntField("Value", configs[i].value);

            GUILayout.EndVertical();
        }
    }

    private void CreateEvolveJson()
    {
        EvolveDatabaseDTO database = new()
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

    private void CreateLevelConfigs()
    {
        EvolveLevelConfigs levels = new()
        {
            levels = levelConfigs
        };

        string json = JsonUtility.ToJson(levels, true);

        string folder = "Assets/Resources/Json/Evolve";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = $"{folder}/evolveLevelConfigs.json";
        File.WriteAllText(filePath, json);

        AssetDatabase.Refresh();
        Debug.Log($"생성 완료: {filePath}");
    }
}
