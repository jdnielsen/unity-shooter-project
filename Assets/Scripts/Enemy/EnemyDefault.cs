using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefault : EnemyBase
{
    private Animator _animator;

    // Start is called before the first frame update
    protected override void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL.");
        }

        _forwardSpeed = 4f;
        _strafeSpeed = 2f;
        _rotationSpeed = 12f;

        _deathAnimationTime = 2.8f;

        _minTimeToNextAttack = 3f;
        _maxTimeToNextAttack = 7f;
        _minTimeToNextFire = 2f;

        _nextAttack = Time.time + TimeToNextAttack();
        _nextFire = Time.time + TimeToNextFire();

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void HandleAttack()
    {
        base.HandleAttack();

        if (Time.time > _nextAttack)
        {
            FireLaser();
        }
        else if (Time.time > _nextFire)
        {
            if (_player != null)
            {
                if (IsPointedAtPossibleTarget(true, .95f))
                {
                    FireLaser();
                }
            }
        }
    }

    protected override void HandleMove()
    {
        base.HandleMove();

        switch (_movementPattern)
        {
            case MovementPattern.ForwardOnly:
                MoveForwardOnly();
                break;
            case MovementPattern.ChasePlayer:
                MoveChasePlayer();
                break;
            case MovementPattern.TurnToBottom:
                MoveTurnToBottom();
                break;
            case MovementPattern.Strafing:
                MoveStrafing();
                break;
            default:
                MoveForwardOnly();
                break;
        }
    }

    protected override void OnDeath()
    {
        _animator.SetTrigger("OnEnemyDeath");
        base.OnDeath();
    }

    void FireLaser()
    {
        GameObject enemyLaser = Instantiate(_enemyAttackPrefab, transform.position, transform.rotation);
        enemyLaser.transform.Rotate(new Vector3(0f, 0f, 180f));

        _audioSource.clip = _enemyAttackSoundClip;
        _audioSource.Play();

        _nextAttack = Time.time + TimeToNextAttack();
        _nextFire = Time.time + TimeToNextFire();
    }
}
