using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretBeam : EnemyTurretBase
{
    [SerializeField]
    GameObject _beamFocus;

    // Start is called before the first frame update
    protected override void Start()
    {
        _pointValue = 100;

        _rotationSpeed = 20f;

        _minTimeToNextMove = 4f;
        _minTimeToNextAttack = .25f;
        _nextAttack = Time.time + TimeToNextAttack();
        _nextMove = Time.time;
        _isMoving = true;

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
        if (_isMoving && Time.time > _nextMove)
        {
            TrackPlayer();
            if (IsPointedAtPossibleTarget(true, .995f))
            {
                _isMoving = false;
                _isAttacking = true;
                _nextAttack = Time.time + 0.25f;
            }
        }
    }

    protected override void HandleAttack()
    {
        if (_isAttacking && Time.time > _nextAttack)
        {
            FireBeam();

            _isAttacking = false;
            _isMoving = true;
            _nextMove = Time.time + 4f;
        }
    }

    void FireBeam()
    {
        GameObject enemyBeam = Instantiate(_enemyAttackPrefab, _beamFocus.transform.position, transform.rotation);
        enemyBeam.transform.parent = transform;

        _audioSource.clip = _enemyAttackSoundClip;
        _audioSource.Play();
    }

    protected override void TakeDamage()
    {
        base.TakeDamage();
    }
}
