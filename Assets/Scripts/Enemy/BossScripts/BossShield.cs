using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShield : MonoBehaviour
{
    int _shieldStrength = 0;
    int _maxShieldStrength = 5;
    float _shieldRechargeTime = 5f;
    float _nextRecharge;
    float _shieldInvulnerableTime = .5f;
    float _nextShieldVulnerable;

    bool _isActivated;
    
    AudioSource _audioSource;
    CircleCollider2D _collider;
    SpriteRenderer _renderer;
    Color _shieldColor;
    float _shieldAlpha;

    [SerializeField]
    AudioClip _shieldDamageSoundClip;
    [SerializeField]
    AudioClip _shieldChargeSoundClip;
    [SerializeField]
    AudioClip _shieldDischargeSoundClip;

    public int ShieldStrength
    {
        get
        {
            return _shieldStrength;
        }

        set
        {
            _shieldStrength = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource is NULL.");
        }

        _collider = GetComponent<CircleCollider2D>();
        if (_collider == null)
        {
            Debug.LogError("CircleCollider2D is NULL.");
        }

        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
        {
            Debug.LogError("SpriteRenderer is NULL.");
        }

        _shieldColor = _renderer.color;
        //_shieldAlpha = _renderer.color.a;
        //_renderer.material.color = new Color(_shieldColor.r, _shieldColor.g, _shieldColor.b, 0f);

        _nextRecharge = Time.time;
        _nextShieldVulnerable = Time.time;
        _isActivated = false;
        _collider.enabled = false;
        _renderer.color = new Color(_shieldColor.r, _shieldColor.g, _shieldColor.b, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _nextRecharge && ShieldStrength <= 0 && _isActivated)
        {
            ChargeShield(true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (Time.time > _nextShieldVulnerable)
            {
                TakeDamage();
            }
        }
    }

    //void OnTriggerStay2D(Collider2D other)
    //{
    //    if (other.tag == "Laser")
    //    {
    //        Destroy(other.gameObject);
    //    }
    //}

    void TakeDamage()
    {
        ShieldStrength--;

        _audioSource.clip = _shieldDamageSoundClip;
        _audioSource.Play();

        if (ShieldStrength <= 0)
        {
            _nextRecharge = Time.time + _shieldRechargeTime;
            ChargeShield(false);
        }
    }

    public void Activate(bool value)
    {
        _isActivated = value;
        ChargeShield(value);
    }

    void ChargeShield(bool value)
    {
        _collider.enabled = value;
        StartCoroutine(ShieldChargeRoutine(value));
        if (value)
        {
            _nextShieldVulnerable = Time.time + _shieldInvulnerableTime;
            ShieldStrength = _maxShieldStrength;
            _audioSource.clip = _shieldChargeSoundClip;
        }
        else
        {
            _audioSource.clip = _shieldDischargeSoundClip;
        }
        _audioSource.Play();
    }

    IEnumerator ShieldChargeRoutine(bool isCharging)
    {
        float step = 0.03f;
        float targetAlpha = .4f;

        if (isCharging)
        {
            while (_renderer.color.a < targetAlpha)
            {
                _renderer.color = new Color(_shieldColor.r, _shieldColor.g, _shieldColor.b, 
                                                     _renderer.color.a + step);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            while (_renderer.color.a > 0f)
            {
                _renderer.color = new Color(_shieldColor.r, _shieldColor.g, _shieldColor.b,
                                                     _renderer.color.a - step);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
