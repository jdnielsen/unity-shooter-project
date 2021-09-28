using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySmart : EnemyBase
{
    [SerializeField]
    private GameObject _forwardLeftLaser;
    [SerializeField]
    private GameObject _forwardRightLaser;
    [SerializeField]
    private GameObject _rearLeftLaser;
    [SerializeField]
    private GameObject _rearRightLaser;

    // Start is called before the first frame update
    protected override void Start()
    {
        _pointValue = 25;

        _forwardSpeed = 4f;
        _strafeSpeed = 2f;
        _rotationSpeed = 12f;

        _deathAnimationTime = .25f;

        _minTimeToNextAttack = 4f;
        _maxTimeToNextAttack = 6f;
        _minTimeToNextFire = 2f;

        _nextAttack = Time.time + TimeToNextAttack();
        _nextFire = Time.time + 1f;

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void HandleAttack()
    {
        if (Time.time > _nextAttack)
        {
            ForwardAttack();
        }
        else if (Time.time > _nextFire)
        {
            if (_player != null)
            {
                if (GameManager.ObjectRoughlyPointedAtTarget(transform.up, transform.position, _player.transform.position, .92f))
                {
                    ForwardAttack();
                }
                else if (GameManager.ObjectRoughlyPointedAtTarget(-transform.up, transform.position, _player.transform.position, .92f))
                {
                    RearAttack();
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
        Instantiate(_enemyDeathPrefab, transform.position, transform.rotation);
        base.OnDeath();
    }

    void ForwardAttack()
    {
        GameObject forwardRightLaser = Instantiate(_enemyAttackPrefab, _forwardRightLaser.transform.position, transform.rotation);
        forwardRightLaser.transform.Rotate(new Vector3(0f, 0f, 180f));
        GameObject forwardLeftLaser = Instantiate(_enemyAttackPrefab, _forwardLeftLaser.transform.position, transform.rotation);
        forwardLeftLaser.transform.Rotate(new Vector3(0f, 0f, 180f));

        PlayAttackSound();

        _nextAttack = Time.time + TimeToNextAttack();
        _nextFire = Time.time + TimeToNextFire();
    }

    void RearAttack()
    {
        GameObject rearRightLaser = Instantiate(_enemyAttackPrefab, _rearRightLaser.transform.position, transform.rotation);
        GameObject rearLeftLaser = Instantiate(_enemyAttackPrefab, _rearLeftLaser.transform.position, transform.rotation);

        PlayAttackSound();

        _nextAttack = Time.time + TimeToNextAttack();
        _nextFire = Time.time + TimeToNextFire();
    }

    void PlayAttackSound()
    {
        _audioSource.clip = _enemyAttackSoundClip;
        _audioSource.Play();
    }
}
