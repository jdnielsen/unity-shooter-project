using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretBase : EnemyBase
{
    bool _isActivated;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Activate(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!_isDead && _isActivated)
        {
            HandleMove();
            HandleAttack();
        }
    }

    protected override void HandleMove()
    {
        base.HandleMove();
    }

    protected override void HandleAttack()
    {
        base.HandleAttack();
    }

    protected override void OnDeath()
    {
        Instantiate(_enemyDeathPrefab, transform.position, transform.rotation);
        base.OnDeath();
    }

    public void Activate(bool activate)
    {
        _nextAttack = Time.time + TimeToNextAttack();
        _isActivated = activate;
        _collider.enabled = activate;
    }
}
