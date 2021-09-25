using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Missile : MonoBehaviour
{
    // rigid body
    private Rigidbody2D rb;

    // speed variables
    private float _speed = 10.0f;
    private float _rotationSpeed = 240.0f;

    // target
    private GameObject _target;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        LockOnTarget();
    }

    void FixedUpdate()
    {
        // changing direction
        if (_target != null && !_target.GetComponent<EnemyBase>().IsDead())
        {
            Vector2 direction = (Vector2)_target.transform.position - rb.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            rb.angularVelocity = -rotateAmount * _rotationSpeed;
        }
        else
        {
            // eliminate angular velocity
            rb.angularVelocity = 0;
            // find a new target
            LockOnTarget();
        }


        // forward velocity
        rb.velocity = transform.up * _speed;


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
