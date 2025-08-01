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

    public void CollisionCallback(Collider other, TravelState travelStateData)
    {
        Mob mob = other.GetComponent<Mob>();
        if (mob == null)
        {
            return;
        }

        ApplyImpact(_impactType, mob, mob.transform.position, travelStateData);
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
                ApplyDamageAndStatusToMob(mob);
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

    private void ApplyDamageAndStatusToMob(Mob mob)
    {
        mob.TakeDamage(_damage);
        if (_onHitStatus != null)
        {
            foreach (BaseStatusEffect status in _onHitStatus)
            {
                mob.AddStatus(status);
            }
        }
    }
}
