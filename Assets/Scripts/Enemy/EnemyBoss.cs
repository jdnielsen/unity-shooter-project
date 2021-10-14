using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    enum BossPhase
    {
        Entrance,
        PhaseOnePrep,
        PhaseOneActivated,
        PhaseTwoPrep,
        PhaseTwoActivated,
        PhaseThreePrep,
        PhaseThreeActivated,
        Death
    }

    [SerializeField]
    GameObject _core;
    [SerializeField]
    GameObject _shield;
    [SerializeField]
    GameObject _turretLaserFR;
    [SerializeField]
    GameObject _turretLaserFL;
    [SerializeField]
    GameObject _turretLaserB;
    [SerializeField]
    GameObject _turretMissileF;
    [SerializeField]
    GameObject _turretMissileBR;
    [SerializeField]
    GameObject _turretMissileBL;
    [SerializeField]
    GameObject _turretBeamR;
    [SerializeField]
    GameObject _turretBeamL;
    [SerializeField]
    GameObject _explosionPrefab;
    [SerializeField]
    GameObject _explosionLargePrefab;

    float _yEntranceTarget = 15f;
    float _yPhaseOneAndTwoTarget = 7.5f;
    float _yPhaseThreeTarget = 5f;
    float _zTarget = 0f;
    Vector3 _targetPosition;

    float _minX = -4.5f;
    float _maxX = 4.5f;

    BossPhase _phase;
    float _nextPhase;

    float _speed;
    float _strafeSpeed;
    float _rotationSpeed;

    float _nextMove;
    float _minTimeToNextMove = 4f;
    float _maxTimeToNextMove = 6f;

    // Start is called before the first frame update
    void Start()
    {
        _speed = 5f;
        _strafeSpeed = 1.5f;
        _rotationSpeed = 30f;

        _phase = BossPhase.Entrance;
        _nextPhase = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _nextPhase)
        {
            switch (_phase)
            {
                case BossPhase.Entrance:
                    EntrancePhase();
                    break;
                case BossPhase.PhaseOnePrep:
                    PhaseOnePreparation();
                    break;
                case BossPhase.PhaseOneActivated:
                    PhaseOneActivated();
                    break;
                case BossPhase.PhaseTwoPrep:
                    PhaseTwoPreparation();
                    break;
                case BossPhase.PhaseTwoActivated:
                    PhaseTwoActivated();
                    break;
                case BossPhase.PhaseThreePrep:
                    PhaseThreePreparation();
                    break;
                case BossPhase.PhaseThreeActivated:
                    PhaseThreeActivated();
                    break;
                case BossPhase.Death:
                    PhaseDeath();
                    break;
                default:
                    break;
            }
        }
    }

    void EntrancePhase()
    {
        if (transform.position.y < _yEntranceTarget)
        {
            transform.Translate(_speed * Vector3.up * Time.deltaTime);
        }
        else if (transform.position.z > _zTarget)
        {
            transform.Translate(_speed * Vector3.back * Time.deltaTime);
        }
        else
        {
            _phase = BossPhase.PhaseOnePrep;
            _speed = 2f;
        }
    }

    void PhaseOnePreparation()
    {
        if (transform.position.y > _yPhaseOneAndTwoTarget)
        {
            transform.Translate(_speed * Vector3.down * Time.deltaTime);
        }
        else
        {
            _turretLaserFL.GetComponent<EnemyTurretBase>().Activate(true);
            _turretLaserFR.GetComponent<EnemyTurretBase>().Activate(true);
            _turretMissileF.GetComponent<EnemyTurretBase>().Activate(true);
            _targetPosition = transform.position;
            _nextMove = Time.time + Random.Range(_minTimeToNextMove, _maxTimeToNextMove);
            _phase = BossPhase.PhaseOneActivated;
        }
    }

    void PhaseOneActivated()
    {
        if (Time.time > _nextMove)
        {
            _targetPosition.x = Random.Range(_minX, _maxX);
            _targetPosition.y = transform.position.y;
            _nextMove = Time.time + Random.Range(_minTimeToNextMove, _maxTimeToNextMove);
        }
        if (Vector3.Distance(transform.position, _targetPosition) > 0.1f)
        {
            float direction = Mathf.Sign(_targetPosition.x - transform.position.x);
            transform.Translate(Vector3.right * _strafeSpeed * Time.deltaTime * direction, Space.World);
        }
        if (_turretLaserFL == null && _turretLaserFR == null && _turretMissileF == null)
        {
            _nextPhase = Time.time + 2f;
            _targetPosition.x = 0f;
            _phase = BossPhase.PhaseTwoPrep;
        }
    }

    void PhaseTwoPreparation()
    {
        Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
        if (Vector3.Distance(transform.position, _targetPosition) > 0.1f)
        {
            float direction = Mathf.Sign(_targetPosition.x - transform.position.x);
            transform.Translate(Vector3.right * _strafeSpeed * Time.deltaTime * direction, Space.World);
        }
        else if (transform.rotation != toRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);
        }
        else
        {
            _turretLaserB.GetComponent<EnemyTurretBase>().Activate(true);
            _turretMissileBL.GetComponent<EnemyTurretBase>().Activate(true);
            _turretMissileBR.GetComponent<EnemyTurretBase>().Activate(true);
            _nextMove = Time.time + Random.Range(_minTimeToNextMove, _maxTimeToNextMove);
            _targetPosition.x = 0;
            _phase = BossPhase.PhaseTwoActivated;
        }
    }

    void PhaseTwoActivated()
    {
        if (Time.time > _nextMove)
        {
            _targetPosition.x = Random.Range(_minX, _maxX);
            _nextMove = Time.time + Random.Range(_minTimeToNextMove, _maxTimeToNextMove);
        }
        if (Vector3.Distance(transform.position, _targetPosition) > 0.2f)
        {
            float direction = Mathf.Sign(_targetPosition.x - transform.position.x);
            transform.Translate(Vector3.right * _strafeSpeed * Time.deltaTime * direction, Space.World);
        }
        if (_turretLaserB == null && _turretMissileBL == null && _turretMissileBR == null)
        {
            _nextPhase = Time.time + 2f;
            _speed = 1f;
            _targetPosition.x = 0;
            _targetPosition.y = _yPhaseThreeTarget;
            _phase = BossPhase.PhaseThreePrep;
        }
    }

    void PhaseThreePreparation()
    {
        if (Mathf.Abs(transform.position.x) > .2f)
        {
            float direction = Mathf.Sign(_targetPosition.x - transform.position.x);
            transform.Translate(Vector3.right * _strafeSpeed * Time.deltaTime * direction, Space.World);
        }
        else if (transform.position.y > _yPhaseThreeTarget)
        {
            transform.Translate(_speed * Vector3.up * Time.deltaTime);
        }
        else
        {
            _turretBeamL.GetComponent<EnemyTurretBase>().Activate(true);
            _turretBeamR.GetComponent<EnemyTurretBase>().Activate(true);
            _core.SetActive(true);
            _shield.SetActive(true);
            _shield.GetComponent<BossShield>().Activate(true);
            _phase = BossPhase.PhaseThreeActivated;
        }
    }

    void PhaseThreeActivated()
    {
        if (_core == null)
        {
            OnDeath();
            _speed = 0.8f;
            _phase = BossPhase.Death;
        }
    }

    void PhaseDeath()
    {
        transform.Translate(_speed * Vector3.down * Time.deltaTime);
    }

    void OnDeath()
    {
        if (_turretBeamL != null)
        {
            _turretBeamL.GetComponent<EnemyTurretBase>().Activate(false);
        }
        if (_turretBeamR != null)
        {
            _turretBeamR.GetComponent<EnemyTurretBase>().Activate(false);
        }

        _shield.GetComponent<BossShield>().Activate(false);

        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        float minX = -4f;
        float maxX = 4f;
        float minY = 0f;
        float maxY = 4f;

        //List<Vector3> explosionPositions = new List<Vector3>
        //{
        //    new Vector3(-3.1f, 1.5f),
        //    new Vector3(1.8f, 2.7f),
        //    new Vector3(-1.2f, .2f),
        //    new Vector3(2.5f, 1.9f),
        //    new Vector3(-2.8f, 3.6f),
        //    new Vector3(3.3f, 1.2f),
        //    new Vector3(0f, 3.4f),
        //    new Vector3(-1.6f, 1.8f),
        //    new Vector3(2.9f, 2.3f),
        //    new Vector3(-3.6f, .9f)
        //};

        for (int i = 0; i < 10; i++)
        {
            Vector3 randPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
            Instantiate(_explosionPrefab, transform.position - randPosition, transform.rotation);
            yield return new WaitForSeconds(0.4f);
        }

        Instantiate(_explosionLargePrefab, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.4f);
        Destroy(this.gameObject);
    }
}
