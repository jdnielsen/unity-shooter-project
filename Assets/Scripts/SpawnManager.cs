using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // enemy variables
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private float _enemySpawnRate = 5.0f;
    int _totalEnemiesSpawned = 0;
    int _totalEnemiesDestroyed = 0;
    // powerup variables
    [SerializeField]
    private List<GameObject> _powerups;

    List<int> _chanceRanges;
    int _totalChances;

    private bool _stopSpawning = false;

    // ui manager
    private UIManager _uiManager;

    // spawn data
    SpawnData[] _possibleSpawns;

    // wave data
    WaveData[] _enemyWaves;

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
        foreach (GameObject powerup in _powerups)
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
        WaveData first = new WaveData(new SpawnData[] { Top }, 5, 0, 1, 3, 
                                      new MovementPattern[] { MovementPattern.ForwardOnly },
                                      "FIRST WAVE APPROACHING\n---\nGET READY!", "FIRST WAVE DEFEATED!");
        WaveData second = new WaveData(new SpawnData[] { Left, Right }, 5, 0, 1, 3, 
                                       new MovementPattern[] { MovementPattern.TurnToBottom },
                                      "SECOND WAVE APPROACHING\n---\nGET READY!", "SECOND WAVE DEFEATED!");
        WaveData third = new WaveData(new SpawnData[] { Top }, 10, 0, 2, 5, 
                                      new MovementPattern[] { MovementPattern.ForwardOnly, MovementPattern.Strafing },
                                      "THIRD WAVE APPROACHING\n---\nGET READY!", "THIRD WAVE DEFEATED!");
        WaveData fourth = new WaveData(new SpawnData[] { Top, TopLeft, TopRight }, 10, 0, 1, 3, 
                                       new MovementPattern[] { MovementPattern.ForwardOnly, MovementPattern.TurnToBottom, MovementPattern.ChasePlayer },
                                      "FOURTH WAVE APPROACHING\n---\nGET READY!", "FOURTH WAVE DEFEATED!");
        _enemyWaves = new WaveData[] { first, second, third, fourth };
    }


    //// Update is called once per frame
    //void Update()
    //{
    //}

    public void StartEnemySpawns()
    {
        StartCoroutine(EnemySpawnRoutine());
        StartCoroutine(PowerupSpawnRoutine());
    }

    IEnumerator EnemySpawnRoutine()
    {
        yield return new WaitForSeconds(1f);
        foreach (WaveData wave in _enemyWaves)
        {
            _uiManager.DisplayWaveText(wave.WaveEnterMessage);
            yield return new WaitForSeconds(3f);

            int enemiesSpawnedThisWave = 0;
            int enemiesDestroyedTarget = _totalEnemiesDestroyed + wave.EnemiesInWave;

            while (enemiesSpawnedThisWave < wave.EnemiesInWave)
            {
                if (_stopSpawning)
                {
                    break;
                }

                int enemiesToSpawn = Random.Range(wave.MinEnemiesInSpawn, wave.MaxEnemiesInSpawn);
                if ((wave.EnemiesInWave - enemiesSpawnedThisWave) < enemiesToSpawn)
                {
                    enemiesToSpawn = wave.EnemiesInWave - enemiesSpawnedThisWave;
                }

                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    int randomSpawn = Random.Range(0, wave.PossibleSpawns.Length);
                    int randomPattern = Random.Range(0, wave.PossibleMovementPatterns.Length);
                    EnemySpawn(wave.PossibleSpawns[randomSpawn], wave.PossibleMovementPatterns[randomPattern]);
                    enemiesSpawnedThisWave++;
                }

                yield return new WaitForSeconds(_enemySpawnRate);
            }
            while (_totalEnemiesDestroyed < enemiesDestroyedTarget)
            {
                if (_stopSpawning)
                {
                    break;
                }

                yield return new WaitForSeconds(1f);
            }

            _uiManager.DisplayWaveText(wave.WaveExitMessage);
            yield return new WaitForSeconds(3f);
        }
        _stopSpawning = true;
        _uiManager.WinGame();
    }

    void EnemySpawn(SpawnData spawnData, MovementPattern pattern)
    {
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
        GameObject newEnemy = Instantiate(_enemyPrefab, enemyPos, Quaternion.identity, _enemyContainer.transform);
        if (spawnData.rotationAngle != 0)
        {
            newEnemy.transform.Rotate(new Vector3(0f, 0f, spawnData.rotationAngle));
        }
        newEnemy.GetComponent<Enemy>().SetupEnemy(spawnData, pattern);

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
            for (int i = 0; i < _powerups.Count; i++)
            {
                if (randRoll < _chanceRanges[i])
                {
                    powerupId = i;
                    break;
                }
            }



            Instantiate(_powerups[powerupId], powerupPos, Quaternion.identity);

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
