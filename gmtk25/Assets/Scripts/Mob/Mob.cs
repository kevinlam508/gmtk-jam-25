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
        public GameObject[] Instances;
        public int Count;
    }

    [Tooltip("True if you want to auto-initalize the enemy on awake (AKA the enemy was not spawned by SpawnerCore")]
    [SerializeField] bool InitializeOnAwake = false;
    [SerializeField] private Health _health;
    [SerializeField] private MobMovement _movement;
    [SerializeField] MobScriptableObject MobStats = null;
    [SerializeField] Transform artParentTransform;
    private MobTypesNonFlag mobTypeNoFlag; 

    private FxAttachPoint[] _attachPoints;
    private float _currentArmor;

    private List<StatusEffectInstance> _statusEffects = new List<StatusEffectInstance>();
    private Dictionary<Type, StatusFx> _statusFxes = new Dictionary<Type, StatusFx>();

  

    private void Awake()
    {
        if (InitializeOnAwake)
        {
            if (MobStats != null)
            {
                InitializeEnemy(MobStats, MobStats.MobTypeNonFlag);
            }
        }
    }

    /// <summary>
    /// Sets up the Default Enemy prefab with the stats that are included on a MobScriptableObject
    /// </summary>
    /// <param name="mobStats"></param>
    public void InitializeEnemy(MobScriptableObject mobStats, MobTypesNonFlag mobTypeNonFlag)
    {
        MobStats = mobStats;
        GetComponent<MobMovement>().SetMobMovementSpeed(MobStats.MobSpeed);
        GetComponent<MobMovement>().OnMovementFinish += Mob_OnMovementFinish;
        _health.SetMaxHealth(MobStats.MobHealth);
        GameObject modelInstance = Instantiate(MobStats.MobModel, artParentTransform.position, Quaternion.identity, artParentTransform);
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        collider.center = mobStats.ColliderCenter;
        collider.radius = MobStats.ColliderRadius;
        collider.height = MobStats.ColliderHeight;
        this.mobTypeNoFlag = mobTypeNonFlag;

        HitGlow glow = modelInstance.GetComponentInChildren<HitGlow>();
        if (glow != null)
        {
            _health.DamageTaken.AddListener((_, __) => glow.Hit());
        }
    }

    /// <summary>
    /// When the enemy finishes it's movement, tell the health system that we have completed our movement so we can despawn the enemy, also handle any effects that should be
    /// Done to player here
    /// </summary>
    private void Mob_OnMovementFinish()
    {
        //damage player?
        _health.EnemyFinish();
    }

    private void Start()
    {
        _attachPoints = GetComponentsInChildren<FxAttachPoint>();
    }

    private void Update()
    {
        if (MobStats == null)
        {
            Debug.LogWarning("WARNING NO STATS ATTACHED TO ENEMY; EITHER SPAWN THIS ENEMY FROM SPAWNER CORE OR MARK InitializeOnAwake = true and assign MobStats in the Inspector");
        }
 
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
            GameObject[] fxInstances = new GameObject[effect.FxData.Length];
            for (int i = 0; i < fxInstances.Length; i++)
            {
                var fxData = effect.FxData[i];
                GameObject fx = fxData.Prefab;
                if (fx == null)
                {
                    return;
                }

                float scale = 1;
                foreach (var settings in fxData.TypeSize)
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
                    if (point.LocationType == fxData.Location)
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
                float parentScale = attachPoint.transform.lossyScale.x;
                fxInstance.transform.localScale = Vector3.one * scale / parentScale;
                fxInstances[i] = fxInstance;
            }

            fxInfo = new StatusFx 
            { 
                Instances = fxInstances
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
            foreach (GameObject go in fxInfo.Instances)
            {
                Destroy(go);
            }
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


    private void OnDisable()
    {
        GetComponent<MobMovement>().OnMovementFinish -= Mob_OnMovementFinish;
    }
}
