using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInWave
{
    public int EnemiesInWave { get; set; }

    public int MinEnemiesInSpawn { get; set; }

    public int MaxEnemiesInSpawn { get; set; }

    public SpawnData[] PossibleSpawns { get; set; }

    public MovementPattern[] PossibleMovementPatterns { get; set; }

    public EnemyType EnemyType { get; set; }

    public float EntryDelay { get; set; }

    public EnemyInWave(int numEnemies, int minEnemiesSpawn, int maxEnemiesSpawn, SpawnData[] spawns, 
                           MovementPattern[] patterns, EnemyType type = EnemyType.Default, float entryDelay = 0f)
    {
        EnemiesInWave = numEnemies;
        MinEnemiesInSpawn = minEnemiesSpawn;
        MaxEnemiesInSpawn = maxEnemiesSpawn;
        PossibleSpawns = spawns;
        PossibleMovementPatterns = patterns;
        EnemyType = type;
        EntryDelay = entryDelay;
    }
}

public class Wave
{
    public EnemyInWave[] EnemiesInWave { get; set; }

    public string WaveEnterMessage { get; set; }

    public string WaveExitMessage { get; set; }

    public float EnterMessageDuration { get; set; }

    public float ExitMessageDuration { get; set; }

    public Wave(EnemyInWave[] enemiesInWave, string enterMessage = "", string exitMessage = "", 
                float enterDuration = 0f, float exitDuration = 0f)
    {
        EnemiesInWave = enemiesInWave;
        WaveEnterMessage = enterMessage;
        WaveExitMessage = exitMessage;
        EnterMessageDuration = enterDuration;
        ExitMessageDuration = exitDuration;
    }
}

