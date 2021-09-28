using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlternate : EnemyBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        _pointValue = 25;

        _forwardSpeed = 4f;
        _strafeSpeed = 2f;
        _rotationSpeed = 12f;

        _deathAnimationTime = 0.25f;

        _isMoving = true;
        _isAttacking = false;

        _minTargetY = Random.Range(3f, 5f);
        _minTargetX = -9f;
        _maxTargetX = 9f;

        _targetPosition = new Vector3(transform.position.x, _minTargetY);

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void HandleAttack()
    {
        if (_isAttacking && Time.time > _nextAttack)
        {
            GameObject enemyBeam = Instantiate(_enemyAttackPrefab, transform.position, transform.rotation);
            enemyBeam.transform.parent = transform;

            _audioSource.clip = _enemyAttackSoundClip;
            _audioSource.Play();

            _isAttacking = false;
            RandomizeTargetPosition();
            _isMoving = true;
            _nextMove = Time.time + 3.5f;
        }
    }

    protected override void HandleMove()
    {
        if (_isMoving && Time.time > _nextMove)
        {
            if (transform.position.y > _targetPosition.y)
            {
                transform.Translate(Vector3.down * _forwardSpeed * Time.deltaTime);
            }
            else 
            {
                TrackPlayer();
                if (Vector3.Distance(transform.position, _targetPosition) > 0.1f)
                {
                    float direction = Mathf.Sign(_targetPosition.x - transform.position.x);
                    transform.Translate(Vector3.right * _strafeSpeed * Time.deltaTime * direction, Space.World);
                }
                else if (IsPointedAtPlayer())
                {
                    _isMoving = false;
                    _isAttacking = true;
                    _nextAttack = Time.time + 0.25f;
                }
            }
        }
    }

    protected override void OnDeath()
    {
        Instantiate(_enemyDeathPrefab, transform.position, transform.rotation);
        base.OnDeath();
    }
}
