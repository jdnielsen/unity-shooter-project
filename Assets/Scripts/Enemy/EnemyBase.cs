using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Default,
    Alternate
}

public enum MovementPattern
{
    ForwardOnly,
    ChasePlayer,
    TurnToBottom,
    Strafing
}

public abstract class EnemyBase : MonoBehaviour
{
    // screen boundaries
    protected const float minX = -12f, maxX = 12f, minY = -6f, maxY = 12f;

    protected Player _player;
    protected AudioSource _audioSource;
    protected Collider2D _collider;
    protected SpawnManager _spawnManager;

    // sound clips
    [SerializeField]
    protected AudioClip _explosionSoundClip;
    [SerializeField]
    protected AudioClip _enemyAttackSoundClip;
    [SerializeField]
    protected GameObject _enemyAttackPrefab;
    [SerializeField]
    protected GameObject _enemyDeathPrefab;

    // remaining times
    protected float _nextMove;
    protected float _nextFire;
    protected float _nextAttack;

    // movement
    protected MovementPattern _movementPattern;
    protected bool _isMoving;

    protected float _forwardSpeed;
    protected float _strafeSpeed;
    protected float _rotationSpeed;

    protected float _minTimeToNextMove;
    protected float _maxTimeToNextMove;

    // attack
    protected bool _isAttacking;

    protected float _minTimeToNextAttack;
    protected float _maxTimeToNextAttack;
    protected float _minTimeToNextFire;
    protected float _maxTimeToNextFire;
    protected int _minFiresInAttack;
    protected int _maxFiresInAttack;

    // enemy health and death
    protected int _enemyHealth = 1;
    protected bool _isDead;
    protected float _deathAnimationTime;

    // spawn
    protected SpawnData _spawnData;
    protected Vector3 _spawnPosition;

    // target position
    protected float _minTargetX;
    protected float _maxTargetX;
    protected float _minTargetY;
    protected float _maxTargetY;
    protected Vector3 _targetPosition;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource is NULL.");
        }

        _collider = GetComponent<BoxCollider2D>();
        if (_collider == null)
        {
            Debug.LogError("BoxCollider2D is NULL.");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL.");
        }

        _isDead = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (transform.position.x < minX || transform.position.x > maxX || 
            transform.position.y < minY || transform.position.y > maxY)
        {
            ResetPosition();
        }
        if (!_isDead)
        {
            HandleMove();
            HandleAttack();
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isDead)
        {
            if (other.tag == "Player")
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage();
                }

                TakeDamage();
            }

            if (other.tag == "Laser")
            {
                Destroy(other.gameObject);
                if (_player != null)
                {
                    _player.AddToScore(10);
                }

                TakeDamage();
            }
        }
    }

    public void SetupEnemy(SpawnData spawnData, MovementPattern pattern)
    {
        _spawnData = spawnData;
        _movementPattern = pattern;
    }

    protected void ResetPosition()
    {
        if (_spawnData == null)
        {
            _spawnData = new SpawnData();
        }

        float xPos, yPos;
        if (_spawnData.minX >= _spawnData.maxX)
        {
            xPos = _spawnData.minX;
        }
        else
        {
            xPos = Random.Range(_spawnData.minX, _spawnData.maxX);
        }

        if (_spawnData.minY >= _spawnData.maxY)
        {
            yPos = _spawnData.minY;
        }
        else
        {
            yPos = Random.Range(_spawnData.minY, _spawnData.maxY);
        }

        transform.rotation = Quaternion.identity;
        transform.Rotate(new Vector3(0f, 0f, _spawnData.rotationAngle));

        transform.position = new Vector3(xPos, yPos);
        _spawnPosition = transform.position;
    }

    protected virtual void HandleMove()
    {

    }

    protected virtual void HandleAttack()
    {

    }

    protected virtual void TakeDamage()
    {
        _enemyHealth--;
        if (_enemyHealth <= 0)
        {
            OnDeath();
        }
    }

    protected virtual void OnDeath()
    {
        _forwardSpeed = 0;
        _strafeSpeed = 0;
        _rotationSpeed = 0;
        _isDead = true;
        _collider.enabled = false;
        _audioSource.clip = _explosionSoundClip;
        _audioSource.Play();
        _spawnManager.EnemyDestroyed();
        Destroy(this.gameObject, _deathAnimationTime);
    }

    public bool IsDead()
    {
        return _isDead;
    }

    protected void TrackPlayer()
    {
        if (_player == null)
        {
            return;
        }

        Vector3 direction = transform.position - _player.transform.position;
        direction.Normalize();

        Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);
    }

    protected bool IsPointedAtPlayer()
    {
        if (_player == null)
        {
            return false;
        }

        Vector3 direction = transform.position - _player.transform.position;
        direction.Normalize();
        return transform.up == direction;
    }

    protected float TimeToNextMove()
    {
        if (_minTimeToNextMove >= _maxTimeToNextMove)
        {
            return _minTimeToNextMove;
        }
        else
        {
            return Random.Range(_minTimeToNextMove, _maxTimeToNextMove);
        }
    }

    protected float TimeToNextAttack()
    {
        if (_minTimeToNextAttack >= _maxTimeToNextAttack)
        {
            return _minTimeToNextAttack;
        }
        else
        {
            return Random.Range(_minTimeToNextAttack, _maxTimeToNextAttack);
        }
    }

    protected float TimeToNextFire()
    {
        if (_minTimeToNextFire >= _maxTimeToNextFire)
        {
            return _minTimeToNextFire;
        }
        else
        {
            return Random.Range(_minTimeToNextFire, _maxTimeToNextFire);
        }
    }

    protected int NumberOfFiresInAttack()
    {
        if (_minFiresInAttack >= _maxFiresInAttack)
        {
            return _minFiresInAttack;
        }
        else
        {
            return Random.Range(_minFiresInAttack, _maxFiresInAttack + 1);
        }
    }

    protected void RandomizeTargetPosition()
    {
        if (_targetPosition == null)
        {
            _targetPosition = new Vector3();
        }

        float x, y;

        if (_minTargetX >= _maxTargetX)
        {
            x = _minTargetX;
        }
        else
        {
            x = Random.Range(_minTargetX, _maxTargetX);
        }

        if (_minTargetY >= _maxTargetY)
        {
            y = _minTargetY;
        }
        else
        {
            y = _maxTargetY;
        }

        _targetPosition.x = x;
        _targetPosition.y = y;
    }
}
