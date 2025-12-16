using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public PlayerInfoSO playerInfo;

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
        // 세이브 데이터에 보유 장비 아이디, 등급, 레벨 저장
        foreach (var info in playerInfo.equips)
        {
            data.equips.Add(new EquipSaveData
            {
                equipID = info.equip.id,
                grade = info.grade,
                level = info.level
            });
        }
        // 세이브 데이터에 보유 소지품 아이디, 갯수 저장
        foreach (var stack in playerInfo.stuffs)
        {
            data.stuffs.Add(new StuffSaveData
            {
                stuffID = stack.stuff.id,
                amount = stack.amount
            });
        }
        // 세이브 데이터에 현재 스테이지 클리어 여부 저장
        foreach (var clear in playerInfo.stages)
        {
            data.stages.Add(new StageSaveData
            {
                stageID = clear.stage.id,
                isClear = clear.isClear
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
            InitNewGame();
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

        // 복구 전 정리
        playerInfo.equips.Clear();
        playerInfo.stuffs.Clear();
        playerInfo.stages.Clear();

        // 저장했던 장비 목록 불러오기
        foreach(var equip in data.equips)
        {
            // 데이터베이스에서 ID로 데이터 추출
            EquipData equipData = EquipDatabase.GetEquip(equip.equipID);
            // 해당 데이터로 런타임 데이터 생성
            EquipDataRuntime equipRuntime = new EquipDataRuntime(equipData);
            // 장비 데이터 없으면 넘기기(방어 코드)
            if (equipData == null) continue;

            playerInfo.equips.Add(new EquipInfo
            {
                equip = equipRuntime,
                grade = equip.grade,
                level = equip.level
            });
        }
        // 저장했던 소지품 불러오기
        foreach(var stuff in data.stuffs)
        {
            // 데이터베이스에서 ID로 데이터 추출
            StuffData stuffData = StuffDatabase.GetStuff(stuff.stuffID);
            // 해당 데이터 토대로 런타임 데이터 생성
            StuffDataRuntime stuffRuntime = new StuffDataRuntime(stuffData);
            // 소지품 데이터 없으면 넘기기
            if (stuffData == null) continue;

            // 런타임 소지품 리스트에 추가
            playerInfo.stuffs.Add(new StuffStack
            {
                stuff = stuffRuntime,
                amount = stuff.amount
            });
        }
        // 저장했던 스테이지 불러오기
        foreach(var stage in data.stages)
        {
            // 데이터베이스에서 ID로 데이터 추출
            StageData stageData = StageDatabase.GetStage(stage.stageID);
            if (stageData == null) continue;
            // 데이터 토대로 런타임 데이터 생성
            StageDataRuntime stageRuntime = new StageDataRuntime(stageData);

            // 스테이지 목록에 추가
            playerInfo.stages.Add(new StageClear
            {
                stage = stageRuntime,
                isClear = stage.isClear
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

    /// <summary>
    /// 새 게임 시작 시 데이터 초기화
    /// </summary>
    private void InitNewGame()
    {
        playerInfo.gold = 0;
        playerInfo.gem = 0;
        playerInfo.maxStamina = 60;
        playerInfo.stamina = playerInfo.maxStamina;

        playerInfo.equips.Clear();
        playerInfo.stuffs.Clear();

        // 첫 스테이지만 해금된 스테이지 데이터
        foreach (var stage in StageDatabase.stageDict)
        {
            // 스테이지 데이터베이스의 값 불러오기
            StageData stageData = stage.Value;
            if (stageData == null) continue;
            // 데이터 토대로 런타임 데이터 생성
            StageDataRuntime stageRuntime = new StageDataRuntime(stageData);

            if (stage.Key == "Stage_01")
            {
                playerInfo.stages.Add(new StageClear
                {
                    stage = stageRuntime,
                    isClear = true
                });
            }
            else
            {
                playerInfo.stages.Add(new StageClear
                {
                    stage = stageRuntime,
                    isClear = false
                });
            }
        }
    }
}
