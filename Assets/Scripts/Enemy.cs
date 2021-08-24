using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementPattern
{
    ForwardOnly = 0,
    ChasePlayer = 1,
    TurnToBottom = 2,
    Strafing = 3
}

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;
    [SerializeField]
    private AudioClip _explosionSoundClip;
    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private GameObject _enemyLaserPrefab;

    private Player _player;
    private Animator _animator;
    private AudioSource _audioSource;
    private BoxCollider2D _collider;
    private SpawnManager _spawnManager;
    private bool _isDead;
    private float _nextFire;

    public SpawnData _spawnData;
    Vector3 _spawnPos;
    MovementPattern _movementPattern = MovementPattern.ForwardOnly;
    // float _currentAngle = 0;
    // float _angleChange = 5f;
    float _strafeSpeed = 2f;
    float _rotationSpeed = 12f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL.");
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
        _nextFire = Time.time + Random.Range(3f, 7f);

        _spawnPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -6f || transform.position.y > 12f || 
            transform.position.x > 12f || transform.position.x < -12f)
        {
            HandleScreenExit();
        }

        if (!_isDead)
        {
            HandleMovement();
            if (Time.time > _nextFire)
            {
                FireLaser();
            }
        }
    }

    public void SetupEnemy(SpawnData spawnData, MovementPattern pattern)
    {
        _spawnData = spawnData;

        //int randomPattern = Random.Range(0, 4);
        //_movementPattern = (MovementPattern)randomPattern;

        _movementPattern = pattern;
    }

    void HandleMovement()
    {
        switch (_movementPattern)
        {
            case MovementPattern.ForwardOnly:
                MoveForwardOnly();
                break;
            //case MovementPattern.Swerving:
            //    MoveSwerving();
            //    break;
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

    void MoveForwardOnly()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    //void MoveSwerving()
    //{
    //    if (Mathf.Abs(_currentAngle) >= 20f)
    //    {
    //        _angleChange = -_angleChange;
    //    }
    //    _currentAngle += _angleChange * Time.deltaTime;
    //    transform.Rotate(0f, 0f, _angleChange * Time.deltaTime);

    //    transform.Translate(Vector3.down * _speed * Time.deltaTime);
    //}

    void MoveTurnToBottom()
    {
        Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);
        
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    void MoveChasePlayer()
    {
        Vector3 direction = transform.position - _player.transform.position;
        direction.Normalize();

        Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);

        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    void MoveStrafing()
    {
        if (_spawnPos.x < 0)
        {
            transform.Translate(Vector3.right * _strafeSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.right * -_strafeSpeed * Time.deltaTime);
        }
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    void HandleScreenExit()
    {
        float xPos, yPos;
        if (_spawnData.minX == _spawnData.maxX)
        {
            xPos = _spawnData.minX;
        }
        else
        {
            xPos = Random.Range(_spawnData.minX, _spawnData.maxX);
        }

        if (_spawnData.minY == _spawnData.maxY)
        {
            yPos = _spawnData.minY;
        }
        else
        {
            yPos = Random.Range(_spawnData.minY, _spawnData.maxY);
        }

        transform.rotation = Quaternion.identity;
        transform.Rotate(new Vector3(0f, 0f, _spawnData.rotationAngle));

        transform.position = new Vector3(xPos, yPos, transform.position.z);
        _spawnPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
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

                OnDeath();
            }

            if (other.tag == "Laser")
            {
                Destroy(other.gameObject);
                if (_player != null)
                {
                    _player.AddToScore(10);
                }

                OnDeath();
            }
        }
    }

    private void OnDeath()
    {
        _animator.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _isDead = true;
        _collider.enabled = false;
        _audioSource.clip = _explosionSoundClip;
        _audioSource.Play();
        _spawnManager.EnemyDestroyed();
        Destroy(this.gameObject, 2.8f);
    }

    private void FireLaser()
    {
        _nextFire = Time.time + Random.Range(3f, 7f);
        GameObject laser = Instantiate(_enemyLaserPrefab, transform.position, transform.rotation);
        laser.transform.Rotate(new Vector3(0f, 0f, 180f));

        _audioSource.clip = _laserSoundClip;
        _audioSource.Play();
    }

    public bool IsDead()
    {
        return _isDead;
    }
}
