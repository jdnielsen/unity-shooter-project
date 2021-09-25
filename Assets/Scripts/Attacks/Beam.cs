using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    private float _beamGrowthRate = 0.05f;
    private float _beamGrowthPeriod = 0.01f;
    private float _beamDuration = 2.5f;
    private float _chargeDuration = 0.5f;
    private float _timeElapsed;
    private Vector3 _scaleChange;

    // Start is called before the first frame update
    void Start()
    {
        _scaleChange = new Vector3(0, _beamGrowthRate, 0);
        _timeElapsed = 0f;
        StartCoroutine(ExtendBeam());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ExtendBeam()
    {
        yield return new WaitForSeconds(_chargeDuration);
        _timeElapsed += _chargeDuration;

        while (_timeElapsed < _beamDuration)
        {
            transform.localScale += _scaleChange;
            yield return new WaitForSeconds(_beamGrowthPeriod);
            _timeElapsed += _beamGrowthPeriod;
        }

        Destroy(this.gameObject);
    }
}
