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

    public long lastStaminaTime;

    //public List<EquipSaveData> equips = new List<EquipSaveData>();
    public List<StuffSaveData> stuffs = new List<StuffSaveData>();
    //public List<EvolveSaveData> evolves = new List<EvolveSaveData>();
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
}
// 해금 진화 저장 데이터
[System.Serializable]
public class EvolveSaveData
{
    public string evolveID;
}
