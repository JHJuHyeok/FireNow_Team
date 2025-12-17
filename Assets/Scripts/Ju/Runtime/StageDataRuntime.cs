using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDataRuntime : MonoBehaviour
{
    public string id;
    public string stageName;
    public Sprite sprite;
    public string descript;

    /// <summary>
    /// 데이터를 입력받아 런타임 스테이지 데이터로 치환하는 함수
    /// </summary>
    /// <param name="data"> 스테이지 JSON 데이터</param>
    public StageDataRuntime(StageData data)
    {
        id = data.id;
        stageName = data.stageName;
        sprite = Resources.Load<Sprite>(data.spritePath);
        descript = data.descript;
    }
}
