using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public PlayerInfoSO playerInfo;

    // 정상 세이브 경로
    public string savePath => Path.Combine(Application.persistentDataPath, "save.json");
    // 저장 도중 임시 파일 경로
    public string tempPath => Path.Combine(Application.persistentDataPath, "save_tmp.json");
    // 세이브 백업 파일 경로
    public string backUpPath => Path.Combine(Application.persistentDataPath, "save_backUp.json");

    //씬마다 다른 세이브 매니저 인스턴스를 가질 수도 있어서 DDL로 만들고, 로딩씬에서 불러오겠습니다.-윤성원 12/28 17:24
    private static SaveManager instance;

    private void Awake()
    {
        //이미 인스턴스 있을수도 있으니까 씬 중복 배치 방지
        if (instance != null && instance != this)
        {
            //중복된건 제거
            Destroy(gameObject);
            return;
        }
        //해당 오브젝트를 단일 인스턴스로 등록
        instance = this;
        //DDL 심기-단일 인스턴스 메뉴씬->전투씬 이어서 사용할 수 있게
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 세이브  함수
    /// </summary>
    public void Save()
    {
        // 세이브 데이터에 현재 정보 저장
        SaveData data = new SaveData
        {
            gold = playerInfo.gold,
            gem = playerInfo.gem,
            stamina = playerInfo.stamina,
            maxStamina = playerInfo.maxStamina,
            accountLevel = playerInfo.accountLevel,

            lastStageId = playerInfo.lastStageId,
            lastEvolveId = playerInfo.lastEvolveId,
            lastSpecialEvolveId = playerInfo.lastSpecialEvolveId,
            
            lastStaminaTime = playerInfo.lastStaminaTime,
            evolveUnlockSlotCount = playerInfo.evolveUnlockSlotCount // 임시-윤성원
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

        // 세이브 데이터 Json 변환
        string json = JsonUtility.ToJson(data, true);

        try
        {
            // 임시로 저장
            File.WriteAllText(tempPath, json);

            // 기존 세이브를 백업 파일에 백업
            if (File.Exists(savePath))
                File.Copy(savePath, backUpPath, true);

            // 임시 파일 내용을 세이브 파일에 덮어쓰기
            File.Copy(tempPath, savePath, true);

            // 임시 파일 제거
            File.Delete(tempPath);
        }
        catch
        {
            // 저장에 실패할 경우 다음 로드 때 표시할 것
            Debug.LogError("Save Failed");
        }
        // 저장 확인용 로그
        Debug.Log($"Game Saved : {savePath}");
    }
    /// <summary>
    /// 로드 함수
    /// </summary>
    public void Load()
    {
        SaveData data = null;

        // 세이브 파일 있으면 정상 데이터, 백업만 있으면 백업 데이터, 없으면 새 게임 로드
        if (File.Exists(savePath))
            data = LoadData(savePath);
        else if (data == null && File.Exists(backUpPath))
            data = LoadData(savePath);
        else if (data == null)
        {
            InitNewGame();
            return;
        }

        GetSaveData(data);
    }

    private SaveData LoadData(string path)
    {
        try
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch
        {
            return null;
        }
    }

    private void GetSaveData(SaveData data)
    {
        playerInfo.gold = data.gold;
        playerInfo.gem = data.gem;
        playerInfo.maxStamina = data.maxStamina;
        playerInfo.accountLevel = data.accountLevel;

        playerInfo.lastStageId = data.lastStageId;
        playerInfo.lastEvolveId = data.lastEvolveId;
        playerInfo.lastSpecialEvolveId = data.lastSpecialEvolveId;
        playerInfo.evolveUnlockSlotCount = data.evolveUnlockSlotCount; // 임시->윤성원

        // 스태미나 회복
        RestoreStamina(data);

        // 복구 전 정리
        playerInfo.equips.Clear();
        playerInfo.stuffs.Clear();

        // 저장했던 장비 목록 불러오기
        foreach (var equip in data.equips)
        {
            // 데이터베이스에서 ID로 데이터 추출
            EquipData equipData = EquipDatabase.GetEquip(equip.equipID);
            // 장비 데이터 없으면 넘기기(방어 코드)
            if (equipData == null) continue;
            // 해당 데이터로 런타임 데이터 생성
            EquipDataRuntime equipRuntime = new EquipDataRuntime(equipData);

            playerInfo.equips.Add(new EquipInfo
            {
                equip = equipRuntime,
                grade = equip.grade,
                level = equip.level
            });
        }
        // 저장했던 소지품 불러오기
        foreach (var stuff in data.stuffs)
        {
            // 데이터베이스에서 ID로 데이터 추출
            StuffData stuffData = StuffDatabase.GetStuff(stuff.stuffID);
            // 소지품 데이터 없으면 넘기기
            if (stuffData == null) continue;
            // 해당 데이터 토대로 런타임 데이터 생성
            StuffDataRuntime stuffRuntime = new StuffDataRuntime(stuffData);

            // 런타임 소지품 리스트에 추가
            playerInfo.stuffs.Add(new StuffStack
            {
                stuff = stuffRuntime,
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

    /// <summary>
    /// 새 게임 시작 시 데이터 초기화
    /// </summary>
    private void InitNewGame()
    {
        playerInfo.gold = 0;
        playerInfo.gem = 0;
        playerInfo.maxStamina = 60;
        playerInfo.stamina = playerInfo.maxStamina;
        playerInfo.accountLevel = 1;

        playerInfo.equips.Clear();
        playerInfo.stuffs.Clear();

        /* 스테이지, 진화 목록 초기화
        playerInfo.lastStageId = "";
        playerInfo.lastEvolveId = "";
        playerInfo.lastSpecialEvolveId = "";
        */
    }
}
