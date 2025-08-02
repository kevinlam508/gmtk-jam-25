using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpawnRateScriptableObject", menuName = "Scriptable Objects/SpawnRateScriptableObject")]
public class SpawnRateScriptableObject : ScriptableObject
{    
    public List<SpawnRateListItem> SpawnRates = new List<SpawnRateListItem>();
}

[System.Serializable]
public class SpawnRateListItem
{
    [Range(0, 1.0f)]
    public float percentChanceToSpawn = 0.0f;
    public MobTypesNonFlag MobTypeToSpawn;
}

public enum MobTypesNonFlag
{
    Pawn,
    Tank,
    Speedster,
    Swarmer,
    CrowdControlResist,
    Splitter,
    ArmorWhenHit,
    Count
}
