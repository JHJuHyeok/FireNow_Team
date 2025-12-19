using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    private EquipData equip;

    private void Start()
    {
        equip = EquipLoader.Load();

        Debug.Log($"장비 이름 : {equip.equipName}");
        Debug.Log($"장비 설명 : {equip.descript}");
        Debug.Log($"아이콘 경로 : {equip.spriteName}");
        Debug.Log($"장착 부위 : {equip.part}");

        Debug.Log($"노말 등급의 최대 레벨 : {equip.equipGrades[0].maxLevel}");
    }
}
