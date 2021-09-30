using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    [SerializeField]
    GameObject _enemy;
    EnemyAgile _enemyScript;

    // Start is called before the first frame update
    void Start()
    {
        _enemyScript = _enemy.GetComponent<EnemyAgile>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        _enemyScript.DamageTrigger(other);
    }
}
