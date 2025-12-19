using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer
{
    public static void InitializeAllData()
    {
        AbilityDatabase.Initialize();
        EnemyDatabase.Initialize();
        EquipDatabase.Initialize();
        EvolveDatabase.Initialize();
        ItemDatabase.Initialize();
        StageDatabase.Initialize();
        StuffDatabase.Initialize();
        WaveDatabase.Initialize();
    }
}
