using UnityEngine;
using System;

public abstract class BaseStatusEffect : ScriptableObject
{

    [SerializeField] private float _duration;
    [SerializeField] private FxPlacementData[] _fxData;

    public float Duration => _duration;
    public FxPlacementData[] FxData => _fxData;

    public virtual object NewDataStore() => null;

    public virtual void OnApplied(Mob mob, float resistance, object dataStore) { }
    public virtual void OnRemoved(Mob mob, float resistance, object dataStore) { }
    public virtual void Tick(Mob mob, float resistance, object dataStore) { }

}

[Serializable]
public class FxPlacementData
{
    [Serializable]
    public class TypeSettings
    {
        public MobTypes Type;
        public float Scale;
    }

    public GameObject Prefab;
    public TypeSettings[] TypeSize;
    public FxAttachPoint.Location Location;
}
