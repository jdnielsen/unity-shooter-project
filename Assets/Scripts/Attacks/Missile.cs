using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // speed variables
    private float _speed = 10f;
    private float _rotationSpeed = 90f;

    // target
    private GameObject _target;

    // Start is called before the first frame update
    void Start()
    {
        LockOnTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (_target != null && !_target.GetComponent<EnemyBase>().IsDead())
        {
            Vector3 direction = _target.transform.position - transform.position;
            direction.Normalize();

            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);
        }
        else
        {
            LockOnTarget();
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

    private void LockOnTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = float.MaxValue;

        foreach (GameObject e in enemies)
        {
            if (!e.GetComponent<EnemyBase>().IsDead())
            {
                float distance = Vector3.Distance(e.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _target = e;
                }
            }
        }
    }
}
