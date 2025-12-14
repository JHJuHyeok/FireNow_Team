using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public PlayerInfoSO playerInfo;
    public StuffDatabase stuffDatabase;

    // 특정 운영체제에서 사용 가능한 로컬 경로 + save.json
    string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    /// <summary>
    /// 세이브  함수
    /// </summary>
    public void Save()
    {
        // 세이브 데이터에 현재 재화 저장
        SaveData data = new SaveData
        {
            gold = playerInfo.gold,
            gem = playerInfo.gem,
            stamina = playerInfo.stamina,
            maxStamina = playerInfo.maxStamina
        };
        // 세이브 데이터에 현재 소지품 아이디, 갯수 저장
        foreach (var stack in playerInfo.stuffs)
        {
            data.stuffs.Add(new StuffSaveData
            {
                stuffID = stack.stuff.id,
                amount = stack.amount
            });
        }

        // 세이브 데이터 Json 변환
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        // 저장 확인용 로그
        Debug.Log("Game Saved");
    }
    /// <summary>
    /// 로드 함수
    /// </summary>
    public void Load()
    {
        // 세이브 경로가 없으면 return
        if (!File.Exists(SavePath))
        {
            Debug.Log("No Save File");
            return;
        }

        // 저장 경로에서 세이브 데이터 불러오기
        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        playerInfo.gold = data.gold;
        playerInfo.gem = data.gem;
        playerInfo.maxStamina = data.maxStamina;

        // 스태미나 회복
        RestoreStamina(data);

        // 소지품 복구 전 정리
        playerInfo.stuffs.Clear();

        // 저장했던 소지품
        foreach(var stuff in data.stuffs)
        {
            StuffDataRuntime stuffData = stuffDatabase.GetStuff(stuff.stuffID);
            if (stuffData == null) continue;

            playerInfo.stuffs.Add(new StuffStack
            {
                stuff = stuffData,
                amount = stuff.amount
            });
        }


        // 게임 로드 확인용 로그
        Debug.Log("Game Loaded");
    }

    /// <summary>
    /// 스태미나 회복 함수
    /// </summary>
    /// <param name="data">
    /// 게임 종료 시간과 스태미나 관련 정보를 포함한 세이브 데이터
    /// </param>
    private void RestoreStamina(SaveData data)
    {
        DateTime lastTime = new DateTime(data.lastStaminaTime);
        TimeSpan passed = DateTime.UtcNow - lastTime;

        int recoverd = (int)(passed.TotalMinutes / 5);
        playerInfo.stamina = Mathf.Min(playerInfo.maxStamina, data.stamina + recoverd);
    }
}
