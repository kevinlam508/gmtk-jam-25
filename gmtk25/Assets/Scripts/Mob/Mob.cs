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
    [SerializeField] private MobMovement _movement;
    [SerializeField] MobScriptableObject MobStats;

    private float _currentArmor;

    private List<StatusEffectInstance> _statusEffects = new List<StatusEffectInstance>();

    private void Awake()
    {
        if (MobStats != null)
        {
            GetComponent<MobMovement>().SetMobMovementSpeed(MobStats.MobSpeed);
            //_health.SetMaxHealth(MobStats.MobHealth);

        }
    }

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

        effect.OnApplied(this, MobStats.MobCCResist, newInstance.DataStore);
    }

    public void TakeDamage(int amount)
    {
        // Reduced by armor, but min of 1
        amount -= Mathf.FloorToInt(MobStats.MobArmorGainPerHit);
        amount = Mathf.Max(amount, 1);
        _health.TakeDamage(amount);

        _currentArmor += MobStats.MobArmorGainPerHit;
    }

    public void ApplySpeedMultiplier(float multiplier) => _movement.ApplySpeedMultiplier(multiplier);

    private void TickStatusEffects()
    {
        for (int i = _statusEffects.Count - 1; i >= 0; i--)
        {
            StatusEffectInstance instance = _statusEffects[i];
            instance.TimePassed += Time.deltaTime;
            instance.Data.Tick(this, MobStats.MobCCResist, instance.DataStore);

            if (instance.TimePassed > instance.Data.Duration)
            {
                instance.Data.OnRemoved(this, MobStats.MobCCResist, instance.DataStore);
                _statusEffects.RemoveAt(i);
            }
        }
    }
}
