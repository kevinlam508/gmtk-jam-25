using UnityEngine;
using System;
using System.Collections.Generic;

public class Mob : MonoBehaviour
{
    [Serializable] // Allow debug inspector render
    private class StatusEffectInstance
    {
        public BaseStatusEffect Data;

        // Instance data
        public float TimePassed;
        public object DataStore;
    }

    [SerializeField] private Health _health;

    private List<StatusEffectInstance> _statusEffects = new List<StatusEffectInstance>();

    private void Update()
    {
        TickStatusEffects();
    }

    public void AddStatus(BaseStatusEffect effect)
    {
        var newInstance = new StatusEffectInstance
        {
            Data = effect,
            TimePassed = 0,
            DataStore = effect.NewDataStore()
        };
        _statusEffects.Add(newInstance);

        effect.OnApplied(this, newInstance.DataStore);
    }

    public void TakeDamage(int amount)
    {
        _health.TakeDamage(amount);
    }

    private void TickStatusEffects()
    {
        for (int i = _statusEffects.Count - 1; i >= 0; i--)
        {
            StatusEffectInstance instance = _statusEffects[i];
            instance.TimePassed += Time.deltaTime;
            instance.Data.Tick(this, instance.DataStore);

            if (instance.TimePassed > instance.Data.Duration)
            {
                instance.Data.OnRemoved(this, instance.DataStore);
                _statusEffects.RemoveAt(i);
            }
        }
    }
}
