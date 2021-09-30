using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    Vector3 _originalCameraPosition;

    void Start()
    {
        _originalCameraPosition = transform.localPosition;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, _originalCameraPosition.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = _originalCameraPosition;
    }


}
