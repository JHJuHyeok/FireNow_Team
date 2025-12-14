using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//JSON 파일을 읽어서 EquipData로 변환 하는 로더
//게임로직 분리- 지금 주혁님 로더 어떻게 하실지 아직 몰라서 테스트용으로 따로 작성
public class TEST_EquipLoader
{
    private const string BASE_PATH = "Json/";

    public static EquipData Load(string fileName)
    {
        TextAsset json = Resources.Load<TextAsset>(BASE_PATH + fileName);

        if (json == null)
        {
            Debug.LogError("EquipData JSON not found : " + fileName);
            return null;
        }

        return JsonUtility.FromJson<EquipData>(json.text);
    }
}
