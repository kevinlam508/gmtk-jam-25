using UnityEngine;

[CreateAssetMenu(fileName = "MobScriptableObject", menuName = "Scriptable Objects/MobScriptableObject")]
public class MobScriptableObject : ScriptableObject
{
    public MobTypes MobType;
    public MobTypesNonFlag MobTypeNonFlag;
    public int MobHealth = 2;
    public float MobSpeed = 5;
    public int MobAttackDamage = 1;
    [Range(0, 1)] public float MobCCResist = 0;
    public float MobArmorGainPerHit = 0;

    public GameObject MobModel;
    public Vector3 ColliderCenter;
    public float ColliderRadius;
    public float ColliderHeight;

    public AudioClip onHitSFX;
    public AudioClip onDeathSFX;

    

}


[System.Flags]
public enum MobTypes
{
    None,
    Pawn                = 1 << 1,
    Tank                = 1 << 2,
    Speedster           = 1 << 3,
    Swarmer             = 1 << 4,
    CrowdControlResist  = 1 << 5,
    Splitter            = 1 << 6,
    ArmorWhenHit        = 1 << 7,
    Count
}
