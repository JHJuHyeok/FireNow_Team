using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public string id;
    public string stageName;
    public string spritePath;
    public string descript;
}

[System.Serializable]
public class StageDatabase
{
    public List<StageData> stages;
}
