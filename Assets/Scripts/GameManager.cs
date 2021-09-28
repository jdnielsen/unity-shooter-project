using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        _isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver)
        {
            SceneManager.LoadScene(1); // current game scene
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    public static bool ObjectRoughlyPointedAtTarget(Vector3 objectHeading, Vector3 objectPosition, Vector3 targetPosition, float threshold = .95f)
    {
        Vector3 direction = objectPosition - targetPosition;
        direction.Normalize();
        return Vector3.Dot(direction, objectHeading) > threshold;
    }
}
