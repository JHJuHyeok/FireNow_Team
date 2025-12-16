using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTestor : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    private PlayerInfoSO playerInfo;

    private void Awake()
    {
        // 데이터베이스 불러오기
        EquipDatabase.Initialize();
        StuffDatabase.Initialize();
        StageDatabase.Initialize();

        // 저장된 데이터 불러오기
        saveManager.Load();

        playerInfo = saveManager.playerInfo;
    }

    public void CheckPlayerInfo()
    {
        Debug.Log($"{playerInfo.gold}");
        Debug.Log($"{playerInfo.gem}");
        Debug.Log($"{playerInfo.stamina}");
        Debug.Log($"{playerInfo.maxStamina}");

        Debug.Log($"{playerInfo.equips[0].equip.id}");
    }

    public void ChangePlayerInfo()
    {
        playerInfo.gold = 1000;
        playerInfo.gem = 3000;
        playerInfo.stamina = 50;

        TextAsset json = Resources.Load<TextAsset>("Json/Weapon_01");
        EquipData data = JsonUtility.FromJson<EquipData>(json.text);

        playerInfo.equips.Add(new EquipInfo
        {
            equip = new EquipDataRuntime(data),
            level = 1,
            grade = Grade.normal
        });

        saveManager.Save();
    }
}
