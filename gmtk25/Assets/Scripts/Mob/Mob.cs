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

    private class StatusFx
    {
        public GameObject Instance;
        public int Count;
    }

    [SerializeField] private Health _health;
    [SerializeField] private MobMovement _movement;
    [SerializeField] MobScriptableObject MobStats;

    private FxAttachPoint[] _attachPoints;
    private float _currentArmor;

    private List<StatusEffectInstance> _statusEffects = new List<StatusEffectInstance>();
    private Dictionary<Type, StatusFx> _statusFxes = new Dictionary<Type, StatusFx>();

    private void Awake()
    {
        if (MobStats != null)
        {
            GetComponent<MobMovement>().SetMobMovementSpeed(MobStats.MobSpeed);
            //_health.SetMaxHealth(MobStats.MobHealth);

        }
    }

    private void Start()
    {
        _attachPoints = GetComponentsInChildren<FxAttachPoint>();
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

        SpawnStatusFx(effect);
    }

    private void SpawnStatusFx(BaseStatusEffect effect)
    {
        if (!_statusFxes.TryGetValue(effect.GetType(), out StatusFx fxInfo))
        {
            GameObject fx = effect.FxData.Prefab;
            if (fx == null)
            {
                return;
            }

            float scale = 1;
            foreach (var settings in effect.FxData.TypeSize)
            {
                if (settings.Type == MobStats.MobType)
                {
                    scale = settings.Scale;
                    break;
                }
            }

            FxAttachPoint attachPoint = null;
            foreach (FxAttachPoint point in _attachPoints)
            {
                if (point.LocationType == effect.FxData.Location)
                {
                    attachPoint = point;
                    break;
                }
            }

            if (attachPoint == null)
            {
                return;
            }

            GameObject fxInstance = Instantiate(fx, attachPoint.transform);
            fxInstance.transform.localScale = Vector3.one * scale;

            fxInfo = new StatusFx 
            { 
                Instance = fxInstance
            };
            _statusFxes.Add(effect.GetType(), fxInfo);
        }

        fxInfo.Count++;
    }

    private void RemoveStatusFx(BaseStatusEffect effect)
    {
        if (!_statusFxes.TryGetValue(effect.GetType(), out StatusFx fxInfo))
        {
            return;
        }

        fxInfo.Count--;

        if (fxInfo.Count == 0)
        {
            _statusFxes.Remove(effect.GetType());
            Destroy(fxInfo.Instance);
        }
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
                RemoveStatusFx(instance.Data);
            }
        }
    }
}
