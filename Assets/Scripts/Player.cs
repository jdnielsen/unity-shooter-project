using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _score = 0;
    private AudioSource _audioSource;
    // spawn manager
    private SpawnManager _spawnManager;
    // ui manager
    private UIManager _uiManager;
    // prefabs
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    // laser variables
    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private float _laserOffset = 1.0f;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _nextFire = 0.0f;
    // shield variables
    [SerializeField]
    private GameObject _shield;
    private Renderer _shieldRenderer;
    [SerializeField]
    private int _shieldStrength = 0;
    // damage variables
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject _rightEngineFire;
    [SerializeField]
    private GameObject _leftEngineFire;
    //powerup variables
    [SerializeField]
    private AudioClip _powerupSoundClip;
    [SerializeField]
    private float _powerupActiveTime = 5.0f;
    private float _tripleShotRemainingTime = 0f;
    private float _speedBoostRemainingTime = 0f;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private float _speedMultiplier = 2;
    // thrusters
    [SerializeField]
    private float _thrusterSpeedIncrease = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        // spawn manager
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL.");
        }

        // ui manager
        _uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL.");
        }

        // shield renderer
        if (_shield == null)
        {
            Debug.LogError("Player Shield is NULL.");
        }
        _shieldRenderer = _shield.GetComponent<Renderer>();
        if (_shieldRenderer == null)
        {
            Debug.LogError("Player Shield Renderer is NULL.");
        }
        _shieldRenderer.material.color = new Color(1f, 1f, 1f, 0f);

        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        // direction
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        // speed
        float _adjustedSpeed = _speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _adjustedSpeed += _thrusterSpeedIncrease;
        }

        if (_isSpeedBoostActive)
        {
            transform.Translate(direction * _adjustedSpeed * _speedMultiplier * Time.deltaTime);
        }
        else
        {
            transform.Translate(direction * _adjustedSpeed * Time.deltaTime);
        }

        // game boundaries
        transform.position = new Vector3(transform.position.x, 
                                    Mathf.Clamp(transform.position.y, -3.8f, 0), 
                                    transform.position.z);

        if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, transform.position.z);
        }
    }

    void FireLaser()
    {
        _nextFire = Time.time + _fireRate;
        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab,
                transform.position,
                Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab,
                transform.position + new Vector3(0, _laserOffset, 0),
                Quaternion.identity);
        }
        _audioSource.clip = _laserSoundClip;
        _audioSource.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyLaser")
        {
            Destroy(other.gameObject);
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (_shieldStrength > 0)
        {
            ShieldTakeDamage();
            return; 
        }

        _lives--;
        _uiManager.UpdateLives(_lives);

        switch (_lives)
        {
            case 2:
                _rightEngineFire.SetActive(true);
                break;
            case 1:
                _leftEngineFire.SetActive(true);
                break;
            case 0:
                _spawnManager.OnPlayerDeath();
                Instantiate(_explosionPrefab, transform.position, transform.rotation);
                Destroy(this.gameObject);
                break;
            default:
                break;
        }
    }

    public void TripleShotActivate()
    {
        _tripleShotRemainingTime = _powerupActiveTime;
        PlayPowerupSound();
        if (!_isTripleShotActive)
        {
            _isTripleShotActive = true;
            StartCoroutine(TripleShotDeactivateRoutine());
        }
    }

    IEnumerator TripleShotDeactivateRoutine()
    {
        while (_tripleShotRemainingTime > 0)
        {
            yield return new WaitForSeconds(1f);
            _tripleShotRemainingTime = _tripleShotRemainingTime - 1f;
        }
        _isTripleShotActive = false;
    }

    public void SpeedBoostActivate()
    {
        _speedBoostRemainingTime = _powerupActiveTime;
        PlayPowerupSound();
        if (!_isSpeedBoostActive)
        {
            _isSpeedBoostActive = true;
            StartCoroutine(SpeedBoostDeactivateRoutine());
        }
    }

    IEnumerator SpeedBoostDeactivateRoutine()
    {
        while (_speedBoostRemainingTime > 0)
        {
            yield return new WaitForSeconds(1f);
            _speedBoostRemainingTime = _speedBoostRemainingTime - 1f;
        }
        _isSpeedBoostActive = false;
    }

    public void ShieldActivate()
    {
        _shieldStrength = 3;
        StartCoroutine(ChangeShieldAppearanceCoroutine(true));
        _shield.SetActive(true);
        PlayPowerupSound();
    }

    public void ShieldTakeDamage()
    {
        _shieldStrength--;
        StartCoroutine(ChangeShieldAppearanceCoroutine(false));
        if (_shieldStrength <= 0)
        {
            _shield.SetActive(false);
        }
    }

    IEnumerator ChangeShieldAppearanceCoroutine(bool isRestoringShield)
    {
        float targetAlpha = _shieldStrength * 0.333f;
        float step = 0.03f;

        if (isRestoringShield)
        {
            while (_shieldRenderer.material.color.a < targetAlpha)
            {
                _shieldRenderer.material.color = new Color(1f, 1f, 1f, _shieldRenderer.material.color.a + step);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            while (_shieldRenderer.material.color.a > targetAlpha)
            {
                _shieldRenderer.material.color = new Color(1f, 1f, 1f, _shieldRenderer.material.color.a - step);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    void PlayPowerupSound()
    {
        _audioSource.clip = _powerupSoundClip;
        _audioSource.Play();
    }

    public void AddToScore(int n)
    {
        _score += n;
        _uiManager.UpdateScore(_score);
    }

    public int GetScore()
    {
        return _score;
    }
}
