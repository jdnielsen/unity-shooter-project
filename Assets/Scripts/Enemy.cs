using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
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
    private bool _isDead;
    private float _nextFire;

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

        _isDead = false;
        _nextFire = Time.time + Random.Range(3f, 7f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.0f)
        {
            float randomX = Random.Range(-8.0f, 8.0f);
            transform.position = new Vector3(randomX, 7.0f, transform.position.z);
        }

        if (!_isDead && Time.time > _nextFire)
        {
            FireLaser();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
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

    private void OnDeath()
    {
        _animator.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _isDead = true;
        _collider.enabled = false;
        _audioSource.clip = _explosionSoundClip;
        _audioSource.Play();
        Destroy(this.gameObject, 2.8f);
    }

    private void FireLaser()
    {
        _nextFire = Time.time + Random.Range(3f, 7f);
        Instantiate(_enemyLaserPrefab, transform.position, new Quaternion(0, 0, 180, 0));
        _audioSource.clip = _laserSoundClip;
        _audioSource.Play();
    }
}
