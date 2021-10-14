using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretMissile : EnemyTurretBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        _pointValue = 50;

        _rotationSpeed = 15f; 
        
        _minTimeToNextAttack = 3f;
        _minTimeToNextAttack = 5f;
        _nextAttack = Time.time + TimeToNextAttack();

        _enemyHealth = 5;
        _deathAnimationTime = 0.25f;

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void HandleMove()
    {
        TrackPlayer();
        base.HandleMove();
    }

    protected override void HandleAttack()
    {
        if (Time.time > _nextAttack)
        {
            FireMissile();
        }
    }

    void FireMissile()
    {
        GameObject enemyMissile = Instantiate(_enemyAttackPrefab, transform.position, transform.rotation);
        enemyMissile.transform.Rotate(new Vector3(0f, 0f, 180f));

        _audioSource.clip = _enemyAttackSoundClip;
        _audioSource.Play();

        _nextAttack = Time.time + TimeToNextAttack();
    }
}
