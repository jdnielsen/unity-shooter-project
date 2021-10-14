using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretLaser : EnemyTurretBase
{
    [SerializeField]
    GameObject _rightLaser;
    [SerializeField]
    GameObject _leftLaser;

    // Start is called before the first frame update
    protected override void Start()
    {
        if (_rightLaser == null)
        {
            Debug.Log("Turret right laser is NULL.");
        }
        if (_leftLaser == null)
        {
            Debug.Log("Turret left laser is NULL.");
        }

        _pointValue = 50;

        _rotationSpeed = 15f;

        _minTimeToNextAttack = 2f;
        _maxTimeToNextAttack = 4f;
        _nextAttack = Time.time + TimeToNextAttack();

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
        if (Time.time > _nextAttack && IsPointedAtPossibleTarget(true, .95f))
        {
            FireLaser();
        }
    }

    void FireLaser()
    {
        GameObject forwardRightLaser = Instantiate(_enemyAttackPrefab, _rightLaser.transform.position, transform.rotation);
        forwardRightLaser.transform.Rotate(new Vector3(0f, 0f, 180f));
        GameObject forwardLeftLaser = Instantiate(_enemyAttackPrefab, _leftLaser.transform.position, transform.rotation);
        forwardLeftLaser.transform.Rotate(new Vector3(0f, 0f, 180f));

        _audioSource.clip = _enemyAttackSoundClip;
        _audioSource.Play();

        _nextAttack = Time.time + TimeToNextAttack();
    }
}
