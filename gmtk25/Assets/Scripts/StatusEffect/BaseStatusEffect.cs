using UnityEngine;

public abstract class BaseStatusEffect : ScriptableObject
{
    [SerializeField] private float _duration;

    public float Duration => _duration;

    public virtual object NewDataStore() => null;

    public virtual void OnApplied(Mob mob, float resistance, object dataStore) { }
    public virtual void OnRemoved(Mob mob, float resistance, object dataStore) { }
    public virtual void Tick(Mob mob, float resistance, object dataStore) { }

}
