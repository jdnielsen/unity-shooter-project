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
    private Text _enemyWaveText;
    [SerializeField]
    private Image _ammoMeter;
    [SerializeField]
    private Image _thrusterMeter;
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
        _gameOverText.text = "GAME OVER";
        _restartGameText.gameObject.SetActive(false);
        _enemyWaveText.gameObject.SetActive(false);

        // game manager
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


    public void UpdateAmmo(int ammo, int maxAmmo = 15)
    {
        float ammoLevel = (float)ammo / maxAmmo;
        _ammoMeter.fillAmount = ammoLevel;
    }

    public void UpdateThruster(float energy, float maxEnergy)
    {
        float energyLevel = energy / maxEnergy;
        _thrusterMeter.fillAmount = energyLevel;
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

    public void WinGame()
    {
        _isGameOver = true;
        _gameOverText.text = "YOU WIN!";
        _gameOverText.gameObject.SetActive(true);
        _restartGameText.gameObject.SetActive(true);
        _gameManager.GameOver();
    }

    public void DisplayWaveText(string text)
    {
        _enemyWaveText.text = text;
        StartCoroutine(DisplayWaveTextRoutine());
    }

    IEnumerator DisplayWaveTextRoutine()
    {
        _enemyWaveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _enemyWaveText.gameObject.SetActive(false);
    }
}
