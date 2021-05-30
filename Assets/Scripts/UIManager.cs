using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartGameText;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Sprite[] _liveSprites;
    private bool _isGameOver;

    [SerializeField]
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _isGameOver = false;
        _gameOverText.gameObject.SetActive(false);
        _restartGameText.gameObject.SetActive(false);

        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLives(int lives)
    {
        if (lives > 3 || lives < 0)
        {
            Debug.LogError("Invalid number of lives.");
        }
        else
        {
            _livesImg.sprite = _liveSprites[lives];
            if (lives == 0)
            {
                GameOverSequence();
            }
        }
    }

    void GameOverSequence()
    {
        _isGameOver = true;
        StartCoroutine(GameOverTextFlashRoutine());
        _restartGameText.gameObject.SetActive(true);
        _gameManager.GameOver();
    }

    IEnumerator GameOverTextFlashRoutine()
    {
        while (_isGameOver)
        {
            if (_gameOverText.gameObject.activeSelf)
            {
                _gameOverText.gameObject.SetActive(false);
            }
            else
            {
                _gameOverText.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
