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
    // powerup variables
    [SerializeField]
    private List<GameObject> _powerups;

    List<int> _chanceRanges;
    int _totalChances;

    private bool _stopSpawning = false;

    // spawn data
    SpawnData Top = new SpawnData(-8f, 8f);
    SpawnData TopLeft = new SpawnData(-8f, 0f, 40f);
    SpawnData TopRight = new SpawnData(0f, 12f, -40f);
    SpawnData Left = new SpawnData(-12f, -12f, 95f, 0f, 6f);
    SpawnData Right = new SpawnData(12f, 12f, -90f, 0f, 6f);

    SpawnData[] PossibleSpawns; 

    // Start is called before the first frame update
    void Start()
    {
        _chanceRanges = new List<int>();
        _totalChances = 0;
        foreach (GameObject powerup in _powerups)
        {
            _totalChances += powerup.GetComponent<Powerup>()._chances;
            _chanceRanges.Add(_totalChances);
        }

        PossibleSpawns = new SpawnData[] { Top, TopLeft, TopRight, Left, Right };
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartEnemySpawns()
    {
        StartCoroutine(EnemySpawnRoutine());
        StartCoroutine(PowerupSpawnRoutine());
    }

    IEnumerator EnemySpawnRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while(!_stopSpawning)
        {
            int randomSpawn = Random.Range(0, PossibleSpawns.Length);
            EnemySpawn(PossibleSpawns[randomSpawn]);
            yield return new WaitForSeconds(_enemySpawnRate);
        }
    }

    void EnemySpawn(SpawnData spawnData)
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
        newEnemy.GetComponent<Enemy>().SetupEnemy(spawnData);
    }

    void EnemySpawnFromTop()
    {
        float randomX = Random.Range(-8.0f, 8.0f);
        Vector3 enemyPos = new Vector3(randomX, 7.0f, 0.0f);
        GameObject newEnemy = Instantiate(_enemyPrefab, enemyPos, Quaternion.identity);
        newEnemy.transform.parent = _enemyContainer.transform;
    }

    void EnemySpawnFromTopLeft()
    {
        float randomX = Random.Range(-8.0f, 0.0f);
        Vector3 enemyPos = new Vector3(randomX, 7.0f, 0.0f);
        GameObject newEnemy = Instantiate(_enemyPrefab, enemyPos, Quaternion.identity);
        newEnemy.transform.Rotate(new Vector3(0f, 0f, 40f));
        newEnemy.transform.parent = _enemyContainer.transform;
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
}
