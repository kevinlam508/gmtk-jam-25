using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class SpawnerCore : MonoBehaviour
{
    public static SpawnerCore Instance { get; private set; }

    public bool enableDebug = false;
    public int DebugStartAtSpecificRound = 1;
    public float delayUntilGameStarts = 2.3f;
    public float downTimeAfterWave = 1f;

    public MobScriptableObject[] enemyGameStatArray;//Holds each mobs individual stats
    public GameObject basePawnPrefab;//the base pawn prefab that needs thats to function

    public Dictionary<MobTypesNonFlag, MobScriptableObject> EnemyStatDictionary = new Dictionary<MobTypesNonFlag, MobScriptableObject>();//dictionary of stats for each mob


    public float defaultTimeBetweenSpawns = 1.5f;
    public float chanceForADoubleSpawn = 0.15f;//will be used in later rounds to determine the possibility of 2 units spawning at one time
    public float defaultTimeModiferBetweenSpawns = 0.3f;
    float timeUntilNextSpawn = 0;
    int specialSpawnRateValueRemaining = 0;
    float baseSpecialSpawnChance = 0.00f;
    float specialSpawnChance = 0.00f;


    float doubleSpawnChance = 0.0f;


    [SerializeField]
    List<SpawnRateScriptableObject> SpawnRatesPerWave = new List<SpawnRateScriptableObject>();//list of SpawnRateSO's that will determine the chance each indiviudal mob is spawned once a pariticular wave is reached
    [SerializeField]
    List<SpawnRateScriptableObject> SpecialSpawnRates = new List<SpawnRateScriptableObject>();


    [SerializeField]
    SpawnRateScriptableObject currentSpawnRateStats;//the current loaded  in SpawnRates
    int waveNumber = 0;
    int NumberOfSpawnsForThisWave;
    MobTypes agentTypesUnlocked;//not really used anymore teehee


    public Transform playerTarget;//The target location that enemies will set movement too

    public List<float> previousRandomGenerationNumbers = new List<float>();
    public List<int> previousRandomGenerationInt = new List<int>();

    public float AngleRange = 135f;//range that enemies can spawn in
    public float AngleOffset = 0;//The offset from 0 that the range will start from
    public float MaxDistanceForSpawn = 5f;//the max distance that an enemy can spawn from the player
    public float MinDistanceForSpawn = 5f;//the min distance that an enemy can spawn from the player
    public int debugCurveSmoothness = 30;//The amount of smoothness that is shown in the debug arc
    bool waitingOnEnemiesToDie = false;
    int enemiesOnScreen = 0;

    private void Awake()
    {
        previousRandomGenerationNumbers.Clear();
        PopulateEnemyDictionary();
        Instance = this;

                  

        if (enableDebug)
        {
            waveNumber = DebugStartAtSpecificRound;
        }
        else
        {
            StartCoroutine(WaitToStartGame(delayUntilGameStarts));
        }
        SetSpawnRates();
    }

    private IEnumerator WaitToStartGame(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        StartWave();
    }

    private void Start()
    {
        AddCharmSelection.Instance.GetClosedEvent().AddListener(StartWave);
    }

    // Update is called once per frame
    void Update()
    {
        if (enableDebug)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartWave();
            }
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

        if (waitingOnEnemiesToDie)
        {
            EndWaveStage2();
        }
    }

    /// <summary>
    /// Uses the defaultTimeBetweenSpawns and comes up with the next mob spawn time with a little bit of randomness
    /// </summary>
    /// <returns></returns>
    private float DetermineNextSpawnTime()
    {
        return defaultTimeBetweenSpawns + (defaultTimeModiferBetweenSpawns * GaussianRandom());
    }

    /// <summary>
    /// Populates the enemy stats dictionary with the values in the stat Array. (Stat array needs to be the same size as MobTypesNonflag.Count
    /// </summary>
    void PopulateEnemyDictionary()
    {
        for (int i = 0; i < (int)MobTypesNonFlag.Count; i++)
        {
            EnemyStatDictionary.Add((MobTypesNonFlag)i, enemyGameStatArray[i]);            
        }        
    }

    public void StartFirstWave()
    {
        waveNumber = 1;
        agentTypesUnlocked |= MobTypes.Pawn;//100 -> Tnk 60 -> Spd 45 -> Swm 30 -> CCR 25
        currentSpawnRateStats = SpawnRatesPerWave[0];
        defaultTimeBetweenSpawns = 2.5f;
        //EndWave();
        //extra stuff
    }

    public void StartWave()
    {
        if (waveNumber == 0)
        {
            StartFirstWave();
        }

        if (specialSpawnRateValueRemaining > 0)
        {
            specialSpawnChance = baseSpecialSpawnChance;
            specialSpawnRateValueRemaining = 0;
        }

        NumberOfSpawnsForThisWave = GetNumberOfEnemySpawns(waveNumber);
        timeUntilNextSpawn = .4f;
    }

    void OnEnemyDeath(GameObject enemyThatDied, bool playerKill)
    {
        Destroy(enemyThatDied);
        enemiesOnScreen--;
    }

    void ShuffleSpawnRates(int numberOfEnemiestoShuffleFor)
    {
        specialSpawnRateValueRemaining = numberOfEnemiestoShuffleFor;

        int index = DetermineRandomInt(SpecialSpawnRates.Count);

        if (SpecialSpawnRates.Count > index)
        {
            currentSpawnRateStats = SpecialSpawnRates[index];
        }
        else
        {
            Debug.LogWarning("WARNING OUT OF RANGE FOR SUFFLESPAWNRATES");
            currentSpawnRateStats = SpecialSpawnRates[0];
        }

    }

    void SetSpawnRates()
    {
        //need to add 7 as well
        if (waveNumber >= 6)
        {
            agentTypesUnlocked |= MobTypes.Splitter;
            currentSpawnRateStats = SpawnRatesPerWave[5];
        }
        else if (waveNumber >= 5)
        {
            agentTypesUnlocked |= MobTypes.CrowdControlResist;
            currentSpawnRateStats = SpawnRatesPerWave[4];
        }
        else if (waveNumber >= 4)
        {
            agentTypesUnlocked |= MobTypes.Swarmer;
            currentSpawnRateStats = SpawnRatesPerWave[3];
        }
        else if (waveNumber >= 3)
        {
            agentTypesUnlocked |= MobTypes.Speedster;
            currentSpawnRateStats = SpawnRatesPerWave[2];
        }
        else if (waveNumber >= 2)
        {
            agentTypesUnlocked |= MobTypes.Tank; // 40 -> 25 -> 
            currentSpawnRateStats = SpawnRatesPerWave[1];
        }
        else if (waveNumber >= 1)
        {
            agentTypesUnlocked |= MobTypes.Pawn;//100 -> Tnk 60 -> Spd 45 -> Swm 30 -> CCR 25
            currentSpawnRateStats = SpawnRatesPerWave[0];
        }
    }


    private IEnumerator EndWaveStage3(float timeTowait)
    {
        yield return new WaitForSeconds(timeTowait);
        waveNumber++;


        if (waveNumber > 4)
        {
            defaultTimeBetweenSpawns = 2.5f - (0.1f * (waveNumber - 5));
            defaultTimeBetweenSpawns = Mathf.Clamp(defaultTimeBetweenSpawns, 1.9f, 2.5f);
        }

        if (waveNumber >= 10 && waveNumber <= 20)
        {
            float scaleFactor = (waveNumber - 10) / 10f; // Starts at 0 / 10 goes up to 10 / 10
            float chanceIncrease = Mathf.Pow(scaleFactor, 1.6f) * 0.10f;
            chanceForADoubleSpawn = 0.15f + chanceIncrease;
            chanceForADoubleSpawn = Mathf.Clamp(chanceForADoubleSpawn, 0, 0.25f);
        }

        SetSpawnRates();


        AddCharmSelection.Instance.ShowAndGenerate();
    }


    void EndWaveStage2()
    {
        if (enemiesOnScreen <= 0)
        {
            waitingOnEnemiesToDie = false;
            StartCoroutine(EndWaveStage3(downTimeAfterWave));
        }
        else
        {
            waitingOnEnemiesToDie = true;
        }
    }

    void EndWave(GameObject enemyThatEndedWave, bool playerKill)
    {
        if (enemyThatEndedWave)
        {
            enemiesOnScreen--;
            Destroy(enemyThatEndedWave);
        }
        EndWaveStage2();        
      //  if (!onAwake)        
    }

    int GetNumberOfEnemySpawns(int waveNumber)
    {
        float scaledEnemyNumber;
        int enemyNumber;
        if (waveNumber <= 10)
        {
            scaledEnemyNumber = Mathf.Pow(waveNumber, 1.2f);
            return 4 + (int)Mathf.Floor(scaledEnemyNumber);//1 : 4 | 2: 5 | 3: 6| 4: 7
        }
        else
        {
            int baseEnemies = 15;//Keep base always at 13 so this does not go out of wack
            int wavesPast10 = waveNumber - 10;

            scaledEnemyNumber = Mathf.Pow(wavesPast10, 1.25f);

            enemyNumber = baseEnemies + (int)Mathf.Floor(scaledEnemyNumber);
            Debug.Log(enemyNumber);
            return enemyNumber;
        }
    }

    bool CheckForSpecialSpawn()
    {
        if (waveNumber < 9 || specialSpawnRateValueRemaining > 0)
        {
            return false;
        }

        float randomValue = DetermineRandomValue();

        if (randomValue <= specialSpawnChance)
        {
            ShuffleSpawnRates(Random.Range(5, 14));
            Debug.Log("SEPCIAL SPAWN RATE DETERMINED");
        }
        else
        {
            float scaleFactor = Mathf.Clamp01((waveNumber - 9) / 21f);//21 because we want it to cap out at 30. 30 - 9 = 21. 
            float chanceIncrease = Mathf.Lerp(0.002f, 0.009f, Mathf.Pow(scaleFactor, 1.3f));
            specialSpawnChance += chanceIncrease;
            return true;
        }


        specialSpawnChance = Mathf.Clamp(specialSpawnChance, 0, 0.18f);


        return false;
    }


    public bool CheckForDoubleSpawn()
    {
        if (waveNumber < 9 || NumberOfSpawnsForThisWave <= 2)
        {
            return false;
        }

        float randomValue = Random.Range(0.0f, 1.0f);

        if (randomValue <= chanceForADoubleSpawn)
        {
            Debug.Log("Double Spawn is true");
            return true;
        }


        return false;
    }

    public void SpawnEnemy(bool calledFromDoubleSpawn = false)
    {
        //check for double spawn
        float randomValue = DetermineRandomValue();

        if (!calledFromDoubleSpawn)
        {
            CheckForSpecialSpawn();//only comes into play at round 9

            if (CheckForDoubleSpawn())
            {
                SpawnEnemy(true);
            }
        }

        MobTypesNonFlag enemyType = ReturnMobType(randomValue);

        //GameObject enemyToSpawn = EnemyDictionary[enemyType];
        MobScriptableObject enemyStats = EnemyStatDictionary[enemyType];

       // Debug.Log(enemyType.ToString());

        GameObject newEnemy = Instantiate(basePawnPrefab, DeterminePosition(playerTarget.position), Quaternion.identity);
        newEnemy.GetComponent<MobMovement>().SetTargetTransform(playerTarget, true);
        newEnemy.GetComponent<Mob>().InitializeEnemy(enemyStats, enemyType);
        enemiesOnScreen++;

        if (enemyType == MobTypesNonFlag.Swarmer)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject swamerExtra = Instantiate(basePawnPrefab, DeterminePosition(playerTarget.position), Quaternion.identity);
                swamerExtra.GetComponent<MobMovement>().SetTargetTransform(playerTarget, true);
                swamerExtra.GetComponent<Mob>().InitializeEnemy(enemyStats, enemyType);
                swamerExtra.GetComponent<Health>().GetOnDeathEvent().AddListener(OnEnemyDeath);
                enemiesOnScreen++;
            }
        }

        NumberOfSpawnsForThisWave--;
        if (specialSpawnRateValueRemaining > 0)
        {
            specialSpawnRateValueRemaining--;
            if (specialSpawnRateValueRemaining <= 0)
            {
                specialSpawnChance = baseSpecialSpawnChance;
                SetSpawnRates();//resets to normal spawn rates
            }
        }


        if (NumberOfSpawnsForThisWave <= 0)
        {
            newEnemy.GetComponent<Health>().GetOnDeathEvent().AddListener(EndWave);

        }
        else
        {
            newEnemy.GetComponent<Health>().GetOnDeathEvent().AddListener(OnEnemyDeath);
        }

    }


    /// <summary>
    /// Returns a Random Mob type based on a 0 - 1.0 value by consulting the SpawnRate list for the wave
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    MobTypesNonFlag ReturnMobType(float value)
    {

        foreach (SpawnRateListItem spawnRate in currentSpawnRateStats.SpawnRates)
        {
            //basically we have a 0 - 1 value. If the value is lower than the item in the list then we spawn it
            //the list items are in a range so 0 - 0.3 would be the first list item, 3.1 - 5.0 etc.
            //So if you check a value of .4 it would fail the first range and succeed the second range!
            if (value <= spawnRate.percentChanceToSpawn)
            {
                //Debug.Log(value);
                return spawnRate.MobTypeToSpawn;
            }
        }

        return MobTypesNonFlag.Pawn;
    }

    /// <summary>
    /// Determines the SpawnPosition between the Min and Max Spawn Distance from a target position
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    Vector3 DeterminePosition(Vector3 targetPosition)
    {
        float DistanceFromTarget = Random.Range(MinDistanceForSpawn, MaxDistanceForSpawn);
        Vector2 direction2d = GetDirection(Mathf.Deg2Rad * AngleRange, Mathf.Deg2Rad * AngleOffset) * DistanceFromTarget;
        return targetPosition + new Vector3(direction2d.x, 0, direction2d.y);
    }

    /// <summary>
    /// Gets a RandomDirection between within the angle range starting from angle min
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="angleMin"></param>
    /// <returns></returns>
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
        Gizmos.DrawLine(playerTarget.position, playerTarget.position + (rangeBeginning * MaxDistanceForSpawn));
        Gizmos.DrawLine(playerTarget.position, playerTarget.position + (rangeEnd * MaxDistanceForSpawn));

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
        Gizmos.DrawLine(playerTarget.position + currentMaxWorldPos, playerTarget.position + nextMaxWorldPos);
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
       // Debug.Log(sum);
        return sum;
    }


    int DetermineRandomInt(int maxNumber)
    {
        int RandomValue = 0;
        bool success = false;
        int maxTries = 20;
        int currentTries = 0;

        while(!success)
        {
            RandomValue = Random.Range(0, maxNumber + 1);
            success = true;
            currentTries++;
            if (currentTries > maxTries)
            {
                Debug.Log("reached max");
                break;
            }

 
            if (previousRandomGenerationInt.Count > 0)
            {
                if (previousRandomGenerationInt[previousRandomGenerationInt.Count - 1] == RandomValue)
                {
                    success = false;
                    continue;
                }
            }

           /* if (previousRandomGenerationInt.Count > 3)
            {
                int previousValue = RandomValue;
                int aboveCount = 0;
                int patternNumber = 3;
                //if the last 5 values have been increasing or decreasing
                for (int i = previousRandomGenerationInt.Count - 1; i >= previousRandomGenerationInt.Count - 4; i--)
                {
                    patternNumber = previousRandomGenerationInt[i];
                    previousValue
                }
            }*/
        }

        previousRandomGenerationInt.Add(RandomValue);

        if (previousRandomGenerationInt.Count > 10)
        {
            previousRandomGenerationInt.RemoveAt(0);
        }

        //Debug.Log(randomValue);
        return RandomValue;
    }


    /// <summary>
    /// Determines a Random Value between 0 and 1.0 that can be used to determine things like the next value that will be used to spawn an enemy.
    /// </summary>
    /// <returns></returns>
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

        //Debug.Log(randomValue);
        return randomValue;
    }

}
