using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAgile : EnemyBase
{
    [SerializeField]
    private GameObject _laserPosition;
    private Transform _container;
    private float _evasionDistance = 2.5f;

    // Start is called before the first frame update
    protected override void Start()
    {
        _container = transform.parent;

        _pointValue = 25;

        _forwardSpeed = 5f;
        _strafeSpeed = 3f;
        _rotationSpeed = 18f;

        _deathAnimationTime = 0.25f;

        _minTimeToNextAttack = 3f;
        _minTimeToNextFire = .25f;
        _minTimeToNextMove = .6f;
        _minFiresInAttack = 2;

        _isMoving = true;
        _isAttacking = false;
        _nextMove = Time.time;
        _nextAttack = Time.time + _minTimeToNextAttack;

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

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        
    }

    protected override void HandleAttack()
    {
        if (_isAttacking && Time.time > _nextAttack)
        {
            StartCoroutine(FireBarrage());
        }
    }

    IEnumerator FireBarrage()
    {
        int count = 0;
        while (count < _minFiresInAttack)
        {
            FireLaser();
            count++;
            yield return new WaitForSeconds(_minTimeToNextFire);
        }
        _nextAttack = Time.time + _minTimeToNextAttack;
        _isAttacking = false;
    }

    protected override void HandleMove()
    {
        if (Time.time > _nextMove)
        {
            if (transform.position.y > _targetPosition.y)
            {
                _container.Translate(Vector3.down * _forwardSpeed * Time.deltaTime);
            }
            else
            {
                TrackPlayer();
                if (Vector3.Distance(transform.position, _targetPosition) > 0.1f)
                {
                    float direction = Mathf.Sign(_targetPosition.x - transform.position.x);
                    _container.Translate(Vector3.right * _strafeSpeed * Time.deltaTime * direction, Space.World);
                }
                else if (IsPointedAtPossibleTarget(true, .93f) && Time.time > _nextAttack)
                {
                    _isAttacking = true;
                    _nextMove = Time.time + _minTimeToNextMove;
                }
            }
        }
    }

    protected override void OnDeath()
    {
        Instantiate(_enemyDeathPrefab, transform.position, transform.rotation);
        base.OnDeath();
        Destroy(_container.gameObject, _deathAnimationTime);
    }

    public void DamageTrigger(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    public void DetectionTrigger(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            List<float> possibleEvasions = new List<float>();
            if ((transform.position.x + _evasionDistance) <= _maxTargetX)
            {
                possibleEvasions.Add(_evasionDistance);
            }
            if ((transform.position.x - _evasionDistance) >= _minTargetX)
            {
                possibleEvasions.Add(-_evasionDistance);
            }
            float evasion = possibleEvasions[Random.Range(0, possibleEvasions.Count)];

            _targetPosition.x = transform.position.x + evasion;
        }
    }

    void FireLaser()
    {
        GameObject enemyLaser = Instantiate(_enemyAttackPrefab, _laserPosition.transform.position, transform.rotation);
        enemyLaser.transform.Rotate(new Vector3(0f, 0f, 180f));

        _audioSource.clip = _enemyAttackSoundClip;
        _audioSource.Play();

        _nextAttack = Time.time + TimeToNextAttack();
        _nextFire = Time.time + TimeToNextFire();
    }
}
