using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EquipLoader
{
    public static EquipData Load()
    {
        TextAsset json = Resources.Load<TextAsset>("Json/Weapon_01");
        return JsonUtility.FromJson<EquipData>(json.text);
    }
}
