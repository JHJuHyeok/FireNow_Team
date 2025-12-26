using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolveDataRuntime
{
    public string id;
    public string evolveName;
    public EvolveNodeType nodeType;
    public string gainStat;
    public Sprite activeSprite;
    public Sprite deactiveSprite;
    public string descript;

    /// <summary>
    /// Json 데이터를 받아 런타임 진화 데이터레 대입시키는 함수
    /// </summary>
    /// <param name="data"> 진화 Json 데이터 </param>
    public EvolveDataRuntime(EvolveData data)
    {
        id = data.id;
        evolveName = data.evolveName;
        nodeType = data.nodeType;
        gainStat = data.gainStat;
        activeSprite = AtlasManager.GetSprite("Evolution_Tap_Atlas", data.activeSpriteName);
        deactiveSprite = AtlasManager.GetSprite("Evolution_Tap_Atlas", data.deactiveSpriteName);
        descript = data.descript;
    }
}
