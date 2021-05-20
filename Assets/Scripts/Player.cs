using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private int _lives = 3;
    // spawn manager reference
    private SpawnManager _spawnManager;
    // prefabs
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    // laser variables
    [SerializeField]
    private float _laserOffset = 1.0f;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _nextFire = 0.0f;
    // shield variables
    [SerializeField]
    private GameObject _shield;
    //powerup variables
    [SerializeField]
    private float _powerupActiveTime = 5.0f;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private float _speedMultiplier = 2;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL.");
        }
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
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        //transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
        //transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        if (_isSpeedBoostActive)
        {
            transform.Translate(direction * _speed * _speedMultiplier * Time.deltaTime);
        }
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }

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
    }

    public void TakeDamage()
    {
        if (_isShieldActive)
        {
            ShieldDeactivate();
            return; 
        }

        _lives--;

        if (_lives <= 0)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActivate()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotDeactivateRoutine());
    }

    IEnumerator TripleShotDeactivateRoutine()
    {
        yield return new WaitForSeconds(_powerupActiveTime);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActivate()
    {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostDeactivateRoutine());
    }

    IEnumerator SpeedBoostDeactivateRoutine()
    {
        yield return new WaitForSeconds(_powerupActiveTime);
        _isSpeedBoostActive = false;
    }

    public void ShieldActivate()
    {
        _isShieldActive = true;
        _shield.SetActive(true);
    }

    public void ShieldDeactivate()
    {
        _isShieldActive = false;
        _shield.SetActive(false);
    }
}
