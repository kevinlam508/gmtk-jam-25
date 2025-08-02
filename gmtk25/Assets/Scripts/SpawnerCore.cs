using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnerCore : MonoBehaviour
{
    public bool overrideAngle = false;
    [SerializeField]
    float overRideAngle = 135f;


    public GameObject[] enemyGameObjectPrefabs;

    public Dictionary<MobTypesNonFlag, GameObject> EnemyDictionary = new Dictionary<MobTypesNonFlag, GameObject>();

    public float defaultTimeBetweenSpawns = 1.5f;
    public float chanceForADoubleSpawn = 0.15f;//will be used in later rounds to determine the possibility of 2 units spawning at one time
    public float defaultTimeModiferBetweenSpawns = 0.3f;
    float timeUntilNextSpawn = 0;


    [SerializeField]
    List<SpawnRateScriptableObject> SpawnRatesPerWave = new List<SpawnRateScriptableObject>();

    SpawnRateScriptableObject currentSpawnRateStats;
    int waveNumber = 0;
    int NumberOfSpawnsForThisWave;
    MobTypes agentTypesUnlocked;


    public Transform testTarget;
    public GameObject testObj;

    public List<float> previousRandomGenerationNumbers = new List<float>();

    public float AngleRange = 135f;
    public float AngleOffset = 0;
    public float MaxDistanceForSpawn = 5f;
    public float MinDistanceForSpawn = 5f;
    public int debugCurveSmoothness = 30;


    private void Awake()
    {
        previousRandomGenerationNumbers.Clear();
        PopulateEnemyDictionary();
        EndWave(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartWave();
        }

        if (NumberOfSpawnsForThisWave > 0)
        {
            timeUntilNextSpawn -= Time.deltaTime;
            if (timeUntilNextSpawn <= 0)
            {
                SpawnEnemy();
                
                timeUntilNextSpawn = DetermineNextSpawnTime();

            }
        }
    }

    private float DetermineNextSpawnTime()
    {
        return defaultTimeBetweenSpawns + (defaultTimeModiferBetweenSpawns * GaussianRandom());
    }

    /// <summary>
    /// Gets a Random Percentage on a Normal Bell curve
    /// </summary>
    /// <returns>Percentage using Normal Randomness not uniform</returns>
    public static float GaussianRandom()
    {
        //is it from 0 to 1?
        float sum = 0;
        for (int i = 0; i < 3; i++)
        {
            sum += UnityEngine.Random.Range(-1.0f, 1.0f);
        }

        sum = sum / 3f;//to get it to be a percent
        Debug.Log(sum);
        return sum;
    }


    void PopulateEnemyDictionary()
    {
        for (int i = 0; i < (int)MobTypesNonFlag.Count; i++)
        {
            EnemyDictionary.Add((MobTypesNonFlag)i, enemyGameObjectPrefabs[i]);
        }

        
    }

    float DetermineRandomValue()
    {

        float randomValue = 0;
        bool success = false;
        int maxTries = 20;
        int currenttries = 0;
        while (!success) 
        {
            randomValue = Random.Range(0, 1.0f);
            success = true;

            currenttries++;
            if (currenttries > maxTries)
            {
                Debug.Log("reached max");
                break;
            }

            if (previousRandomGenerationNumbers.Count > 0)
            {
                if (Mathf.Abs(previousRandomGenerationNumbers[previousRandomGenerationNumbers.Count - 1] - randomValue) < 0.02f)
                {
                    //if the previous rolled value was less than 0.02f different try again
                    Debug.LogWarning("FAILURE for less than 0.02");
                    success = false;
                    continue;
                }
            }

            if (previousRandomGenerationNumbers.Count > 1)
            {
                if (Mathf.Abs(previousRandomGenerationNumbers[previousRandomGenerationNumbers.Count - 1] - randomValue) < 0.1f && Mathf.Abs(previousRandomGenerationNumbers[previousRandomGenerationNumbers.Count - 2] - randomValue) < 0.1f)
                {
                    //if the previous 2 rolled values was less than 0.1f different try again
                    Debug.LogWarning("FAILURE for less than 0.1");
                    success = false;
                    continue;
                }
            }

           /* if (previousRandomGenerationNumbers.Count > 3)
            {
                float previousValue = randomValue;
                int aboveCount = 0;
                //if the last 5 values have been increasing or decreasing
                for (int i = previousRandomGenerationNumbers.Count - 1; i >= previousRandomGenerationNumbers.Count - 4; i--)
                {
                    bool previousabove = previousRandomGenerationNumbers[i] > previousValue;
                    if (previousabove)
                        aboveCount++;
                }

                if (aboveCount == 4 || aboveCount == 0)
                {
                    Debug.LogWarning("FAILURE for ascending / decending");
                    success = false;
                    continue;
                }
            } */
        }


        previousRandomGenerationNumbers.Add(randomValue);

        if (previousRandomGenerationNumbers.Count > 10)
        {
            previousRandomGenerationNumbers.RemoveAt(0);
        }

        Debug.Log(randomValue);
        return randomValue;
    }

    public void StartWave()
    {
        NumberOfSpawnsForThisWave = GetNumberOfEnemySpawns(waveNumber);
        timeUntilNextSpawn = .4f;
    }


    void EndWave(bool onAwake = false)
    {
        waveNumber++;

        switch (waveNumber)
        {
            case 1:
                agentTypesUnlocked |= MobTypes.Pawn;//100 -> Tnk 60 -> Spd 45 -> Swm 30 -> CCR 25
                currentSpawnRateStats = SpawnRatesPerWave[0];
                break;
            case 2:
                agentTypesUnlocked |= MobTypes.Tank; // 40 -> 25 -> 
                currentSpawnRateStats = SpawnRatesPerWave[1];
                break;
            case 3:
                agentTypesUnlocked |= MobTypes.Speedster;
                currentSpawnRateStats = SpawnRatesPerWave[2];
                break;
            case 4:
                agentTypesUnlocked |= MobTypes.Swarmer;
                currentSpawnRateStats = SpawnRatesPerWave[3];
                break;
            case 5:
                agentTypesUnlocked |= MobTypes.CrowdControlResist;
                currentSpawnRateStats = SpawnRatesPerWave[4];
                break;
            case 6:
                agentTypesUnlocked |= MobTypes.Splitter;
                currentSpawnRateStats = SpawnRatesPerWave[5];
                break;
            default:
                break;
        }

        if (!onAwake)
        {

        }

        
    }

    int GetNumberOfEnemySpawns(int waveNumber)
    {
        if (waveNumber <= 10)
        {
            return 4 + (waveNumber - 1);
        }
        else
        {
            int baseEnemies = 13;//Keep base always at 13 so this does not go out of wack
            int wavesPast10 = waveNumber - 10;

            float scaledEnemyNumber = Mathf.Pow(wavesPast10, 1.5f);

            return baseEnemies + (int)Mathf.Floor(scaledEnemyNumber);
        }
    }


    MobTypesNonFlag ReturnMobType(float value)
    {
        foreach (SpawnRateListItem spawnRate in currentSpawnRateStats.SpawnRates)
        {
            //basically we have a 0 - 1 value. If the value is lower than the item in the list then we spawn it
            //the list items are in a range so 0 - 0.3 would be the first list item, 3.1 - 5.0 etc.
            //So if you check a value of .4 it would fail the first range and succeed the second range!
            if (value <= spawnRate.percentChanceToSpawn)
            {
                return spawnRate.MobTypeToSpawn;
            }
        }

        return MobTypesNonFlag.Pawn;
    }

    public void SpawnEnemy()
    {
        //check for double spawn
        float randomValue = DetermineRandomValue();

        MobTypesNonFlag enemyType = ReturnMobType(randomValue);

        GameObject enemyToSpawn = EnemyDictionary[enemyType];

        Debug.Log(enemyType.ToString());

        GameObject newEnemy = Instantiate(enemyToSpawn, DeterminePosition(testTarget.position), Quaternion.identity);
        newEnemy.GetComponent<MobMovement>().SetTargetTransform(testTarget, true);
        
        NumberOfSpawnsForThisWave--;
        if (NumberOfSpawnsForThisWave < 0)
        {
            //queue up an ondeath event on the last enemy
        }

    }

    Vector3 DeterminePosition(Vector3 targetPosition)
    {
        float DistanceFromTarget = Random.Range(MinDistanceForSpawn, MaxDistanceForSpawn);
        Vector2 direction2d = GetDirection(Mathf.Deg2Rad * AngleRange, Mathf.Deg2Rad * AngleOffset) * DistanceFromTarget;
        return targetPosition + new Vector3(direction2d.x, 0, direction2d.y);
    }

    Vector2 GetDirection(float angle, float angleMin)
    {
        float random = Random.value * angle + angleMin;
        return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
    }

    void OnDrawGizmos()
    {
        float angleRange = Mathf.Deg2Rad * AngleRange;
        float angleOffset = Mathf.Deg2Rad * AngleOffset;
        float angleCombined = angleOffset + angleRange;

        Vector3 rangeBeginning = new Vector3(Mathf.Cos(angleOffset), 0, Mathf.Sin(angleOffset));
        Vector3 rangeEnd = new Vector3(Mathf.Cos(angleCombined), 0, Mathf.Sin(angleCombined));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(testTarget.position, testTarget.position + (rangeBeginning * MaxDistanceForSpawn));
        Gizmos.DrawLine(testTarget.position, testTarget.position + (rangeEnd * MaxDistanceForSpawn));

        //Visual you can use to see the maxAndMinDistance from spawn
        for (int i = 0; i < debugCurveSmoothness; i++)
        {
            float currentPoint = angleOffset + angleRange * (i / (float)debugCurveSmoothness);
            float nextPoint = angleOffset + angleRange * ((i + 1) / (float)debugCurveSmoothness);
            DrawCurveHelper(MaxDistanceForSpawn, currentPoint, nextPoint, Color.yellow);
            DrawCurveHelper(MinDistanceForSpawn, currentPoint, nextPoint, Color.cyan);
        }        
    }


    void DrawCurveHelper(float distanceToDrawCurve, float currentPoint, float nextPoint, Color gizmosColor)
    {
        Gizmos.color = gizmosColor;
        Vector3 currentMaxWorldPos = new Vector3(Mathf.Cos(currentPoint), 0, Mathf.Sin(currentPoint)) * distanceToDrawCurve;
        Vector3 nextMaxWorldPos = new Vector3(Mathf.Cos(nextPoint), 0, Mathf.Sin(nextPoint)) * distanceToDrawCurve;
        Gizmos.DrawLine(testTarget.position + currentMaxWorldPos, testTarget.position + nextMaxWorldPos);
    }

}
