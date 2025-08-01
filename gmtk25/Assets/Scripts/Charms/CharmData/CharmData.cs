using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharmData", menuName = "Scriptable Objects/Charms/CharmData")]
public class CharmData : ScriptableObject
{
    // Descending order is order impact types are applied when combined
    // e.g. chain + splash means chain happens first, then splash at the chain locations, then single hits on the splash hits
    [Flags]
    public enum ImpactTypes
    {
        Single = 0,
        Splash = 1 << 0,
        Chain = 1 << 1,
    }
    private static ImpactTypes MaxImpactType = ImpactTypes.Chain;


    public class TravelState
    {
        public int CollisionCount;
        public int HitCount;

        public CharmData FrontNeighbor;
        public CharmData BackNeighbor;
    }

    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private bool _canShove;

    [Header("Impact")]
    [SerializeField] private ImpactTypes _impactType;
    [SerializeField] private int _explosionCount;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private int _chainCount;
    [SerializeField] private float _chainRadius;

    [Header("Effects")]
    [SerializeField] private int _damage;
    [SerializeField] private BaseStatusEffect[] _onHitStatus;
    [SerializeField] private BaseReturnEffect[] _returnEffect;

    [Header("Appearance")]
    [SerializeField] private GameObject _prefab;

    public float Speed => _speed;
    public bool CanShove => _canShove;
    public BaseReturnEffect[] ReturnEffects => _returnEffect;

    public GameObject Prefab => _prefab;

    public TravelState NewTravelStateData() => new TravelState();

    private CharmData Clone()
    {
        CharmData data = new CharmData();
        data._explosionCount = _explosionCount;
        data._explosionRadius = _explosionRadius;
        data._chainCount = _chainCount;
        data._chainRadius = _chainRadius;

        data._damage = _damage;
        data._onHitStatus = new BaseStatusEffect[_onHitStatus.Length];
        Array.Copy(_onHitStatus, data._onHitStatus, _onHitStatus.Length);
        data._returnEffect = new BaseReturnEffect[_returnEffect.Length];
        Array.Copy(_returnEffect, data._returnEffect, _returnEffect.Length);

        return data;
    }

    public void CollisionCallback(Collider other, TravelState travelStateData)
    {
        Mob mob = other.GetComponent<Mob>();
        if (mob == null)
        {
            return;
        }

        CombineEffects(this, travelStateData.FrontNeighbor, travelStateData.BackNeighbor)
            .ApplyImpact(_impactType, mob, mob.transform.position, travelStateData);
        travelStateData.CollisionCount++;
    }

    private void ApplyImpact(ImpactTypes types, Mob mob, Vector3 location, TravelState travelStateData)
    {
        ImpactTypes effectToApply = MaxImpactType;
        while (effectToApply != ImpactTypes.Single && (effectToApply & types) == 0)
        {
            effectToApply = (ImpactTypes)(((int)effectToApply) >> 1);
        }

        ImpactTypes remainingEffects = types & (~effectToApply);
        switch (effectToApply)
        {
            case ImpactTypes.Single:
                ApplyDamageAndStatusToMob(mob, travelStateData);
                break;
            case ImpactTypes.Splash:
                ApplySplash(remainingEffects, mob, location, travelStateData);
                break;
            case ImpactTypes.Chain:
                ApplyChain(remainingEffects, mob, location, travelStateData);
                break;
        }
    }

    private void ApplyChain(ImpactTypes types, Mob mob, Vector3 location, TravelState travelStateData)
    {
        // Apply to first link of the chain
        ApplyImpact(types, mob, location, travelStateData);

        // Try to find chain targets
        HashSet<Mob> alreadyHit = new HashSet<Mob>();
        alreadyHit.Add(mob);
        for (int i = 0; i < _chainCount; i++)
        {
            Mob nextHit = null;
            RaycastHit[] hits = Physics.SphereCastAll(location, _chainRadius, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                Mob chained = hit.collider.GetComponent<Mob>();
                if (chained == null || alreadyHit.Contains(chained))
                {
                    continue;
                }

                nextHit = chained;
                break;
            }

            // Ran out of targets
            if (nextHit == null)
            {
                break;
            }

            Debug.DrawLine(location, nextHit.transform.position, Color.red, 100f);
            location = nextHit.transform.position;
            ApplyImpact(types, nextHit, location, travelStateData);
            alreadyHit.Add(nextHit);
        }
    }

    private void ApplySplash(ImpactTypes types, Mob mob, Vector3 location, TravelState travelStateData)
    {
        if (_explosionCount > 0 && travelStateData.CollisionCount >= _explosionCount)
        {
            return;
        }

        RaycastHit[] hits = Physics.SphereCastAll(location, _explosionRadius, Vector3.up, 0);
        foreach (RaycastHit hit in hits)
        {
            Mob splashed = hit.collider.GetComponent<Mob>();
            if (splashed == null)
            {
                continue;
            }

            ApplyImpact(types, splashed, splashed.transform.position, travelStateData);
        }
    }

    private void ApplyDamageAndStatusToMob(Mob mob, TravelState travelStateData)
    {
        mob.TakeDamage(_damage);
        if (_onHitStatus != null)
        {
            foreach (BaseStatusEffect status in _onHitStatus)
            {
                mob.AddStatus(status);
            }
        }

        travelStateData.HitCount++;
    }

    private static CharmData CombineEffects(CharmData main, CharmData sub1, CharmData sub2)
    {
        if (sub1 == null && sub2 == null)
        {
            return main;
        }

        CharmData result = main.Clone();
        result._impactType |= (sub1?._impactType ?? ImpactTypes.Single)
                        | (sub2?._impactType ?? ImpactTypes.Single);
        result._explosionCount += WeightedAdd(sub1?._explosionCount, sub2?._explosionCount, 1);
        result._explosionRadius += WeightedAdd(sub1?._explosionRadius, sub2?._explosionRadius, 1);
        result._chainCount += WeightedAdd(sub1?._chainCount, sub2?._chainCount, 1);
        result._chainRadius += WeightedAdd(sub1?._chainRadius, sub2?._chainRadius, 1);

        result._damage += WeightedAdd(sub1?._damage, sub2?._damage, 1);
        result._onHitStatus = CombineArrays(result._onHitStatus, sub1?._onHitStatus, sub2?._onHitStatus);
        //result._returnEffect = CombineArrays(result._returnEffect, sub1?._returnEffect, sub2?._returnEffect);
        return result;
    }

    private static int WeightedAdd(int? value1, int? value2, float weight)
    {
        return (int)Mathf.Ceil(((value1 ?? 0) + (value2 ?? 0)) * weight);
    }

    private static float WeightedAdd(float? value1, float? value2, float weight)
    {
        return ((value1 ?? 0) + (value2 ?? 0)) * weight;
    }

    public static T[] CombineArrays<T>(T[] main, T[] sub1, T[] sub2)
    {
        T[] result = new T[main.Length + (sub1?.Length ?? 0) + (sub2?.Length ?? 0)];
        Array.Copy(main, result, main.Length);

        int start = main.Length;
        if (sub1 != null)
        {
            Array.Copy(sub1, 0, result, start, sub1.Length);
            start += sub1.Length;
        }
        if (sub2 != null)
        {
            Array.Copy(sub2, 0, result, start, sub2.Length);
        }

        return result;
    }
}
