﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // enemy variables
    [SerializeField]
    private List<GameObject> _enemyPrefabs;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private float _enemySpawnRate = 5.0f;
    int _enemyTypesSpawnedThisWave;
    int _totalEnemiesSpawned = 0;
    int _totalEnemiesDestroyed = 0;
    // powerup variables
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
        _totalEnemiesDestroyed = 0;

        SpawnData Top = new SpawnData(-8f, 8f);
        SpawnData TopLeft = new SpawnData(-8f, 0f, 40f);
        SpawnData TopRight = new SpawnData(0f, 12f, -40f);
        SpawnData Left = new SpawnData(-12f, -12f, 95f, 0f, 6f);
        SpawnData Right = new SpawnData(12f, 12f, -90f, 0f, 6f);

        _possibleSpawns = new SpawnData[] { Top, TopLeft, TopRight, Left, Right };

        // enemy waves
        EnemyInWave firstWaveDefaultEnemies = 
            new EnemyInWave(5, 1, 3, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.ForwardOnly });
        EnemyInWave secondWaveDefaultEnemies = 
            new EnemyInWave(5, 1, 3, new SpawnData[] { Left, Right },
                            new MovementPattern[] { MovementPattern.TurnToBottom });
        EnemyInWave secondWaveAltEnemies =
            new EnemyInWave(1, 1, 1, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.ForwardOnly }, 
                            EnemyType.Alternate, 4f);
        EnemyInWave thirdWaveDefaultEnemies = 
            new EnemyInWave(10, 2, 5, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.ForwardOnly, MovementPattern.Strafing });
        EnemyInWave thirdWaveAltEnemies =
            new EnemyInWave(2, 2, 2, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.ForwardOnly },
                            EnemyType.Alternate, 4f);
        EnemyInWave fourthWaveDefaultEnemies = 
            new EnemyInWave(10, 1, 3, new SpawnData[] { Top, TopLeft, TopRight },
                            new MovementPattern[] { MovementPattern.ForwardOnly, MovementPattern.TurnToBottom, MovementPattern.ChasePlayer });
        EnemyInWave fourthWaveAltEnemies =
            new EnemyInWave(5, 1, 2, new SpawnData[] { Top },
                            new MovementPattern[] { MovementPattern.ForwardOnly },
                            EnemyType.Alternate, 2f);

        Wave firstWave = new Wave(new EnemyInWave[] { firstWaveDefaultEnemies },
                                  "FIRST WAVE APPROACHING\n---\nGET READY!", "FIRST WAVE DEFEATED!",
                                  3f, 3f);
        Wave secondWave = new Wave(new EnemyInWave[] { secondWaveDefaultEnemies, secondWaveAltEnemies },
                                   "SECOND WAVE APPROACHING\n---\nGET READY!", "SECOND WAVE DEFEATED!",
                                   3f, 3f);
        Wave thirdWave = new Wave(new EnemyInWave[] { thirdWaveDefaultEnemies, thirdWaveAltEnemies },
                                  "THIRD WAVE APPROACHING\n---\nGET READY!", "THIRD WAVE DEFEATED!",
                                  3f, 3f);
        Wave fourthWave = new Wave(new EnemyInWave[] { fourthWaveDefaultEnemies, fourthWaveAltEnemies },
                                  "FOURTH WAVE APPROACHING\n---\nGET READY!", "FOURTH WAVE DEFEATED!",
                                  3f, 3f);

        _enemyWaves = new Wave[] { firstWave, secondWave, thirdWave, fourthWave };
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
                    break;
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
                int randomSpawn = Random.Range(0, enemyWave.PossibleSpawns.Length);
                int randomPattern = Random.Range(0, enemyWave.PossibleMovementPatterns.Length);
                EnemySpawn(enemyWave.EnemyType, enemyWave.PossibleSpawns[randomSpawn], enemyWave.PossibleMovementPatterns[randomPattern]);
                remainingEnemiesInWave--;
            }

            yield return new WaitForSeconds(_enemySpawnRate);
        }
        _enemyTypesSpawnedThisWave++;
    }

    void EnemySpawn(EnemyType type, SpawnData spawnData, MovementPattern pattern)
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
        newEnemy.GetComponent<EnemyBase>().SetupEnemy(spawnData, pattern);

        _totalEnemiesSpawned++;
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

            Instantiate(_powerupPrefabs[powerupId], powerupPos, Quaternion.identity);

            float nextPowerupSpawn = Random.Range(3.0f, 7.0f);

            yield return new WaitForSeconds(nextPowerupSpawn);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void EnemyDestroyed()
    {
        _totalEnemiesDestroyed++;
    }
}
