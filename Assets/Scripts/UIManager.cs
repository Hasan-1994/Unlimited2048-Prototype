using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public BoardManager boardManager;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<UIManager>();
            return _instance;
        }
    }

    private static UIManager _instance;

    NumberPool _numberPool;

    [SerializeField]
    GameObject gameOverPanel;

    [SerializeField]
    TMP_Text scoreText;

    [SerializeField]
    TMP_Text bestText;

    [SerializeField]
    TMP_Text Timer;

    public Button btnAutoPlayFor10Seconds;

    public Button btnAutoPlayFor30Seconds;

    public Button btnAutoPlayFor60Seconds;

    int score;

    int best;

    private float startTime;

    private void Start()
    {
        best = PlayerPrefs.GetInt("Best");
        bestText.text = best.ToString();
        boardManager = GameObject.FindObjectOfType<BoardManager>();
        startTime = Time.time;
    }

    public void Update()
    {
        score.ToString();
        float elapsedTime = Time.time - startTime;
        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        Timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void OnInput(int toAdd)
    {
        UpdateScoreText (toAdd);

        best = PlayerPrefs.GetInt("Best", 0);

        if (score > best)
        {
            best = score;
            PlayerPrefs.SetInt("Best", best);
            bestText.text = best.ToString();
        }
    }

    void UpdateScoreText(int toAdd)
    {
        enableButtonForShop();
        score += toAdd;
        scoreText.text = score.ToString();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void CreateNewGame()
    {
        SceneManager.LoadScene(0);
    }

    [ContextMenu("ResetAllPrefs")]
    void ResetAllPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public void enableButtonForShop()
    {
        if (score >= 50)
        {
            btnAutoPlayFor10Seconds.gameObject.SetActive(true);
        }
        if (score >= 100)
        {
            btnAutoPlayFor30Seconds.gameObject.SetActive(true);
        }
        if (score >= 1000)
        {
            btnAutoPlayFor60Seconds.gameObject.SetActive(true);
        }
        
    }

    public void AutoPlayFor10Seconds()
    {
        boardManager.AutoPlayActionsFor10Seconds();
        score -= 50;
        if (score < 50)
        {
            btnAutoPlayFor10Seconds.gameObject.SetActive(false);
        }
        scoreText.text = score.ToString();
    }

    public void AutoPlayFor30Seconds()
    {
        boardManager.AutoPlayActionsFor30Seconds();
        score -= 100;
        if (score < 100)
        {
            btnAutoPlayFor30Seconds.gameObject.SetActive(false);
        }
        scoreText.text = score.ToString();
    }

    public void AutoPlayFor60Seconds()
    {
        boardManager.AutoPlayActionsFor60Seconds();
        score -= 1000;
        if (score < 1000)
        {
            btnAutoPlayFor60Seconds.gameObject.SetActive(false);
        }
        scoreText.text = score.ToString();
    }

    [SerializeField]
    GameState gameState;

    public enum GameState
    {
        Playing,
        GameOver
    }
}
