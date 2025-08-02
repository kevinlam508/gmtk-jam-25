using UnityEngine;

[CreateAssetMenu(fileName = "MobScriptableObject", menuName = "Scriptable Objects/MobScriptableObject")]
public class MobScriptableObject : ScriptableObject
{
    public MobTypes MobType;
    public int MobHealth = 2;
    public float MobSpeed = 5;
    public int MobAttackDamage = 1;
    [Range(0, 1)] public float MobCCResist = 0;
    public float MobArmorGainPerHit = 0;

    GameObject MobModel;

    

}


[System.Flags]
public enum MobTypes
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
