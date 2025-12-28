using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 재화 및 보유 품목 저장 데이터
[System.Serializable]
public class SaveData
{
    public int gold;
    public int gem;
    public int stamina;
    public int maxStamina;
    public int accountLevel;

    public long lastStaminaTime;

    public List<EquipSaveData> equips = new();
    public List<StuffSaveData> stuffs = new();

    public string lastStageId;
    public string lastEvolveId;
    public string lastSpecialEvolveId;
    public int evolveUnlockSlotCount;
}

// 보유 소지품 저장 데이터
[System.Serializable]
public class StuffSaveData
{
    public string stuffID;
    public int amount;
}
// 보유 장비 저장 데이터
[System.Serializable]
public class EquipSaveData
{
    public string equipID;
    public Grade grade;
    public int level;
}
