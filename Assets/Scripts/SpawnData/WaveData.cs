using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveData
{
    int _enemiesRemaining;
    int _enemiesInWave;
    int _minEnemiesInSpawn;
    int _maxEnemiesInSpawn;
    SpawnData[] _possibleSpawns;
    MovementPattern[] _possiblePatterns;
    string _waveEnterMessage;
    string _waveExitMessage;

    public int EnemiesRemaining
    {
        get { return _enemiesRemaining; }
        set { _enemiesRemaining = value; }
    }

    public int EnemiesInWave
    {
        get { return _enemiesInWave; }
        set { _enemiesInWave = value; }
    }

    public int MinEnemiesInSpawn
    {
        get { return _minEnemiesInSpawn; }
        set { _minEnemiesInSpawn = value; }
    }

    public int MaxEnemiesInSpawn
    {
        get { return _maxEnemiesInSpawn; }
        set { _maxEnemiesInSpawn = value; }
    }

    public SpawnData[] PossibleSpawns
    {
        get { return _possibleSpawns; }
        set { _possibleSpawns = value; }
    }

    public MovementPattern[] PossibleMovementPatterns
    {
        get { return _possiblePatterns; }
        set { _possiblePatterns = value; }
    }

    public string WaveEnterMessage
    {
        get { return _waveEnterMessage; }
        set { _waveEnterMessage = value; }
    }

    public string WaveExitMessage
    {
        get { return _waveExitMessage; }
        set { _waveExitMessage = value; }
    }

    public WaveData(SpawnData[] possibleSpawnLocations, int minEnemiesInWave, int maxEnemiesInWave,
                    int minEnemiesInSpawn, int maxEnemiesInSpawn, MovementPattern[] possiblePatterns, 
                    string waveEnterMessage, string waveExitMessage)
    {
        // spawn locations
        PossibleSpawns = possibleSpawnLocations;

        // movement patterns
        PossibleMovementPatterns = possiblePatterns;

        // enemies in entire wave
        if (maxEnemiesInWave > minEnemiesInWave)
        {
            EnemiesInWave = Random.Range(minEnemiesInWave, maxEnemiesInWave + 1);
        }
        else
        {
            EnemiesInWave = minEnemiesInWave;
        }

        // enemies in spawn
        MinEnemiesInSpawn = minEnemiesInSpawn;
        MaxEnemiesInSpawn = maxEnemiesInSpawn;

        EnemiesRemaining = EnemiesInWave;

        // ui messages
        WaveEnterMessage = waveEnterMessage;
        WaveExitMessage = waveExitMessage;
    }
}
