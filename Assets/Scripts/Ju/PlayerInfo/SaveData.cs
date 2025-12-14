using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int gold;
    public int gem;
    public int stamina;
    public int maxStamina;

    public long lastStaminaTime;

    //public List<EquipSaveData> equips = new List<EquipSaveData>();
    //public List<StuffSaveData> stuffs = new List<StuffSaveData>();
    //public List<EvolveSaveData> evolves = new List<EvolveSaveData>();
}

[System.Serializable]
public class StuffSaveData
{
    public string stuffID;
    public int amount;
}
[System.Serializable]
public class EquipSaveData
{
    public string equipID;
}
[System.Serializable]
public class EvolveSaveData
{
    public string evolveID;
}
