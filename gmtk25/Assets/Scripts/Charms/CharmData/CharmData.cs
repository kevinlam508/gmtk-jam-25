using UnityEngine;
using System;
using System.Collections;
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

    [SerializeField] private bool _isBead;

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
    [SerializeField] private AudioClip _hitSfx;
    [SerializeField] private AudioClip _chainSfx;
    [SerializeField] private AudioClip _splashSfx;

    [Header("Appearance")]
    [SerializeField] private GameObject _prefab;
    [SerializeField] private GameObject _hitFx;
    [SerializeField] private GameObject _splashFx;
    [SerializeField] private float _splashExpandDuration = .1f;
    [SerializeField] private GameObject _chainFx;
    [SerializeField] private float _chainJumpDelay = .1f;

    [Header("UI")]
    [SerializeField] private Sprite _uiSprite;
    [SerializeField] private string _tooltipName;
    [SerializeField] private string _tooltipDescription;

    public float Speed => _speed;
    public bool CanShove => _canShove;
    public BaseReturnEffect[] ReturnEffects => _returnEffect;

    public GameObject Prefab => _prefab;

    public Sprite UiSprite => _uiSprite;
    public bool IsBead => _isBead;

    public string TooltipName => _tooltipName;
    // [Multiline(20)]
    // [Space(10)]
    public string TooltipDescription => _tooltipDescription;

    public TravelState NewTravelStateData() => new TravelState();

    private CharmData Clone()
    {
        CharmData data = CreateInstance<CharmData>();
        data._explosionCount = _explosionCount;
        data._explosionRadius = _explosionRadius;
        data._chainCount = _chainCount;
        data._chainRadius = _chainRadius;

        data._damage = _damage;
        data._onHitStatus = new BaseStatusEffect[_onHitStatus.Length];
        Array.Copy(_onHitStatus, data._onHitStatus, _onHitStatus.Length);
        data._returnEffect = new BaseReturnEffect[_returnEffect.Length];
        Array.Copy(_returnEffect, data._returnEffect, _returnEffect.Length);

        data._hitFx = _hitFx;
        data._splashFx = _splashFx;

        return data;
    }

    public void CollisionCallback(Collider other, TravelState travelStateData, CharmInstance instance, 
        Action<int> stateChangeCallback)
    {
        Mob mob = other.GetComponent<Mob>();
        if (mob == null)
        {
            return;
        }

        // Pass a copy through the actual effects so that collision count can be updated immediately
        // without affecting actual effect
        // But need to get the hits back out of the result
        TravelState copy = new TravelState
        {
            CollisionCount = travelStateData.CollisionCount,

            FrontNeighbor = travelStateData.FrontNeighbor,
            BackNeighbor = travelStateData.BackNeighbor,
        };
        travelStateData.CollisionCount++;

        instance.StartCoroutine(RunEffects());


        IEnumerator RunEffects()
        {
            CharmData combined = CombineEffects(this, travelStateData.FrontNeighbor, travelStateData.BackNeighbor);
            yield return combined.ApplyImpact(combined._impactType, mob, mob.transform.position, copy, instance);
            travelStateData.HitCount += copy.HitCount;

            bool changeState = false;
            foreach (BaseReturnEffect re in _returnEffect)
            {
                changeState = changeState || re.ShouldSwitchState(travelStateData);
            }
            stateChangeCallback?.Invoke(changeState ? 1 : 0);
        }
    }

    private IEnumerator ApplyImpact(ImpactTypes types, Mob mob, Vector3 location, TravelState travelStateData, 
        CharmInstance instance)
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
                ApplyDamageAndStatusToMob(mob, travelStateData, instance);
                break;
            case ImpactTypes.Splash:
                yield return ApplySplash(remainingEffects, mob, location, travelStateData, instance);
                break;
            case ImpactTypes.Chain:
                yield return ApplyChain(remainingEffects, mob, location, travelStateData, instance);
                break;
        }
    }

    private IEnumerator ApplyChain(ImpactTypes types, Mob mob, Vector3 location, TravelState travelStateData,
        CharmInstance instance)
    {
        // Apply to first link of the chain
        instance.StartCoroutine(ApplyImpact(types, mob, location, travelStateData, instance));

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
                yield break;
            }

            instance.PlayChain(_chainSfx);

            GameObject chainFx = Instantiate(_chainFx);
            chainFx.transform.position = location;
            TweenToTarget tween = chainFx.GetComponent<TweenToTarget>();
            tween.Init(nextHit.transform.position, _chainJumpDelay);

            location = nextHit.transform.position;
            yield return new WaitForSeconds(_chainJumpDelay);
            instance.StartCoroutine(ApplyImpact(types, nextHit, location, travelStateData, instance));

            alreadyHit.Add(nextHit);
        }
    }

    private IEnumerator ApplySplash(ImpactTypes types, Mob mob, Vector3 location, TravelState travelStateData,
        CharmInstance instance)
    {
        if (_explosionCount > 0 && travelStateData.CollisionCount >= _explosionCount)
        {
            yield break;
        }

        instance.PlaySplash(_splashSfx);

        GameObject newSplashFx = Instantiate(_splashFx);
        newSplashFx.transform.position = mob.transform.position;
        newSplashFx.transform.localScale = Vector3.one * _explosionRadius;

        HashSet<Mob> alreadyHit = new HashSet<Mob>();
        float timePassed = 0;
        while (timePassed < _splashExpandDuration)
        {
            yield return null;
            timePassed += Time.deltaTime;

            float radiusForTick = _explosionRadius * timePassed / _splashExpandDuration;
            RaycastHit[] hits = Physics.SphereCastAll(location, radiusForTick, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                Mob splashed = hit.collider.GetComponent<Mob>();
                if (splashed == null || alreadyHit.Contains(splashed))
                {
                    continue;
                }

                alreadyHit.Add(splashed);
                instance.StartCoroutine(ApplyImpact(types, splashed, splashed.transform.position, travelStateData, instance));
            }
        }
    }

    private void ApplyDamageAndStatusToMob(Mob mob, TravelState travelStateData, CharmInstance instance)
    {
        // Mob died during the animation, skip
        if (mob == null)
        {
            return;
        }

        if (HasHitEffects())
        {
            instance.PlayHit(_hitSfx);

            GameObject newHitFx = Instantiate(_hitFx);
            newHitFx.transform.position = mob.transform.position;
        }

        if (_damage > 0)
        {
            mob.TakeDamage(_damage);
        }

        if (_onHitStatus != null)
        {
            foreach (BaseStatusEffect status in _onHitStatus)
            {
                mob.AddStatus(status);
            }
        }

        travelStateData.HitCount++;
    }

    private bool HasHitEffects()
    {
        return _damage > 0 || (_onHitStatus != null && _onHitStatus.Length > 0);
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
        result._explosionRadius += WeightedAdd(sub1?._explosionRadius, sub2?._explosionRadius, .5f);
        result._chainCount += WeightedAdd(sub1?._chainCount, sub2?._chainCount, 1);
        result._chainRadius += WeightedAdd(sub1?._chainRadius, sub2?._chainRadius, 1);

        result._damage += WeightedAdd(sub1?._damage, sub2?._damage, .75f);
        result._onHitStatus = CombineArrays(result._onHitStatus, sub1?._onHitStatus, sub2?._onHitStatus);
        //result._returnEffect = CombineArrays(result._returnEffect, sub1?._returnEffect, sub2?._returnEffect);

        result._splashFx = main._splashFx;
        result._chainFx = main._chainFx;
        result._hitFx = main._hitFx;
        result._hitSfx = main._hitSfx;
        result._chainSfx = main._chainSfx;
        result._splashSfx = main._splashSfx;
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
