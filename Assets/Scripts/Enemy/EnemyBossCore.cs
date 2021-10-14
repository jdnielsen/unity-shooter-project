using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossCore : EnemyBase
{
    [SerializeField]
    GameObject _enemyBoss;

    // Start is called before the first frame update
    protected override void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }

        _collider = GetComponent<CircleCollider2D>();
        if (_collider == null)
        {
            Debug.LogError("CircleCollider2D is NULL.");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource is NULL.");
        }

        _renderer = transform.parent.GetComponent<SpriteRenderer>();
        if (_renderer == null)
        {
            Debug.LogError("Sprite Renderer is NULL.");
        }

        _pointValue = 1000;
        _isDead = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isDead)
        {
            if (other.tag == "Laser")
            {
                Destroy(other.gameObject);

                TakeDamage();
            }
        }
    }

    protected override void OnDeath()
    {
        Instantiate(_enemyDeathPrefab, transform.position, transform.rotation);

        if (_player != null)
        {
            _player.AddToScore(_pointValue);
        }

        _isDead = true;
        _collider.enabled = false;
        Destroy(this.gameObject);
    }
}
