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
    // camera
    private GameCamera _camera;
    // prefabs
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _missilePrefab;
    // missile variables
    [SerializeField]
    private AudioClip _missileSoundClip;
    // laser variables
    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioClip _laserFailSoundClip;
    [SerializeField]
    private float _laserOffset = 1.0f;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _nextFire = 0.0f;
    [SerializeField]
    private int _maxAmmo = 15;
    [SerializeField]
    private int _currentAmmo;
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
    private float _nextDamage = 0f;
    //powerup variables
    [SerializeField]
    private AudioClip _powerupSoundClip;
    [SerializeField]
    private AudioClip _powerdownSoundClip;
    [SerializeField]
    private float _powerupActiveTime = 5.0f;
    private float _tripleShotRemainingTime = 0f;
    private float _speedBoostRemainingTime = 0f;
    private float _missileRemainingTime = 0f;
    [SerializeField]
    private float _speedMultiplier = 2;
    // thrusters
    [SerializeField]
    private float _thrusterSpeedIncrease = 2.0f;
    [SerializeField]
    private float _thrusterEnergyMax = 6f;
    [SerializeField]
    private float _thrusterEnergyCurrent;

    GameObject asteroid;

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

        // camera
        _camera = GameObject.Find("Main Camera").GetComponent<GameCamera>();
        if (_camera == null)
        {
            Debug.LogError("Camera is NULL.");
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

        // audio source
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Audio Source is NULL.");
        }

        // ammo
        _currentAmmo = _maxAmmo;
        // thrusters
        _thrusterEnergyCurrent = _thrusterEnergyMax;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        FireLaser();
    }

    void HandleMovement()
    {
        // direction
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        // speed
        float _adjustedSpeed = _speed;
        if (Input.GetKey(KeyCode.LeftShift) && _thrusterEnergyCurrent > 0f)
        {
            _adjustedSpeed += _thrusterSpeedIncrease;
            _thrusterEnergyCurrent = Mathf.MoveTowards(_thrusterEnergyCurrent, 0f, 50f * Time.deltaTime);
        }
        else
        {
            _thrusterEnergyCurrent = Mathf.MoveTowards(_thrusterEnergyCurrent, _thrusterEnergyMax, 20f * Time.deltaTime);
        }
        _uiManager.UpdateThruster(_thrusterEnergyCurrent, _thrusterEnergyMax);

        if (_speedBoostRemainingTime > 0f)
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
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            _nextFire = Time.time + _fireRate;
            // homing missile
            if (_missileRemainingTime > 0f)
            {
                Instantiate(_missilePrefab,
                    transform.position + new Vector3(0, _laserOffset, 0),
                    Quaternion.identity);
                _audioSource.clip = _missileSoundClip;
                _audioSource.Play();
            }
            // triple shot
            else if (_tripleShotRemainingTime > 0f)
            {
                Instantiate(_tripleShotPrefab,
                    transform.position,
                    Quaternion.identity);
                _audioSource.clip = _laserSoundClip;
                _audioSource.Play();
            }
            // normal laser
            else if (_currentAmmo > 0)
            {
                Instantiate(_laserPrefab,
                    transform.position + new Vector3(0, _laserOffset, 0),
                    Quaternion.identity);
                _audioSource.clip = _laserSoundClip;
                _audioSource.Play();
                _currentAmmo--;
                _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);
            }
            else
            {
                _audioSource.clip = _laserFailSoundClip;
                _audioSource.Play();
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyLaser")
        {
            Destroy(other.gameObject);
            TakeDamage();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {

        if (other.tag == "EnemyBeam" && Time.time > _nextDamage)
        {
            _nextDamage = Time.time + 0.5f;
            TakeDamage();
        }
    }


    public void TakeDamage()
    {
        StartCoroutine(_camera.Shake(.25f, .5f));
        if (_shieldStrength > 0)
        {
            ShieldTakeDamage();
            return; 
        }

        _lives--;
        UpdateHealth();
    }


    void UpdateHealth()
    {
        _uiManager.UpdateLives(_lives);

        switch (_lives)
        {
            case 3:
                _rightEngineFire.SetActive(false);
                _leftEngineFire.SetActive(false);
                break;
            case 2:
                _rightEngineFire.SetActive(true);
                _leftEngineFire.SetActive(false);
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


    public void AmmoPickup()
    {
        PlayPowerupSound();
        StartCoroutine(FillAmmoCoroutine());
    }

    IEnumerator FillAmmoCoroutine()
    {
        while (_currentAmmo < _maxAmmo)
        {
            _currentAmmo++;
            _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void AntiAmmoPickup()
    {
        PlayPowerdownSound();
        StartCoroutine(DrainAmmoCoroutine());
    }

    IEnumerator DrainAmmoCoroutine()
    {
        while (_currentAmmo > 0)
        {
            _currentAmmo--;
            _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);
            yield return new WaitForSeconds(0.05f);
        }
    }


    public void HealthPickup()
    {
        PlayPowerupSound();
        if (_lives < 3)
        {
            _lives++;
        }
        UpdateHealth();
    }

    public void HomingMissileActivate()
    {
        PlayPowerupSound();
        if (_missileRemainingTime <= 0f)
        {
            StartCoroutine(HomingMissileDeactivateCoroutine());
        }
        else
        {
            _missileRemainingTime = _powerupActiveTime;
        }
    }

    IEnumerator HomingMissileDeactivateCoroutine()
    {
        _missileRemainingTime = _powerupActiveTime;
        while (_missileRemainingTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            _missileRemainingTime = _missileRemainingTime - 1f;
        }
    }

    public void TripleShotActivate()
    {
        PlayPowerupSound();
        if (_tripleShotRemainingTime <= 0f)
        {
            StartCoroutine(TripleShotDeactivateRoutine());
        }
        else
        {
            _tripleShotRemainingTime = _powerupActiveTime;
        }
    }

    IEnumerator TripleShotDeactivateRoutine()
    {
        _tripleShotRemainingTime = _powerupActiveTime;
        while (_tripleShotRemainingTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            _tripleShotRemainingTime = _tripleShotRemainingTime - 1f;
        }
    }

    public void SpeedBoostActivate()
    {
        PlayPowerupSound();
        if (_speedBoostRemainingTime <= 0f)
        {
            StartCoroutine(SpeedBoostDeactivateRoutine());
        }
        else
        {
            _speedBoostRemainingTime = _powerupActiveTime;
        }
    }

    IEnumerator SpeedBoostDeactivateRoutine()
    {
        _speedBoostRemainingTime = _powerupActiveTime;
        while (_speedBoostRemainingTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            _speedBoostRemainingTime = _speedBoostRemainingTime - 1f;
        }
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

    void PlayPowerdownSound()
    {
        _audioSource.clip = _powerdownSoundClip;
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
