using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
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
    //powerup variables
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private float _tripleShotActiveTime = 5.0f;

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
        transform.Translate(direction * _speed * Time.deltaTime);

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
        yield return new WaitForSeconds(_tripleShotActiveTime);
        _isTripleShotActive = false;
    }
}
