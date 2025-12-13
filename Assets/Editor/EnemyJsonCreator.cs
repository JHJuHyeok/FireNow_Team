using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EnemyJsonCreator : EditorWindow
{
    string id;
    string enemyName;
    int hp;
    float speed;
    float damage;
    string dropItem;

    string idleAnimation;
    string deadAnimation;

    MoveType moveType = MoveType.chase;

    [MenuItem("Tools/Enemy JSON Creator")]
    public static void ShowWindow()
    {
        GetWindow<EnemyJsonCreator>("Enemy Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Enemy JSON 기본 정보", EditorStyles.boldLabel);

        id = EditorGUILayout.TextField("몬스터 ID", id);
        enemyName = EditorGUILayout.TextField("몬스터 이름", enemyName);
        hp = EditorGUILayout.IntField("몬스터 체력", hp);
        speed = EditorGUILayout.FloatField("몬스터 속도", speed);
        damage = EditorGUILayout.FloatField("몬스터 공격력", damage);
        dropItem = EditorGUILayout.TextField("드랍 아이템 ID", dropItem);

        GUILayout.Space(10);
        GUILayout.Label("Enemy 애니메이션 경로", EditorStyles.boldLabel);

        idleAnimation = EditorGUILayout.TextField("통상 애니메이션", idleAnimation);
        deadAnimation = EditorGUILayout.TextField("사망 애니메이션", deadAnimation);

        GUILayout.Space(10);
        GUILayout.Label("Enemy 이동 방식", EditorStyles.boldLabel);

        moveType = (MoveType)EditorGUILayout.EnumPopup("이동 방식", moveType);

        GUILayout.Space(15);

        if (GUILayout.Button("Json 파일 생성"))
        {
            CreateEnemyJson();
        }
    }

    private void CreateEnemyJson()
    {
        EnemyData data = new EnemyData
        {
            id = id,
            enemyName = enemyName,
            hp = hp,
            speed = speed,
            damage = damage,
            dropItem = dropItem,
            idleAnimation = idleAnimation,
            deadAnimation = deadAnimation,
            moveType = moveType
        };

        string json = JsonUtility.ToJson(data, true);

        string folder = "Assets/Resources/Json/Enemy";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = $"{folder}/{id}.json";
        File.WriteAllText(filePath, json);

        AssetDatabase.Refresh();
        Debug.Log($"생성 완료: {filePath}");
    }
}
