using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public string id;               // 스테이지 ID
    public string stageName;        // 스테이지 명칭
    public string spriteName;       // 아틀라스에서 불러올 스프라이트 이름
    public string descript;         // 스테이지 설명
}

[System.Serializable]
public class StageDatabaseDTO
{
    public List<StageData> stages;
}
