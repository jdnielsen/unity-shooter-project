using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggressive : EnemyBase
{
    float _rammingSpeed = 8f;
    float _normalSpeed = 4f;
    float _rammingDistance = 9f;

    [SerializeField]
    GameObject _thruster;

    // Start is called before the first frame update
    protected override void Start()
    {
        _pointValue = 15;

        _forwardSpeed = _normalSpeed;
        _rotationSpeed = 20f;

        _isAttacking = false;

        _deathAnimationTime = 0.25f;

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void HandleAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (IsPointedAtPlayer() && (distanceToPlayer < _rammingDistance))
        {
            _isAttacking = true;
            _forwardSpeed = _rammingSpeed;
            _thruster.SetActive(true);
        }
        else if (distanceToPlayer > _rammingDistance)
        {
            _isAttacking = false;
            _forwardSpeed = _normalSpeed;
            _thruster.SetActive(false);
        }
    }

    protected override void HandleMove()
    {
        if (!_isAttacking)
        {
            MoveChasePlayer();
        }
        else
        {
            MoveForwardOnly();
        }
    }

    protected override void OnDeath()
    {
        Instantiate(_enemyDeathPrefab, transform.position, transform.rotation);
        base.OnDeath();
    }
}
