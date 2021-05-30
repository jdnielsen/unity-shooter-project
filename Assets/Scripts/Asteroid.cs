using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 18.0f;

    [SerializeField]
    private GameObject _explosionPrefab;

    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 1 * _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.tag == "Player")
        //{
        //    Player player = other.GetComponent<Player>();
        //    if (player != null)
        //    {
        //        player.TakeDamage();
        //    }
        //    Instantiate(_explosionPrefab, transform.position, transform.rotation);
        //    Destroy(this.gameObject);
        //}

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            Instantiate(_explosionPrefab, transform.position, transform.rotation);
            _spawnManager.StartEnemySpawns();
            Destroy(this.gameObject, 0.25f);
        }
    }
}
