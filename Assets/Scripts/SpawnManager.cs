using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // enemy variables
    [SerializeField]
    private List<GameObject> _enemyPrefabs;
    [SerializeField]
    GameObject _enemyBossPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private float _enemySpawnRate = 5.0f;
    int _enemyTypesSpawnedThisWave;
    int _totalEnemiesSpawned = 0;

    // powerup variables
    [SerializeField]
    private GameObject _powerupContainer;
    [SerializeField]
    private List<GameObject> _powerupPrefabs;

    List<int> _chanceRanges;
    int _totalChances;

    private bool _stopSpawning = false;

    // ui manager
    private UIManager _uiManager;

    // spawn data
    SpawnData[] _possibleSpawns;

    // wave data
    Wave[] _enemyWaves;

    // Start is called before the first frame update
    void Start()
    {
        // ui manager
        _uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL.");
        }

        // powerup odds of spawning
        _chanceRanges = new List<int>();
        _totalChances = 0;
        foreach (GameObject powerup in _powerupPrefabs)
        {
            _totalChances += powerup.GetComponent<Powerup>()._chances;
            _chanceRanges.Add(_totalChances);
        }

        // enemy spawns
        //_totalEnemiesDestroyed = 0;

        SpawnData Top = new SpawnData(-8f, 8f);
        SpawnData TopLeft = new SpawnData(-8f, 0f, 40f);
        SpawnData TopRight = new SpawnData(0f, 12f, -40f);
        SpawnData Left = new SpawnData(-12f, -12f, 95f, 0f, 6f);
        SpawnData Right = new SpawnData(12f, 12f, -90f, 0f, 6f);
        SpawnData Boss = new SpawnData(0f, 0f, 0f, -15f, -15f);

        _possibleSpawns = new SpawnData[] { Top, TopLeft, TopRight, Left, Right, Boss };

        // enemy waves
        EnemyInWave firstWaveDefaultEnemies = 
            new EnemyInWave(5, 1, 3, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.ForwardOnly }, 
                            EnemyType.Default, 0f, 10f);
        EnemyInWave firstWaveAggroEnemies =
            new EnemyInWave(1, 1, 1, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.Default },
                            EnemyType.Aggressive, 6f, 10f);
        //EnemyInWave firstWaveAgileEnemies =
        //    new EnemyInWave(1, 1, 1, new SpawnData[] { Top },
        //                    new MovementPattern[] { MovementPattern.Default },
        //                    EnemyType.Agile, 8f, 10f);
        EnemyInWave secondWaveDefaultEnemies = 
            new EnemyInWave(5, 1, 3, new SpawnData[] { Left, Right },
                            new MovementPattern[] { MovementPattern.TurnToBottom },
                            EnemyType.Default, 0f, 10f);
        EnemyInWave secondWaveAltEnemies =
            new EnemyInWave(1, 1, 1, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.Default }, 
                            EnemyType.Alternate, 4f, 20f);
        EnemyInWave secondWaveSmartEnemies =
            new EnemyInWave(1, 1, 1, new SpawnData[] { Left, Right },
                            new MovementPattern[] { MovementPattern.ForwardOnly },
                            EnemyType.Smart, 6f, 20f);
        EnemyInWave thirdWaveDefaultEnemies = 
            new EnemyInWave(6, 2, 5, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.ForwardOnly, MovementPattern.Strafing },
                            EnemyType.Default, 0f, 10f);
        EnemyInWave thirdWaveAltEnemies =
            new EnemyInWave(2, 2, 2, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.Default },
                            EnemyType.Alternate, 4f, 25f);
        EnemyInWave thirdWaveAggroEnemies =
            new EnemyInWave(2, 1, 2, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.Default },
                            EnemyType.Aggressive, 8f, 20f);
        EnemyInWave fourthWaveDefaultEnemies = 
            new EnemyInWave(6, 1, 3, new SpawnData[] { Top, TopLeft, TopRight },
                            new MovementPattern[] { MovementPattern.ForwardOnly, MovementPattern.TurnToBottom, MovementPattern.ChasePlayer },
                            EnemyType.Default, 0f, 20f);
        EnemyInWave fourthWaveAltEnemies =
            new EnemyInWave(2, 1, 2, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.Default },
                            EnemyType.Alternate, 2f, 35f);
        EnemyInWave fourthWaveSmartEnemies =
            new EnemyInWave(2, 1, 3, new SpawnData[] { TopLeft, TopRight },
                            new MovementPattern[] { MovementPattern.ForwardOnly },
                            EnemyType.Smart, 10f, 20f);
        EnemyInWave fourthWaveAgileEnemies =
            new EnemyInWave(2, 1, 1, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.Default },
                            EnemyType.Agile, 8f, 15f);
        EnemyInWave fifthWaveDefaultEnemies =
            new EnemyInWave(6, 1, 3, new SpawnData[] { Top, TopLeft, TopRight },
                            new MovementPattern[] { MovementPattern.ForwardOnly, MovementPattern.TurnToBottom, MovementPattern.ChasePlayer },
                            EnemyType.Default, 0f, 20f);
        EnemyInWave fifthWaveMoreDefaultEnemies =
            new EnemyInWave(6, 1, 3, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.ForwardOnly },
                            EnemyType.Default, 16f, 20f);
        EnemyInWave fifthWaveAltEnemies =
            new EnemyInWave(3, 1, 2, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.Default },
                            EnemyType.Alternate, 4f, 35f);
        EnemyInWave fifthWaveSmartEnemies =
            new EnemyInWave(2, 1, 3, new SpawnData[] { TopLeft, TopRight },
                            new MovementPattern[] { MovementPattern.ForwardOnly },
                            EnemyType.Smart, 20f, 20f);
        EnemyInWave fifthWaveAgileEnemies =
            new EnemyInWave(3, 1, 1, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.Default },
                            EnemyType.Agile, 16f, 15f);
        EnemyInWave bossWaveBossEnemy =
            new EnemyInWave(1, 1, 1, new SpawnData[] { Boss },
                            new MovementPattern[] { MovementPattern.Default },
                            EnemyType.Boss, 0f, 0f);
        EnemyInWave bossWaveDefaultEnemies =
            new EnemyInWave(6, 2, 4, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.ForwardOnly },
                            EnemyType.Default, 12f, 20f); ;
        EnemyInWave bossWaveMoreDefaultEnemies =
            new EnemyInWave(6, 2, 4, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.ForwardOnly },
                            EnemyType.Default, 24f, 20f); ; 
        EnemyInWave bossWaveEvenMoreDefaultEnemies =
             new EnemyInWave(6, 2, 4, new SpawnData[] { Top },
                             new MovementPattern[] { MovementPattern.ForwardOnly },
                             EnemyType.Default, 36f, 20f); ;


        Wave firstWave = new Wave(new EnemyInWave[] { firstWaveDefaultEnemies, firstWaveAggroEnemies },
                                  "FIRST WAVE APPROACHING\n---\nGET READY!", "FIRST WAVE DEFEATED!",
                                  3f, 3f);
        Wave secondWave = new Wave(new EnemyInWave[] { secondWaveDefaultEnemies, secondWaveAltEnemies, secondWaveSmartEnemies },
                                   "SECOND WAVE APPROACHING\n---\nGET READY!", "SECOND WAVE DEFEATED!",
                                   3f, 3f);
        Wave thirdWave = new Wave(new EnemyInWave[] { thirdWaveDefaultEnemies, thirdWaveAltEnemies, thirdWaveAggroEnemies },
                                  "THIRD WAVE APPROACHING\n---\nGET READY!", "THIRD WAVE DEFEATED!",
                                  3f, 3f);
        Wave fourthWave = new Wave(new EnemyInWave[] { fourthWaveDefaultEnemies, fourthWaveAltEnemies, fourthWaveSmartEnemies, fourthWaveAgileEnemies },
                                  "FOURTH WAVE APPROACHING\n---\nGET READY!", "FOURTH WAVE DEFEATED!",
                                  3f, 3f);
        Wave fifthWave = new Wave(new EnemyInWave[] { fifthWaveDefaultEnemies, fifthWaveMoreDefaultEnemies, fifthWaveAgileEnemies, fifthWaveAltEnemies, fifthWaveSmartEnemies },
                                  "FIFTH WAVE APPROACHING\n---\nGET READY!", "FIFTH WAVE DEFEATED!",
                                  3f, 3f);
        Wave bossWave = new Wave(new EnemyInWave[] { bossWaveBossEnemy, bossWaveDefaultEnemies, bossWaveMoreDefaultEnemies, bossWaveEvenMoreDefaultEnemies },
                                 "CAUTION\n---\nLARGE ENEMY APPROACHING\n---\nGET READY!", "ENEMY BOSS DEFEATED!",
                                 3f, 3f);

        _enemyWaves = new Wave[] { firstWave, secondWave, thirdWave, fourthWave, fifthWave, bossWave };
        //_enemyWaves = new Wave[] { firstWave, bossWave };
    }


    //// Update is called once per frame
    //void Update()
    //{
    //}

    public void StartEnemySpawns()
    {
        StartCoroutine(WaveRoutine());
        StartCoroutine(PowerupSpawnRoutine());
    }

    IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(1f);
        foreach (Wave wave in _enemyWaves)
        {
            _uiManager.DisplayWaveText(wave.WaveEnterMessage);
            yield return new WaitForSeconds(wave.EnterMessageDuration);

            int enemyTypesToSpawn = 0;
            _enemyTypesSpawnedThisWave = 0;
            foreach (EnemyInWave enemyType in wave.EnemiesInWave)
            {
                StartCoroutine(SpawnEnemiesRoutine(enemyType));
                enemyTypesToSpawn++;
            }

            yield return new WaitForSeconds(2f);

            while (_enemyContainer.transform.childCount > 0 || _enemyTypesSpawnedThisWave < enemyTypesToSpawn)
            {
                if (_stopSpawning)
                {
                    StopCoroutine(WaveRoutine());
                }
                yield return new WaitForSeconds(1f);
            }

            _uiManager.DisplayWaveText(wave.WaveExitMessage);
            yield return new WaitForSeconds(wave.ExitMessageDuration);
        }
        _stopSpawning = true;
        _uiManager.WinGame();
    }

    IEnumerator SpawnEnemiesRoutine(EnemyInWave enemyWave)
    {
        yield return new WaitForSeconds(enemyWave.EntryDelay);

        int remainingEnemiesInWave = enemyWave.EnemiesInWave;
        while (remainingEnemiesInWave > 0)
        {
            if (_stopSpawning)
            {
                break;
            }

            int enemiesToSpawn = Random.Range(enemyWave.MinEnemiesInSpawn, enemyWave.MaxEnemiesInSpawn);
            if (enemiesToSpawn > remainingEnemiesInWave)
            {
                enemiesToSpawn = remainingEnemiesInWave;
            }

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                bool enemyHasShield = false;
                int randomSpawn = Random.Range(0, enemyWave.PossibleSpawns.Length);
                int randomPattern = Random.Range(0, enemyWave.PossibleMovementPatterns.Length);
                float randomShieldChance = Random.Range(0f, 100f);
                if (randomShieldChance <= enemyWave.ChanceToHaveShield)
                {
                    enemyHasShield = true;
                }
                EnemySpawn(enemyWave.EnemyType, 
                           enemyWave.PossibleSpawns[randomSpawn], 
                           enemyWave.PossibleMovementPatterns[randomPattern], 
                           enemyHasShield);
                remainingEnemiesInWave--;
            }

            yield return new WaitForSeconds(_enemySpawnRate);
        }
        _enemyTypesSpawnedThisWave++;
    }

    void EnemySpawn(EnemyType type, SpawnData spawnData, MovementPattern pattern, bool hasShield = false)
    {
        // type
        int typeID = 0;
        switch (type)
        {
            case EnemyType.Default:
                typeID = 0;
                break;
            case EnemyType.Alternate:
                typeID = 1;
                break;
            case EnemyType.Aggressive:
                typeID = 2;
                break;
            case EnemyType.Smart:
                typeID = 3;
                break;
            case EnemyType.Agile:
                typeID = 4;
                break;
            case EnemyType.Boss:
                BossSpawn();
                return;
            default:
                typeID = 0;
                break;
        }

        // position
        float xPos, yPos;
        if (spawnData.minX == spawnData.maxX)
        {
            xPos = spawnData.minX;
        }
        else
        {
            xPos = Random.Range(spawnData.minX, spawnData.maxX);
        }

        if (spawnData.minY == spawnData.maxY)
        {
            yPos = spawnData.minY;
        }
        else
        {
            yPos = Random.Range(spawnData.minY, spawnData.maxY);
        }

        Vector3 enemyPos = new Vector3(xPos, yPos, 0f);
        GameObject newEnemy = Instantiate(_enemyPrefabs[typeID], enemyPos, Quaternion.identity, _enemyContainer.transform);
        if (spawnData.rotationAngle != 0)
        {
            newEnemy.transform.Rotate(new Vector3(0f, 0f, spawnData.rotationAngle));
        }
        EnemyBase enemyScript = newEnemy.GetComponent<EnemyBase>();
        if (enemyScript == null)
        {
            enemyScript = newEnemy.transform.GetChild(0).GetComponent<EnemyBase>();
        }
        enemyScript.SetupEnemy(spawnData, pattern);
        if (hasShield)
        {
            enemyScript.ActivateShield(1);
        }

        _totalEnemiesSpawned++;
    }

    void BossSpawn()
    {
        Vector3 bossSpawnPosition = new Vector3(0, -15, 5);
        Instantiate(_enemyBossPrefab, bossSpawnPosition, Quaternion.identity, _enemyContainer.transform);
    }

    IEnumerator PowerupSpawnRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (!_stopSpawning)
        {
            float randomX = Random.Range(-8.0f, 8.0f);
            Vector3 powerupPos = new Vector3(randomX, 7.0f, 0.0f);

            int randRoll = Random.Range(0, _totalChances);
            int powerupId = 0;
            for (int i = 0; i < _powerupPrefabs.Count; i++)
            {
                if (randRoll < _chanceRanges[i])
                {
                    powerupId = i;
                    break;
                }
            }

            Instantiate(_powerupPrefabs[powerupId], powerupPos, Quaternion.identity, _powerupContainer.transform);

            float nextPowerupSpawn = Random.Range(3.0f, 7.0f);

            yield return new WaitForSeconds(nextPowerupSpawn);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
