using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField] // 0: Triple Shot, 1: Speed, 2: Shields
    private int _powerupId;
    [SerializeField]
    public int _chances;
    [SerializeField]
    private GameObject _powerupExplosion;
    private bool _isDestroyed = false;
    private Transform _playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.Find("Player").transform;
        if (_playerTransform == null)
        {
            Debug.LogError("Player is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isDestroyed)
        {
            if (Input.GetKey(KeyCode.C))
            {
                Vector3 direction = _playerTransform.position - transform.position;
                direction.Normalize();

                transform.Translate(direction * _speed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
            }
        }

        if (transform.position.y < -5.0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                switch (_powerupId)
                {
                    case 0:
                        player.TripleShotActivate();
                        break;
                    case 1:
                        player.SpeedBoostActivate();
                        break;
                    case 2:
                        player.ShieldActivate();
                        break;
                    case 3:
                        player.AmmoPickup();
                        break;
                    case 4:
                        player.HealthPickup();
                        break;
                    case 5:
                        player.HomingMissileActivate();
                        break;
                    case 6:
                        player.AntiAmmoPickup();
                        break;
                    default:
                        Debug.Log("Powerup ID not found.");
                        break;
                }
                
            }

            Destroy(this.gameObject);
        }
        else if (other.tag == "EnemyLaser")
        {
            DestroyPowerup();
        }
    }

    void DestroyPowerup()
    {
        _isDestroyed = true;
        Instantiate(_powerupExplosion, transform.position, transform.rotation);
        Destroy(this.gameObject, .2f);
    }
}
