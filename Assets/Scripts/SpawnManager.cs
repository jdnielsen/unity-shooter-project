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

    private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        
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
            float randomX = Random.Range(-8.0f, 8.0f);
            Vector3 enemyPos = new Vector3(randomX, 7.0f, 0.0f);
            GameObject newEnemy = Instantiate(_enemyPrefab, enemyPos, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemySpawnRate);
        }
    }

    IEnumerator PowerupSpawnRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (!_stopSpawning)
        {
            float randomX = Random.Range(-8.0f, 8.0f);
            Vector3 powerupPos = new Vector3(randomX, 7.0f, 0.0f);
            int powerupId = Random.Range(0, _powerups.Count);
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
