using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer
{
    public void Initializing()
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
