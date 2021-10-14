using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    // speed variables
    private float _speed = 8f;
    private float _rotationSpeed = 60f;

    // target
    private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_player != null)
        {
            Vector3 direction = _player.transform.position - transform.position;
            direction.Normalize();

            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);
        }

        // travel forward
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        // destroy out of bounds
        if (transform.position.y > 8.0f ||
            transform.position.y < -8.0f ||
            transform.position.x < -11.3f ||
            transform.position.x > 11.3f)
        {
            Destroy(this.gameObject);
        }
    }
}
